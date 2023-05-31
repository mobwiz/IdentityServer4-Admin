// Copyright (c) Luther R.D. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityServer4.Storage.FreeSql.Services.Dto
{
    public class TicketDataDto
    {
        public string Key { get; set; }
        public string Base64Data { get; set; }
        public string SubjectId { get; set; }

        public DateTime LastActivity { get; set; }
        public DateTime ExpireTime { get; set; }

        public byte[] Data
        {
            get
            {
                try
                {
                    return Convert.FromBase64String(Base64Data);
                }
                catch (Exception)
                {
                    return new byte[0];
                }
            }
        }
    }
}
