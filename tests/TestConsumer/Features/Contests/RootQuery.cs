﻿using System.Threading;
using System.Threading.Tasks;
using AutoApiGen.Attributes;
using Mediator;

namespace TestConsumer.Features.Contests;

public static class RootQuery
{
    [GetEndpoint("{Name}")]
    public record Query(string Name) : IQuery<string>;

    public class RootQueryHandler : IQueryHandler<Query, string>
    {
        public ValueTask<string> Handle(Query query, CancellationToken cancellationToken) => 
            ValueTask.FromResult($"Welcome to AutoApiGen TempConsumer project, {query.Name}!");
    }
}