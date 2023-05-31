// Copyright (c) Luther R.D. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using FluentAssertions;
using IdentityServer4.Admin.WebApi.Controllers.Account;
using IdentityServer4.Admin.WebApi.Utils;
using Mobwiz.Common.BaseTypes;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Ubiety.Dns.Core;

namespace IdentityServer4.Admin.WebApi.Tests.Account
{
    public class AccountControllerTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public AccountControllerTest(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task WebLogin_With_WrongUsername_Should_Fail()
        {
            // arrange
            var client = _factory.CreateClient();

            // act
            var response = await client.PostAsJsonAsync("/api/admin/account/webLogin", new LoginRequest
            {
                Password = "password",
                Username = "username",
                ValidateCode = ""
            });

            // 
            response.EnsureSuccessStatusCode();

            var strResult = await response.Content.ReadAsStringAsync();

            var result = JsonConvert.DeserializeObject<BaseResult>(strResult);

            result.Should().NotBeNull();
            result.Code.Should().Be(400400);
        }

        [Fact]
        public async Task WebLogin_With_WrongUsername_Should_BeLocked()
        {
            // arrange
            var client = _factory.CreateClient();

            var securityChecker = _factory.Services.GetRequiredService<SecurityChecker>();
            await securityChecker.ClearFailedLogin("username");

            var securityOpt = _factory.Services.GetRequiredService<IOptions<SecurityOptions>>();
            var securityOptValue = securityOpt.Value;
            // act

            for (var i = 0; i < securityOptValue.MaxTry; i++)
            {
                var response = await client.PostAsJsonAsync("/api/admin/account/webLogin", new LoginRequest
                {
                    Password = "password",
                    Username = "username",
                    ValidateCode = ""
                });
                response.EnsureSuccessStatusCode();
            }

            // 
            {
                var response = await client.PostAsJsonAsync("/api/admin/account/webLogin", new LoginRequest
                {
                    Password = "password",
                    Username = "username",
                    ValidateCode = ""
                });

                var strResult = await response.Content.ReadAsStringAsync();

                var result = JsonConvert.DeserializeObject<BaseResult>(strResult);

                result.Should().NotBeNull();
                result.Code.Should().Be(400402);
            }
        }


        [Fact]
        public async Task WebLogin_With_CorrectUsernamePasword_Should_Ok()
        {
            // arrange
            var client = _factory.CreateClient();

            var securityChecker = _factory.Services.GetRequiredService<SecurityChecker>();
            await securityChecker.ClearFailedLogin("username");


            var response = await client.PostAsJsonAsync("/api/admin/account/webLogin", new LoginRequest
            {
                Password = "123456",
                Username = "admin",
                ValidateCode = ""
            });

            response.EnsureSuccessStatusCode();

            var strResult = await response.Content.ReadAsStringAsync();

            var result = JsonConvert.DeserializeObject<BaseResult>(strResult);

            result.Should().NotBeNull();
            result.Code.Should().Be(0);


            var result2 = await client.GetFromJsonAsync<BaseResult>("/api/admin/identityResource/list");

            result2.Should().NotBeNull();
            result2.Code.Should().Be(0);
        }


        [Fact]
        public async Task GetUserInfo_WithoutLogin_Should_Return401()
        {
            // arrange
            var client = _factory.CreateClient();
            var response = await client.GetAsync("/api/admin/account/getUserInfo");
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
        }


        [Fact]
        public async Task GetUserInfo_WithLogin_Should_Return200()
        {
            // arrange

            var client = _factory.CreateClient();

            var securityChecker = _factory.Services.GetRequiredService<SecurityChecker>();
            await securityChecker.ClearFailedLogin("username");


            var response = await client.PostAsJsonAsync("/api/admin/account/webLogin", new LoginRequest
            {
                Password = "123456",
                Username = "admin",
                ValidateCode = ""
            });

            response.EnsureSuccessStatusCode();

            var strResult = await response.Content.ReadAsStringAsync();

            var result = JsonConvert.DeserializeObject<BaseResult>(strResult);

            result.Should().NotBeNull();
            result.Code.Should().Be(0);



            var response2 = await client.GetAsync("/api/admin/account/getUserInfo");
            response2.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

            var strResult2 = await response2.Content.ReadAsStringAsync();


        }
    }
}
