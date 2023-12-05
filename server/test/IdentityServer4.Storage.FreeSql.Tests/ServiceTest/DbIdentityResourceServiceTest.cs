// Copyright (c) Luther R.D. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using FluentAssertions;
using IdentityServer4.Storage.FreeSql.Services;
using IdentityServer4.Storage.FreeSql.Services.Dto;
using IdentityServer4.Storage.FreeSql.Services.Requests;
using Mobwiz.Common.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using NetTaste;

namespace IdentityServer4.Storage.FreeSql.Tests.ServiceTest
{
    [Collection("DatabaseIntergationTest")] // To avoid parallel test
    public class DbIdentityResourceServiceTest : IClassFixture<ServerFixture>
    {
        private readonly ServerFixture _fixture;

        public DbIdentityResourceServiceTest(ServerFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task Create_IdentityResource_Should_Ok()
        {
            _fixture.CleanDb();

            var service = _fixture.ServiceProvider.GetRequiredService<IDbIdentityResourceService>();

            var identityResource = new IdentityResourceDto
            {
                Id = 1,
                Enabled = 1,
                Emphasize = 0,
                Name = "Test",
                DisplayName = "Test Displayname",
                Claims = new string[] { "scope1" },
                Required = 1,
            };

            var id = await service.CreateIdentityResourceAsync(new CreateIdentityResourceRequest
            {
                Resource = identityResource,
                Operator = "user1"
            });

            //id.Should().Be(1);
        }

        [Fact]
        public async Task Create_IdentityResource_WithSamename_Should_Fail()
        {
            _fixture.CleanDb();

            var service = _fixture.ServiceProvider.GetRequiredService<IDbIdentityResourceService>();

            var identityResource = new IdentityResourceDto
            {
                Id = 1,
                Enabled = 1,
                Emphasize = 0,
                Name = "Test",
                DisplayName = "Test Displayname",
                Claims = new string[] { "scope1" },
                Required = 1,
            };

            var id = await service.CreateIdentityResourceAsync(new CreateIdentityResourceRequest
            {
                Resource = identityResource,
                Operator = "user1"
            });

            //id.Should().Be(1);

            await Assert.ThrowsAsync<BllException>(async () =>
            {

                identityResource = new IdentityResourceDto
                {
                    Id = 2,
                    Enabled = 1,
                    Emphasize = 0,
                    Name = "Test",
                    DisplayName = "Test Displayname",
                    Claims = new string[] { "scope1" },
                    Required = 1,
                };
                await service.CreateIdentityResourceAsync(new CreateIdentityResourceRequest
                {
                    Resource = identityResource,
                    Operator = "user1"
                });
            });
        }

        [Fact]
        public async Task Create_IdentityResource_WithSameApiScopeName_Should_Fail()
        {
            _fixture.CleanDb();

            var apiScopeService = _fixture.ServiceProvider.GetRequiredService<IDbApiScopeService>();
            await apiScopeService.CreateApiScopeAsync(new CreateApiScopeRequest
            {
                ApiScope = new ApiScopeDto
                {
                    Id = 1,
                    DisplayName = "Test Name",
                    Enabled = 1,
                    Emphasize = 0,
                    Name = "TestScope",
                    Required = 1,
                    Claims = new string[] { }
                },
                Operator = "user1"
            });

            var service = _fixture.ServiceProvider.GetRequiredService<IDbIdentityResourceService>();

            

            //id.Should().Be(1);

            await Assert.ThrowsAsync<BllException>(async () =>
            {
                var identityResource = new IdentityResourceDto
                {
                    Id = 1,
                    Enabled = 1,
                    Emphasize = 0,
                    Name = "TestScope",
                    DisplayName = "Test Displayname",
                    Claims = new string[] { "scope1" },
                    Required = 1,
                };

                var id = await service.CreateIdentityResourceAsync(new CreateIdentityResourceRequest
                {
                    Resource = identityResource,
                    Operator = "user1"
                });
            });
        }

        [Fact]
        public async Task Update_IdentityResource_Should_Ok()
        {
            _fixture.CleanDb();

            var service = _fixture.ServiceProvider.GetRequiredService<IDbIdentityResourceService>();

            var identityResource = new IdentityResourceDto
            {
                Id = 1,
                Enabled = 1,
                Emphasize = 0,
                Name = "Test",
                DisplayName = "Test Displayname",
                Claims = new string[] { "scope1" },
                Required = 1,
            };

            var id = await service.CreateIdentityResourceAsync(new CreateIdentityResourceRequest
            {
                Resource = identityResource,
                Operator = "user1"
            });

            var obj = await service.GetIdentityResourceByIdAsync(new GetIdentityResourceByIdRequest { Id = id });

            obj.Should().NotBeNull();
            obj.Id.Should().BeGreaterThan(0);
            obj.Name.Should().Be("Test");
            obj.DisplayName.Should().Be("Test Displayname");
            obj.Enabled.Should().Be(1);
            obj.Required.Should().Be(1);
            obj.Emphasize.Should().Be(0);


            await service.UpdateIdentityResourceAsync(new UpdateIdentityResourceRequest
            {
                Resource = new IdentityResourceDto
                {
                    Id = id,
                    Name = "Test new",
                    DisplayName = "test newnewnew",
                    Claims = new string[]
                    {
                        "scope2"
                    },
                    Required = 0,
                    Emphasize = 1,
                    Enabled = 0
                }
            });

            obj = await service.GetIdentityResourceByIdAsync(new GetIdentityResourceByIdRequest { Id = id });

            obj.Should().NotBeNull();
            obj.Id.Should().BeGreaterThan(0);
            obj.Name.Should().Be("Test new");
            obj.DisplayName.Should().Be("test newnewnew");
            obj.Enabled.Should().Be(0);
            obj.Required.Should().Be(0);
            obj.Emphasize.Should().Be(1);
            obj.Claims.Should().NotBeNull().And.HaveCount(1).And.Contain("scope2");

        }

        [Fact]
        public async Task Update_IdentityResource_WithSamename_Should_Fail()
        {
            _fixture.CleanDb();

            var service = _fixture.ServiceProvider.GetRequiredService<IDbIdentityResourceService>();

            var identityResource = new IdentityResourceDto
            {
                Id = 1,
                Enabled = 1,
                Emphasize = 0,
                Name = "Test",
                DisplayName = "Test Displayname",
                Claims = new string[] { "scope1" },
                Required = 1,
            };

            var id1 = await service.CreateIdentityResourceAsync(new CreateIdentityResourceRequest
            {
                Resource = identityResource,
                Operator = "user1"
            });


            identityResource = new IdentityResourceDto
            {
                Id = 2,
                Enabled = 1,
                Emphasize = 0,
                Name = "Test 2",
                DisplayName = "Test Displayname 2",
                Claims = new string[] { "scope2" },
                Required = 1,
            };

            var id2 = await service.CreateIdentityResourceAsync(new CreateIdentityResourceRequest
            {
                Resource = identityResource,
                Operator = "user1"
            });

            await Assert.ThrowsAsync<BllException>(async () =>
            {
                await service.UpdateIdentityResourceAsync(new UpdateIdentityResourceRequest
                {
                    Resource = new IdentityResourceDto
                    {
                        Id = id1,
                        Name = "Test 2",
                        DisplayName = "test newnewnew",
                        Claims = new string[]
                        {
                        "scope2"
                        },
                        Required = 0,
                        Emphasize = 1,
                        Enabled = 0
                    }
                });
            });
        }

        [Fact]
        public async Task Query_IdentityResource_Should_Ok()
        {
            _fixture.CleanDb();

            var service = _fixture.ServiceProvider.GetRequiredService<IDbIdentityResourceService>();

            for (var i = 0; i < 13; i++)
            {
                var identityResource = new IdentityResourceDto
                {
                    Id = 1,
                    Enabled = 1,
                    Emphasize = 0,
                    Name = $"Test {i}",
                    DisplayName = $"Test Displayname - {i}",
                    Claims = new string[] { "scope-all", $"scope-{i}" },
                    Required = 1,
                };

                await service.CreateIdentityResourceAsync(new CreateIdentityResourceRequest
                {
                    Resource = identityResource,
                    Operator = "user1"
                });
            }

            var result = await service.QueryIdentityResourcesAsync(new QueryIdentityResourcesRequest
            {
                PageIndex = 1,
                PageSize = 10
            });

            result.Items.Should().HaveCount(10);
            result.TotalCount.Should().Be(13 + 2);

            var obj = result.Items.Where(p => p.IsPreset == 0).First();
            obj.Should().NotBeNull();

            obj.Id.Should().BeGreaterThan(0);
            obj.Name.Should().Be("Test 0");
            obj.DisplayName.Should().Be("Test Displayname - 0");
            obj.Enabled.Should().Be(1);
            obj.Required.Should().Be(1);
            obj.Emphasize.Should().Be(0);
            obj.Claims.Should().HaveCount(2).And.Contain(new string[] { "scope-all", "scope-0" });


            result = await service.QueryIdentityResourcesAsync(new QueryIdentityResourcesRequest
            {
                Keyword = "Test 1",
                PageIndex = 1,
                PageSize = 10
            });

            result.Items.Should().HaveCount(4);
            result.TotalCount.Should().Be(4);

            result = await service.QueryIdentityResourcesAsync(new QueryIdentityResourcesRequest
            {
                Enabled = Mobwiz.Common.BaseTypes.BoolCondition.False,
                PageIndex = 1,
                PageSize = 10
            });

            result.Items.Should().HaveCount(0);
            result.TotalCount.Should().Be(0);


            result = await service.QueryIdentityResourcesAsync(new QueryIdentityResourcesRequest
            {
                PageIndex = 2,
                PageSize = 10
            });

            result.Items.Should().HaveCount(3 + 2);
            result.TotalCount.Should().Be(13 + 2);


            var all = await service.GetAllIdentityResourcesAsync();
            all.Should().HaveCount(13 + 2);

            obj = all.Where(p => p.IsPreset == 0).FirstOrDefault();

            obj.Id.Should().BeGreaterThan(0);
            obj.Name.Should().Be("Test 0");
            obj.DisplayName.Should().Be("Test Displayname - 0");
            obj.Enabled.Should().Be(1);
            obj.Required.Should().Be(1);
            obj.Emphasize.Should().Be(0);
            obj.Claims.Should().HaveCount(2).And.Contain(new string[] { "scope-all", "scope-0" });

            //
        }

        [Fact]
        public async Task FindByNames_Should_OK()
        {
            _fixture.CleanDb();

            var service = _fixture.ServiceProvider.GetRequiredService<IDbIdentityResourceService>();

            for (var i = 0; i < 13; i++)
            {
                var identityResource = new IdentityResourceDto
                {
                    Id = 1,
                    Enabled = 1,
                    Emphasize = 0,
                    Name = $"Test {i}",
                    DisplayName = $"Test Displayname - {i}",
                    Claims = new string[] { "scope-all", $"scope-{i}" },
                    Required = 1,
                };

                await service.CreateIdentityResourceAsync(new CreateIdentityResourceRequest
                {
                    Resource = identityResource,
                    Operator = "user1"
                });
            }


            var result = await service.FindIdentityResourcesByScopeNameAsync(new FindIdentityResourcesByScopeNameRequest
            {
                ScopeNames = new string[] { "Test 0", "Test 2", "Test 10", "NotExisted" }
            });

            result.Should().HaveCount(3);

            var obj = result.FirstOrDefault();

            obj.Id.Should().BeGreaterThan(0);
            obj.Name.Should().Be("Test 0");
            obj.DisplayName.Should().Be("Test Displayname - 0");
            obj.Enabled.Should().Be(1);
            obj.Required.Should().Be(1);
            obj.Emphasize.Should().Be(0);
            obj.Claims.Should().HaveCount(2).And.Contain(new string[] { "scope-all", "scope-0" });
        }

        [Fact]
        public async Task RemoveIdentityResourceAsync_Should_Ok()
        {
            _fixture.CleanDb();
            var service = _fixture.ServiceProvider.GetRequiredService<IDbIdentityResourceService>();

            var identityResource = new IdentityResourceDto
            {
                Id = 1,
                Enabled = 1,
                Emphasize = 0,
                Name = "Test",
                DisplayName = "Test Displayname",
                Claims = new string[] { "scope1" },
                Required = 1,
            };

            var id = await service.CreateIdentityResourceAsync(new CreateIdentityResourceRequest
            {
                Resource = identityResource,
                Operator = "user1"
            });


            var obj = await service.GetIdentityResourceByIdAsync(new GetIdentityResourceByIdRequest { Id = id });
            obj.Should().NotBeNull();

            await service.RemoveIdentityResourceAsync(new RemoveIdentityResourceRequest { Id = id });

            obj = await service.GetIdentityResourceByIdAsync(new GetIdentityResourceByIdRequest { Id = id });
            obj.Should().BeNull();
        }

        [Fact]
        public async Task Remove_PresetIdentityResource_Should_ThrowBllException()
        {
            _fixture.CleanDb();

            var service = _fixture.ServiceProvider.GetRequiredService<IDbIdentityResourceService>();

            var result = await service.FindIdentityResourcesByScopeNameAsync(new FindIdentityResourcesByScopeNameRequest
            {
                ScopeNames = new string[] { "openid" }
            });

            var res = result.FirstOrDefault();

            res.Should().NotBeNull();

            await Assert.ThrowsAsync<BllException>(async () =>
            {
                await service.RemoveIdentityResourceAsync(new RemoveIdentityResourceRequest { Id = res.Id });
            });
        }
    }
}
