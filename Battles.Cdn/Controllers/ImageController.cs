using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Battles.Cdn.FileServices;
using Battles.Cdn.Infrastructure;

namespace Battles.Cdn.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class ImageController : BaseController
    {
        private readonly ImageManager _imageManager;

        public ImageController(ImageManager imageManager)
        {
            _imageManager = imageManager;
        }

        [HttpPost("")]
        public IActionResult Post(IFormFile image)
        {
            var (saved, fileName) = _imageManager.SaveImage(UserId, image);
            if (!saved)
            {
                return BadRequest("Failed To Save Image");
            }

            return Ok(fileName);
        }
    }
}