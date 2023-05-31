// Copyright (c) Luther R.D. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using FluentAssertions;
using Mobwiz.Common.BaseTypes;

namespace Mobwiz.Common.Tests.BaseTypes
{
    public class BaseResultTest
    {
        [Fact]
        public void BaseResultConstructor_Should_Work()
        {
            var result = new BaseResult(0);
            Assert.NotNull(result);
            result.Code.Should().Be(0);
            result.Message.Should().BeNullOrWhiteSpace();
        }
    }
}
