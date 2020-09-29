using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SocialNetworkAnalyzer.Core.DataService;
using SocialNetworkAnalyzer.Core.Datasets;
using SocialNetworkAnalyzer.EntityFrameworkCore;
using SocialNetworkAnalyzer.Models;

namespace SocialNetworkAnalyzer.Controllers
{
    public class DatasetController : Controller
    {
        public DatasetController(ILogger<HomeController> logger, IDatasetManager datasetManager, IInputDataProcessor inputDataProcessor)
        {
            _logger = logger;
            this.datasetManager = datasetManager;
            this.inputDataProcessor = inputDataProcessor;
        }

        private readonly ILogger<HomeController> _logger;

        private readonly IDatasetManager datasetManager;

        private readonly IInputDataProcessor inputDataProcessor;

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm]NewDatasetRequest request)
        {
            var id = await datasetManager.CreateDatasetAndGetId(new Entities.Dataset
            {
                Description = request.Description,
                Name = request.Name
            });

            return RedirectToAction("Detail", new { id = id });
        }

        public async Task<ViewResult> Detail(int id)
        {
            var dataset = await datasetManager.GetWithInfo(id);
            var model = new DatasetDetailViewModel
            {
                Description = dataset.Description,
                Name = dataset.Name,
                Id = dataset.Id,
                Users = dataset.NumOfUsers,
                NumOfFriendsAvg = dataset.NumOfFriendsAvg,
                Edges = dataset.NumOfConnections,
            };

            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<JsonResult> GetGraphEdges(int id)
        {
            var edges = await datasetManager.GetEdges(id);
            return new JsonResult(edges);
        }

        public async Task<JsonResult> GetGraphNodes(int id)
        {
            var edges = await datasetManager.GetNodes(id);
            return new JsonResult(edges);
        }

        public async Task<IActionResult> Index()
        {
            var datasets = await datasetManager.GetAll();
            var model = datasets.Select(x => new DatasetDetailViewModel
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description
            }).ToList();
            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public async Task<IActionResult> UploadNewDataset(FileUploadRequest fileUploadRequest)
        {
            if (fileUploadRequest.FormFile == null)
            {
                return RedirectToAction("Detail", "Dataset", new { id = fileUploadRequest.DatasetId });
            }
            if (fileUploadRequest.FormFile.Length > 0 && fileUploadRequest.FormFile.ContentType == "text/plain")
            {
                var entryLines = new List<NewEntryLine>();

                using (var stream = new MemoryStream())
                {
                    await fileUploadRequest.FormFile.CopyToAsync(stream);
                    stream.Position = 0;
                    using (var streamReader = new StreamReader(stream))
                    {
                        string line;
                        while ((line = streamReader.ReadLine()) != null)
                        {
                            if (!inputDataProcessor.TryParseInputLine(line, out int user1, out int user2))
                            {
                                _logger.LogWarning("Input file is not valid!");
                                return RedirectToAction("Detail", "Dataset", new { id = fileUploadRequest.DatasetId });
                            }

                            entryLines.Add(new NewEntryLine
                            {
                                User1RemoteId = user1,
                                User2RemoteId = user2
                            });
                        }
                    }
                }

                await inputDataProcessor.Proccess(entryLines, fileUploadRequest.DatasetId);
            }

            return RedirectToAction("Detail", new { id = fileUploadRequest.DatasetId.Value });
        }
    }
}