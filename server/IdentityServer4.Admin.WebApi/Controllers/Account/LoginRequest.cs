// Copyright (c) Luther R.D. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.ComponentModel.DataAnnotations;

namespace IdentityServer4.Admin.WebApi.Controllers.Account
{
    /// <summary>
    /// Login Request   
    /// </summary>
    public class LoginRequest
    {
        /// <summary>
        /// Username
        /// </summary>
        [Required]
        [MaxLength(255)]
        public string Username { get; set; }

        /// <summary>
        /// Password
        /// </summary>
        [Required]
        [MaxLength(255)]
        public string Password { get; set; }


        /// <summary>
        /// Validate Code
        /// </summary>
        [MaxLength(16)]
        public string ValidateCode { get; set; }
    }
}