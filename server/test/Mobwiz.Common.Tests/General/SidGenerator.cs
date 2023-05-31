// Copyright (c) Luther R.D. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Mobwiz.Common.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;

namespace Mobwiz.Common.Tests.General
{
    public class SidGeneratorTest
    {
        [Theory]
        [InlineData(999999999999999999)]
        [InlineData(1111111111111111111)]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-2)]
        public void GeneratorShouldThrowException(long val)
        {
            Action act = () => SidGenerator.GenerateSidByLong("app", val);
            act.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Theory]
        [InlineData(10000, "app5gfwv2m5b")]
        [InlineData(1000, "app2afa4lh28v")]
        [InlineData(1, "app229sg3fj5zi6")]
        [InlineData(2, "app33ikv5u2ayzb")]
        [InlineData(20, "app89ri1hwmzwg")]
        public void GeneratorShouldWork(long id, string sid)
        {
            var val = SidGenerator.GenerateSidByLong("app", id);

            val.Should().NotBeNullOrWhiteSpace();
            val.Should().Be(sid);
        }
    }
}
