using Microsoft.Extensions.Configuration;

namespace ClaimManagement.Infrastructure.Config;

public static class RequiredDocuments
{
    // the documents that must be uploaded and verified for a claim type (from appsettings.json)
    public static List<string> For(IConfiguration configuration, string claimType)
    {
        return configuration.GetSection("ClaimRules:RequiredDocuments:" + claimType)
            .GetChildren()
            .Select(c => c.Value ?? "")
            .Where(v => v != "")
            .ToList();
    }
}