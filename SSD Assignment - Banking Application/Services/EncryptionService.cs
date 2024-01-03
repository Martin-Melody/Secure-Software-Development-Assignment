using Banking_Application;
using SSD_Assignment___Banking_Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SSD_Assignment___Banking_Application.Services
{
    public class EncryptionService : IEncryptionService
    {
        private readonly CryptoManager _cryptoManager;

        public EncryptionService(CryptoManager cryptoManager)
        {
            _cryptoManager = cryptoManager;
        }

        public string Encrypt(string plainText)
        {
            // Convert the plain text to a byte array and encrypt it
            byte[] encryptedBytes = _cryptoManager.EncryptText(plainText);
            return Convert.ToBase64String(encryptedBytes);
        }

        public string Decrypt(string cipherText)
        {
            // Convert the cipher text from Base64 to a byte array and decrypt it
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            return _cryptoManager.DecryptText(cipherBytes);
        }

        public string CalculateRowHash(Bank_Account row)
        {
            return _cryptoManager.CalculateSHA256(row);
        }
    }
}
