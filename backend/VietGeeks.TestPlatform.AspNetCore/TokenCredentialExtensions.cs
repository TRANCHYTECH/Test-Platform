﻿using Azure.Core;
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
                return new EnvironmentCredential(tokenCredentialOptions);
            }

            // Use the user assigned managed identity via env AZURE_CLIENT_ID
            return new DefaultAzureCredential();
        }
    }
}
