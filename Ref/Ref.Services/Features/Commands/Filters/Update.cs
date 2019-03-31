﻿using MediatR;
using Ref.Data.Models;
using Ref.Data.Repositories;
using Ref.Services.Features.Shared;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Ref.Services.Features.Commands.Filters
{
    public class Update
    {
        public class Cmd : IRequest<Result>
        {
            public int Id { get; set; }
            public int UserId { get; set; }
            public int CityId { get; set; }
            public int FlatAreaFrom { get; set; }
            public int FlatAreaTo { get; set; }
            public int PriceFrom { get; set; }
            public int PriceTo { get; set; }
            public string Name { get; set; }
            public NotificationType Notification { get; set; }
            public string ShouldContain { get; set; }
            public string ShouldNotContain { get; set; }
        }

        public class Result : BaseResult { }

        public class Handler : IRequestHandler<Cmd, Result>
        {
            private readonly IFilterRepository _filterRepository;

            public Handler(IFilterRepository filterRepository)
            {
                _filterRepository = filterRepository;
            }

            public async Task<Result> Handle(Cmd request, CancellationToken cancellationToken)
            {
                try
                {
                    var entity = await _filterRepository.GetAsync(request.Id, request.UserId);

                    if(entity is null)
                    {
                        return new Result { Message = "No such filter" };
                    }

                    await _filterRepository.UpdateAsync(new Filter
                    {
                        Id = request.Id,
                        UserId = request.UserId,
                        Property = PropertyType.Flat,
                        Deal = DealType.Sale,
                        Market = MarketType.Secondary,
                        FlatAreaFrom = request.FlatAreaFrom,
                        FlatAreaTo = request.FlatAreaTo,
                        PriceFrom = request.PriceFrom,
                        PriceTo = request.PriceTo,
                        Notification = request.Notification,
                        CityId = request.CityId,
                        Name = request.Name,
                        LastCheckedAt = entity.LastCheckedAt,
                        ShouldContain = request.ShouldContain,
                        ShouldNotContain = request.ShouldNotContain
                    });

                    return new Result();
                }
                catch (Exception ex)
                {
                    return new Result { Message = ex.Message };
                }
            }
        }
    }
}