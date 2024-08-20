﻿using AutoApiGen.Extensions;

namespace AutoApiGen.Templates;

internal static class ControllerTemplate
{
    internal readonly record struct Data(
        string MediatorPackageName,
        string Namespace,
        string? BaseRoute,
        string Name,
        List<MethodTemplate.Data> Methods,
        List<RequestTemplate.Data> Requests
    ) : ITemplateData;

    public static string Render(
        Data data,
        Func<RequestTemplate.Data, string> renderRequest,
        Func<MethodTemplate.Data, string> renderMethod
    ) => $$"""
        //--------------------------------------------------------------------------------
        // <auto-generated>
        //     This code was generated by a AutoApiGen tool.
        // </auto-generated>
        //--------------------------------------------------------------------------------
        namespace {{data.Namespace}};
        {{data.Requests.RenderAndJoin(renderRequest)}}

        {{data.BaseRoute.ApplyIfNotNullOrEmpty(baseRoute =>
            $"[global::Microsoft.AspNetCore.Mvc.Route(\"{baseRoute}\")]")
        }}
        public partial class {{data.Name}}Controller(
            {{data.MediatorPackageName}}.IMediator mediator
        ) : global::Microsoft.AspNetCore.Mvc.ControllerBase
        {   
            private readonly {{data.MediatorPackageName}}.IMediator _mediator = mediator;
         
            {{data.Methods.RenderAndJoin(renderMethod, separator: "\n\n")}}
        }
        """;
}
