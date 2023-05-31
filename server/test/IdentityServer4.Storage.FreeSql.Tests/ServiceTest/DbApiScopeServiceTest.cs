// Copyright (c) Luther R.D. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using FluentAssertions;
using IdentityServer4.Storage.FreeSql.Services;
using IdentityServer4.Storage.FreeSql.Services.Dto;
using IdentityServer4.Storage.FreeSql.Services.Requests;
using Mobwiz.Common.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityServer4.Storage.FreeSql.Tests.ServiceTest
{
    [Collection("DatabaseIntergationTest")]
    public class DbApiScopeServiceTest : IClassFixture<ServerFixture>
    {
        private readonly ServerFixture _fixture;
        public DbApiScopeServiceTest(ServerFixture fixture)
        {
            this._fixture = fixture;
        }

        // begin : empty parameters check

        [Fact]
        public async Task CreateApiScope_WithNullParameter_Should_ThrowExcpetion()
        {
            //_fixture.CleanDb();
            var service = _fixture.ServiceProvider.GetRequiredService<IDbApiScopeService>();
            await Assert.ThrowsAsync<ArgumentNullException>(() => service.CreateApiScopeAsync(null));
        }

        [Fact]
        public async Task UpdateApiScope_WithNull_Should_ThrowException()
        {
            //_fixture.CleanDb();
            var service = _fixture.ServiceProvider.GetRequiredService<IDbApiScopeService>();
            await Assert.ThrowsAsync<ArgumentNullException>(() => service.UpdateApiScopeAsync(null));
        }

        [Fact]
        public async Task DeleteApiScope_WithNull_Should_ThrowException()
        {
            //_fixture.CleanDb();
            var service = _fixture.ServiceProvider.GetRequiredService<IDbApiScopeService>();
            await Assert.ThrowsAsync<ArgumentNullException>(() => service.RemoveApiScopeAsync(null));
        }

        // end   :  empty 

        [Fact]
        public async Task AddApiScope_Should_Work()
        {
            _fixture.CleanDb();
            var service = _fixture.ServiceProvider.GetRequiredService<IDbApiScopeService>();
            var result = await service.CreateApiScopeAsync(new CreateApiScopeRequest
            {
                ApiScope = new ApiScopeDto
                {
                    Id = 1,
                    DisplayName = "Test2 DisplayName",
                    Enabled = 1,
                    Emphasize = 0,
                    Name = "Test Name",
                    Required = 1,
                    Claims = new string[] { "scop0" }
                },
                Operator = "user1"
            });
            result.Should().Be(1);
        }

        [Fact]
        public async Task AddApiScope_WithSameName_Should_Fail()
        {
            _fixture.CleanDb();

            var service = _fixture.ServiceProvider.GetRequiredService<IDbApiScopeService>();

            await service.CreateApiScopeAsync(new CreateApiScopeRequest
            {
                ApiScope = new ApiScopeDto
                {
                    Id = 1,
                    DisplayName = "Test",
                    Enabled = 1,
                    Emphasize = 0,
                    Name = "Test Name",
                    Required = 1,
                    Claims = new string[] { }
                },
                Operator = "user1"
            });

            // 检查应该抛出 LrdException
            await Assert.ThrowsAsync<BllException>(async () =>
            {
                var result = await service.CreateApiScopeAsync(new CreateApiScopeRequest
                {
                    ApiScope = new ApiScopeDto
                    {
                        Id = 1,
                        DisplayName = "Test",
                        Enabled = 1,
                        Emphasize = 0,
                        Name = "Test Name",
                        Required = 1,
                        Claims = new string[] { }
                    },
                    Operator = "user1"
                });
            });

        }

        [Fact]
        public async Task AddApiScope_WithSameIdentityResourceName_Should_Fail()
        {
            _fixture.CleanDb();

            var identityService = _fixture.ServiceProvider.GetRequiredService<IDbIdentityResourceService>();

            var sameIdentityResourceName = "TestIdentityResource";
            var identityResource = new IdentityResourceDto
            {
                Id = 1,
                Enabled = 1,
                Emphasize = 0,
                Name = sameIdentityResourceName,
                DisplayName = "Test Displayname",
                Claims = new string[] { "scope1" },
                Required = 1,
            };

            var id = await identityService.CreateIdentityResourceAsync(new CreateIdentityResourceRequest
            {
                Resource = identityResource,
                Operator = "user1"
            });



            var service = _fixture.ServiceProvider.GetRequiredService<IDbApiScopeService>();
            // 检查应该抛出 LrdException
            await Assert.ThrowsAsync<BllException>(async () =>
            {
                await service.CreateApiScopeAsync(new CreateApiScopeRequest
                {
                    ApiScope = new ApiScopeDto
                    {
                        Id = 1,
                        DisplayName = "Test",
                        Enabled = 1,
                        Emphasize = 0,
                        Name = sameIdentityResourceName,
                        Required = 1,
                        Claims = new string[] { }
                    },
                    Operator = "user1"
                });
            });

        }

        [Fact]
        public async Task GetApiScope_Should_Work()
        {
            _fixture.CleanDb();
            var service = _fixture.ServiceProvider.GetRequiredService<IDbApiScopeService>();
            var result = await service.CreateApiScopeAsync(new CreateApiScopeRequest
            {
                ApiScope = new ApiScopeDto
                {
                    Id = 1,
                    DisplayName = "Test2 DisplayName",
                    Enabled = 1,
                    Emphasize = 0,
                    Name = "Test Name",
                    Required = 1,
                    Claims = new string[] { }
                },
                Operator = "user1"
            });
            result.Should().Be(1);

            var apiScope = await service.GetApiScopeByIdAsync(new GetApiScopeByIdRequest { Id = 1 });

            apiScope.Should().NotBeNull();
            apiScope.Name.Should().Be("Test Name");
            apiScope.DisplayName.Should().Be("Test2 DisplayName");
            apiScope.Id.Should().Be(1);
            apiScope.Enabled.Should().Be(1);
            apiScope.Emphasize.Should().Be(0);
            apiScope.Required.Should().Be(1);
            apiScope.Claims.Should().BeEmpty();
        }

        [Fact]
        public async Task UpdateApiScope_Should_Work()
        {
            _fixture.CleanDb();
            var service = _fixture.ServiceProvider.GetRequiredService<IDbApiScopeService>();
            var result = await service.CreateApiScopeAsync(new CreateApiScopeRequest
            {
                ApiScope = new ApiScopeDto
                {
                    Id = 1,
                    DisplayName = "Test2 DisplayName",
                    Enabled = 1,
                    Emphasize = 0,
                    Name = "Test Name",
                    Required = 1,
                    Claims = new string[] { }
                },
                Operator = "user1"
            });

            result.Should().Be(1);

            var apiScope = await service.GetApiScopeByIdAsync(new GetApiScopeByIdRequest { Id = 1 });

            await service.UpdateApiScopeAsync(new UpdateApiScopeRequest
            {
                ApiScope = new ApiScopeDto
                {
                    Id = apiScope.Id,
                    DisplayName = "Test2 DisplayName updated",
                    Enabled = 0,
                    Emphasize = 1,
                    Name = "Test Name updated 33",
                    Required = 0,
                    Claims = new string[] { "scope1" }
                },
                Operator = "user2"
            });

            apiScope = await service.GetApiScopeByIdAsync(new GetApiScopeByIdRequest { Id = 1 });

            apiScope.Should().NotBeNull();
            apiScope.Name.Should().Be("Test Name updated 33");
            apiScope.DisplayName.Should().Be("Test2 DisplayName updated");
            apiScope.Id.Should().Be(1L);
            apiScope.Enabled.Should().Be(0);
            apiScope.Emphasize.Should().Be(1);
            apiScope.Claims.Should().Contain("scope1");
            apiScope.UpdateBy.Should().Be("user2");
        }


        [Fact]
        public async Task UpdateApiScope_WithSameName_Should_Fail()
        {
            _fixture.CleanDb();
            var service = _fixture.ServiceProvider.GetRequiredService<IDbApiScopeService>();
            var result = await service.CreateApiScopeAsync(new CreateApiScopeRequest
            {
                ApiScope = new ApiScopeDto
                {
                    Id = 1,
                    DisplayName = "Test2 DisplayName",
                    Enabled = 1,
                    Emphasize = 0,
                    Name = "Test Name",
                    Required = 1,
                    Claims = new string[] { }
                },
                Operator = "user1"
            });

            result.Should().Be(1);

            result = await service.CreateApiScopeAsync(new CreateApiScopeRequest
            {
                ApiScope = new ApiScopeDto
                {
                    Id = 1,
                    DisplayName = "Test2 DisplayName",
                    Enabled = 1,
                    Emphasize = 0,
                    Name = "Name2", // same name to update
                    Required = 1,
                    Claims = new string[] { }
                },
                Operator = "user1"
            });

            var apiScope = await service.GetApiScopeByIdAsync(new GetApiScopeByIdRequest { Id = 1 });


            await Assert.ThrowsAsync<BllException>(async () =>
            {
                await service.UpdateApiScopeAsync(new UpdateApiScopeRequest
                {
                    ApiScope = new ApiScopeDto
                    {
                        Id = apiScope.Id,
                        DisplayName = "Test2 DisplayName updated",
                        Enabled = 0,
                        Emphasize = 1,
                        Name = "Name2", // same name
                        Required = 0,
                        Claims = new string[] { "scope1" }
                    },
                    Operator = "user2"
                });
            });
        }

        [Fact]
        public async Task UpdateApiScope_UpdateSelf_Should_Ok()
        {
            _fixture.CleanDb();
            var service = _fixture.ServiceProvider.GetRequiredService<IDbApiScopeService>();
            var result = await service.CreateApiScopeAsync(new CreateApiScopeRequest
            {
                ApiScope = new ApiScopeDto
                {
                    Id = 1,
                    DisplayName = "Test2 DisplayName",
                    Enabled = 1,
                    Emphasize = 0,
                    Name = "Test Name",
                    Required = 1,
                    Claims = new string[] { }
                },
                Operator = "user1"
            });

            result.Should().Be(1);

            var apiScope = await service.GetApiScopeByIdAsync(new GetApiScopeByIdRequest { Id = 1 });

            await service.UpdateApiScopeAsync(new UpdateApiScopeRequest
            {
                ApiScope = new ApiScopeDto
                {
                    Id = apiScope.Id,
                    DisplayName = "Test2 DisplayName updated",
                    Enabled = 0,
                    Emphasize = 1,
                    Name = "Test Name", // same name but is self
                    Required = 0,
                    Claims = new string[] { "scope1" }
                },
                Operator = "user2"
            });

        }


        [Fact]
        public async Task Remove_Should_OK()
        {
            _fixture.CleanDb();
            var service = _fixture.ServiceProvider.GetRequiredService<IDbApiScopeService>();
            var result = await service.CreateApiScopeAsync(new CreateApiScopeRequest
            {
                ApiScope = new ApiScopeDto
                {
                    Id = 1,
                    DisplayName = "Test2 DisplayName",
                    Enabled = 1,
                    Emphasize = 0,
                    Name = "Test Name",
                    Required = 1,
                    Claims = new string[] { }
                },
                Operator = "user1"
            });

            result.Should().Be(1);

            await service.RemoveApiScopeAsync(new RemoveApiScopeRequest { Id = 1 });

            var scope = await service.GetApiScopeByIdAsync(new GetApiScopeByIdRequest { Id = 1 });

            scope.Should().BeNull();
        }

        [Fact]
        public async Task Query_Should_Ok()
        {
            _fixture.CleanDb();
            var service = _fixture.ServiceProvider.GetRequiredService<IDbApiScopeService>();

            var idex = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 };

            foreach (var i in idex)
            {
                await service.CreateApiScopeAsync(new CreateApiScopeRequest
                {
                    ApiScope = new ApiScopeDto
                    {
                        Id = 1,
                        DisplayName = $"Test DisplayName{i}",
                        Enabled = 1,
                        Emphasize = 0,
                        Name = $"Scope{i}",
                        Required = 1,
                        Claims = new string[] { }
                    },
                    Operator = "user1"
                });
            }

            // check1
            var pagedResult = await service.QueryApiScopesAsync(new QueryApiScopesRequest
            {
                Keyword = "",
                Enabled = Mobwiz.Common.BaseTypes.BoolCondition.All,
                PageIndex = 1,
                PageSize = 5
            });

            pagedResult.Should().NotBeNull();
            pagedResult.TotalCount.Should().Be(11);
            pagedResult.Items.Count().Should().Be(5);

            // check2
            pagedResult = await service.QueryApiScopesAsync(new QueryApiScopesRequest
            {
                Keyword = "",
                Enabled = Mobwiz.Common.BaseTypes.BoolCondition.All,
                PageIndex = 3,
                PageSize = 5
            });

            pagedResult.Should().NotBeNull();
            pagedResult.TotalCount.Should().Be(11);
            pagedResult.Items.Count().Should().Be(1);

            // check3
            pagedResult = await service.QueryApiScopesAsync(new QueryApiScopesRequest
            {
                Keyword = "Scope6",
                Enabled = Mobwiz.Common.BaseTypes.BoolCondition.All,
                PageIndex = 1,
                PageSize = 5
            });

            pagedResult.Should().NotBeNull();
            pagedResult.TotalCount.Should().Be(1);
            pagedResult.Items.Count().Should().Be(1);

            // check3
            pagedResult = await service.QueryApiScopesAsync(new QueryApiScopesRequest
            {
                Keyword = "Scope1",
                Enabled = Mobwiz.Common.BaseTypes.BoolCondition.All,
                PageIndex = 1,
                PageSize = 5
            });

            pagedResult.Should().NotBeNull();
            pagedResult.TotalCount.Should().Be(3);
            pagedResult.Items.Count().Should().Be(3);

        }


        [Fact]
        public async Task FindByNames_Should_OK()
        {
            _fixture.CleanDb();
            var service = _fixture.ServiceProvider.GetRequiredService<IDbApiScopeService>();

            var idex = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 };

            foreach (var i in idex)
            {
                await service.CreateApiScopeAsync(new CreateApiScopeRequest
                {
                    ApiScope = new ApiScopeDto
                    {
                        Id = 1,
                        DisplayName = $"Test DisplayName{i}",
                        Enabled = 1,
                        Emphasize = 0,
                        Name = $"Scope{i}",
                        Required = 1,
                        Claims = new string[] { }
                    },
                    Operator = "user1"
                });
            }

            // check 1
            var list = await service.FindApiScopesByNameAsync(new FindApiScopesByNameRequest
            {
                Names = new string[] { "scope1", "scope2", "scope9" }
            });

            list.Should().NotBeNullOrEmpty();
            list.Count().Should().Be(3);


            // check 2
            list = await service.FindApiScopesByNameAsync(new FindApiScopesByNameRequest
            {
                Names = new string[] { "scope1", "scope2", "scope92" }
            });

            list.Should().NotBeNullOrEmpty();
            list.Count().Should().Be(2);
        }

        [Fact]
        public async Task GetAllScope_Should_OK()
        {
            _fixture.CleanDb();
            var service = _fixture.ServiceProvider.GetRequiredService<IDbApiScopeService>();

            var idex = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 };

            foreach (var i in idex)
            {
                await service.CreateApiScopeAsync(new CreateApiScopeRequest
                {
                    ApiScope = new ApiScopeDto
                    {
                        Id = 1,
                        DisplayName = $"Test DisplayName{i}",
                        Enabled = 1,
                        Emphasize = 0,
                        Name = $"Scope{i}",
                        Required = 1,
                        Claims = new string[] { "scope1" }
                    },
                    Operator = "user1"
                });
            }

            var scopes = await service.GetAllApiScopesAsync();

            scopes.Should().NotBeNullOrEmpty();
            scopes.Count().Should().Be(11);
        }


    }
}
