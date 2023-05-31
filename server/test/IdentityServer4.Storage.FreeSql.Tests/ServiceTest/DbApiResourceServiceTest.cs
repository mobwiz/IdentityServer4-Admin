// Copyright (c) Luther R.D. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using FluentAssertions;
using IdentityServer4.Storage.FreeSql.Services;
using IdentityServer4.Storage.FreeSql.Services.Dto;
using IdentityServer4.Storage.FreeSql.Services.Requests;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityServer4.Storage.FreeSql.Tests.ServiceTest
{
    [Collection("DatabaseIntergationTest")]
    public class DbApiResourceServiceTest : IClassFixture<ServerFixture>
    {
        private readonly ServerFixture _serverFixture;
        public DbApiResourceServiceTest(ServerFixture serverFixture)
        {
            _serverFixture = serverFixture;
        }


        [Fact]
        public async Task Create_ApiResource_Should_Ok()
        {
            _serverFixture.CleanDb();

            var service = _serverFixture.ServiceProvider.GetRequiredService<IDbApiResourceService>();

            var id = await service.CreateApiResourceAsync(new CreateApiResourceRequest
            {
                ApiResource = new ApiResourceDto
                {
                    Name = "test",
                    DisplayName = "test api resource",
                    Enabled = 1,
                    Scopes = new[] { "scope1" },
                    Secrets = new[] { "hellosecret" },
                },
                Operator = "user1"
            });

            id.Should().Be(1);
        }

        [Fact]
        public async Task Get_ApiResource_Should_Ok()
        {
            _serverFixture.CleanDb();

            var service = _serverFixture.ServiceProvider.GetRequiredService<IDbApiResourceService>();

            var id = await service.CreateApiResourceAsync(new CreateApiResourceRequest
            {
                ApiResource = new ApiResourceDto
                {
                    Name = "test",
                    DisplayName = "test api resource",
                    Enabled = 1,
                    Scopes = new[] { "scope1" },
                    Secrets = new[] { "hellosecret" },
                },
                Operator = "user1"
            });

            id.Should().Be(1);

            var obj = await service.GetApiResourceByIdAsync(new GetApiResourceByIdRequest { Id = id });

            // 检查 obj 的值是否等于前面传入的值
            obj.Should().NotBeNull();
            obj.Name.Should().Be("test");
            obj.DisplayName.Should().Be("test api resource");
            obj.Enabled.Should().Be(1);
            obj.Scopes.Should().HaveCount(1).And.Contain("scope1");
            obj.Secrets.Should().HaveCount(1).And.Contain("hellosecret");
            obj.CreatedBy.Should().Be("user1");
            obj.CreateTime.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(5));
        }

        [Fact]
        public async Task UpdateApiResource_Should_Ok()
        {
            _serverFixture.CleanDb();
            var service = _serverFixture.ServiceProvider.GetRequiredService<IDbApiResourceService>();

            var id = await service.CreateApiResourceAsync(new CreateApiResourceRequest
            {
                ApiResource = new ApiResourceDto
                {
                    Name = "test",
                    DisplayName = "test api resource",
                    Enabled = 1,
                    Scopes = new[] { "scope1" },
                    Secrets = new[] { "hellosecret" },
                },
                Operator = "user1"
            });

            id.Should().Be(1);

            var obj = await service.GetApiResourceByIdAsync(new GetApiResourceByIdRequest { Id = id });
            // 检查 obj 的值是否等于前面传入的值
            obj.Should().NotBeNull();
            obj.Name.Should().Be("test");
            obj.DisplayName.Should().Be("test api resource");
            obj.Enabled.Should().Be(1);
            obj.Scopes.Should().HaveCount(1).And.Contain("scope1");
            obj.Secrets.Should().HaveCount(1).And.Contain("hellosecret");
            obj.CreatedBy.Should().Be("user1");
            obj.CreateTime.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(5));

            await service.UpdateApiResourceAsync(new UpdateApiResourceRequest
            {
                ApiResource = new ApiResourceDto
                {
                    Id = obj.Id,
                    Name = "test2",
                    DisplayName = "updated display name",
                    Enabled = 0,
                    Scopes = new[] { "scope2" },
                    Secrets = new[] { "updatedsecret" }
                },
                Operator = "user2"
            });

            obj = await service.GetApiResourceByIdAsync(new GetApiResourceByIdRequest { Id = id });

            // 检查 obj 的值是否等于前面传入的值
            obj.Should().NotBeNull();
            obj.Name.Should().Be("test2");
            obj.DisplayName.Should().Be("updated display name");
            obj.Enabled.Should().Be(0);
            obj.Scopes.Should().HaveCount(1).And.Contain("scope2");
            obj.Secrets.Should().HaveCount(1).And.Contain("updatedsecret");
            obj.CreatedBy.Should().Be("user1");
            obj.CreateTime.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(5));
            obj.UpdateBy.Should().Be("user2");
            obj.UpdateTime.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(5));

        }

        [Fact]
        public async Task RemoveApiresource_Should_Ok()
        {
            _serverFixture.CleanDb();

            var service = _serverFixture.ServiceProvider.GetRequiredService<IDbApiResourceService>();

            var id = await service.CreateApiResourceAsync(new CreateApiResourceRequest
            {
                ApiResource = new ApiResourceDto
                {
                    Name = "test",
                    DisplayName = "test api resource",
                    Enabled = 1,
                    Scopes = new[] { "scope1" },
                    Secrets = new[] { "hellosecret" },
                },
                Operator = "user1"
            });


            var obj = await service.GetApiResourceByIdAsync(new GetApiResourceByIdRequest { Id = id });
            obj.Should().NotBeNull();

            await service.RemoveApiResourceAsync(new RemoveApiResourceRequest { Id = id, Operator = "user3" });
            obj = await service.GetApiResourceByIdAsync(new GetApiResourceByIdRequest { Id = id });

            obj.Should().BeNull();
        }

        [Fact]
        public async Task GetAllShould_Ok()
        {
            _serverFixture.CleanDb();

            var service = _serverFixture.ServiceProvider.GetRequiredService<IDbApiResourceService>();

            for (var i = 0; i < 15; i++)
            {
                await service.CreateApiResourceAsync(new CreateApiResourceRequest
                {
                    ApiResource = new ApiResourceDto
                    {
                        Name = $"test {i}",
                        DisplayName = $"test api resource {i}",
                        Enabled = (byte)(i % 2),
                        Scopes = new[] { $"scope {i}" },
                        Secrets = new[] { $"hellosecret {i}" },
                    },
                    Operator = "user1"
                });
            }

            var all = await service.GetAllEnabledApiResources();

            all.Should().NotBeNull();
            all.Should().HaveCount(7);
        }

        [Fact]
        public async Task QueryApiResourcesAsync_Should_Ok()
        {
            _serverFixture.CleanDb();

            var service = _serverFixture.ServiceProvider.GetRequiredService<IDbApiResourceService>();

            for (var i = 0; i < 15; i++)
            {
                await service.CreateApiResourceAsync(new CreateApiResourceRequest
                {
                    ApiResource = new ApiResourceDto
                    {
                        Name = $"test {i}",
                        DisplayName = $"test api resource {i}",
                        Enabled = (byte)(i % 2),
                        Scopes = new[] { $"scope {i}" },
                        Secrets = new[] { $"hellosecret {i}" },
                    },
                    Operator = "user1"
                });
            }

            // 1
            var allResult = await service.QueryApiResourcesAsync(new QueryApiResourcesRequest
            {
                PageIndex = 1,
                PageSize = 10
            });

            allResult.Should().NotBeNull();
            allResult.TotalCount.Should().Be(15);
            allResult.Items.Should().NotBeNull().And.HaveCount(10);

            // 2
            allResult = await service.QueryApiResourcesAsync(new QueryApiResourcesRequest
            {
                PageIndex = 2,
                PageSize = 10
            });

            allResult.Should().NotBeNull();
            allResult.TotalCount.Should().Be(15);
            allResult.Items.Should().NotBeNull().And.HaveCount(5);

            // 3
            allResult = await service.QueryApiResourcesAsync(new QueryApiResourcesRequest
            {
                PageIndex = 1,
                PageSize = 10,
                Enabled = Mobwiz.Common.BaseTypes.BoolCondition.True,
            });

            allResult.Should().NotBeNull();
            allResult.TotalCount.Should().Be(7);
            allResult.Items.Should().NotBeNull().And.HaveCount(7);

            // 4
            allResult = await service.QueryApiResourcesAsync(new QueryApiResourcesRequest
            {
                PageIndex = 1,
                PageSize = 10,
                // Enabled = Mobwiz.Common.BaseTypes.BoolCondition.True,
                Keyword = "test 1"
            });

            allResult.Should().NotBeNull();
            allResult.TotalCount.Should().Be(6);
            allResult.Items.Should().NotBeNull().And.HaveCount(6);

            // 5
            allResult = await service.QueryApiResourcesAsync(new QueryApiResourcesRequest
            {
                PageIndex = 1,
                PageSize = 10,
                Enabled = Mobwiz.Common.BaseTypes.BoolCondition.False,
                Keyword = "test 1"
            });

            allResult.Should().NotBeNull();
            allResult.TotalCount.Should().Be(3);
            allResult.Items.Should().NotBeNull().And.HaveCount(3);
        }

        [Fact]
        public async Task FindApiResourcesByNamesAsync_Should_Ok()
        {
            _serverFixture.CleanDb();

            var service = _serverFixture.ServiceProvider.GetRequiredService<IDbApiResourceService>();

            for (var i = 0; i < 15; i++)
            {
                await service.CreateApiResourceAsync(new CreateApiResourceRequest
                {
                    ApiResource = new ApiResourceDto
                    {
                        Name = $"test {i}",
                        DisplayName = $"test api resource {i}",
                        Enabled = (byte)(i % 2),
                        Scopes = new[] { $"scope {i}" },
                        Secrets = new[] { $"hellosecret {i}" },
                    },
                    Operator = "user1"
                });
            }

            var result = await service.FindApiResourcesByNamesAsync(new FindApiResourcesByNamesRequest
            {
                Names = new[] {
                    "test 1", "test 3", "test 0"
                }
            });

            result.Should().NotBeNull().And.HaveCount(2);

            var obj = result.First();
            obj.Should().NotBeNull();
            obj.Name.Should().Be("test 1");

        }

        [Fact]
        public async Task FindApiResourcesByScopeNamesAsync_Should_Ok()
        {
            _serverFixture.CleanDb();

            var service = _serverFixture.ServiceProvider.GetRequiredService<IDbApiResourceService>();

            for (var i = 0; i < 15; i++)
            {
                await service.CreateApiResourceAsync(new CreateApiResourceRequest
                {
                    ApiResource = new ApiResourceDto
                    {
                        Name = $"test {i}",
                        DisplayName = $"test api resource {i}",
                        Enabled = (byte)(i % 2),
                        Scopes = new[] { $"scope {i}" },
                        Secrets = new[] { $"hellosecret {i}" },
                    },
                    Operator = "user1"
                });
            }

            var result = await service.FindApiResourcesByScopeNamesAsync(new FindApiResourcesByScopeNameRequest
            {
                ScopeNames = new[] {
                    "test 1", "test 3", "test 0", "scope 1", "scope 2", "scope 3"
                }
            });

            result.Should().NotBeNull().And.HaveCount(2);

            var obj = result.First();
            obj.Should().NotBeNull();
            obj.Name.Should().Be("test 1");

        }
    }
}
