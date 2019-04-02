using MediatR;
using Ref.Data.Models;
using Ref.Data.Repositories;
using Ref.Services.Features.Shared;
using Ref.Services.Helpers;
using System;
using System.Linq;
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
            public string ShouldContain { get; set; }
            public string ShouldNotContain { get; set; }
        }

        public class Result : BaseResult { }

        public class Handler : IRequestHandler<Cmd, Result>
        {
            private readonly IFilterRepository _filterRepository;
            private readonly IUserRepository _userRepository;

            public Handler(
                IFilterRepository filterRepository,
                IUserRepository userRepository)
            {
                _filterRepository = filterRepository;
                _userRepository = userRepository;
            }

            public async Task<Result> Handle(Cmd request, CancellationToken cancellationToken)
            {
                var noFilterByUser = 0;
                var maximumForCurrentSubscription = 0;

                try
                {
                    var user = await _userRepository.GetAsync(request.UserId);

                    noFilterByUser = (await _filterRepository.FindByAsync(f => f.UserId == request.UserId)).Count();

                    maximumForCurrentSubscription = SubscriptionProvider.MaxNumberOfFilters(user.Subscription);
                }
                catch (Exception ex)
                {
                    return new Result { Message = ex.Message };
                }

                if (noFilterByUser <= maximumForCurrentSubscription)
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
                            Name = request.Name,
                            LastCheckedAt = DateTime.Now,
                            ShouldContain = request.ShouldContain.ToLowerInvariant(),
                            ShouldNotContain = request.ShouldNotContain.ToLowerInvariant()
                        });

                        return new Result();
                    }
                    catch (Exception ex)
                    {
                        return new Result { Message = ex.Message };
                    }
                }
                else
                {
                    return new Result
                    {
                        Message = $"Maximum number of filters - {maximumForCurrentSubscription + 1} has been exceeded."
                    };
                }
            }
        }
    }
}