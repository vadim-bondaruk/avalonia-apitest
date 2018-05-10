using System.Threading.Tasks;

namespace DataEncryptionLibrary.Interfaces
{
    public interface IKeyVaultAuthentication
    {
        Task<string> GetToken(string authority, string resource, string scope);
    }
}