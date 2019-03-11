using MediatR;
using Ref.Data.Models;
using Ref.Data.Repositories;
using Ref.Services.Features.Shared;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Ref.Services.Features.Commands.Filters
{
    public class Create
    {
        public class Cmd : IRequest<Result>
        {
            public int UserId { get; set; }
            public int CityId { get; set; }
            public int FlatAreaFrom { get; set; }
            public int FlatAreaTo { get; set; }
            public int PriceFrom { get; set; }
            public int PriceTo { get; set; }
            public NotificationType Notification { get; set; }
            public string Name { get; set; }
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
                    var result = await _filterRepository.CreateAsync(new Filter
                    {
                        UserId = request.UserId,
                        Property = PropertyType.Flat,
                        Deal = DealType.Sale,
                        Market = MarketType.Secondary,
                        CityId = request.CityId,
                        FlatAreaFrom = request.FlatAreaFrom,
                        FlatAreaTo = request.FlatAreaTo,
                        PriceFrom = request.PriceFrom,
                        PriceTo = request.PriceTo,
                        Notification = request.Notification,
                        Name = request.Name
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