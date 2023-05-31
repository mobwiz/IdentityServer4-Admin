// Copyright (c) Luther R.D. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityServer4.Stores.Serialization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace IdentityServer4.Storage.FreeSql
{
    internal static class Helpers
    {
        private static readonly JsonSerializerOptions _jsonSerializerSettings = new JsonSerializerOptions
        {
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            WriteIndented = false,
            Converters = { new ClaimConverter(), new ClaimsPrincipalConverter() }
        };


        public static JsonSerializerOptions GetDefaultJsonSerializerSettings() { return _jsonSerializerSettings; }

    }
}
