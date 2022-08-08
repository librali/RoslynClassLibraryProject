﻿using CsCodeGenerator;
using CsCodeGenerator.Enums;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Text;

namespace LittleToyDocumentor;

[Generator]
public class Generator : ISourceGenerator
{
    private const string FileHeader = @"// <auto-generated>
// Code generated by LittleToy Documentation Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>
#nullable enable
#pragma warning disable 1591";

    /// <inheritdoc/>
    public void Execute(GeneratorExecutionContext context)
    {
        // Retrieve the populated receiver
        if (!(context.SyntaxReceiver is SyntaxReceiver receiver))
        {
            return;
        }

        var file = new FileModel("Documenation")
        {
            UsingDirectives = new List<string>
            {
                "System;",
                "Unity.Entities;",
                "Unity.Mathematics;",
                "Plugins.basegame.Events;",
                "DOTSNET;",
                "Unity.Collections;"
            },
            Header = FileHeader,
        };
        file.Classes.Add(new ClassModel("DocumentationClass")
        {
            Comment = $"Found {receiver.MemberAccessExpressionSyntaxes.Count} candidates",
            SingleKeyWord = KeyWord.Partial,
        });
        context.AddSource(file.Name, SourceText.From(file.ToString(), Encoding.UTF8));
    }

    /// <inheritdoc/>
    public void Initialize(GeneratorInitializationContext context)
    {
        context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
    }

    internal class SyntaxReceiver : ISyntaxReceiver
    {
        public List<MemberAccessExpressionSyntax> MemberAccessExpressionSyntaxes = new();

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            if (syntaxNode is InvocationExpressionSyntax invocationExpressionSyntax)
            {
                if (invocationExpressionSyntax.Expression is MemberAccessExpressionSyntax memberAccessExpressionSyntax)
                {
                    if (memberAccessExpressionSyntax.OperatorToken.IsKind(SyntaxKind.DotToken) && memberAccessExpressionSyntax.Name.ToFullString().StartsWith("AddComponent"))
                    {
                        MemberAccessExpressionSyntaxes.Add(memberAccessExpressionSyntax);
                    }
                }
            }
        }
    }
}
