using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResiduosBackend.DTO;
using ResiduosBackend.Interfaces;

namespace ResiduosBackend.Controllers;

/// <summary>
/// Catálogo de partes del avatar para personalización (cuerpo, cara, sombrero).
/// Es solo lectura: el catálogo se siembra en BD vía migración y rara vez cambia.
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class AvatarPartsController : ControllerBase
{
    private readonly IAvatarPartService _service;

    public AvatarPartsController(IAvatarPartService service)
    {
        _service = service;
    }

    /// <summary>
    /// Devuelve el catálogo completo agrupado por slot.
    /// El cliente Unity lo descarga una vez al iniciar sesión y lo cachea.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<AvatarPartsCatalogoDTO>> GetCatalogo()
    {
        var catalogo = await _service.ObtenerCatalogoAsync();
        return Ok(catalogo);
    }
}
