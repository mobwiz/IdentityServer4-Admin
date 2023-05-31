// Copyright (c) Luther R.D. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using FluentAssertions;
using Mobwiz.Common.Crypto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Mobwiz.Common.Tests.Crypto
{
    public class RsaUtilTest
    {
        private const string FilePub = "test_pub.txt";
        private const string FilePriviate = "test_prv.txt";

        private void DeleteKeys()
        {
            if (File.Exists(FilePub))
            {
                File.Delete(FilePub);
            }
            if (File.Exists(FilePriviate))
            {
                File.Delete(FilePriviate);
            }
        }

        private void InitKeys()
        {
            if (!File.Exists(FilePub) || !File.Exists(FilePriviate))
            {
                using (var rsa = RSA.Create(2048))
                {
                    var privateKey = rsa.ExportRSAPrivateKey();
                    var publicKey = rsa.ExportSubjectPublicKeyInfo();
                    var privateKeyStr = Convert.ToBase64String(privateKey);
                    var publicKeyStr = Convert.ToBase64String(publicKey);
                    //Console.WriteLine($"privateKey: {privateKeyStr}");
                    //Console.WriteLine($"publicKey: {publicKeyStr}");

                    File.WriteAllText(FilePub, publicKeyStr);
                    File.WriteAllText(FilePriviate, privateKeyStr);
                }
            }
        }

        [Fact]
        public void TestEncrypt()
        {
            InitKeys();
            var data = Encoding.UTF8.GetBytes("hello world");
            var publicKey = File.ReadAllText(FilePub);
            var privateKey = File.ReadAllText(FilePriviate);
            var rsaUtil = new RsaUtil();
            var encrypted = rsaUtil.Encrypt(data, Convert.FromBase64String(publicKey));
            var decrypted = rsaUtil.Decrypt(encrypted, Convert.FromBase64String(privateKey));
            Assert.Equal(data, decrypted);
        }

        [Fact]
        public void TestSignData()
        {
            InitKeys();
            var data = Encoding.UTF8.GetBytes("hello world");
            var publicKey = File.ReadAllText(FilePub);
            var privateKey = File.ReadAllText(FilePriviate);
            var rsaUtil = new RsaUtil();

            var sigauture = rsaUtil.SignData(data, Convert.FromBase64String(privateKey));

            var result = rsaUtil.VerifyData(data, sigauture, Convert.FromBase64String(publicKey));

            result.Should().BeTrue();
        }
    }
}
