﻿namespace Dhgms.AspNetCoreContrib.Controllers
{
    using System.Threading;
    using System.Threading.Tasks;
    using Abstractions;
    using MediatR;
    using System;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    public abstract class QueryOnlyController<TInheritingClass, TListRequestDto, TListQueryResponse, TListResponse, TViewQueryResponse, TViewResponse>
        : Controller
        where TInheritingClass : QueryOnlyController<TInheritingClass, TListRequestDto, TListQueryResponse, TListResponse, TViewQueryResponse, TViewResponse>
    {
        private readonly IAuditableQueryFactory<TListRequestDto, TListQueryResponse, TViewQueryResponse> _queryFactory;

        protected QueryOnlyController(
            IAuthorizationService authorizationService,
            ILogger<TInheritingClass> logger,
            IMediator mediator,
            IAuditableQueryFactory<TListRequestDto, TListQueryResponse, TViewQueryResponse> queryFactory)
        {
            AuthorizationService = authorizationService ??
                                         throw new ArgumentNullException(nameof(authorizationService));
            Logger = logger ??
                                         throw new ArgumentNullException(nameof(logger));

            Mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

            _queryFactory = queryFactory ??
                                            throw new ArgumentNullException(nameof(queryFactory));
        }

        protected IAuthorizationService AuthorizationService { get; }

        protected ILogger<TInheritingClass> Logger { get; }

        protected IMediator Mediator { get; }


        public async Task<IActionResult> ListAsync(
            [FromQuery]TListRequestDto requestDto,
            CancellationToken cancellationToken)
        {
            var eventId = await GetOnListEventIdAsync();
            Logger.LogDebug(eventId, "Entered ListAsync");

            var user = HttpContext.User;

            var methodAuthorization = await AuthorizationService.AuthorizeAsync(user, await GetListPolicyAsync());
            if (!methodAuthorization.Succeeded)
            {
                // not found rather than forbidden
                return NotFound();
            }

            var query = await _queryFactory.GetListQueryAsync(requestDto, user, cancellationToken).ConfigureAwait(false);
            var result = await Mediator.Send(query, cancellationToken).ConfigureAwait(false);

            var viewResult = await GetListActionResultAsync(result);
            Logger.LogDebug(eventId, "Finished ListAsync");

            return viewResult;
        }

        public async Task<IActionResult> ViewAsync(
            long id,
            CancellationToken cancellationToken)
        {
            var eventId = await GetOnViewEventIdAsync();
            Logger.LogDebug(eventId, "Entered ViewAsync");

            var user = HttpContext.User;

            if (id < 1)
            {
                return NotFound();
            }

            var methodAuthorization = await AuthorizationService.AuthorizeAsync(user, await GetViewPolicyAsync());
            if (!methodAuthorization.Succeeded)
            {
                // not found rather than forbidden
                return NotFound();
            }

            var query = await _queryFactory.GetViewQueryAsync(id, user, cancellationToken).ConfigureAwait(false);
            var result = await Mediator.Send(query, cancellationToken).ConfigureAwait(false);

            if (result == null)
            {
                return NotFound();
            }

            var resourceAuthorization = await AuthorizationService.AuthorizeAsync(user, result, await GetViewPolicyAsync());
            if (!resourceAuthorization.Succeeded)
            {
                // not found rather than forbidden
                return NotFound();
            }

            var viewResult = await GetViewActionResultAsync(result);
            Logger.LogDebug(eventId, "Finished ViewAsync");

            return viewResult;
        }

        protected abstract Task<EventId> GetOnListEventIdAsync();

        protected abstract Task<EventId> GetOnViewEventIdAsync();

        protected abstract Task<AuthorizationPolicy> GetListPolicyAsync();

        protected abstract Task<AuthorizationPolicy> GetViewPolicyAsync();

        protected abstract Task<IActionResult> GetListActionResultAsync(TListQueryResponse listResponse);

        protected abstract Task<IActionResult> GetViewActionResultAsync(TViewQueryResponse listResponse);
    }
}
