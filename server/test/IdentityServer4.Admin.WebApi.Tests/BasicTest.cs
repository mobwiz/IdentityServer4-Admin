// Copyright (c) Luther R.D. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace IdentityServer4.Admin.WebApi.Tests
{
    public class BasicTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public BasicTest(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Health_Check_Should_Ok()
        {
            // arrange
            var client = _factory.CreateClient();

            // act
            var response = await client.GetAsync("/health");

            // assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }

        [Fact]
        public async Task Authorize_Check_Should_Return401()
        {
            // arrange
            var client = _factory.CreateClient();

            // act
            var response = await client.GetAsync("/api/admin/identityResource/list");

            // assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
            //response.EnsureSuccessStatusCode();
        }
    }
}