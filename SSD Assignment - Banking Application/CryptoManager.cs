﻿using System;
using System.IO;
using System.Security.Cryptography;
using System.Linq;

namespace SSD_Assignment___Banking_Application
{
    public class CryptoManager
    {
        private CngKey _key;
        private string _keyName = "eckey1";

        public CryptoManager()
        {
            _key = LoadOrCreateKey(_keyName);
        }

        private static CngKey LoadOrCreateKey(string keyName)
        {
            CngProvider provider = CngProvider.MicrosoftSoftwareKeyStorageProvider;
            CngKeyCreationParameters keyCreationParameters = new CngKeyCreationParameters
            {
                Provider = provider,
                ExportPolicy = CngExportPolicies.AllowPlaintextExport
            };

            if (!CngKey.Exists(keyName, provider))
            {
                return CngKey.Create(new CngAlgorithm("AES"), keyName, keyCreationParameters);
            }
            else
            {
                return CngKey.Open(keyName, provider);
            }
        }

        private static byte[] GenerateIV()
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                byte[] iv = new byte[16]; // 128 bits for AES
                rng.GetBytes(iv);
                return iv;
            }
        }

        public byte[] EncryptText(string plainText)
        {
            using (Aes aesAlg = new AesCng(_keyName))
            {
                aesAlg.IV = GenerateIV();

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                        byte[] encryptedData = msEncrypt.ToArray();
                        byte[] combinedData = aesAlg.IV.Concat(encryptedData).ToArray();
                        return combinedData;
                    }
                }
            }
        }

        public string DecryptText(byte[] combinedData)
        {
            byte[] iv = combinedData.Take(16).ToArray();
            byte[] cipherText = combinedData.Skip(16).ToArray();

            using (Aes aesAlg = new AesCng(_keyName))
            {
                aesAlg.IV = iv;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                using (var msDecrypt = new MemoryStream(cipherText))
                {
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (var srDecrypt = new StreamReader(csDecrypt))
                        {
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}
