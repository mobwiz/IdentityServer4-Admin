// Copyright (c) Luther R.D. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace IdentityServer4.Storage.FreeSql.Tests
{
    public class ExtensionTest : IClassFixture<ServerFixture>
    {
        private readonly ServerFixture _fixture;
        public ExtensionTest(ServerFixture fixture)
        {
            this._fixture = fixture;
        }        
    }
}