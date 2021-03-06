﻿using MediatR;
using Ref.Data.Models;
using Ref.Data.Repositories;
using Ref.Services.Features.Shared;
using Ref.Services.Helpers;
using System;
using System.Linq;
using System.Net;
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
            public int? FlatAreaFrom { get; set; }
            public int? FlatAreaTo { get; set; }
            public int? PriceFrom { get; set; }
            public int? PriceTo { get; set; }
            public int? PricePerMeterFrom { get; set; }
            public int? PricePerMeterTo { get; set; }
            public NotificationType Notification { get; set; }
            public PropertyType Property { get; set; }
            public string Name { get; set; }
            public string ShouldContain { get; set; }
            public string ShouldNotContain { get; set; }
            public int? DistrictId { get; set; }
            public int AllowFromAgency { get; set; }
        }

        public class Result : BaseResult
        {
            public bool TooMuch { get; set; }
        }

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
                            Property = request.Property,
                            Deal = DealType.Sale,
                            Market = MarketType.Secondary,
                            CityId = request.CityId,
                            FlatAreaFrom = request.FlatAreaFrom,
                            FlatAreaTo = request.FlatAreaTo,
                            PriceFrom = request.PriceFrom,
                            PriceTo = request.PriceTo,
                            PricePerMeterFrom = request.PricePerMeterFrom,
                            PricePerMeterTo = request.PricePerMeterTo,
                            Notification = request.Notification,
                            Name = WebUtility.HtmlEncode(request.Name),
                            LastCheckedAt = DateTime.Now,
                            ShouldContain = request.ShouldContain is null ? string.Empty : WebUtility.HtmlEncode(request.ShouldContain.ToLowerInvariant()),
                            ShouldNotContain = request.ShouldNotContain is null ? string.Empty : WebUtility.HtmlEncode(request.ShouldNotContain.ToLowerInvariant()),
                            DistrictId = request.DistrictId,
                            AllowFromAgency = request.AllowFromAgency == 1 ? true : false
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
                    var s = maximumForCurrentSubscription + 1 > 0
                            ? $"{maximumForCurrentSubscription + 1} filtry"
                            : $"{maximumForCurrentSubscription + 1} filtr";

                    return new Result
                    {
                        Message = $"Możesz utworzyć - {s} dla Twojego planu.",
                        TooMuch = true
                    };
                }
            }
        }
    }
}