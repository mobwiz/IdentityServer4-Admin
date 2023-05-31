// Copyright (c) Luther R.D. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Mobwiz.Common.Exceptions;
using System.Security.Cryptography;
using System.Text;

namespace Mobwiz.Common.Crypto
{
    public class RsaUtil
    {
        public byte[] Decrypt(byte[] data, byte[] privateKey)
        {
            using var rsa = RSA.Create();
            rsa.ImportRSAPrivateKey(privateKey, out int bytesRead);

            if (rsa.KeySize < 2048) throw new BllException(500, "RSA key must be greater or equal than 2048");

            int bufferSize = rsa.KeySize / 8;

            var buffer = new byte[bufferSize];
            using (MemoryStream msIn = new MemoryStream(data), msOut = new MemoryStream())
            {
                while (true)
                {
                    int readSize = msIn.Read(buffer, 0, bufferSize);
                    if (readSize <= 0) break;

                    var temp = new byte[readSize];
                    Array.Copy(buffer, 0, temp, 0, readSize);
                    var rawBytes = rsa.Decrypt(temp, RSAEncryptionPadding.OaepSHA256);

                    msOut.Write(rawBytes, 0, rawBytes.Length);
                }

                return msOut.ToArray();
            }
        }

        public byte[] Encrypt(byte[] data, byte[] publicKey)
        {
            using (var rsa = RSA.Create())
            {
                rsa.ImportSubjectPublicKeyInfo(publicKey, out int bytesRead);
                if (rsa.KeySize < 2048) throw new BllException(500, "RSA key must be greater or equal than 2048");

                // refer to this.... https://crypto.stackexchange.com/questions/42097/what-is-the-maximum-size-of-the-plaintext-message-for-rsa-oaep
                // 默认用 sha256，所以这里的这么计算
                int bufferSize = rsa.KeySize / 8 - 2 * 256 / 8 - 2;
                var buffer = new byte[bufferSize];
                using (MemoryStream msIn = new MemoryStream(data), msOut = new MemoryStream())
                {
                    while (true)
                    {
                        int readSize = msIn.Read(buffer, 0, bufferSize);
                        if (readSize <= 0) break;

                        var temp = new byte[readSize];
                        Array.Copy(buffer, 0, temp, 0, readSize);
                        var encrypedBytes = rsa.Encrypt(temp, RSAEncryptionPadding.OaepSHA256);

                        msOut.Write(encrypedBytes, 0, encrypedBytes.Length);
                    }

                    return msOut.ToArray();
                }
            }
        }

        public byte[] SignData(byte[] data, byte[] privateKey)
        {
            using (var rsa = RSA.Create())
            {
                rsa.ImportRSAPrivateKey(privateKey, out int bytesRead);
                if (rsa.KeySize < 2048) throw new BllException(500, "RSA key must be greater or equal than 2048");

                var signed = rsa.SignData(data, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

                return signed;
            }
        }

        public bool VerifyData(byte[] data, byte[] signature, byte[] publicKey)
        {
            using (var rsa = RSA.Create())
            {
                rsa.ImportSubjectPublicKeyInfo(publicKey, out int bytesRead);
                if (rsa.KeySize < 2048) throw new BllException(500, "RSA key must be greater or equal than 2048");

                var signed = rsa.VerifyData(data, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

                return signed;
            }
        }
    }
}
