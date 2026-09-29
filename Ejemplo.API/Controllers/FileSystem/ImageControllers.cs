using Ejemplo.DAL.interfaces;
using Ejemplo.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Ejemplo.API.Controllers;

[ApiController]
[Route("api/images")]
public class ImagesController : ControllerBase
{
    private IUnitOfWork dataBase;
    private IWebHostEnvironment env;

    public ImagesController(IUnitOfWork unitOfWork, IWebHostEnvironment env)
    {
        this.dataBase = unitOfWork;
        this.env = env;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetImage(long id, CancellationToken cancellationToken)
    {
        Ejemplo.Domain.FileSystem.File? file = await this.dataBase.FileRepository.GetById(id);
        
        if (file is null)
        {
            throw new BusinessNotFoundException("La imagen no existe en la base de datos");
        }

        string quesoy = file.QueSoy();

        FileStorageService fileStorage = new FileStorageService(this.env);
        var physicalPath = fileStorage.GetPhysicalPath(file.StoragePath);

        if (string.IsNullOrWhiteSpace(physicalPath) || !System.IO.File.Exists(physicalPath))
        {
            throw new BusinessNotFoundException("No se encontró la imagen física");
        }

        var contentType = string.IsNullOrWhiteSpace(file.ContentType) ? "application/octet-stream" : file.ContentType;
        return PhysicalFile(physicalPath, contentType);
    }
}