using DataEncryptionLibrary.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace DataEncryptionLibrary.Helpers
{
    public class RsaEncryptor : IRsaEncryptor
    {
        private X509Certificate2 cert;
        private readonly IConfiguration _config;
        public RsaEncryptor(IConfiguration config)
        {
            _config = config;
            if (cert == null)
                cert = GetCertificate().Result;
        }

        public byte[] EncryptRsa(byte[] input)
        {
            byte[] bytesEncrypted;
            using (RSA csp = (RSA)cert.PublicKey.Key)
            {
                bytesEncrypted = csp.Encrypt(input, RSAEncryptionPadding.Pkcs1);
            }
            return bytesEncrypted;
        }

        public byte[] DecryptRsa(byte[] encrypted)
        {
            byte[] bytesDecrypted;
            using (RSA csp = (RSA)cert.PrivateKey)
            {
                bytesDecrypted = csp.Decrypt(encrypted, RSAEncryptionPadding.Pkcs1);
            }
            return bytesDecrypted;
        }

        public async Task<X509Certificate2> GetCertificate()
        {
            X509Store certStore = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            certStore.Open(OpenFlags.ReadOnly);
            X509Certificate2Collection certCollection = certStore.Certificates.Find(X509FindType.FindByThumbprint,
                                        // Replace below with your cert's thumbprint
                                        "9DCB2EC05637DBD73C9CACBE56D252319D858776", false);
            X509Certificate2 cert = new X509Certificate2();
            // Get the first cert with the thumbprint
            if (certCollection.Count > 0)
            {
                cert = certCollection[0];
            }

            certStore.Close();
            return cert;
            //var _keyVaultAuth = new KeyVaultAuthentication(_config);
            //var keyVaultClient = new KeyVaultClient(_keyVaultAuth.GetToken);
            //var secret = await keyVaultClient.GetSecretAsync(_config["KeyVault:IdentitySigningRSAKeyURL"]);
            //var bytes = System.Convert.FromBase64String(secret.Value);
            //return new X509Certificate2(bytes);
        }

    }
}
