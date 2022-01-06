// See https://aka.ms/new-console-template for more information

using System.Reflection;
using System.Text;
using Claunia.PropertyList;
using CommandLine;
using GenerateXCodeBuildExportOptions;
using GenerateXCodeBuildExportOptions.Model;

await Parser.Default.ParseArguments<ArgumentOption>(args).MapResult(async option =>
{
    try
    {
        if (option.ProvisioningProfiles.Any())
        {
            Console.WriteLine("Start installing provisioning profiles...");
            await Task.WhenAll(option.ProvisioningProfiles.Select(InstallProvisioningProfileAsync));
            var provisioningProfileDatum =
                await Task.WhenAll(option.ProvisioningProfiles.Select(ExtractProvisioningProfileMetaDatumAsync));

            GenerateExportOptions(provisioningProfileDatum, option.Output);
        }


        Console.WriteLine("Done");
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex);
        throw;
    }
}, errors =>
{
    foreach (var err in errors)
    {
        Console.WriteLine(err.ToString());
    }

    return Task.FromResult(1);
});

void GenerateExportOptions(IEnumerable<ProvisioningProfileMetaData?> provisioningProfileMetaDatum,
    string optionOutput)
{
    provisioningProfileMetaDatum =
        provisioningProfileMetaDatum.Where(x => x != null);

    // Use Assembly.GetExecutingAssembly().GetManifestResourceNames() to get the Manifest Resource name.

    var exportOptionsBootstrapStream = Assembly.GetExecutingAssembly()
        .GetManifestResourceStream($"{nameof(GenerateXCodeBuildExportOptions)}.Resources.ExportOptions.plist");

    var rootDict = (PropertyListParser.Parse(exportOptionsBootstrapStream) as NSDictionary)!;
    var provisioningProfiles = new NSDictionary();
    rootDict["provisioningProfiles"] = provisioningProfiles;

    foreach (var provisioningProfileMetadata in provisioningProfileMetaDatum.Cast<ProvisioningProfileMetaData>())
    {
        provisioningProfiles.Add(provisioningProfileMetadata.BundleIdentifier, provisioningProfileMetadata.Uuid);
    }

    var outputFileInfo = new FileInfo(optionOutput);
    PropertyListParser.SaveAsXml(rootDict, outputFileInfo);
}

async Task<ProvisioningProfileMetaData> ExtractProvisioningProfileMetaDatumAsync(string provisioningProfileFilePath)
{
    var rootDict = (await GetProvisioningProfileAsNsDictionaryAsync(provisioningProfileFilePath))!;
    var provisioningProfileUuid = ExtractUuidFromProvisioningProfile(rootDict);

    var applicationIdentifierPrefixes = (rootDict.ObjectForKey("ApplicationIdentifierPrefix") as NSArray)!;
    var applicationIdentifier =
        (rootDict.ObjectForKey("Entitlements") as NSDictionary)!.ObjectForKey("application-identifier").ToString()!;
    var applicationIdentifierPrefix = applicationIdentifierPrefixes.FirstOrDefault(x => applicationIdentifier.StartsWith(x.ToString()!));
    var bundleIdentifier = applicationIdentifier.Replace($"{applicationIdentifierPrefix}.", "");
    return new ProvisioningProfileMetaData(bundleIdentifier, provisioningProfileUuid);
}

async Task InstallProvisioningProfileAsync(string provisioningProfileFilePath)
{
    var macOsProvisioningProfileDirectoryPath =
        Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) +
        @"/Library/MobileDevice/Provisioning Profiles";
    // If the file has already been installed, ignore it.

    var provisioningProfileFileInfo = new FileInfo(provisioningProfileFilePath);
    if (provisioningProfileFileInfo.DirectoryName == macOsProvisioningProfileDirectoryPath)
    {
        Console.WriteLine(
            $"Provisioning profile: {provisioningProfileFilePath} has already been installed. Skipping...");
        return;
    }

    var rootDict = (await GetProvisioningProfileAsNsDictionaryAsync(provisioningProfileFilePath))!;
    var provisioningProfileUuid = ExtractUuidFromProvisioningProfile(rootDict);
    File.Copy(provisioningProfileFilePath,
        Path.Combine(macOsProvisioningProfileDirectoryPath,
            Path.ChangeExtension(provisioningProfileUuid, provisioningProfileFileInfo.Extension)), true);

    Console.WriteLine($"Done installing Provisioning profile: {provisioningProfileFilePath}");
}

async Task<NSDictionary?> GetProvisioningProfileAsNsDictionaryAsync(string provisioningProfileFilePath)
{
    var mobileProvisionString = await File.ReadAllTextAsync(provisioningProfileFilePath);
    var startIndex =
        mobileProvisionString.IndexOf("<?xml version=\"1.0\" encoding=\"UTF-8\"?>", StringComparison.Ordinal);
    const string plistXmlClosingTag = "</plist>";
    var endIndex = mobileProvisionString.IndexOf(plistXmlClosingTag, StringComparison.Ordinal);
    var mobileProvisionContent =
        mobileProvisionString.Substring(startIndex, endIndex - startIndex + plistXmlClosingTag.Length);

    var rootDict = PropertyListParser.Parse(Encoding.UTF8.GetBytes(mobileProvisionContent)) as NSDictionary;
    return rootDict;
}

string ExtractUuidFromProvisioningProfile(NSDictionary nsDictionary)
{
    return nsDictionary.ObjectForKey("UUID").ToString()!;
}
