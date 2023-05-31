// Copyright (c) Luther R.D. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mobwiz.Common.Exceptions
{
    public class BllException : Exception
    {
        public int Code { get; set; }
        public object?[] MessageArgs { get; set; }
        public BllException(int code, string message, params object?[] args)
            : base(message)
        {
            this.MessageArgs = args;
            this.Code= code;
        }
    }
}
