// Copyright (c) Luther R.D. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace Mobwiz.Common.General
{
    public interface IIdGenerator
    {
        void Dispose();
        long GenerateId(string key, DateTime dateTime = default, int inc = 1);
        Task<long> GenerateIdAsync(string key, DateTime dateTime = default, int inc = 1);
        long GetIdByTime(DateTime dateTime, byte nodeId = 0, ushort inc = 0);
    }
}