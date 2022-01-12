using Ardalis.SmartEnum;

namespace GenerateXCodeBuildExportOptions.Models;

public struct DownloadedProvisioningProfile
{
    public string ProvisioningProfileFilePath { get; set; }

    public ProvisioningProfileType Type { get; set; }
}

public sealed class ProvisioningProfileType : SmartEnum<ProvisioningProfileType>
{
    public static readonly ProvisioningProfileType IosAppDevelopment = new("IOS_APP_DEVELOPMENT", 0);
    public static readonly ProvisioningProfileType IosAppAdhoc = new("IOS_APP_ADHOC", 1);
    public static readonly ProvisioningProfileType IosAppAppStore = new("IOS_APP_STORE", 2);

    ProvisioningProfileType(string profileType, int value) : base(profileType, value)
    {
    }
}
