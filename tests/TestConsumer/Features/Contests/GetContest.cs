﻿using System.Threading;
using System.Threading.Tasks;
using AutoApiGen.Attributes;
using MediatR;

namespace TestConsumer.Features.Contests;

public static class GetContest
{
    [GetEndpoint("contests/{Id:int}")]
    public record Query(int Id) : IRequest<Contest>;
    
    public class Handler : IRequestHandler<Query, Contest>
    {
        public Task<Contest> Handle(Query query, CancellationToken cancellationToken)
        {
            var id = query.Id;

            return Task.FromResult(new Contest(id, "Contest"));
        }
    }
}
