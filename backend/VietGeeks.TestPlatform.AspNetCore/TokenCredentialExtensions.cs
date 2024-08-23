using Azure.Core;
using Azure.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace VietGeeks.TestPlatform.AspNetCore
{
    public static class TokenCredentialExtensions
    {
        public static TokenCredential GetTokenCredential(this IWebHostEnvironment environment, TokenCredentialOptions? tokenCredentialOptions = null)
        {
            if (environment.IsDevelopment())
            {
                var token = new EnvironmentCredential(tokenCredentialOptions);
                return token;
            }

            return new ManagedIdentityCredential();
        }
    }
}
