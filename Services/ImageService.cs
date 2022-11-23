using Amazon;
using Amazon.Rekognition;
using Amazon.Rekognition.Model;
using Amazon.Runtime;
using ImageRecognitionApi.Domain.Services;
using ImageRecognitionApi.Domain.Services.Communications;
using Microsoft.AspNetCore.Http;
using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ImageRecognitionApi.Services
{
    public class ImageService : IImageService
    {
        private readonly float similarityThreshold;
        private readonly String accessKeyID = "a";
        private readonly String secretKey = "b";
        public ImageService()
        {
            similarityThreshold = 70F;
        }
        public async Task<ImageResponse> CompareImages(IFormFile source, IFormFile target)
        {
            var credentials = new BasicAWSCredentials(accessKeyID, secretKey);
            AmazonRekognitionClient rekognitionClient = new AmazonRekognitionClient(credentials, RegionEndpoint.USEast1);

            // Setting Image Source
            Amazon.Rekognition.Model.Image imageSource = new Amazon.Rekognition.Model.Image();
            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    await source.OpenReadStream().CopyToAsync(memoryStream);
                    imageSource.Bytes = memoryStream;
                }
            }
            catch (Exception e)
            {
               throw new Exception("Failed to load source image" + e.Message);
            }

            // Setting Image Target
            Amazon.Rekognition.Model.Image imageTarget = new Amazon.Rekognition.Model.Image();
            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    await target.OpenReadStream().CopyToAsync(memoryStream);
                    imageTarget.Bytes = memoryStream;
                }
            }
            catch (Exception e)
            {
                throw new Exception("Failed to load source imtargetage" + e.Message);
            }

            // Call operation
            CompareFacesRequest compareFacesRequest = new CompareFacesRequest()
            {
                SourceImage = imageSource,
                TargetImage = imageTarget,
                SimilarityThreshold = similarityThreshold
            };
            CompareFacesResponse compareFacesResponse = await rekognitionClient.CompareFacesAsync(compareFacesRequest);

            // Display results
            float result = 0;
            float pleft = 0;
            float ptop = 0;

            foreach (CompareFacesMatch match in compareFacesResponse.FaceMatches)
            {
                if (match.Similarity > result)
                {
                    result = match.Similarity;
                    ComparedFace face = match.Face;
                    BoundingBox position = face.BoundingBox;
                    pleft = position.Left;
                    ptop = position.Top;
                }
            }

            var response = new
            {
                similarity = result,
                leftPosition = pleft,
                topPosition = ptop,
                facesDetected = compareFacesResponse.UnmatchedFaces.Count + compareFacesResponse.FaceMatches.Count
            };
            return new ImageResponse(response);
        }
    }
}
