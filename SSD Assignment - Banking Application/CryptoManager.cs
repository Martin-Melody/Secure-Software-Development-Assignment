using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SSD_Assignment___Banking_Application
{
    public class CryptoManager
    {
        private Aes aes;

        public CryptoManager()
        {
            InitializeAes();
        }

        private void InitializeAes()
        {
            string cryptoKeyName = "key name";
            CngProvider keyStorageProvider = CngProvider.MicrosoftSoftwareKeyStorageProvider;

            // Check if the key already exists.
            if (!CngKey.Exists(cryptoKeyName, keyStorageProvider))
            {
                // If it doesn't exist, create it.
                CngKeyCreationParameters keyCreationParameters = new CngKeyCreationParameters()
                {
                    Provider = keyStorageProvider,
                    ExportPolicy = CngExportPolicies.None,
                };

                // You may need to specify additional parameters depending on your security requirements.
                CngAlgorithm aesAlgorithm = new CngAlgorithm("AES");

                // Create the key.
                CngKey.Create(aesAlgorithm, cryptoKeyName, keyCreationParameters);
            }

            // Now that we've ensured the key exists, create the AesCng object.
            aes = new AesCng(cryptoKeyName, keyStorageProvider);
        }

        public string ConverToBase64(byte[] encryptedData)
        {
            return Convert.ToBase64String(encryptedData);
        }


        public byte[] EncryptText(string text)
        {
            byte[] plaintextData = Encoding.ASCII.GetBytes(text);
            return Encrypt(plaintextData);
        }

        public string DecryptText(byte[] encryptedData)
        {
            byte[] decryptedData = Decrypt(encryptedData);
            return Encoding.ASCII.GetString(decryptedData);
        }

        private byte[] Encrypt(byte[] plaintextData)
        {
            using (var msEncrypt = new MemoryStream())
            using (var encryptor = aes.CreateEncryptor())
            using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
            {
                csEncrypt.Write(plaintextData, 0, plaintextData.Length);
                csEncrypt.FlushFinalBlock();
                return msEncrypt.ToArray();
            }
        }

        private byte[] Decrypt(byte[] encryptedData)
        {
            using (var msDecrypt = new MemoryStream(encryptedData))
            using (var decryptor = aes.CreateDecryptor())
            using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
            {
                byte[] plaintextData = new byte[encryptedData.Length];
                int bytesRead = csDecrypt.Read(plaintextData, 0, plaintextData.Length);
                Array.Resize(ref plaintextData, bytesRead);
                return plaintextData;
            }
        }
    }
}
