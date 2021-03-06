// Copyright (c) 2019 DHGMS Solutions and Contributors. All rights reserved.
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Dhgms.AspNetCoreContrib.App.Features.Mediatr;
using Dhgms.AspNetCoreContrib.Fakes;
using Dhgms.AspNetCoreContrib.Fakes.EntityFramework;
using Dhgms.AspNetCoreContrib.Fakes.MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RimDev.Stuntman.Core;

namespace Dhgms.AspNetCoreContrib.Examples.WebMvcApp
{
    /// <summary>
    /// Start up logic for the sample Web MVC app.
    /// </summary>
    public class Startup : Dhgms.AspNetCoreContrib.App.BaseStartup
    {
        private readonly StuntmanOptions _stuntmanOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="configuration">Application configuration.</param>
        public Startup(IConfiguration configuration)
            : base(configuration, false)
        {
            _stuntmanOptions = new StuntmanOptions();
        }

        /// <inheritdoc />
        protected override void OnConfigureServices(IServiceCollection services)
        {
            services.AddStuntman(_stuntmanOptions);
            var databaseName = Guid.NewGuid().ToString();
            services.AddTransient(_ => new DbContextOptionsBuilder<FakeDbContext>()
                .UseInMemoryDatabase(databaseName: databaseName)
                .Options);
        }

        /// <inheritdoc />
        protected override void OnConfigure(
            IApplicationBuilder app,
            IWebHostEnvironment env,
            ILoggerFactory loggerFactory)
        {
            app.UseStuntman(_stuntmanOptions);
        }

        /// <inheritdoc />
        protected override Assembly[] GetControllerAssemblies()
        {
            return new[]
            {
                typeof(FakeCrudController).Assembly,
            };
        }

        /// <inheritdoc />
        protected override IMediatrRegistration GetMediatrRegistration()
        {
            return new FakeMediatrRegistration();
        }
    }
}
