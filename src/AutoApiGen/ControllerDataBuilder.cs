﻿using System.Collections.Immutable;
using AutoApiGen.DataObjects;
using AutoApiGen.Extensions;
using AutoApiGen.Wrappers;

namespace AutoApiGen;

internal class ControllerDataBuilder(
    ImmutableArray<EndpointContractDeclarationSyntax> endpoints,
    string? rootNamespace
)
{
    private readonly ImmutableArray<EndpointContractDeclarationSyntax> _endpoints = endpoints;
    private readonly string _controllersNamespace =
        rootNamespace is null 
            ? "Controllers" 
            : $"{rootNamespace}.Controllers";

    
    private readonly Dictionary<string, ControllerData> _controllers = [];

    public ImmutableArray<ControllerData> Build()
    {
        foreach (var endpoint in _endpoints)
            IncludeRequestFrom(endpoint);

        return _controllers.Values.ToImmutableArray();
    }

    private void IncludeRequestFrom(EndpointContractDeclarationSyntax endpoint)
    {
        var routeParameters = endpoint.GetRouteParameters().Select(ParameterData.FromRoute).ToImmutableArray();
        var requestName = endpoint.GetRequestName();

        var request = CreateRequestData(endpoint, routeParameters, requestName);
        var method = CreateMethodData(endpoint, routeParameters, request);
        
        AddRequestToCorrespondingController(endpoint.BaseRoute, request, method);
    }

    private void AddRequestToCorrespondingController(string baseRoute, RequestData request, MethodData method)
    {
        var controllerName = baseRoute.WithCapitalFirstLetter();
        
        if (_controllers.TryGetValue(controllerName, out var controller))
        {
            controller.Methods.Add(method);
            controller.Requests.Add(request);
            return;
        }

        _controllers[controllerName] = new ControllerData(
            _controllersNamespace,
            baseRoute,
            controllerName,
            [method],
            [request]
        );
    }

    private static MethodData CreateMethodData(
        EndpointContractDeclarationSyntax endpoint,
        ImmutableArray<ParameterData> routeParameters,
        RequestData request
    ) => new(
        endpoint.GetHttpMethod(),
        endpoint.GetRelationalRoute(),
        Attributes: "",
        Name: request.Name,
        routeParameters,
        $"{request.Name}Request",
        request.Parameters.Select(p => p.Name).ToImmutableArray(),
        endpoint.GetContractType(),
        endpoint.GetParameters().Select(p => p.Name()).ToImmutableArray(),
        endpoint.GetResponseType()
    );

    private static RequestData CreateRequestData(
        EndpointContractDeclarationSyntax endpoint,
        ImmutableArray<ParameterData> routeParameters,
        string requestName
    )
    {
        var routeParametersNames = routeParameters.Select(p => p.Name).ToImmutableHashSet();
        
        return new RequestData(
            requestName,
            Parameters: endpoint.GetParameters()
                .Where(parameter => !routeParametersNames.Contains(parameter.Name()))
                .Select(ParameterData.FromSyntax)
                .ToImmutableArray()
        );
    }
}