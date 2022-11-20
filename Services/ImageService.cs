using Amazon.Rekognition;
using Amazon.Rekognition.Model;
using ImageRecognitionApi.Domain.Services;
using ImageRecognitionApi.Domain.Services.Communications;
using Microsoft.AspNetCore.Http;
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
        private String sourceImage;
        private String targetImage;
         
        public ImageService()
        {
            similarityThreshold = 70F;
        }
        public async Task<ImageResponse> CompareImages(IFormFile source, IFormFile target)
        {
            AmazonRekognitionClient rekognitionClient = new AmazonRekognitionClient();

            // Setting Image Source
            Amazon.Rekognition.Model.Image imageSource = new Amazon.Rekognition.Model.Image();
            using (var memoryStream = new MemoryStream())
            {
                await source.OpenReadStream().CopyToAsync(memoryStream);

                var fileBytes = memoryStream.ToArray();
                sourceImage = Convert.ToBase64String(fileBytes);
            }
            try
            {
                using (FileStream fs = new FileStream(sourceImage, FileMode.Open, FileAccess.Read))
                {
                    byte[] data = new byte[fs.Length];
                    fs.Read(data, 0, (int)fs.Length);
                    imageSource.Bytes = new MemoryStream(data);
                }
            }
            catch (Exception e)
            {
               throw new Exception("Failed to load source image" + e.Message);
            }

            // Setting Image Target
            Amazon.Rekognition.Model.Image imageTarget = new Amazon.Rekognition.Model.Image();
            using (var memoryStream = new MemoryStream())
            {
                await target.OpenReadStream().CopyToAsync(memoryStream);

                var fileBytes = memoryStream.ToArray();
                targetImage = Convert.ToBase64String(fileBytes);
            }
            try
            {
                using (FileStream fs = new FileStream(targetImage, FileMode.Open, FileAccess.Read))
                {
                    byte[] data = new byte[fs.Length];
                    data = new byte[fs.Length];
                    fs.Read(data, 0, (int)fs.Length);
                    imageTarget.Bytes = new MemoryStream(data);
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
            foreach (CompareFacesMatch match in compareFacesResponse.FaceMatches)
            {
                ComparedFace face = match.Face;
                BoundingBox position = face.BoundingBox;
                Console.WriteLine("Face at " + position.Left
                      + " " + position.Top
                      + " matches with " + match.Similarity
                      + "% confidence.");
                result = match.Similarity > result ? match.Similarity : result;
            }

            Console.WriteLine("There was " + compareFacesResponse.UnmatchedFaces.Count + " face(s) that did not match");

            return new ImageResponse(result);
        }
    }
}
