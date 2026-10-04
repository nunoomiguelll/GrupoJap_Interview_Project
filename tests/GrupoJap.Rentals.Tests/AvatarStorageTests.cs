using GrupoJap.Rentals.Services;

namespace GrupoJap.Rentals.Tests;

public class AvatarStorageTests
{
    [Fact]
    public void DetectExtension_Jpeg() =>
        Assert.Equal(".jpg", AvatarStorage.DetectExtension([0xFF, 0xD8, 0xFF, 0xE0, 0, 0, 0, 0, 0, 0, 0, 0]));

    [Fact]
    public void DetectExtension_Png() =>
        Assert.Equal(".png", AvatarStorage.DetectExtension([0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, 0, 0, 0, 0]));

    [Fact]
    public void DetectExtension_WebP() =>
        Assert.Equal(".webp", AvatarStorage.DetectExtension("RIFF\0\0\0\0WEBP"u8.ToArray()));

    [Fact]
    public void DetectExtension_RejectsOtherContent()
    {
        Assert.Null(AvatarStorage.DetectExtension("GIF89a......"u8.ToArray()));
        Assert.Null(AvatarStorage.DetectExtension("<svg onload=x>"u8.ToArray()));
        Assert.Null(AvatarStorage.DetectExtension("MZ\x90\0\x03\0\0\0\x04\0\0\0"u8.ToArray()));
    }

    [Fact]
    public void DetectExtension_RejectsEmptyOrTooShort()
    {
        Assert.Null(AvatarStorage.DetectExtension([]));
        Assert.Null(AvatarStorage.DetectExtension([0xFF, 0xD8]));
    }
}
