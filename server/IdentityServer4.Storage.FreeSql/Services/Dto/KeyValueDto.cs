// Copyright (c) Luther R.D. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityServer4.Storage.FreeSql.Services.Dto
{
    public class KeyValueDto
    {
        public string Key { get; set; }
        public string Value { get; set; }

        public string CreatedBy { get; set; }
        public string UpdateBy { get; set; }
        public DateTime ExpireTime { get; set; }
    }
}
