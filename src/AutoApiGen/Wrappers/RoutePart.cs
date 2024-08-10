﻿using AutoApiGen.Exceptions;

namespace AutoApiGen.Wrappers;

/*[Closed]*/
public abstract record RoutePart
{
    public sealed record LiteralRoutePart(string Value) : RoutePart;
    public sealed record RawParameterRoutePart(string Name, string? Type = null, string? Default = null) : RoutePart;
    public sealed record OptionalParameterRoutePart(string Name, string? Type = null) : RoutePart;
    public sealed record CatchAllParameterRoutePart(string Name, string? Type = null, string? Default = null) : RoutePart;

    public static RoutePart Parse(string part) => part switch
    {
        not ['{', .., '}'] => new LiteralRoutePart(part),

        ['{', .., '}'] when Regexes.RawParameterRoutePartRegex.Match(part) is { Success: true } match =>
            new RawParameterRoutePart(
                match.Groups["name"].Value,
                match.Groups["type"].Value is { Length: > 0 } type ? type : null,
                match.Groups["default"].Value is { Length: > 0 } defaultValue ? defaultValue : null
            ),

        ['{', .., '?', '}'] when Regexes.OptionalParameterRoutePartRegex.Match(part) is { Success: true } match =>
            new OptionalParameterRoutePart(
                match.Groups["name"].Value,
                match.Groups["type"].Value is { Length: > 0 } type ? type : null
            ),

        ['{', '*', .., '}'] when Regexes.CatchAllParameterRoutePartRegex.Match(part) is { Success: true } match =>
            new CatchAllParameterRoutePart(
                match.Groups["name"].Value,
                match.Groups["type"].Value is { Length: > 0 } type ? type : null,
                match.Groups["default"].Value is { Length: > 0 } defaultValue ? defaultValue : null
            ),

        _ => throw new ArgumentException("Invalid syntax", nameof(part))
    };
    
    public static string Format(RoutePart part) => part switch
    {
        LiteralRoutePart(var value) => value,
        
        RawParameterRoutePart(var name, var type, var defaultValue) =>
            $$"""{{{name}}{{FormatType(type)}}{{FormatDefault(defaultValue)}}}""",
        
        OptionalParameterRoutePart(var name, var type) => 
            $$"""{{{name}}{{FormatType(type)}}}""",
        
        CatchAllParameterRoutePart(var name, var type, var defaultValue) =>    
            $$"""{{{name}}{{FormatType(type)}}{{FormatDefault(defaultValue)}}}""",
        
        _ => throw new ThisIsUnionException()
    };

    private static string FormatType(string? type) => type is null ? "" : $":{type}";
    private static string FormatDefault(string? defaultValue) => defaultValue is null ? "" : $"={defaultValue}";
}
