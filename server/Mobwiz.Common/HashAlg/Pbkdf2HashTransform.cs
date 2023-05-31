using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Mobwiz.Common.HashAlg
{
    /// <summary>
    /// Pbkdf2 密码 hash 算法
    /// </summary>
    public class Pbkdf2HashTransform : IHashAlgorithm
    {
        // pbkdf2 迭代 10000 次
        private const int IterationCount = 10000;

        /// <summary>
        /// 计算 hash
        /// </summary>
        /// <param name="key"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public string ComputeHash(string key, string input)
        {
            // 进行一次            
            var salt = Encoding.UTF8.GetBytes(key);
            string hashed = BitConverter.ToString(KeyDerivation.Pbkdf2(
                password: input,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: IterationCount,
                numBytesRequested: 256 / 8 // 输出 256 位，32 字节的 hash，hex 64 位
                )).Replace("-", "");
            return hashed;
        }
    }
}
