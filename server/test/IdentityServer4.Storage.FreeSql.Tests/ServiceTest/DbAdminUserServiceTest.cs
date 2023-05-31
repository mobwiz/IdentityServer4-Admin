// Copyright (c) Luther R.D. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using FluentAssertions;
using IdentityServer4.Storage.FreeSql.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityServer4.Storage.FreeSql.Tests.ServiceTest
{
    [CollectionDefinition("DatabaseIntergationTest", DisableParallelization = true)]
    public class DatabaseIntergationTestCollectionDefinition { }

    [Collection("DatabaseIntergationTest")]
    public class DbAdminUserServiceTest : IClassFixture<ServerFixture>
    {
        private ServerFixture _fixture;
        public DbAdminUserServiceTest(ServerFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task Create_AdminUser_Should_Ok()
        {
            _fixture.CleanDb();

            var service = _fixture.ServiceProvider.GetRequiredService<IDbAdminUserService>();

            var id = await service.CreateAdminUserAsync(new Services.Requests.CreateAdminUserRequest
            {
                UserName = "admin",
                Password = "123456",
                Operator = "system"
            });

            id.Should().Be(1);
        }

        [Fact]
        public async Task Validate_AdminUser_WithCorrentPassword_Should_Ok()
        {
            _fixture.CleanDb();

            var service = _fixture.ServiceProvider.GetRequiredService<IDbAdminUserService>();

            var id = await service.CreateAdminUserAsync(new Services.Requests.CreateAdminUserRequest
            {
                UserName = "admin",
                Password = "123456",
                Operator = "system"
            });

            id.Should().Be(1);

            var user = await service.ValidateUserAsync(new Services.Requests.ValidateUserRequest
            {
                Username = "admin",
                Password = "123456"
            });

            user.Should().NotBeNull();
            user.Username.Should().Be("admin");
        }

        [Fact]
        public async Task Validate_AdminUser_WithWrongPassword_Should_Ok()
        {
            _fixture.CleanDb();

            var service = _fixture.ServiceProvider.GetRequiredService<IDbAdminUserService>();

            var id = await service.CreateAdminUserAsync(new Services.Requests.CreateAdminUserRequest
            {
                UserName = "admin",
                Password = "123456",
                Operator = "system"
            });

            id.Should().Be(1);

            var user = await service.ValidateUserAsync(new Services.Requests.ValidateUserRequest
            {
                Username = "admin",
                Password = "badpassword"
            });

            user.Should().BeNull();
        }

        [Fact]
        public async Task ResetPassword_WithCorrectOld_Should_Ok()
        {
            _fixture.CleanDb();

            var service = _fixture.ServiceProvider.GetRequiredService<IDbAdminUserService>();

            var id = await service.CreateAdminUserAsync(new Services.Requests.CreateAdminUserRequest
            {
                UserName = "admin",
                Password = "123456",
                Operator = "system"
            });

            id.Should().Be(1);

            var result = await service.UpdatePasswordByOldAsync(new Services.Requests.UpdatePasswordByOldRequest
            {
                Id = id,
                OldPassword = "123456",
                NewPassword = "newpassword"
            });

            result.Should().BeTrue();

            var user = await service.ValidateUserAsync(new Services.Requests.ValidateUserRequest
            {
                Username = "admin",
                Password = "newpassword"
            });

            user.Should().NotBeNull();
        }

        [Fact]
        public async Task ResetPassword_WithBadOld_Should_Ok()
        {
            _fixture.CleanDb();

            var service = _fixture.ServiceProvider.GetRequiredService<IDbAdminUserService>();

            var id = await service.CreateAdminUserAsync(new Services.Requests.CreateAdminUserRequest
            {
                UserName = "admin",
                Password = "123456",
                Operator = "system"
            });

            id.Should().Be(1);

            var result = await service.UpdatePasswordByOldAsync(new Services.Requests.UpdatePasswordByOldRequest
            {
                Id = id,
                OldPassword = "badpassword",
                NewPassword = "newpassword"
            });

            result.Should().BeFalse();

            var user = await service.ValidateUserAsync(new Services.Requests.ValidateUserRequest
            {
                Username = "admin",
                Password = "newpassword"
            });

            user.Should().BeNull();
        }

        [Fact]
        public async Task UpdateLoginInfo_Should_Ok()
        {
            _fixture.CleanDb();

            var service = _fixture.ServiceProvider.GetRequiredService<IDbAdminUserService>();

            var id = await service.CreateAdminUserAsync(new Services.Requests.CreateAdminUserRequest
            {
                UserName = "admin",
                Password = "123456",
                Operator = "system"
            });

            id.Should().Be(1);

            var user = await service.ValidateUserAsync(new Services.Requests.ValidateUserRequest
            {
                Username = "admin",
                Password = "123456"
            });

            user.Should().NotBeNull();

            await service.UpdateLoginInfoAsync(new Services.Requests.UpdateLoginInfoRequest
            {
                Id = id,
                LoginIp = "127.0.0.1",
            });

            user = await service.GetAdminUserAsync(new Services.Requests.GetAdminUserByIdRequest { Id = id });
            user.Should().NotBeNull();
            user.LastLoginIp.Should().Be("127.0.0.1");
            user.LastLoginTime.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(5));

        }
    }
}
