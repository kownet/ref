using MediatR;
using Ref.Data.Repositories;
using Ref.Services.Features.Shared;
using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Ref.Services.Features.Queries.Filters
{
    public class FilterById
    {
        public class Query : IRequest<Result>
        {
            public Query(string guid, int filterId)
            {
                Guid = guid;
                FilterId = filterId;
            }

            public string Guid { get; private set; }
            public int FilterId { get; private set; }
        }

        public class Result : BaseResult
        {
            public FilterResult Filter { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result>
        {
            private readonly IFilterRepository _filterRepository;
            private readonly IUserRepository _userRepository;
            private readonly ICitiesRepository _citiesRepository;

            public Handler(
                IFilterRepository filterRepository,
                IUserRepository userRepository,
                ICitiesRepository citiesRepository)
            {
                _filterRepository = filterRepository;
                _userRepository = userRepository;
                _citiesRepository = citiesRepository;
            }

            public async Task<Result> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    var user = (await _userRepository.FindByAsync(u => u.Guid == request.Guid))
                        .FirstOrDefault();

                    if (!(user is null))
                    {
                        var filter = (await _filterRepository.FindByAsync(f => f.Id == request.FilterId && f.UserId == user.Id))
                        .FirstOrDefault();

                        if (!(filter is null))
                        {
                            var city = (await _citiesRepository.FindByAsync(f => f.Id == filter.CityId))
                                .FirstOrDefault();

                            return new Result
                            {
                                Filter = new FilterResult
                                {
                                    Id = filter.Id,
                                    CityId = filter.CityId,
                                    FlatAreaFrom = filter.FlatAreaFrom,
                                    FlatAreaTo = filter.FlatAreaTo,
                                    PriceFrom = filter.PriceFrom,
                                    PriceTo = filter.PriceTo,
                                    UserId = filter.UserId,
                                    Name = WebUtility.HtmlDecode(filter.Name),
                                    Notification = filter.Notification,
                                    Property = filter.Property,
                                    ShouldContain = WebUtility.HtmlDecode(filter.ShouldContain),
                                    ShouldNotContain = WebUtility.HtmlDecode(filter.ShouldNotContain),
                                    PricePerMeterFrom = filter.PricePerMeterFrom,
                                    PricePerMeterTo = filter.PricePerMeterTo,
                                    DistrictId = filter.DistrictId,
                                    HasDistricts = city.HasDistricts,
                                    AllowFromAgency = filter.AllowFromAgency,
                                    AllowPrivate = filter.AllowPrivate
                                }
                            };
                        }
                        else
                        {
                            return new Result { Message = "Nie ma takiego filtra" };
                        }
                    }
                    else
                    {
                        return new Result { Message = "Nie ma takiego użytkownika" };
                    }
                }
                catch (Exception ex)
                {
                    return new Result { Message = ex.Message };
                }
            }
        }
    }
}