// Copyright (c) Luther R.D. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using FluentAssertions;
using IdentityServer4.Storage.FreeSql.Services;
using IdentityServer4.Storage.FreeSql.Services.Dto;
using IdentityServer4.Stores;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityServer4.Storage.FreeSql.Tests.ServiceTest
{
    [Collection("DatabaseIntergationTest")]

    public class DbKeyValueServiceTest : IClassFixture<ServerFixture>
    {
        private readonly ServerFixture _serverFixture;
        public DbKeyValueServiceTest(ServerFixture serverFixture)
        {
            _serverFixture = serverFixture;
        }

        private const string Test_Key1 = "key1";
        private const string Test_Value1 = "value1";

        private const string Test_Key2 = "key2";
        private const string Test_Value2 = "value2";


        [Fact]
        public async Task SetItem_WithNull_Should_ThrowException()
        {
            var service = _serverFixture.ServiceProvider.GetRequiredService<IDbKeyValueService>();

            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await service.SetItemAsync(null);
            });
        }

        [Fact]
        public async Task SetItem_WithEmptyKey_Should_ThrowException()
        {
            var service = _serverFixture.ServiceProvider.GetRequiredService<IDbKeyValueService>();

            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                var dto = new Services.Requests.SetKeyValueItemRequest
                {
                    Key = string.Empty,
                    Value = Test_Value1
                };
                await service.SetItemAsync(dto);
            });
        }

        [Fact]
        public async Task SetItem_WithValidData_Should_Ok()
        {
            _serverFixture.CleanDb();
            var service = _serverFixture.ServiceProvider.GetRequiredService<IDbKeyValueService>();

            var dto = new Services.Requests.SetKeyValueItemRequest
            {
                Key = Test_Key1,
                Value = Test_Value1
            };
            await service.SetItemAsync(dto);

            var result = await service.GetItemAsync(Test_Key1);
            result.Should().NotBeNull();
            result.Key.Should().Be(Test_Key1);
            result.Value.Should().Be(Test_Value1);
        }

        [Fact]
        public async Task RemoveItem_WithValidData_Should_Ok()
        {
            _serverFixture.CleanDb();
            var service = _serverFixture.ServiceProvider.GetRequiredService<IDbKeyValueService>();

            var dto = new Services.Requests.SetKeyValueItemRequest
            {
                Key = Test_Key1,
                Value = Test_Value1
            };
            await service.SetItemAsync(dto);

            var result = await service.GetItemAsync(Test_Key1);
            result.Should().NotBeNull();

            result.Key.Should().Be(Test_Key1);
            result.Value.Should().Be(Test_Value1);

            await service.RemoveItemAsync(Test_Key1);
            result = await service.GetItemAsync(Test_Key1);
            result.Should().BeNull();

        }


        [Fact]
        public async Task SetItem_WithSameKey_ShouldUpdate()
        {
            _serverFixture.CleanDb();
            var service = _serverFixture.ServiceProvider.GetRequiredService<IDbKeyValueService>();

            var dto = new Services.Requests.SetKeyValueItemRequest
            {
                Key = Test_Key1,
                Value = Test_Value1,
                Operator = "user1"
            };
            await service.SetItemAsync(dto);

            var result = await service.GetItemAsync(Test_Key1);
            result.Should().NotBeNull();
            result.Key.Should().Be(Test_Key1);
            result.Value.Should().Be(Test_Value1);

            await service.SetItemAsync(new Services.Requests.SetKeyValueItemRequest
            {
                Key = Test_Key1,
                Value = Test_Value2,
                Operator = "user2"
            });

            result = await service.GetItemAsync(Test_Key1);
            result.Should().NotBeNull();
            result.Key.Should().Be(Test_Key1);
            result.Value.Should().Be(Test_Value2);
        }



        [Fact]
        public async Task GetSigningKey_WithExpire_Should_Ok()
        {
            _serverFixture.CleanDb();

            Storage.KeyManager.RotateKeyManager.Reset();
            Storage.KeyManager.RotateKeyManager.KeyExpiration = TimeSpan.FromSeconds(1);

            var keyManager = _serverFixture.ServiceProvider.GetRequiredService<Storage.KeyManager.RotateKeyManager>();


            var interval = TimeSpan.FromSeconds(5);

            var keys = await keyManager.GetKeys();

            var counter = 1;

            while (keys.ValidationKeys.Count < 6)
            {
                Console.WriteLine($"key expiration:{Storage.KeyManager.RotateKeyManager.KeyExpiration}");
                keys.ValidationKeys.Count.Should().Be(counter);
                await Task.Delay(interval);
                keys = await keyManager.GetKeys();
                counter += 1;
                keys.ValidationKeys.Count.Should().Be(counter);
            }

            await Task.Delay(interval);
            keys = await keyManager.GetKeys();
            var sk1 = keys.SigningKey;
            keys.ValidationKeys.Count.Should().Be(6);

            await Task.Delay(interval);
            keys = await keyManager.GetKeys();
            keys.ValidationKeys.Count.Should().Be(6);
            var sk2 = keys.SigningKey;

            sk1.Should().NotBe(sk2);
            //sk1.JsonWebKey.DP.Should().NotBe(sk2.JsonWebKey.DP);
            //sk1.JsonWebKey.DQ.Should().NotBe(sk2.JsonWebKey.DQ);
            //sk1.JsonWebKey.Crv.Should().NotBe(sk2.JsonWebKey.Crv);
            //sk1.JsonWebKey.E.Should().NotBe(sk2.JsonWebKey.E);
            //sk1.JsonWebKey.K.Should().NotBe(sk2.JsonWebKey.K);
            sk1.JsonWebKey.KeyId.Should().NotBe(sk2.JsonWebKey.KeyId);


        }


        [Fact]
        public async Task GetSigningKey_Should_Ok()
        {
            _serverFixture.CleanDb();
            var service = _serverFixture.ServiceProvider.GetRequiredService<IDbKeyValueService>();

            var _store = _serverFixture.ServiceProvider.GetRequiredService<ISigningCredentialStore>();

            var key = await _store.GetSigningCredentialsAsync();

            key.Should().NotBeNull();
        }

    }
}
