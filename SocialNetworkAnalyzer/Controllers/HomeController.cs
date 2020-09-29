using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SocialNetworkAnalyzer.Core.DataService;
using SocialNetworkAnalyzer.EntityFrameworkCore;
using SocialNetworkAnalyzer.Models;

namespace SocialNetworkAnalyzer.Controllers
{
    public class HomeController : Controller
    {
        public HomeController(ILogger<HomeController> logger, IInputDataProcessor inputDataProcessor)
        {
            _logger = logger;
            this.inputDataProcessor = inputDataProcessor;
        }

        private readonly ILogger<HomeController> _logger;

        private readonly IInputDataProcessor inputDataProcessor;

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public async Task<IActionResult> UploadNewDatasetData(FileUploadRequest fileUploadRequest)
        {
            if (fileUploadRequest.FormFile.Length > 0 && fileUploadRequest.FormFile.ContentType == "text/*")
            {
                var filePath = Path.GetTempFileName();

                string[] lines = System.IO.File.ReadAllLines(filePath, Encoding.UTF8);

                var entryLines = new List<NewEntryLine>();
                foreach (string line in lines)
                {
                    var validEntry = false;
                    validEntry = line.Split(" ").Length == 2;
                    if (!validEntry)
                        continue;
                    validEntry = Int32.TryParse(line.Split(" ")[0], out int user1);
                    if (!validEntry)
                        continue;
                    validEntry = Int32.TryParse(line.Split(" ")[1], out int user2);
                    if (!validEntry)
                        continue;

                    entryLines.Add(new NewEntryLine
                    {
                        User1RemoteId = user1,
                        User2RemoteId = user2
                    });
                }

                await inputDataProcessor.Proccess(entryLines, fileUploadRequest.DatasetId);
            }

            return Ok();
        }
    }
}