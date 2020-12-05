using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;

namespace Huby.Controllers
{
    [ApiController]
    [Route("api/s3")]
    public sealed class S3Controller : ControllerBase
    {
        private ILogger<S3Controller> Logger { get; }
        private AmazonS3Client S3Client      { get; }
        private string ImageBucketName       { get; }

        public S3Controller(IConfiguration configuration, ILogger<S3Controller> logger, AmazonS3Client s3)
        {
            Logger = logger;
            S3Client = s3;
            ImageBucketName = configuration["S3:ImageBucketName"];
        }

        [Authorize]
        [HttpGet("image")]
        public ActionResult<string> GetSignedUploadUrl(string format = "image/webp")
        {
            var ext = "";

            switch (format) {
                case "image/webp":
                    ext = "webp";
                    break;

                case "image/jpeg":
                    ext = "jpeg";
                    break;

                case "image/png":
                    ext = "png";
                    break;

                case "image/gif":
                    ext = "gif";
                    break;

                default:
                    return BadRequest($"Invalid image type \"{format}\"");
            }

            var guid = Guid.NewGuid();
            var key = $"{guid}.{ext}";
            var getUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}{HttpContext.Request.Path}/{key}";

            var putUrl = S3Client.GetPreSignedURL(new GetPreSignedUrlRequest
            {
                BucketName  = ImageBucketName,
                Key         = key,
                Verb        = HttpVerb.PUT,
                Expires     = DateTime.UtcNow.AddMinutes(5),
                ContentType = format
            });

            return Created(getUrl, new
            {
                Get = getUrl,
                Put = putUrl,
            });
        }

        [HttpGet("image/{key}")]
        public ActionResult<string> GetPreSignedUrl(string key)
        {
            var url = S3Client.GetPreSignedURL(new GetPreSignedUrlRequest
            {
                BucketName  = ImageBucketName,
                Key         = key,
                Verb        = HttpVerb.GET,
                Expires     = DateTime.UtcNow.AddMinutes(5),
            });

            return Redirect(url);
        }
    }
}
