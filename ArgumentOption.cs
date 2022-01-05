using CommandLine;

namespace GenerateXCodeBuildExportOptions;

public class ArgumentOption
{
    [Option("provisioning-profiles",HelpText = "Provisioning profiles to install.")]
    public IEnumerable<string> ProvisioningProfiles { get; set; } = null!;
}
