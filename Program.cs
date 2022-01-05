// See https://aka.ms/new-console-template for more information

using System.Text;
using Claunia.PropertyList;
using CommandLine;
using GenerateXCodeBuildExportOptions;

await Parser.Default.ParseArguments<ArgumentOption>(args).MapResult(async option =>
{
    try
    {
        if (option.ProvisioningProfiles.Any())
        {
            Console.WriteLine("Start installing provisioning profiles...");
            await Task.WhenAll(option.ProvisioningProfiles.Select(InstallProvisioningProfileAsync));
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

    var mobileProvisionString = await File.ReadAllTextAsync(provisioningProfileFilePath);
    var startIndex =
        mobileProvisionString.IndexOf("<?xml version=\"1.0\" encoding=\"UTF-8\"?>", StringComparison.Ordinal);
    const string plistXmlClosingTag = "</plist>";
    var endIndex = mobileProvisionString.IndexOf(plistXmlClosingTag, StringComparison.Ordinal);
    var mobileProvisionContent =
        mobileProvisionString.Substring(startIndex, endIndex - startIndex + plistXmlClosingTag.Length);
    var rootDict = PropertyListParser.Parse(Encoding.UTF8.GetBytes(mobileProvisionContent)) as NSDictionary;
    var provisioningProfileUuid = rootDict!.ObjectForKey("UUID").ToString()!;
    File.Copy(provisioningProfileFilePath,
        Path.Combine(macOsProvisioningProfileDirectoryPath,
            Path.ChangeExtension(provisioningProfileUuid, provisioningProfileFileInfo.Extension)), true);

    Console.WriteLine($"Done installing Provisioning profile: {provisioningProfileFilePath}");
}
