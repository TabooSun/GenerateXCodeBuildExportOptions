namespace GenerateXCodeBuildExportOptions.Models;

public struct ProvisioningProfileMetaData
{
    public string Uuid { get; }

    public string BundleIdentifier { get; }

    public ProvisioningProfileType ProvisioningProfileType { get; set; }

    public ProvisioningProfileMetaData(string bundleIdentifier, string uuid,
        ProvisioningProfileType provisioningProfileType)
    {
        BundleIdentifier = bundleIdentifier;
        Uuid = uuid;
        ProvisioningProfileType = provisioningProfileType;
    }
}
