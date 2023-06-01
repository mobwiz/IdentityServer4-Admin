// Copyright (c) Luther R.D. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using FluentAssertions;
using IdentityServer4.Storage.FreeSql.Services;
using Microsoft.Extensions.DependencyInjection;
using Mobwiz.Common.Exceptions;
using NetTaste;

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
        private const int TESTTOKENLIFETIME = 3600;

        // test data 2
        private const string TESTCLIENTID2 = "client2";
        private const string TESTCLIENTNAME2 = "Test Client 2";
        private const string TESTCLIENTDESCRIPTION2 = "This client is used for client 2";
        private const string TESTCLIENTURI2 = "http://test9080921920322222.com";
        private const byte TEST_ENABLED2 = 1;
        private const byte TEST_DISABLED2 = 0;
        private const byte TESTDISPLAYORDER2 = 1;
        private readonly string TESTLOGOURI2 = "http://logo2";
        private const string TESTFRONTCHANNELLOGOUTURI2 = "http://logoutfront1_2";
        private const string TESTBACKCHANNELLOGOUTURI2 = "http://logoutfront2_2";
        private readonly string[] TESTALLOWEDSCOPES2 = new[] { "openid", "profile", "test2" };
        private readonly string[] TESTALLOWEDGRANTTYPES2 = new[] { "client_credential" };
        private readonly string[] TESTALLOWEDCORSORIGINS2 = new[] { "http://cors1.com_2" };
        private readonly string[] TESTREDIRECTURIS2 = new[] { "http://redirect1_2", "http://redirect2_2" };
        private readonly string[] TESTPOSTLOGOUTREDIRECTURIS2 = new[] { "http://logouturi1_2", "http://lgouturi2_2" };
        private readonly string[] TESTCLIENTSECRET2 = new[] { "secret1_2", "Secret3_2" };
        private const int TESTTOKENLIFETIME2 = 234;

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
                    TokenLifetime = TESTTOKENLIFETIME,
                    RequireClientSecret = TEST_ENABLED,
                }
            });
        }
        private async Task<long> CreateTestObj2(IDbClientService service, int index)
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
                    TokenLifetime = TESTTOKENLIFETIME,
                    RequireClientSecret = (byte)(index % 2),


                    ClientSecrets = TESTCLIENTSECRET.Select(p => p + $"_{index}").ToArray(),
                    AllowedGrantTypes = TESTALLOWEDGRANTTYPES.Select(p => p + $"_{index}").ToArray(),
                    AllowedScopes = TESTALLOWEDSCOPES.Select(p => p + $"_{index}").ToArray(),
                    AllowedCorsOrigins = TESTALLOWEDCORSORIGINS.Select(p => p + $"_{index}").ToArray(),
                    PostLogoutRedirectUris = TESTPOSTLOGOUTREDIRECTURIS.Select(p => p + $"_{index}").ToArray(),
                    RedirectUris = TESTREDIRECTURIS.Select(p => p + $"_{index}").ToArray(),
                }
            });

            return result;
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
                    TokenLifetime = TESTTOKENLIFETIME,
                    AccessTokenType = Types.EAccessTokenType.Jwt,
                    AllowOfflineAccess = TEST_ENABLED,
                    AllowRememberConsent = TEST_DISABLED,
                    RequirePkce = TEST_ENABLED,
                    RequireConsent = TEST_ENABLED,
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
            obj.TokenLifetime.Should().Be(TESTTOKENLIFETIME);
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

        [Fact]
        public async Task Create_WithNullRequest_Should_ThrowException()
        {
            var service = _fixture.ServiceProvider.GetRequiredService<IDbClientService>();

            await Assert.ThrowsAsync<ArgumentNullException>(async () => await service.CreateClientAsync(null));
        }

        [Fact]
        public async Task Create_WithNullDto_Should_ThrowException2()
        {
            var service = _fixture.ServiceProvider.GetRequiredService<IDbClientService>();

            await Assert.ThrowsAsync<ArgumentNullException>(async () => await service.CreateClientAsync(new Services.Requests.CreateClientRequest
            {
                Client = null
            }));
        }

        [Fact]
        public async Task Create_With_EmptyClientId_Should_ThrowException2()
        {
            var service = _fixture.ServiceProvider.GetRequiredService<IDbClientService>();

            await Assert.ThrowsAsync<ArgumentNullException>(async () => await service.CreateClientAsync(new Services.Requests.CreateClientRequest
            {
                Client = new Services.Dto.ClientDto
                {
                    ClientId = ""
                }
            }));
        }

        [Fact]
        public async Task Update_WithNullRequset_Should_ThrowException()
        {
            var service = _fixture.ServiceProvider.GetRequiredService<IDbClientService>();

            await Assert.ThrowsAsync<ArgumentNullException>(async () => await service.UpdateClientAsync(null));
        }

        [Fact]
        public async Task Update_WithNullDto_Should_ThrowException2()
        {
            var service = _fixture.ServiceProvider.GetRequiredService<IDbClientService>();

            await Assert.ThrowsAsync<ArgumentNullException>(async () => await service.UpdateClientAsync(new Services.Requests.UpdateClientRequest
            {
                Client = null
            }));
        }

        [Fact]
        public async Task Update_With_EmptyClientId_Should_ThrowException2()
        {
            var service = _fixture.ServiceProvider.GetRequiredService<IDbClientService>();

            await Assert.ThrowsAsync<ArgumentNullException>(async () => await service.UpdateClientAsync(new Services.Requests.UpdateClientRequest
            {
                Client = new Services.Dto.ClientDto
                {
                    ClientId = ""
                }
            }));
        }

        [Fact]
        public async Task Update_With_ZeroId_Should_ThrowException()
        {
            var service = _fixture.ServiceProvider.GetRequiredService<IDbClientService>();

            await Assert.ThrowsAsync<ArgumentNullException>(async () => await service.UpdateClientAsync(new Services.Requests.UpdateClientRequest
            {
                Client = new Services.Dto.ClientDto
                {
                    Id = 0,
                    ClientId = "xxxx"
                }
            }));
        }

        [Fact]
        public async Task Update_Should_OK()
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
                    AccessTokenType = Types.EAccessTokenType.Jwt
                }
            });

            // assert
            result.Should().Be(1);

            var client = await service.GetClientByClientIdAsync(new Services.Requests.GetClientByClientIdRequest
            {
                ClientId = TESTCLIENTID
            });

            client.Should().NotBeNull();

            client.ClientName = TESTCLIENTNAME2;
            client.ClientDescription = TESTCLIENTDESCRIPTION2;
            client.ClientUri = TESTCLIENTURI2;
            client.Enabled = TEST_ENABLED2;
            client.AllowedGrantTypes = TESTALLOWEDGRANTTYPES2;
            client.AllowedScopes = TESTALLOWEDSCOPES2;
            client.DisplayOrder = TESTDISPLAYORDER2;
            client.AllowedCorsOrigins = TESTALLOWEDCORSORIGINS2;
            client.LogoUri = TESTLOGOURI2;
            client.RedirectUris = TESTREDIRECTURIS2;
            client.PostLogoutRedirectUris = TESTPOSTLOGOUTREDIRECTURIS2;
            client.FrontChannelLogoutUri = TESTFRONTCHANNELLOGOUTURI2;
            client.BackChannelLogoutUri = TESTBACKCHANNELLOGOUTURI2;
            client.TokenLifetime = TESTTOKENLIFETIME2;
            client.AccessTokenType = Types.EAccessTokenType.Reference;
            client.ClientId = TESTCLIENTID2;
            client.RequireClientSecret = TEST_ENABLED;
            client.RequirePkce = TEST_ENABLED;
            client.RequireConsent = TEST_ENABLED;
            client.AllowOfflineAccess = TEST_ENABLED;
            client.ClientSecrets = TESTCLIENTSECRET2;

            await service.UpdateClientAsync(new Services.Requests.UpdateClientRequest { Client = client });


            var client2 = await service.GetClientByClientIdAsync(new Services.Requests.GetClientByClientIdRequest
            {
                ClientId = TESTCLIENTID2
            });

            // assert
            client2.Should().NotBeNull();
            client2.ClientId.Should().Be(TESTCLIENTID2);
            client2.ClientName.Should().Be(TESTCLIENTNAME2);
            client2.DisplayOrder.Should().Be(TESTDISPLAYORDER2);
            client2.ClientDescription.Should().Be(TESTCLIENTDESCRIPTION2);
            client2.Enabled.Should().Be(TEST_ENABLED2);
            client2.LogoUri.Should().Be(TESTLOGOURI2);
            client2.FrontChannelLogoutUri.Should().Be(TESTFRONTCHANNELLOGOUTURI2);
            client2.BackChannelLogoutUri.Should().Be(TESTBACKCHANNELLOGOUTURI2);
            client2.RequireClientSecret.Should().Be(TEST_ENABLED);
            client2.RequirePkce.Should().Be(TEST_ENABLED);
            client2.RequireConsent.Should().Be(TEST_ENABLED);
            client2.AllowOfflineAccess.Should().Be(TEST_ENABLED);
            client2.TokenLifetime.Should().Be(TESTTOKENLIFETIME2);
            client2.AccessTokenType.Should().Be(Types.EAccessTokenType.Reference);

            client2.AllowedGrantTypes.Should().HaveCount(TESTALLOWEDGRANTTYPES2.Length).And.BeEquivalentTo(TESTALLOWEDGRANTTYPES2);
            client2.AllowedScopes.Should().HaveCount(TESTALLOWEDSCOPES2.Length).And.BeEquivalentTo(TESTALLOWEDSCOPES2);
            client2.AllowedCorsOrigins.Should().HaveCount(TESTALLOWEDCORSORIGINS2.Length).And.BeEquivalentTo(TESTALLOWEDCORSORIGINS2);
            client2.RedirectUris.Should().HaveCount(TESTREDIRECTURIS2.Length).And.BeEquivalentTo(TESTREDIRECTURIS2);
            client2.PostLogoutRedirectUris.Should().HaveCount(TESTPOSTLOGOUTREDIRECTURIS2.Length).And.BeEquivalentTo(TESTPOSTLOGOUTREDIRECTURIS2);
            client2.ClientSecrets.Should().HaveCount(TESTCLIENTSECRET2.Length).And.BeEquivalentTo(TESTCLIENTSECRET2);


        }

        [Fact]
        public async Task Update_WithOtherSameName_Should_ThrowExecption()
        {
            _fixture.CleanDb();
            var service = _fixture.ServiceProvider.GetRequiredService<IDbClientService>();

            // act
            var id1 = await CreateTestObj2(service, 1);
            var id2 = await CreateTestObj2(service, 2);

            var client = await service.GetClientByIdAsync(new Services.Requests.GetClientByIdRequest { Id = id2 });

            client.ClientId = TESTCLIENTID + 1; // same to object 1

            await Assert.ThrowsAsync<BllException>(() => service.UpdateClientAsync(new Services.Requests.UpdateClientRequest
            {
                Client = client,
                Operator = "1111"
            }));


        }
    }
}
