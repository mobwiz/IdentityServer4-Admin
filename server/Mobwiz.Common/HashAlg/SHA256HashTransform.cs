using Scrypt;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Mobwiz.Common.HashAlg
{
    /// <summary>
    /// SHA256
    /// </summary>
    public class SHA256HashTransform : IHashAlgorithm
    {
        /// <summary>
        /// Encrypt
        /// </summary>
        /// <param name="key"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public string ComputeHash(string key, string input)
        {
            // check the input paramters
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }

            using (var sha256 = SHA256.Create())
            {
                var bytes0 = Encoding.UTF8.GetBytes($"{input}:{key}");
                var hash = sha256.ComputeHash(bytes0);
                return string.Join("", hash.Select(item => item.ToString("x2")).ToArray());
            }
        }
    }

    //public class ScryptHashTransform
    //{
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="input"></param>
    //    /// <returns></returns>
    //    public string ComputeHash(string key, string input)
    //    {
    //        var salt = Encoding.UTF8.GetBytes(key);
    //        var encoder = new ScryptEncoder();            

    //        // var hash = SCrypt.ComputeDerivedKey(Encoding.UTF8.GetBytes(input), salt, 16384, 8, 1, null, 32);
    //        var hash = encoder.Encode(input);


    //        return string.Join("", hash.Select(item => item.ToString("x2")).ToArray());
    //    }
    //}
}