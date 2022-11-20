using ImageRecognitionApi.Domain.Services;
using ImageRecognitionApi.Domain.Services.Communications;
using ImageRecognitionApi.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageRecognitionApi.Controllers
{
    [Route("api/images")]
    [Produces("application/json")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly IImageService _imageService;

        public ImagesController(IImageService imageService)
        {
            _imageService = imageService;
        }

        /************************ Compare Images ************************/

        [HttpPost]
        [ProducesResponseType(typeof(ImageResponse), 200)]
        [ProducesResponseType(typeof(BadRequestResult), 404)]
        public async Task<IActionResult> CompareImages(IFormFile source, IFormFile target)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.GetErrorMessages());

            var result = await _imageService.CompareImages(source, target);
            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result);
        }
    }
}
