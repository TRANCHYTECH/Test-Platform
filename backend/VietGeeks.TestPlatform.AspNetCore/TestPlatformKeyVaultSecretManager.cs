using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Configuration;

namespace VietGeeks.TestPlatform.AspNetCore
{
    public class TestPlatformKeyVaultSecretManager : KeyVaultSecretManager
    {
        private const string SharedPrefix = "shared-";
        private readonly IReadOnlyCollection<string> _allowedKeys;
        private readonly string _prefix;

        public TestPlatformKeyVaultSecretManager(string prefix, IReadOnlyCollection<string> allowedKeys)
        {
            ArgumentException.ThrowIfNullOrEmpty(prefix);
            _prefix = $"{prefix}-";
            _allowedKeys = allowedKeys;
        }

        public override string GetKey(KeyVaultSecret secret) => GetKey(secret.Name);

        private string GetKey(string secretName)
        {
            return secretName.StartsWith(_prefix, StringComparison.OrdinalIgnoreCase) ?
                secretName[_prefix.Length..].Replace("--", ConfigurationPath.KeyDelimiter, StringComparison.OrdinalIgnoreCase) :
                secretName[SharedPrefix.Length..].Replace("--", ConfigurationPath.KeyDelimiter, StringComparison.OrdinalIgnoreCase);
        }

        public override bool Load(SecretProperties secret) =>
            secret.Enabled == true && (!secret.ExpiresOn.HasValue || secret.ExpiresOn > DateTimeOffset.UtcNow)
                                   && (secret.Name.StartsWith(_prefix, StringComparison.OrdinalIgnoreCase) || secret.Name.StartsWith(SharedPrefix, StringComparison.OrdinalIgnoreCase))
                                   && _allowedKeys.Contains(GetKey(secret.Name), StringComparer.OrdinalIgnoreCase);
    }
}
