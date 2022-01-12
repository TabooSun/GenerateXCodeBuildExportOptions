using CommandLine;


namespace GenerateXCodeBuildExportOptions;

public class ArgumentOption
{
    public const string ExportOptionsDefaultFileName = "ExportOptions.plist";

    [Option("bundle-identifier", Required = true, HelpText = "Bundle Identifiers that the project has. Run 'xcodeproj show' in the project to find out.")]
    public IEnumerable<string> BundleIdentifiers { get; set; } = null!;

    [Option("app-store-connect-api-private-key-path", Required = true,
        HelpText =
            "The file path to the App Store Connect API private key. Check https://developer.apple.com/documentation/appstoreconnectapi/creating_api_keys_for_app_store_connect_api for detail.")]
    public string AppStoreConnectApiPrivateKeyPath { get; set; } = null!;

    [Option("app-store-connect-api-private-key-id", Required = true,
        HelpText = "The App Store Connect API private key id.")]
    public string AppStoreConnectApiPrivateKeyId { get; set; } = null!;

    [Option("app-store-connect-issuer-id", Required = true,
        HelpText =
            "The App Store Connect issuer id. You can get it from https://appstoreconnect.apple.com/access/api. Go to the API key tab to copy it.")]
    public string AppStoreConnectIssuerId { get; set; } = null!;

    [Option('o', "output", Default = ExportOptionsDefaultFileName, HelpText = "The ExportOptions.plist output path.")]
    public string Output { get; set; } = null!;
}
