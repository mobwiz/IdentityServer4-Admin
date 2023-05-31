using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Mobwiz.Common.HashAlg
{
    /// <summary>
    /// HMACSHA256HashTransform
    /// </summary>
    public class HMACSHA256HashTransform: IHashAlgorithm
    {
        /// <summary>
        /// Encrypt
        /// </summary>
        /// <param name="key"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public string ComputeHash(string key, string input)
        {
            using (var hash = new HMACSHA256(Encoding.UTF8.GetBytes(key)))
            {
                var result = hash.ComputeHash(Encoding.UTF8.GetBytes(input));

                return string.Join("", result.Select(item => item.ToString("x2")).ToArray());
            }
        }
    }
}
