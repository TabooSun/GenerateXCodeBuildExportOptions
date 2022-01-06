namespace GenerateXCodeBuildExportOptions.Model;

public class ProvisioningProfileMetaData
{
    public string Uuid { get; }

    public string BundleIdentifier { get; }

    public ProvisioningProfileMetaData(string bundleIdentifier, string uuid)
    {
        BundleIdentifier = bundleIdentifier;
        Uuid = uuid;
    }
}
