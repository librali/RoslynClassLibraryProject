﻿namespace LittleToyZenjectify;

using CsCodeGenerator;
using CsCodeGenerator.Enums;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[Generator]
public class Generator : ISourceGenerator
{
    private const string FileHeader = @"// <auto-generated>
// Code generated by LittleToy Zenjectify Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>
#nullable enable
#pragma warning disable 1591";

    private const string ZenGenAttributeType = "ZenGen";
    private static readonly AttributeModel RequiredAttribute = new AttributeModel("Required");
    private static readonly AttributeModel SceneObjectsOnlyAttribute = new AttributeModel("SceneObjectsOnly");
    private static readonly AttributeModel AssetsOnlyAttribute = new AttributeModel("AssetsOnly");
    private static readonly AttributeModel SerializeFieldAttribute = new AttributeModel("SerializeField");

    /// <inheritdoc/>
    public void Execute(GeneratorExecutionContext context)
    {
        // Retrieve the populated receiver
        if (!(context.SyntaxReceiver is SyntaxReceiver receiver))
        {
            return;
        }

        var serviceTypes = receiver.ServiceCandidates
            .Select(_ => context.Compilation.GetSemanticModel(_.SyntaxTree).GetDeclaredSymbol(_))
            .Where(_ => _ is not null)
            .Where(IsValidServiceClass);
        var serviceDescriptors = serviceTypes.Select(GetServiceDescriptor).ToList();
        foreach (var installerServices in serviceDescriptors.GroupBy(_ => _.TargetInstallerNameName))
        {
            string installerName = installerServices.Key;
            if (!installerName.EndsWith("Installer"))
            {
                installerName += "Installer";
            }

            var file = CreateInstallerFile(installerName, installerServices);
            context.AddSource(file.Name, SourceText.From(file.ToString(), Encoding.UTF8));
        }
    }

