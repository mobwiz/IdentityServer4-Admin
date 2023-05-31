// Copyright (c) Luther R.D. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Mobwiz.Common.BaseTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mobwiz.Common.Tests.BaseTypes
{
    public class PageResultTest
    {
        [Fact]
        public void PageResultConstructor_Should_Work()
        {
            var pageResult = new PagedResult<int>(100, new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
            Assert.Equal(100, pageResult.TotalCount);
            Assert.Equal(10, pageResult.Items.Count());
        }
    }
}
