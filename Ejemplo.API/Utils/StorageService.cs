using Ejemplo.Domain.FileSystem;
using ImageMagick;

public class FileStorageService
{
    private readonly IWebHostEnvironment _env;
    private readonly string _storagePath;
    private readonly string _relativeStoragePath;

    public FileStorageService(IWebHostEnvironment env)
    {
        _env = env;
        _relativeStoragePath = Path.Combine("Storage", "Images");
        _storagePath = Path.Combine(_env.ContentRootPath, _relativeStoragePath);
    }

    public async Task<Image> SaveImageAsync(byte[] data, string fileName)
    {
        Directory.CreateDirectory(_storagePath);

        var extension = Path.GetExtension(fileName);
        var finalFileName = $"{Guid.NewGuid()}{extension}";
        var physicalPath = Path.Combine(_storagePath, finalFileName);
        var relativeStoragePath = Path.Combine(_relativeStoragePath, finalFileName);

        using var image = new MagickImage(data);

        image.Format = MagickFormat.WebP;
        image.Write(physicalPath);

        var size = new FileInfo(physicalPath).Length;

        return new Image
        {
            FileName = finalFileName,
            ContentType = "image/webp",
            StoragePath = relativeStoragePath,
            Width = (int)image.Width,
            Height = (int)image.Height
        };
    }

    public string GetPhysicalPath(string storagePath)
    {
        if (string.IsNullOrWhiteSpace(storagePath))
        {
            return string.Empty;
        }

        if (Path.IsPathRooted(storagePath))
        {
            return storagePath;
        }

        return Path.Combine(_env.ContentRootPath, storagePath);
    }
    //Eliminar imagen
    public Task DeleteFileAsync(string storagePath)
    {
        if (string.IsNullOrWhiteSpace(storagePath))
            return Task.CompletedTask;

        string physicalPath = GetPhysicalPath(storagePath);

        var fileInfo = new FileInfo(physicalPath);
        if (fileInfo.Exists)
            fileInfo.Delete();

        return Task.CompletedTask;
    }
}
