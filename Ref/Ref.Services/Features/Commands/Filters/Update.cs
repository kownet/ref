using MediatR;
using Ref.Data.Models;
using Ref.Data.Repositories;
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
            public PropertyType Type { get; set; }
            public DealType Deal { get; set; }
            public string Location { get; set; }
            public int FlatAreaFrom { get; set; }
            public int FlatAreaTo { get; set; }
            public int PriceFrom { get; set; }
            public int PriceTo { get; set; }
            public MarketType Market { get; set; }
            public int Newest { get; set; }
        }

        public class Result
        {
            public bool Succeed => string.IsNullOrWhiteSpace(Message);
            public string Message { get; set; }
        }

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
                        Location = request.Location,
                        Type = request.Type,
                        Deal = request.Deal,
                        Market = request.Market,
                        FlatAreaFrom = request.FlatAreaFrom,
                        FlatAreaTo = request.FlatAreaTo,
                        PriceFrom = request.PriceFrom,
                        PriceTo = request.PriceTo
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