// Copyright (c) Luther R.D. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Mobwiz.Common.HashAlg;

namespace Mobwiz.Common.Tests.HashAlg
{
    public class HashTransformTest
    {
        [Fact]
        public void SHA256HashTransform_Should_Work()
        {
            var alg = new SHA256HashTransform();
            var hash = alg.ComputeHash("abc","123456");

            hash.Should().Be("6e3c7ec028ff5b9e11de3030060123501d0d1d3946e245733a1169ae54cc5ac3");
        }

        [Fact]
        public void HMAC256HashTransform_Should_Work()
        {
            var alg = new HMACSHA256HashTransform();
            var hash = alg.ComputeHash("123", "123456");

            hash.Should().Be("9117e65bc9a6a1ed724f2302287f7aa6a8fcff72cb44fb6a51e667e2d523e517");
        }

        [Fact]
        public void Pbkdf2HashTransform_Should_Work()
        {
            var alg = new Pbkdf2HashTransform();
            var hash = alg.ComputeHash("123", "123456");

            hash.Should().Be("78CCC991A839C6A055B750FD9C72BCA73902A0DBB82A089E2362CAD9E8A7B985");

        }
    }
}
