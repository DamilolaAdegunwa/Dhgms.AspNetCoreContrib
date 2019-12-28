﻿// Copyright (c) 2019 DHGMS Solutions and Contributors. All rights reserved.
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using MediatR.Pipeline;
using Microsoft.Extensions.DependencyInjection;

namespace Dhgms.AspNetCoreContrib.App.Features.Mediatr
{
    /// <summary>
    /// Registers a concrete type for MediatR pre processors.
    /// </summary>
    /// <typeparam name="TImplementationType">The type for the request handler.</typeparam>
    /// <typeparam name="TRequest">The type for the mediatr request.</typeparam>
    public sealed class RequestPreProcessorRegistrationHandler<TImplementationType, TRequest>
        : IRequestPreProcessorRegistrationHandler
        where TImplementationType : class, IRequestPreProcessor<TRequest>
    {
        /// <inheritdoc/>
        public void AddRequestPreProcessor(IServiceCollection services)
        {
            services.AddTransient(
                typeof(IRequestPreProcessor<TRequest>),
                typeof(TImplementationType));
        }

        public Type GetRegistrationType => typeof(TImplementationType);
    }
}
