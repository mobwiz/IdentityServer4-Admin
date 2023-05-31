// Copyright (c) Luther R.D. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using FluentAssertions;
using IdentityServer4.Storage.FreeSql.Services;
using Microsoft.Extensions.DependencyInjection;
using NetTopologySuite.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityServer4.Storage.FreeSql.Tests.ServiceTest
{
    [Collection("DatabaseIntergationTest")]
    public class DbClientServiceTest : IClassFixture<ServerFixture>
    {


        private const string TESTCLIENTID = "client1";
        private const string TESTCLIENTNAME = "Test Client 1";
        private const string TESTCLIENTDESCRIPTION = "This client is used for client";
        private const string TESTCLIENTURI = "http://test90809219203.com";
        private const byte TEST_ENABLED = 1;
        private const byte TEST_DISABLED = 0;
        private const byte TESTDISPLAYORDER = 1;
        private readonly string TESTLOGOURI = "http://logo";
        private const string TESTFRONTCHANNELLOGOUTURI = "http://logoutfront1";
        private const string TESTBACKCHANNELLOGOUTURI = "http://logoutfront2";
        private readonly string[] TESTALLOWEDSCOPES = new[] { "openid", "profile" };
        private readonly string[] TESTALLOWEDGRANTTYPES = new[] { "authorization_code", "password" };
        private readonly string[] TESTALLOWEDCORSORIGINS = new[] { "http://cors1.com" };
        private readonly string[] TESTREDIRECTURIS = new[] { "http://redirect1", "http://redirect2" };
        private readonly string[] TESTPOSTLOGOUTREDIRECTURIS = new[] { "http://logouturi1", "http://lgouturi2" };
        private readonly string[] TESTCLIENTSECRET = new[] { "secret1", "Secret3" };

        private readonly ServerFixture _fixture;

        public DbClientServiceTest(ServerFixture fixture)
        {
            _fixture = fixture;
        }

        private async Task CreateTestObj(IDbClientService service)
        {
            var result = await service.CreateClientAsync(new Services.Requests.CreateClientRequest
            {
                Operator = "user1",
                Client = new Services.Dto.ClientDto
                {
                    ClientId = TESTCLIENTID,
                    ClientName = TESTCLIENTNAME,
                    ClientDescription = TESTCLIENTDESCRIPTION,
                    ClientUri = TESTCLIENTURI,
                    Enabled = TEST_ENABLED,
                    AllowedGrantTypes = TESTALLOWEDGRANTTYPES,
                    AllowedScopes = TESTALLOWEDSCOPES,
                    DisplayOrder = TESTDISPLAYORDER,
                    AllowedCorsOrigins = TESTALLOWEDCORSORIGINS,
                    LogoUri = TESTLOGOURI,
                    RedirectUris = TESTREDIRECTURIS,
                    PostLogoutRedirectUris = TESTPOSTLOGOUTREDIRECTURIS,
                    FrontChannelLogoutUri = TESTFRONTCHANNELLOGOUTURI,
                    BackChannelLogoutUri = TESTBACKCHANNELLOGOUTURI,
                    ClientSecrets = TESTCLIENTSECRET,
                    AccessTokenType = Types.EAccessTokenType.Reference,
                    AllowOfflineAccess = TEST_ENABLED,
                    AllowRememberConsent = TEST_ENABLED,
                    RequirePkce = TEST_ENABLED,
                    RequireConsent = TEST_ENABLED,
                    TokenLifetime = 3600,
                    RequireClientSecret = TEST_ENABLED,
                }
            });
        }
        private async Task CreateTestObj2(IDbClientService service, int index)
        {
            var result = await service.CreateClientAsync(new Services.Requests.CreateClientRequest
            {
                Operator = "user1",
                Client = new Services.Dto.ClientDto
                {
                    ClientId = TESTCLIENTID + index,
                    ClientName = TESTCLIENTNAME,
                    ClientDescription = TESTCLIENTDESCRIPTION + index,
                    ClientUri = TESTCLIENTURI + index,
                    Enabled = (byte)(index % 2),
                    DisplayOrder = TESTDISPLAYORDER,
                    LogoUri = TESTLOGOURI,
                    FrontChannelLogoutUri = TESTFRONTCHANNELLOGOUTURI,
                    BackChannelLogoutUri = TESTBACKCHANNELLOGOUTURI,
                    AccessTokenType = Types.EAccessTokenType.Reference,
                    AllowOfflineAccess = (byte)(index % 2),
                    AllowRememberConsent = (byte)(index % 2),
                    RequirePkce = (byte)(index % 2),
                    RequireConsent = (byte)(index % 2),
                    TokenLifetime = 3600,
                    RequireClientSecret = (byte)(index % 2),


                    ClientSecrets = TESTCLIENTSECRET.Select(p => p + $"_{index}").ToArray(),
                    AllowedGrantTypes = TESTALLOWEDGRANTTYPES.Select(p => p + $"_{index}").ToArray(),
                    AllowedScopes = TESTALLOWEDSCOPES.Select(p => p + $"_{index}").ToArray(),
                    AllowedCorsOrigins = TESTALLOWEDCORSORIGINS.Select(p => p + $"_{index}").ToArray(),
                    PostLogoutRedirectUris = TESTPOSTLOGOUTREDIRECTURIS.Select(p => p + $"_{index}").ToArray(),
                    RedirectUris = TESTREDIRECTURIS.Select(p => p + $"_{index}").ToArray(),
                }
            });
        }

        [Fact]
        public async Task CreateClient_Should_Ok()
        {
            // arrange
            _fixture.CleanDb();
            var service = _fixture.ServiceProvider.GetRequiredService<IDbClientService>();

            // act
            var result = await service.CreateClientAsync(new Services.Requests.CreateClientRequest
            {
                Operator = "user1",
                Client = new Services.Dto.ClientDto
                {
                    ClientId = TESTCLIENTID,
                    ClientName = TESTCLIENTNAME,
                    ClientDescription = TESTCLIENTDESCRIPTION,
                    ClientUri = TESTCLIENTURI,
                    Enabled = TEST_ENABLED,
                    AllowedGrantTypes = TESTALLOWEDGRANTTYPES,
                    AllowedScopes = TESTALLOWEDSCOPES,
                    DisplayOrder = TESTDISPLAYORDER,
                    AllowedCorsOrigins = TESTALLOWEDCORSORIGINS,
                    LogoUri = TESTLOGOURI,
                    RedirectUris = TESTREDIRECTURIS,
                    PostLogoutRedirectUris = TESTPOSTLOGOUTREDIRECTURIS,
                    FrontChannelLogoutUri = TESTFRONTCHANNELLOGOUTURI,
                    BackChannelLogoutUri = TESTBACKCHANNELLOGOUTURI,
                }
            });

            // assert
            result.Should().Be(1);
        }

        [Fact]
        public async Task GetClientByIdAsync_Should_Ok()
        {
            // arrange
            _fixture.CleanDb();
            var service = _fixture.ServiceProvider.GetRequiredService<IDbClientService>();

            // act
            await CreateTestObj(service);


            var obj = await service.GetClientByClientIdAsync(new Services.Requests.GetClientByClientIdRequest { ClientId = TESTCLIENTID });

            // assert
            obj.Should().NotBeNull();
            obj.ClientId.Should().Be(TESTCLIENTID);
            obj.ClientName.Should().Be(TESTCLIENTNAME);
            obj.DisplayOrder.Should().Be(TESTDISPLAYORDER);
            obj.ClientDescription.Should().Be(TESTCLIENTDESCRIPTION);
            obj.Enabled.Should().Be(TEST_ENABLED);
            obj.LogoUri.Should().Be(TESTLOGOURI);
            obj.FrontChannelLogoutUri.Should().Be(TESTFRONTCHANNELLOGOUTURI);
            obj.BackChannelLogoutUri.Should().Be(TESTBACKCHANNELLOGOUTURI);
            obj.RequireClientSecret.Should().Be(TEST_ENABLED);
            obj.RequirePkce.Should().Be(TEST_ENABLED);
            obj.RequireConsent.Should().Be(TEST_ENABLED);
            obj.AllowOfflineAccess.Should().Be(TEST_ENABLED);
            obj.TokenLifetime.Should().Be(3600);
            obj.AccessTokenType.Should().Be(Types.EAccessTokenType.Reference);

            obj.AllowedGrantTypes.Should().HaveCount(TESTALLOWEDGRANTTYPES.Length).And.BeEquivalentTo(TESTALLOWEDGRANTTYPES);
            obj.AllowedScopes.Should().HaveCount(TESTALLOWEDSCOPES.Length).And.BeEquivalentTo(TESTALLOWEDSCOPES);
            obj.AllowedCorsOrigins.Should().HaveCount(TESTALLOWEDCORSORIGINS.Length).And.BeEquivalentTo(TESTALLOWEDCORSORIGINS);
            obj.RedirectUris.Should().HaveCount(TESTREDIRECTURIS.Length).And.BeEquivalentTo(TESTREDIRECTURIS);
            obj.PostLogoutRedirectUris.Should().HaveCount(TESTPOSTLOGOUTREDIRECTURIS.Length).And.BeEquivalentTo(TESTPOSTLOGOUTREDIRECTURIS);
            obj.ClientSecrets.Should().HaveCount(TESTCLIENTSECRET.Length).And.BeEquivalentTo(TESTCLIENTSECRET);
        }

        [Fact]
        public async Task RemoveClient_Should_Ok()
        {
            // arrange
            _fixture.CleanDb();
            var service = _fixture.ServiceProvider.GetRequiredService<IDbClientService>();

            // act
            await CreateTestObj(service);

            var obj = await service.GetClientByIdAsync(new Services.Requests.GetClientByIdRequest { Id = 1 });

            obj.Should().NotBeNull();

            await service.RemoveClientAsync(new Services.Requests.RemoveClientRequest { Id = 1 });

            obj = await service.GetClientByIdAsync(new Services.Requests.GetClientByIdRequest { Id = 1 });

            obj.Should().BeNull();
        }

        [Fact]
        public async Task QueryClients_Should_Ok()
        {
            _fixture.CleanDb();

            var service = _fixture.ServiceProvider.GetRequiredService<IDbClientService>();

            for (var i = 0; i < 15; i++)
            {
                await CreateTestObj2(service, i);
            }

            // 1
            var query1 = await service.QueryClientsAsync(new Services.Requests.QueryClientsRequest
            {
                PageIndex = 1,
                PageSize = 9
            });

            query1.Items.Should().HaveCount(9);
            query1.TotalCount.Should().Be(15);

            // 2
            query1 = await service.QueryClientsAsync(new Services.Requests.QueryClientsRequest
            {
                PageIndex = 2,
                PageSize = 9
            });

            query1.Items.Should().HaveCount(6);
            query1.TotalCount.Should().Be(15);


            // 3
            query1 = await service.QueryClientsAsync(new Services.Requests.QueryClientsRequest
            {
                Enabled = Mobwiz.Common.BaseTypes.BoolCondition.True,
                PageIndex = 1,
                PageSize = 9
            });

            query1.Items.Should().HaveCount(7);
            query1.TotalCount.Should().Be(7);

            // 4
            // 3
            query1 = await service.QueryClientsAsync(new Services.Requests.QueryClientsRequest
            {
                Keyword = "client11",
                Enabled = Mobwiz.Common.BaseTypes.BoolCondition.True,
                PageIndex = 1,
                PageSize = 9
            });

            query1.Items.Should().HaveCount(3);
            query1.TotalCount.Should().Be(3);

        }

        [Fact]
        public async Task GetCorsOrigins_Should_Ok()
        {
            _fixture.CleanDb();

            var service = _fixture.ServiceProvider.GetRequiredService<IDbClientService>();

            for (var i = 0; i < 15; i++)
            {
                await CreateTestObj2(service, i);
            }

            var corsList = await service.GetCorsOriginsAsync();

            corsList.Should().HaveCount(7);
        }

        [Fact]
        public async Task GetRedirectUris_Should_Ok()
        {
            _fixture.CleanDb();

            var service = _fixture.ServiceProvider.GetRequiredService<IDbClientService>();

            for (var i = 0; i < 15; i++)
            {
                await CreateTestObj2(service, i);
            }

            var corsList = await service.GetValidRedirectUrisAsync();

            corsList.Should().HaveCount(14);
        }

        [Fact]
        public async Task GetPostLogoutUris_Should_Ok()
        {
            _fixture.CleanDb();

            var service = _fixture.ServiceProvider.GetRequiredService<IDbClientService>();

            for (var i = 0; i < 15; i++)
            {
                await CreateTestObj2(service, i);
            }

            var corsList = await service.GetValidPostLogoutUrisAsync();

            corsList.Should().HaveCount(14);
        }
    }
}
