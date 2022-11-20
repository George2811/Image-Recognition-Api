using ImageRecognitionApi.Domain.Services.Communications;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageRecognitionApi.Domain.Services
{
    public interface IImageService
    {
        Task<ImageResponse> CompareImages(IFormFile source, IFormFile target);
    }
}
