using CommandLine;


namespace GenerateXCodeBuildExportOptions;

public class ArgumentOption
{
    public const string ExportOptionsDefaultFileName = "ExportOptions.plist";

    [Option("provisioning-profiles", Required = true, HelpText = "Provisioning profiles to install and write to ExportOptions.plist.")]
    public IEnumerable<string> ProvisioningProfiles { get; set; } = null!;

    [Option('o', "output", Default = ExportOptionsDefaultFileName, HelpText = "The ExportOptions.plist output path.")]
    public string Output { get; set; } = null!;
}
