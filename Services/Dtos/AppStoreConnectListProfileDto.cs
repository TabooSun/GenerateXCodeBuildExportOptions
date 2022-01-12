using System.Text.Json.Serialization;

namespace GenerateXCodeBuildExportOptions.Services.Dtos
{
    public class AppStoreConnectListProfileDto
    {
        public List<Datum> Data { get; set; } = new();
        public List<Included> Included { get; set; } = new();
        public DatumLinks Links { get; set; } = new();
        public Meta Meta { get; set; } = new();
    }

    [JsonSerializable(typeof(AppStoreConnectListProfileDto))]
    [JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
    public partial class AppStoreConnectListProfileDtoJsonContext : JsonSerializerContext
    {
    }

    public class Datum
    {
        public string Type { get; set; } = "";
        public string Id { get; set; } = "";
        public DatumAttributes Attributes { get; set; } = new();
        public Relationships Relationships { get; set; } = new();
        public DatumLinks Links { get; set; } = new();
    }

    [JsonSerializable(typeof(Datum))]
    [JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
    public partial class DatumJsonContext : JsonSerializerContext
    {
    }

    public class DatumAttributes
    {
        public string Name { get; set; } = "";

        public string ProfileContent { get; set; } = "";

        public string Uuid { get; set; } = "";

        public string ProfileType { get; set; } = "";
    }

    [JsonSerializable(typeof(DatumAttributes))]
    [JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
    public partial class DatumAttributesJsonContext : JsonSerializerContext
    {
    }

    public class DatumLinks
    {
        public Uri? Self { get; set; }
    }

    [JsonSerializable(typeof(DatumAttributes))]
    [JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
    public partial class DatumLinksJsonContext : JsonSerializerContext
    {
    }

    public class Relationships
    {
        public BundleId BundleId { get; set; } = new();
    }

    [JsonSerializable(typeof(Relationships))]
    [JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
    public partial class RelationshipsJsonContext : JsonSerializerContext
    {
    }

    public class BundleId
    {
        public Data Data { get; set; } = new();
        public BundleIdLinks Links { get; set; } = new();
    }

    [JsonSerializable(typeof(BundleId))]
    [JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
    public partial class BundleIdJsonContext : JsonSerializerContext
    {
    }

    public class Data
    {
        public string Type { get; set; } = "";
        public string Id { get; set; } = "";
    }

    [JsonSerializable(typeof(Data))]
    [JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
    public partial class DataJsonContext : JsonSerializerContext
    {
    }

    public class BundleIdLinks
    {
        public Uri? Self { get; set; }
        public Uri? Related { get; set; }
    }

    [JsonSerializable(typeof(BundleIdLinks))]
    [JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
    public partial class BundleIdLinksJsonContext : JsonSerializerContext
    {
    }

    public class Included
    {
        public string Type { get; set; } = "";
        public string Id { get; set; } = "";
        public IncludedAttributes Attributes { get; set; } = new();
        public DatumLinks Links { get; set; } = new();
    }

    [JsonSerializable(typeof(Included))]
    [JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
    public partial class IncludedJsonContext : JsonSerializerContext
    {
    }

    public class IncludedAttributes
    {
        public string Identifier { get; set; } = "";
    }

    [JsonSerializable(typeof(IncludedAttributes))]
    [JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
    public partial class IncludedAttributesJsonContext : JsonSerializerContext
    {
    }

    public class Meta
    {
        public Paging Paging { get; set; } = new();
    }

    [JsonSerializable(typeof(Meta))]
    [JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
    public partial class MetaAttributesJsonContext : JsonSerializerContext
    {
    }

    public class Paging
    {
        public long Total { get; set; }
        public long Limit { get; set; }
    }

    [JsonSerializable(typeof(Paging))]
    [JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
    public partial class PagingJsonContext : JsonSerializerContext
    {
    }
}
