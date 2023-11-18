namespace MinApiOnNet.Extensions;

public static class StringExtensions
{
    public static string NormalizeUrl(this string url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return string.Empty;
        }

        if (!Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out _))
        {
            return string.Empty;
        }

        var result = url.Split('?', '#')[0].TrimEnd('/');

        var parts = result.Split('/')
            .Select(part =>
                int.TryParse(part, out _) ||
                Guid.TryParse(part, out _) ||
                DateTime.TryParse(part, out _)
                    ? "0"
                    : part);

        return string.Join('/', parts);
    }
}