    private static FileModel CreateInstallerFile(string installerName, IEnumerable<ServiceDescriptor> services)
    {
        var file = new FileModel(installerName)
        {
            UsingDirectives = new List<string>
            {
                "Sirenix.OdinInspector;",
                "UnityEngine;",
                "Zenject;"
            },
            Namespace = "",
            Header = FileHeader,
        };

        var monoClassesWithSceneObjInstance = services.Where(s => s.InjectionMethod == InjectionMethod.MonoClassWithSceneObjInstance).ToList();
        var monoClassesWithAssetInstance = services.Where(s => s.InjectionMethod == InjectionMethod.MonoClassWithAssetInstance).ToList();
        var classesWithoutInstance = services.Where(s => s.InjectionMethod == InjectionMethod.ClassWithoutInstance).ToList();
        var prefabs = services.Where(s => s.InjectionMethod == InjectionMethod.Prefab).ToList();
        var signals = services.Where(s => s.InjectionMethod == InjectionMethod.Signal).ToList();

        var classModel = new ClassModel(installerName)
        {
            BaseClass = "MonoInstaller",
        };
        foreach (var service in monoClassesWithSceneObjInstance)
        {
            classModel.Fields.Add(new Field()
            {
                AccessModifier = AccessModifier.Private,
                Attributes = new () { RequiredAttribute, SceneObjectsOnlyAttribute, SerializeFieldAttribute },
                Name = service.ServiceType.Name.LowerFirst(),
                CustomDataType = service.ServiceType.ToDisplayString(),
            });
        }

        foreach (var service in monoClassesWithAssetInstance)
        {
            classModel.Fields.Add(new Field()
            {
                AccessModifier = AccessModifier.Private,
                Attributes = new() { RequiredAttribute, AssetsOnlyAttribute, SerializeFieldAttribute },
                Name = service.ServiceType.Name.LowerFirst(),
                CustomDataType = service.ServiceType.ToDisplayString(),
            });
        }

        foreach (var service in prefabs)
        {
            classModel.Fields.Add(new Field()
            {
                AccessModifier = AccessModifier.Private,
                Attributes = new() { RequiredAttribute, AssetsOnlyAttribute, SerializeFieldAttribute },
                Name = service.ServiceType.Name.LowerFirst(),
                CustomDataType = service.ServiceType.ToDisplayString(),
            });
        }

        var installBindingsMethod = new Method()
        {
            AccessModifier = AccessModifier.Public,
            Parameters = new(),
            BodyLines = new() { "base.InstallBindings();" },
            Name = "InstallBindings",
            BuiltInDataType = BuiltInDataType.Void,
            KeyWords = new() { KeyWord.Override }
        };
        if (monoClassesWithSceneObjInstance.Count > 0)
        {
            var installMethod = new Method()
            {
                AccessModifier = AccessModifier.Private,
                Parameters = new(),
                BodyLines = new(),
                Name = "InstallMonoClassesWithSceneObjInstance",
                BuiltInDataType = BuiltInDataType.Void,
            };
            installBindingsMethod.BodyLines.Add(installMethod.Name + "();");
            foreach (var service in monoClassesWithSceneObjInstance)
            {
                installMethod.BodyLines.Add(GenerateCall(service));
            }

            classModel.Methods.Add(installMethod);
        }

        if (monoClassesWithAssetInstance.Count > 0)
        {
            var installMethod = new Method()
            {
                AccessModifier = AccessModifier.Private,
                Parameters = new(),
                BodyLines = new(),
                Name = "InstallMonoClassesWithAssetInstance",
                BuiltInDataType = BuiltInDataType.Void,
            };
            installBindingsMethod.BodyLines.Add(installMethod.Name + "();");
            foreach (var service in monoClassesWithAssetInstance)
            {
                installMethod.BodyLines.Add(GenerateCall(service));
            }

            classModel.Methods.Add(installMethod);
        }

        if (classesWithoutInstance.Count > 0)
        {
            var installMethod = new Method()
            {
                AccessModifier = AccessModifier.Private,
                Parameters = new(),
                BodyLines = new(),
                Name = "InstallClassesWithoutInstance",
                BuiltInDataType = BuiltInDataType.Void,
            };
            installBindingsMethod.BodyLines.Add(installMethod.Name + "();");
            foreach (var service in classesWithoutInstance)
            {
                installMethod.BodyLines.Add(GenerateCall(service));
            }

            classModel.Methods.Add(installMethod);
        }

        if (prefabs.Count > 0)
        {
            var installMethod = new Method()
            {
                AccessModifier = AccessModifier.Private,
                Parameters = new(),
                BodyLines = new(),
                Name = "InstallPrefabs",
                BuiltInDataType = BuiltInDataType.Void,
            };
            installBindingsMethod.BodyLines.Add(installMethod.Name + "();");
            foreach (var service in prefabs)
            {
                var candidateConstructor = service.CandidateConstructors.FirstOrDefault();
                var typeNames = candidateConstructor?.Parameters.Select(_ => _.Type.ToDisplayString()) ?? Array.Empty<string>();
                var typeParameters = string.Join(", ", typeNames.Union(new[] { service.ServiceType.ToDisplayString() }));
                var call = $"Container.BindFactory<{typeParameters}, {service.ServiceType.ToDisplayString()}.Factory>().FromComponentInNewPrefab({service.ServiceType.Name.LowerFirst()}){service.Suffix};";
                installMethod.BodyLines.Add(call);
            }

            classModel.Methods.Add(installMethod);
        }

        if (signals.Count > 0)
        {
            var installMethod = new Method()
            {
                AccessModifier = AccessModifier.Private,
                Parameters = new(),
                BodyLines = new(),
                Name = "InstallSignals",
                BuiltInDataType = BuiltInDataType.Void,
            };
            installBindingsMethod.BodyLines.Add(installMethod.Name + "();");
            foreach (var service in signals)
            {
                installMethod.BodyLines.Add(GenerateCall(service));
            }

            classModel.Methods.Add(installMethod);
        }

        classModel.Methods.Add(installBindingsMethod);
        file.Classes.Add(classModel);

        if (prefabs.Count > 0)
        {
            foreach (var service in prefabs)
            {
                var candidateConstructor = service.CandidateConstructors.FirstOrDefault();
                var typeNames = candidateConstructor?.Parameters.Select(_ => _.Type.ToDisplayString()) ?? Array.Empty<string>();
                var typeParameters = string.Join(", ", typeNames.Union(new[] { service.ServiceType.ToDisplayString() }));
                var prefabClass = new ClassModel(service.ServiceType.ToDisplayString())
                {
                    SingleKeyWord = KeyWord.Partial,
                };
                var prefabFatory = new ClassModel("Factory")
                {
                    BaseClass = $"PlaceholderFactory<{typeParameters}>",
                };
                prefabClass.NestedClasses.Add(prefabFatory);
                file.Classes.Add(prefabClass);
            }
        }
        return file;
    }

