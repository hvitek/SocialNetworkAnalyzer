using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace SocialNetworkAnalyzer.Models
{
    public class FileUploadRequest
    {
        public int? DatasetId { get; set; }

        public IFormFile FormFile { get; set; }
    }
}