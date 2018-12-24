﻿namespace Dhgms.AspNetCoreContrib.Fakes
{
    using System.Diagnostics.CodeAnalysis;
    using System.Security.Claims;
    using Dhgms.AspNetCoreContrib.Controllers;

    [ExcludeFromCodeCoverage]
    public class FakeCrudUpdateCommand : AuditableRequest<int, int>
    {
        public FakeCrudUpdateCommand(int requestDto, ClaimsPrincipal claimsPrincipal) : base(requestDto, claimsPrincipal)
        {
        }
    }
}
