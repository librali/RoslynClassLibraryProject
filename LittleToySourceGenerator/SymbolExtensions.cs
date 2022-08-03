﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LittleToySourceGenerator;

internal static class SymbolExtensions
{
    public static bool HasAttribute(this ISymbol symbol, string attribute)
    {
        return symbol.GetAttributes().Any(a => a.AttributeClass.Name.Contains(attribute));
    }

    public static bool IsAttribute(this AttributeData attribute, string attributeName)
    {
        return attribute.AttributeClass.Name.Contains(attributeName);
    }

    public static AttributeSyntax FindAttribute(this SyntaxList<AttributeListSyntax> attributeLists, string searchAttributeName)
    {
        return attributeLists.SelectMany(_ => _.Attributes).FirstOrDefault(a => a.Name.ToFullString().Contains(searchAttributeName));
    }

    public static AttributeData GetCustomAttribute(this ITypeSymbol typeSymbol, string searchAttributeName)
    {
        return typeSymbol.GetAttributes().FirstOrDefault(a => a.AttributeClass.Name.Contains(searchAttributeName));
    }

    public static IEnumerable<AttributeData> GetCustomAttributes(this ITypeSymbol typeSymbol, bool inherit)
    {
        foreach (var attribute in typeSymbol.GetAttributes())
        {
            yield return attribute;
        }

        if (inherit)
        {
            var baseType = typeSymbol.BaseType;
            while (baseType != null)
            {
                foreach (var attribute in baseType.GetAttributes())
                {
                    if (AttributeCanBeInherited(attribute))
                    {
                        yield return attribute;
                    }
                }

                baseType = baseType.BaseType;
            }
        }
    }

    public static string GetFieldValue(this AttributeData attributeData, string fieldName)
    {
        var fieldPair = attributeData.NamedArguments.FirstOrDefault(_ => _.Key == fieldName);
        if (fieldPair.Key == null)
        {
            return null;
        }

        var field = fieldPair.Value;
        return field.Value.ToString();
    }

    public static IEnumerable<IFieldSymbol> GetFields(this ITypeSymbol typeSymbol)
    {
        return typeSymbol.GetMembers().OfType<IFieldSymbol>();
    }
    private static bool AttributeCanBeInherited(this AttributeData attribute)
    {
        if (attribute.AttributeClass == null)
        {
            return false;
        }

        foreach (var attributeAttribute in attribute.AttributeClass.GetAttributes())
        {
            var attributeClass = attributeAttribute.AttributeClass;
            if (attributeClass != null && attributeClass.Name == nameof(AttributeUsageAttribute) &&
                attributeClass.ContainingNamespace?.Name == "System")
            {
                foreach (var kvp in attributeAttribute.NamedArguments)
                {
                    if (kvp.Key == nameof(AttributeUsageAttribute.Inherited))
                    {
                        return (bool)kvp.Value.Value!;
                    }
                }

                // Default value of Inherited is true
                return true;
            }
        }

        return false;
    }

}