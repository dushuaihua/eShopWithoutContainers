namespace eShopWithoutContainers.Services.Catalog.API.Controllers;

[ApiController]
public class PicController : ControllerBase
{
    private readonly IWebHostEnvironment _env;
    private readonly CatalogContext _catalogContext;

    public PicController(IWebHostEnvironment env, CatalogContext catalogContext)
    {
        _env = env;
        _catalogContext = catalogContext;
    }

    [HttpGet]
    [Route("api/v1/catalog/items/{catalogItemId:int}/pic")]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> GetImageAsync(int catalogItemId)
    {
        if (catalogItemId <= 0)
        {
            return BadRequest();
        }

        var item = await _catalogContext.CatalogItems.SingleOrDefaultAsync(ci => ci.Id == catalogItemId);

        if (item != null)
        {
            var webRoot = _env.WebRootPath;
            var path = Path.Combine(webRoot, item.PictureFileName);

            string imageFileExtensions = Path.GetExtension(item.PictureFileName);
            string mimeType = GetImageMimeTypeFromImageFileExtensions(imageFileExtensions);

            var buffer = await System.IO.File.ReadAllBytesAsync(path);

            return File(buffer, mimeType);
        }

        return NotFound();
    }

    private string GetImageMimeTypeFromImageFileExtensions(string extensions)
    {
        string mimeType;

        switch (extensions)
        {
            case ".png":
                mimeType = "image/png";
                break;
            case ".gif":
                mimeType = "image/gif";
                break;
            case ".jpg":
            case ".jpeg":
                mimeType = "image/jpeg";
                break;
            case ".bmp":
                mimeType = "image/bmp";
                break;
            case ".tiff":
                mimeType = "image/tiff";
                break;
            case ".wmf":
                mimeType = "image/wmf";
                break;
            case ".jp2":
                mimeType = "image/jp2";
                break;
            case ".svg":
                mimeType = "image/svg+xml";
                break;
            default:
                mimeType = "application/octet-stream";
                break;
        }
        return mimeType;
    }
}
