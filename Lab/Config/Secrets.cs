using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace Lab.Config;

public static class Secrets
{
    public static string BrevoApiKey { get; set; } = string.Empty;
    public static string ResendApiKey { get; set; } = string.Empty;

    public static string MailjetApiKey { get; set; } = string.Empty;
    public static string MailjetSecret { get; set; } = string.Empty;

    public static string PostmarkApiKey { get; set; } = string.Empty;
}