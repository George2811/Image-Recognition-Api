using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageRecognitionApi.Domain.Services.Communications
{
    public class ImageResponse : BaseResponse<double>
    {
        public ImageResponse(double resource) : base(resource)
        {
        }

        public ImageResponse(string message) : base(message)
        {
        }
    }
}
