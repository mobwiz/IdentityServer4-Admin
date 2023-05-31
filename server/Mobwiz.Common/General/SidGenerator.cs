using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mobwiz.Common.General
{
    /// <summary>
    /// 字符串 Id 生成器
    /// </summary>
    public static class SidGenerator
    {
        private const string Chars = "123456789abcdefghijklmnopqrstuvwxyz";
        private const string MaxNum = "999999999999999999";

        /// <summary>
        /// 传入一个 ID 生成器生成的 long Id，返回一个 字符串 ID
        /// </summary>
        /// <param name="objectId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static string GenerateSidByLong(string prefix, long objectId)
        {
            if (objectId >= 999999999999999999 || objectId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(objectId));
            }


            var str = string.Join("", objectId.ToString().Reverse());
            var len = str.Length;

            if (str.Length < MaxNum.Length)
            {
                // 后面补0
                for (var i = 0; i < MaxNum.Length - len; i++)
                {
                    str = str + "0";
                }
            }


            if (long.TryParse(str, out long rid))
            {
                return $"{prefix}{ToBase32(rid, Chars.Length)}";
            }

            throw new Exception("Object id is out of range");
        }

        private static string ToBase32(long val, int size)
        {
            var k = (long)Math.Floor((double)val / size);
            var m = (int)(val % size);
            if (val > size)
            {
                return ToBase32(k, size) + Chars[m];
            }
            return Chars[m] + "";
        }
    }
}
