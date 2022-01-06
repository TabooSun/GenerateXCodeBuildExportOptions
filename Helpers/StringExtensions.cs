namespace GenerateXCodeBuildExportOptions.Helpers;

public static class StringExtensions
{
    public static bool IsNullOrWhiteSpace(this string? value) => string.IsNullOrWhiteSpace(value);
    public static bool IsNotNullOrWhiteSpace(this string? value) => !value.IsNullOrWhiteSpace();

    public static bool IsNullOrEmpty(this string? value) => string.IsNullOrEmpty(value);
    public static bool IsNotNullOrEmpty(this string? value) => !value.IsNullOrEmpty();
}
