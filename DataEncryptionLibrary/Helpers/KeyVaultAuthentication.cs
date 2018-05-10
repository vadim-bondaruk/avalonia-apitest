using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataEncryptionLibrary.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace DataEncryptionLibrary.Services
{
    public class KeyVaultAuthentication : IKeyVaultAuthentication
    {
        private IConfiguration _config;

        public KeyVaultAuthentication(IConfiguration config)
        {
            _config = config;
        }

        public async Task<string> GetToken(string authority, string resource, string scope)
        {
            var authContext = new AuthenticationContext(authority);
            ClientCredential clientCred = new ClientCredential(_config["KeyVault:ApplicationId"], _config["KeyVault:ApplicationKey"]);
            AuthenticationResult result = await authContext.AcquireTokenAsync(resource, clientCred);

            if (result == null)
            {
                throw new InvalidOperationException("Failed to obtain the JWT token");
            }

            return result.AccessToken;
        }
    }
}
