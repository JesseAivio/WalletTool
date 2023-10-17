
using Azure.Security.KeyVault.Secrets;
using Azure.Extensions.AspNetCore.Configuration.Secrets;
namespace WalletTool.UI;

public class CustomKeyVaultSecretManager : KeyVaultSecretManager
{
    public override string GetKey(KeyVaultSecret secret)
    {
        // Convert dashes back to colons
        return secret.Name.Replace('-', ':');
    }
}