namespace GrupoJap.Rentals.Services;

/// <summary>Guarda e remove fotos de perfil em {Uploads:Path}/avatars (pasta partilhada pelas duas aplicações), validando tipo real e tamanho.</summary>
public sealed class AvatarStorage(IWebHostEnvironment environment, IConfiguration configuration)
{
    public const long MaxBytes = 2 * 1024 * 1024;
    private const string PublicFolder = "/uploads/avatars/";

    /// <summary>Pasta física dos uploads: Uploads:Path (relativo à raiz da aplicação) ou "uploads" por omissão.</summary>
    public static string ResolveUploadsRoot(IWebHostEnvironment environment, IConfiguration configuration)
        => Path.GetFullPath(configuration["Uploads:Path"] is { Length: > 0 } path ? path : "uploads", environment.ContentRootPath);

    /// <summary>Deteta a extensão a partir dos primeiros bytes (não confia no nome nem no Content-Type).</summary>
    public static string? DetectExtension(ReadOnlySpan<byte> header)
    {
        if (header.Length >= 3 && header[0] == 0xFF && header[1] == 0xD8 && header[2] == 0xFF)
        {
            return ".jpg";
        }

        if (header.Length >= 8 && header[0] == 0x89 && header[1] == 0x50 && header[2] == 0x4E && header[3] == 0x47
            && header[4] == 0x0D && header[5] == 0x0A && header[6] == 0x1A && header[7] == 0x0A)
        {
            return ".png";
        }

        if (header.Length >= 12 && header[0] == (byte)'R' && header[1] == (byte)'I' && header[2] == (byte)'F' && header[3] == (byte)'F'
            && header[8] == (byte)'W' && header[9] == (byte)'E' && header[10] == (byte)'B' && header[11] == (byte)'P')
        {
            return ".webp";
        }

        return null;
    }

    public async Task<(string? Path, string? Error)> SaveAsync(IFormFile file, CancellationToken cancellationToken = default)
    {
        if (file.Length == 0)
        {
            return (null, "profile.photo_empty");
        }

        if (file.Length > MaxBytes)
        {
            return (null, "profile.photo_too_large");
        }

        var header = new byte[12];
        await using (var probe = file.OpenReadStream())
        {
            var read = await probe.ReadAtLeastAsync(header, header.Length, throwOnEndOfStream: false, cancellationToken);
            var extension = DetectExtension(header.AsSpan(0, read));
            if (extension is null)
            {
                return (null, "profile.photo_invalid");
            }

            var folder = Path.Combine(ResolveUploadsRoot(environment, configuration), "avatars");
            Directory.CreateDirectory(folder);

            var fileName = Guid.NewGuid().ToString("N") + extension;
            await using var target = File.Create(Path.Combine(folder, fileName));
            probe.Position = 0;
            await probe.CopyToAsync(target, cancellationToken);

            return (PublicFolder + fileName, null);
        }
    }

    public void Delete(string? publicPath)
    {
        if (string.IsNullOrEmpty(publicPath) || !publicPath.StartsWith(PublicFolder, StringComparison.Ordinal))
        {
            return;
        }

        // GetFileName impede path traversal; só se apaga dentro da pasta de avatares.
        var fullPath = Path.Combine(ResolveUploadsRoot(environment, configuration), "avatars", Path.GetFileName(publicPath));
        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }
    }
}
