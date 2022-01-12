using System.Net.Http.Headers;
using System.Text.Json;
using GenerateXCodeBuildExportOptions.Models;
using GenerateXCodeBuildExportOptions.Services.Dtos;
using GenerateXCodeBuildExportOptions.Utils;
using Microsoft.IdentityModel.Tokens;

namespace GenerateXCodeBuildExportOptions.Services;

public class AppStoreConnectApiHandler
{
    private static readonly HttpClient HttpClient = new();
    private const string DefaultAppStoreConnectAudience = "appstoreconnect-v1";

    public HashSet<string> BundleIds { get; }

    public string AppStoreConnectApiPrivateKeyPath { get; }

    public string AppStoreConnectApiPrivateKeyId { get; }

    public string AppStoreConnectIssuerId { get; }

    public AppStoreConnectApiHandler(
        HashSet<string> bundleIds,
        string appStoreConnectApiPrivateKeyPath,
        string appStoreConnectApiPrivateKeyId,
        string appStoreConnectIssuerId)
    {
        BundleIds = bundleIds;
        AppStoreConnectApiPrivateKeyPath = appStoreConnectApiPrivateKeyPath;
        AppStoreConnectIssuerId = appStoreConnectIssuerId;
        AppStoreConnectApiPrivateKeyId = appStoreConnectApiPrivateKeyId;
    }

    /// <summary>
    /// Download provisioning profiles and install them.
    ///
    /// Check the API [here](https://developer.apple.com/documentation/appstoreconnectapi/list_and_download_profiles).
    /// </summary>
    /// <returns>The paths of all the installed provisioning profiles.</returns>
    public async Task<IEnumerable<DownloadedProvisioningProfile>> DownloadProvisioningProfilesAsync()
    {
        var appStoreConnectDownloadProvisioningProfileApiUriBuilder =
            new UriBuilder(new Uri("https://api.appstoreconnect.apple.com/v1/profiles"));
        const string bundleIdToken = "bundleId";
        var queries = new QueryCollection
        {
            { "include", bundleIdToken },
            // Filter only IOS provisioning profiles.
            { "filter[profileType]", new HashSet<string> { "IOS_APP_STORE", "IOS_APP_ADHOC" } },
            { "fields[bundleIds]", "identifier" },
            {
                "fields[profiles]",
                new HashSet<string> { bundleIdToken, "profileContent", "name", "uuid", "profileType" }
            },
        };

        appStoreConnectDownloadProvisioningProfileApiUriBuilder.Query = queries.ToString();
        var appStoreConnectDownloadProvisioningProfileApiUri =
            appStoreConnectDownloadProvisioningProfileApiUriBuilder.Uri;

        var jwtToken = GetJwtToken(new List<string>
        {
            $"GET {appStoreConnectDownloadProvisioningProfileApiUri.PathAndQuery}"
        });
        var appStoreConnectListProfileDto =
            await RequestApiAsync(appStoreConnectDownloadProvisioningProfileApiUri, jwtToken);

        return await ProcessDtoAsync(appStoreConnectListProfileDto);
    }

    private async Task<IEnumerable<DownloadedProvisioningProfile>> ProcessDtoAsync(
        AppStoreConnectListProfileDto appStoreConnectListProfileDto)
    {
        var entitledBundleIds = appStoreConnectListProfileDto.Included
            .Where(x => x.Type == "bundleIds" && BundleIds.Contains(x.Attributes.Identifier))
            .Select(x => x.Id);

        var provisioningProfiles = appStoreConnectListProfileDto.Data
            .Where(x => x.Type == "profiles" && entitledBundleIds.Contains(x.Relationships.BundleId.Data.Id))
            .Select(x => x.Attributes);

        return (await Task.WhenAll(provisioningProfiles.Select(x =>
        {
            var provisioningProfileFilePath = Path.Join(Path.GetTempPath(), $"{x.Name}.mobileprovision");
            return File.WriteAllBytesAsync(provisioningProfileFilePath,
                Convert.FromBase64String(x.ProfileContent)).ContinueWith(_ => new DownloadedProvisioningProfile
            {
                ProvisioningProfileFilePath = provisioningProfileFilePath,
                Type = ProvisioningProfileType.FromName(x.ProfileType),
            });
        }))).ToHashSet();
    }

    private string GetJwtToken(List<string> scopes)
    {
        var dtNow = DateTime.UtcNow;
        // ReSharper disable once JoinDeclarationAndInitializer
        TimeSpan timeSpan;
#if DEBUG
        timeSpan = TimeSpan.FromMinutes(20);
#else
        // 5 minutes should be sufficient for typical API call.
        timeSpan = TimeSpan.FromMinutes(5);
#endif

        var expires = dtNow.Add(timeSpan);
        return JwtTokenKeyUtils.CreateTokenAndSign(
            AppStoreConnectApiPrivateKeyPath,
            AppStoreConnectApiPrivateKeyId,
            new SecurityTokenDescriptor
            {
                Issuer = AppStoreConnectIssuerId,
                IssuedAt = dtNow,
                Expires = expires,
                Audience = DefaultAppStoreConnectAudience,
                Claims = new Dictionary<string, object>
                {
                    {
                        "scope", scopes
                    }
                }
            });
    }

    private async Task<AppStoreConnectListProfileDto> RequestApiAsync(
        Uri appStoreConnectDownloadProvisioningProfileApiUri, string jwtToken)
    {
#if DEBUG
        Console.WriteLine(jwtToken);
#endif
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
        var response
            = await HttpClient.GetAsync(appStoreConnectDownloadProvisioningProfileApiUri);
        response.EnsureSuccessStatusCode();

        var responseContent = response.Content;
        var appStoreConnectListProfileDto =
            await JsonSerializer.DeserializeAsync(
                await responseContent.ReadAsStreamAsync(),
                AppStoreConnectListProfileDtoJsonContext.Default.AppStoreConnectListProfileDto);


        if (appStoreConnectListProfileDto == null)
        {
            throw new Exception("Get provisioning profiles response empty");
        }

        return appStoreConnectListProfileDto;
    }
}
