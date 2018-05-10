
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace DataEncryptionLibrary.Interfaces
{
    public interface IRsaEncryptor
    {
        byte[] EncryptRsa(byte[] input);
        byte[] DecryptRsa(byte[] encrypted);
        Task<X509Certificate2> GetCertificate();
    }
}
