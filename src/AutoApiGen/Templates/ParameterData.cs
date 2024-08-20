﻿using AutoApiGen.Models;
using Microsoft.CodeAnalysis;

namespace AutoApiGen.Templates;

internal readonly record struct ParameterData(
    string Attributes,
    string Type,
    string Name,
    string? Default
) : ITemplateData
{
    public static ParameterData FromRoute(RoutePart.ParameterRoutePart parameter) => new(
        Attributes: "[global::Microsoft.AspNetCore.Mvc.FromRoute]",
        parameter.Type ?? "string",
        parameter.Name,
        parameter.Default
    );

    public static ParameterData FromSymbol(IParameterSymbol parameter) => new(
        Attributes: "",
        parameter.Type.ToString(),
        parameter.Name,
        parameter.HasExplicitDefaultValue ? parameter.ExplicitDefaultValue as string : null
    );
}