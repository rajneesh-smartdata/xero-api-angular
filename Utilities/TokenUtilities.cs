using System.IO;
using System.Text.Json;
using Xero.NetStandard.OAuth2.Token;

public static class TokenUtilities
{
    public static void StoreToken(XeroOAuth2Token xeroToken)
    {
        string serializedXeroToken = JsonSerializer.Serialize(xeroToken);
        //System.IO.File.WriteAllText("./xerotoken.json", serializedXeroToken);
        var _filepath = xeroToken.Tenants.Count > 0 ? "./" + xeroToken.Tenants[0].TenantId.ToString().ToLower() + ".json" : "./xerotoken.json";
        System.IO.File.WriteAllText(_filepath, serializedXeroToken);
    }

    public static XeroOAuth2Token GetStoredToken(string TenantId = null)
    {
        var _filepath = !string.IsNullOrEmpty(TenantId) ? "./" + TenantId.ToString().ToLower() + ".json" : "./xerotoken.json";
        string serializedXeroToken = System.IO.File.ReadAllText(_filepath);// "./xerotoken.json";
        var xeroToken = JsonSerializer.Deserialize<XeroOAuth2Token>(serializedXeroToken);

        return xeroToken;
    }

    public static bool TokenExists(string TenantId = null)
    {
        var _filepath = !string.IsNullOrEmpty(TenantId) ? "./" + TenantId.ToString().ToLower() + ".json" : "./xerotoken.json";
        string serializedXeroTokenPath = _filepath;// "./xerotoken.json";
        bool fileExist = File.Exists(serializedXeroTokenPath);

        return fileExist;
    }

    public static void DestroyToken(string TenantId = null)
    {
        var _filepath = !string.IsNullOrEmpty(TenantId) ? "./" + TenantId.ToString().ToLower() + ".json" : "./xerotoken.json";
        string serializedXeroTokenPath = _filepath; // "./xerotoken.json";
        File.Delete(serializedXeroTokenPath);

        return;
    }
}