    /// <inheritdoc/>
    public void Initialize(GeneratorInitializationContext context)
    {
        context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
    }

    private static string GenerateCall(ServiceDescriptor service)
    {
        if (service.InjectionMethod == InjectionMethod.Signal)
        {
            return $"Container.DeclareSignal<{service.ServiceType.ToDisplayString()}>(){service.Suffix};";
        }

        var bindMethod = service.BindInterfacesAndSelf ? "BindInterfacesAndSelf" : "Bind";
        string loadMethod = service.IsLazyLoading ? "Lazy" : "NonLazy";
        string optionalFromInstance = service.FromInstance ? $".FromInstance({service.ServiceType.Name.LowerFirst()})" : string.Empty;
        var call = $"Container.{bindMethod}<{service.ServiceType.ToDisplayString()}>(){optionalFromInstance}.AsSingle().{loadMethod}(){service.Suffix};";
        return call;
    }

    private static bool IsValidServiceClass(INamedTypeSymbol namedTypeSymbol)
    {
        var attribute = namedTypeSymbol.GetCustomAttribute(ZenGenAttributeType);
        if (attribute == null)
        {
            return false;
        }

        if (attribute.ConstructorArguments.Length != 5)
        {
            return false;
        }

        return true;
    }

    private static ServiceDescriptor GetServiceDescriptor(INamedTypeSymbol namedTypeSymbol)
    {
        var attribute = namedTypeSymbol.GetCustomAttribute(ZenGenAttributeType);
        var injectionMethod = (InjectionMethod)(int)attribute.ConstructorArguments[0].Value;
        var installerEnumValue = attribute.ConstructorArguments[1].ToCSharpString().Split('.').Last();
        var bindInterfacesAndSelf = (bool)attribute.ConstructorArguments[2].Value;
        var isLazyLoading = (bool)attribute.ConstructorArguments[3].Value;
        var suffix = (string)attribute.ConstructorArguments[4].Value;
        return new ServiceDescriptor
        {
            ServiceType = namedTypeSymbol,
            InjectionMethod = injectionMethod,
            BindInterfacesAndSelf = bindInterfacesAndSelf,
            IsLazyLoading = isLazyLoading,
            Suffix = suffix,
            TargetInstallerNameName = installerEnumValue,
        };
    }

    internal class SyntaxReceiver : ISyntaxReceiver
    {
        public List<ClassDeclarationSyntax> ServiceCandidates = new List<ClassDeclarationSyntax>();

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            if (syntaxNode is not ClassDeclarationSyntax classDeclarationSyntax)
            {
                return;
            }

            var zenGenAttribute = classDeclarationSyntax.AttributeLists.FindAttribute(ZenGenAttributeType);
            if (zenGenAttribute == null)
            {
                return;
            }

            ServiceCandidates.Add(classDeclarationSyntax);
        }
    }
}
