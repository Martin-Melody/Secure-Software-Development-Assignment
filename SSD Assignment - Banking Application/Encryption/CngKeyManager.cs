using System.Security.Cryptography;

public class CngKeyManager
{
    private string cryptoKeyName = "key name";
    private CngProvider keyStorageProvider = CngProvider.MicrosoftSoftwareKeyStorageProvider;

    public void SetupCngProvider()
    {
        if (!CngKey.Exists(cryptoKeyName, keyStorageProvider))
        {
            CngKeyCreationParameters keyCreationParameters = new CngKeyCreationParameters
            {
                Provider = keyStorageProvider
            };

            // Use the AES algorithm name directly.
           
            CngAlgorithm aesAlgorithm = new CngAlgorithm("AES");

            CngKey.Create(aesAlgorithm, cryptoKeyName, keyCreationParameters);
        }
    }
}
