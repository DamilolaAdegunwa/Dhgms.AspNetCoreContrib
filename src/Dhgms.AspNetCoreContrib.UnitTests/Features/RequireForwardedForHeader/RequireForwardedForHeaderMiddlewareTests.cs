﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Dhgms.AspNetCoreContrib.App.Features.RequireForwardedForHeader;
using Microsoft.AspNetCore.Http;
using Xunit;
using Xunit.Abstractions;

namespace Dhgms.AspNetCoreContrib.UnitTests.Features.RequireForwardedForHeader
{
    /// <summary>
    /// Unit Tests for the RequireForwardedForHeaderMiddleware
    /// </summary>
    public static class RequireForwardedForHeaderMiddlewareTests
    {
        /// <summary>
        /// Unit tests for the constructor method.
        /// </summary>
        public sealed class ConstructorMethod : Foundatio.Logging.Xunit.TestWithLoggingBase
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ConstructorMethod"/> class.
            /// </summary>
            /// <param name="output">XUnit Test Output helper.</param>
            public ConstructorMethod(ITestOutputHelper output) : base(output)
            {
            }

            /// <summary>
            /// Test to ensure an Argument Null Exception is thrown.
            /// </summary>
            [Fact]
            public void ThrowsArgumentNullException()
            {
                var exception = Assert.Throws<ArgumentNullException>(() => new RequireForwardedForHeaderMiddleware(null));

                Assert.Equal("next", exception.ParamName);
            }

            /// <summary>
            /// Tests to ensure an instance is returned.
            /// </summary>
            [Fact]
            public void ReturnsInstance()
            {
                var instance = new RequireForwardedForHeaderMiddleware(Next);
                Assert.NotNull(instance);
            }
        }

        /// <summary>
        /// Unit Tests for the InvokeAsync method.
        /// </summary>
        public sealed class InvokeAsyncMethod
        {
            /// <summary>
            /// Test Data for the Rejection of Http Requests.
            /// </summary>
            public static IEnumerable<object[]> RejectsRequestTestData = GetRejectsRequestTestData();

            /// <summary>
            /// Tests to ensure the middleware rejects requests without
            /// X-Forwarded-... http headers.
            /// </summary>
            [Theory]
            [MemberData(nameof(RejectsRequestTestData))]
            public async Task RejectsRequest(
                HttpContext httpContext,
                WhipcordHttpStatusCode expectedHttpStatusCode)
            {
                var instance = new RequireForwardedForHeaderMiddleware(Next);
                await instance.InvokeAsync(httpContext)
                    .ConfigureAwait(false);

                Assert.Equal((int)expectedHttpStatusCode,
                        httpContext.Response.StatusCode);
            }

            private static IEnumerable<object[]> GetRejectsRequestTestData()
            {
                return new []
                {
                    GetXForwardedForRequestTestData(),
                    GetXForwardedProtoHttpMissingRequestTestData(),
                    GetXForwardedProtoHttpInsecureRequestTestData(),
                    GetXForwardedHostRequestTestData(),
                };
            }

            private static object[] GetXForwardedForRequestTestData()
            {
                return new object[]
                {
                    GetXForwardForHttpContext(),
                    WhipcordHttpStatusCode.ExpectedXForwardedFor,
                };
            }

            private static object[] GetXForwardedProtoHttpMissingRequestTestData()
            {
                return new object[]
                {
                    GetXForwardProtoHttpMissingProto(),
                    WhipcordHttpStatusCode.ExpectedXForwardedProto,
                };
            }

            private static object[] GetXForwardedProtoHttpInsecureRequestTestData()
            {
                return new object[]
                {
                    GetXForwardProtoHttpInsecureProto(),
                    WhipcordHttpStatusCode.ExpectedXForwardedProto,
                };
            }

            private static object[] GetXForwardedHostRequestTestData()
            {
                return new object[]
                {
                    GetXForwardHostHttpHost(),
                    WhipcordHttpStatusCode.ExpectedXForwardedHost,
                };
            }

            private static HttpContext GetXForwardForHttpContext()
            {
                var httpContext = new DefaultHttpContext();

                var headers = httpContext.Request.Headers;
                headers.Add("X-Forwarded-Proto", "https");
                headers.Add("X-Forwarded-Host", "localhost");

                return httpContext;
            }

            private static HttpContext GetXForwardProtoHttpMissingProto()
            {
                var httpContext = new DefaultHttpContext();

                var headers = httpContext.Request.Headers;
                headers.Add("X-Forwarded-For", "192.168.0.1");
                headers.Add("X-Forwarded-Host", "localhost");

                return httpContext;
            }

            private static HttpContext GetXForwardProtoHttpInsecureProto()
            {
                var httpContext = new DefaultHttpContext();

                var headers = httpContext.Request.Headers;
                headers.Add("X-Forwarded-For", "192.168.0.1");
                headers.Add("X-Forwarded-Proto", "http");
                headers.Add("X-Forwarded-Host", "localhost");

                return httpContext;
            }

            private static HttpContext GetXForwardHostHttpHost()
            {
                var httpContext = new DefaultHttpContext();

                var headers = httpContext.Request.Headers;
                headers.Add("X-Forwarded-For", "192.168.0.1");
                headers.Add("X-Forwarded-Proto", "https");

                return httpContext;
            }
        }

        private static Task Next(HttpContext _)
        {
            return Task.CompletedTask;
        }
    }
}
