// Copyright (c) Luther R.D. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using FluentAssertions;
using IdentityServer4.Storage.FreeSql.Entities;
using IdentityServer4.Storage.FreeSql.Services;
using IdentityServer4.Storage.FreeSql.Services.Dto;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityServer4.Storage.FreeSql.Tests.ServiceTest
{
    [Collection("DatabaseIntergationTest")]
    public class DbTicketServiceTest : IClassFixture<ServerFixture>
    {
        private readonly ServerFixture _serverFixture;
        public DbTicketServiceTest(ServerFixture serverFixture)
        {
            _serverFixture = serverFixture;
        }

        [Fact]
        public async Task Create_WithNull_Should_ThrowException()
        {
            var service = _serverFixture.ServiceProvider.GetRequiredService<IDbTicketService>();

            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await service.CreateTicketAsync(null);
            });
        }

        [Fact]
        public async Task Create_WithEmptyKey_Should_ThrowException()
        {
            var service = _serverFixture.ServiceProvider.GetRequiredService<IDbTicketService>();

            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                var dto = new TicketDataDto
                {
                    Key = string.Empty,
                    Base64Data = Test_Base64_1,
                    ExpireTime = DateTime.UtcNow.AddMinutes(30),
                    SubjectId = "1",
                    LastActivity = DateTime.UtcNow
                };
                await service.CreateTicketAsync(dto);
            });
        }

        [Fact]
        public async Task Update_WithNull_Should_ThrowException()
        {
            var service = _serverFixture.ServiceProvider.GetRequiredService<IDbTicketService>();

            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await service.UpdateTicketAsync(null);
            });
        }


        private const string Test_Key1 = "test111111111";
        private const string Test_Base64_1 = "dGVzdDExMTExMTExMQ==";
        private const string Test_Key2 = "test222222222222222222";
        private const string Test_Base64_2 = "dGVzdDIyMjIyMjIyMjIyMjIyMjIyMg==";
        [Fact]
        public async Task Create_Should_OK()
        {
            _serverFixture.CleanDb();

            var service = _serverFixture.ServiceProvider.GetRequiredService<IDbTicketService>();
            var dto = new TicketDataDto
            {
                Key = Test_Key1,
                Base64Data = Test_Base64_1,
                ExpireTime = DateTime.UtcNow.AddMinutes(30),
                SubjectId = "1",
                LastActivity = DateTime.UtcNow
            };

            await service.CreateTicketAsync(dto);

            var obj = await service.GetTicketByKeyAsync(Test_Key1);
            obj.Should().NotBeNull();
            obj.Key.Should().Be(Test_Key1);
            obj.Base64Data.Should().Be(Test_Base64_1);
            obj.SubjectId.Should().Be("1");
            obj.ExpireTime.Should().BeCloseTo(DateTime.UtcNow.AddMinutes(30), TimeSpan.FromSeconds(1));
            obj.LastActivity.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public async Task Update_Should_OK()
        {
            _serverFixture.CleanDb();

            var service = _serverFixture.ServiceProvider.GetRequiredService<IDbTicketService>();
            var dto = new TicketDataDto
            {
                Key = Test_Key1,
                Base64Data = Test_Base64_1,
                ExpireTime = DateTime.UtcNow.AddMinutes(30),
                SubjectId = "1",
                LastActivity = DateTime.UtcNow
            };

            await service.CreateTicketAsync(dto);
            {
                var obj = await service.GetTicketByKeyAsync(Test_Key1);

                await Task.Delay(2000);

                obj.LastActivity = DateTime.UtcNow;
                obj.ExpireTime = DateTime.UtcNow.AddMinutes(30);
                obj.Base64Data = Test_Base64_2;

                await service.UpdateTicketAsync(obj);
            }

            {
                var obj = await service.GetTicketByKeyAsync(Test_Key1);
                obj.Should().NotBeNull();
                obj.Key.Should().Be(Test_Key1);
                obj.Base64Data.Should().Be(Test_Base64_2);
                obj.SubjectId.Should().Be("1");
                obj.ExpireTime.Should().BeCloseTo(DateTime.UtcNow.AddMinutes(30), TimeSpan.FromSeconds(1));
                obj.LastActivity.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
            }
        }

        [Fact]
        public async Task Update_WithNonExiste_Should_ThrowException()
        {
            _serverFixture.CleanDb();

            var service = _serverFixture.ServiceProvider.GetRequiredService<IDbTicketService>();
            var dto = new TicketDataDto
            {
                Key = Test_Key1,
                Base64Data = Test_Base64_1,
                ExpireTime = DateTime.UtcNow.AddMinutes(30),
                SubjectId = "1",
                LastActivity = DateTime.UtcNow
            };

            await service.CreateTicketAsync(dto);
            {
                var obj = await service.GetTicketByKeyAsync(Test_Key1);

                await Task.Delay(2000);

                obj.LastActivity = DateTime.UtcNow;
                obj.ExpireTime = DateTime.UtcNow.AddMinutes(30);
                obj.Base64Data = Test_Base64_2;

                obj.Key = "somekeynotexisted";

                await Assert.ThrowsAsync<ArgumentException>(async () =>
                {
                    await service.UpdateTicketAsync(obj);
                });
            }
        }


        [Fact]
        public async Task RemoveByKey_Should_OK()
        {
            _serverFixture.CleanDb();

            var service = _serverFixture.ServiceProvider.GetRequiredService<IDbTicketService>();
            var dto = new TicketDataDto
            {
                Key = Test_Key1,
                Base64Data = Test_Base64_1,
                ExpireTime = DateTime.UtcNow.AddMinutes(30),
                SubjectId = "1",
                LastActivity = DateTime.UtcNow
            };

            await service.CreateTicketAsync(dto);

            var obj = await service.GetTicketByKeyAsync(Test_Key1);
            obj.Should().NotBeNull();


            await service.RemoveTicketByKeyAsync(Test_Key1);

            obj = await service.GetTicketByKeyAsync(Test_Key1);
            obj.Should().BeNull();

        }

        [Fact]
        public async Task RemoveByUserId_Should_OK()
        {
            _serverFixture.CleanDb();

            var service = _serverFixture.ServiceProvider.GetRequiredService<IDbTicketService>();
            var dto = new TicketDataDto
            {
                Key = Test_Key1,
                Base64Data = Test_Base64_1,
                ExpireTime = DateTime.UtcNow.AddMinutes(30),
                SubjectId = "1",
                LastActivity = DateTime.UtcNow
            };

            await service.CreateTicketAsync(dto);

            var obj = await service.GetTicketByKeyAsync(Test_Key1);
            obj.Should().NotBeNull();


            await service.RemoveTicketByUserIdAsync("1");

            obj = await service.GetTicketByKeyAsync(Test_Key1);
            obj.Should().BeNull();

        }

        [Fact]
        public async Task GetExpiredTickets_Should_Workr()
        {
            _serverFixture.CleanDb();

            var service = _serverFixture.ServiceProvider.GetRequiredService<IDbTicketService>();
            var dto = new TicketDataDto
            {
                Key = Test_Key1,
                Base64Data = Test_Base64_1,
                ExpireTime = DateTime.UtcNow.AddMinutes(-30),
                SubjectId = "1",
                LastActivity = DateTime.UtcNow.AddMinutes(-35)
            };

            await service.CreateTicketAsync(dto);

            var dto2 = new TicketDataDto
            {
                Key = Test_Key2,
                Base64Data = Test_Base64_2,
                ExpireTime = DateTime.UtcNow.AddMinutes(-1),
                SubjectId = "1",
                LastActivity = DateTime.UtcNow.AddMinutes(-36)
            };

            await service.CreateTicketAsync(dto2);

            var lst = await service.GetExpiredTicketsAsync(10);

            lst.Should().NotBeNull();
            lst.Should().HaveCount(2);

        }
    }
}
