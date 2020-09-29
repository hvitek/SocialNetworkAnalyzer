using SocialNetworkAnalyzer.Entities;
using SocialNetworkAnalyzer.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SocialNetworkAnalyzer.Core.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SocialNetworkAnalyzer.Core.Users;
using SocialNetworkAnalyzer.Core.Relationships;
using SocialNetworkAnalyzer.Core.DbContext;

namespace SocialNetworkAnalyzer.Core.Datasets
{
    public class DatasetManager : IDatasetManager
    {
        public DatasetManager(IDbContextProvider contextProvider,
            IUserManager userManager,
            IRelationshipManager relationshipManager)
        {
            this.contextProvider = contextProvider;
            this.userManager = userManager;
            this.relationshipManager = relationshipManager;
        }

        private readonly IDbContextProvider contextProvider;

        private readonly IRelationshipManager relationshipManager;

        private readonly IUserManager userManager;

        public async Task<int> CreateDatasetAndGetId(Dataset dataset)
        {
            var newDataSet = await contextProvider.Get().Datasets.AddAsync(dataset);
            await contextProvider.Get().SaveChangesAsync();

            return newDataSet.Entity.Id;
        }

        public async Task<List<Dataset>> GetAll()
        {
            return await contextProvider.Get().Datasets.ToListAsync();
        }

        public async Task<List<NodeData>> GetEdges(int id)
        {
            var users = await userManager.GetAll(id);
            var potentialEdges = new List<NodeData>();
            var jsonData = new List<NodeData>();
            foreach (var user in users)
            {
                var relationshipsOfUser = await relationshipManager.GetAllRelationshipsOfUser(user.Id);
                potentialEdges.AddRange(relationshipsOfUser.Select(x => new NodeData
                {
                    Id = Guid.NewGuid().ToString(),
                    Source = "n" + x.User1Id.ToString(),
                    Target = "n" + x.User2Id.ToString()
                }
               ).ToList());
            }

            foreach (var item in potentialEdges)
            {
                //check if edge exist
                if (jsonData.Where(x => x.Source == item.Source && x.Target == item.Target).Any())
                {
                    continue;
                }

                jsonData.Add(item);
            }

            return jsonData;
        }

        public async Task<List<NodeData>> GetNodes(int id)
        {
            var users = await userManager.GetAll(id);
            var jsonData = new List<NodeData>();
            foreach (var item in users)
            {
                jsonData.Add(new NodeData { Id = "n" + item.Id.ToString() });
            }

            return jsonData;
        }

        public async Task<DatasetInfo> GetWithInfo(int id)
        {
            var dataset = await contextProvider.Get().Datasets.FirstOrDefaultAsync(x => x.Id == id);
            var users = await userManager.GetAll(id);
            var numberOfRelationships = new List<int>();
            var edges = await GetEdges(id);

            foreach (var user in users)
            {
                var relationshipsOfUser = await relationshipManager.GetAllRelationshipsOfUser(user.Id);
                numberOfRelationships.Add(relationshipsOfUser.Count());
            }

            var relationshipsAvg = 0d;
            if (numberOfRelationships.Any())
            {
                relationshipsAvg = numberOfRelationships.Average();
            }

            return new DatasetInfo
            {
                Id = dataset.Id,
                Description = dataset.Description,
                Name = dataset.Name,
                NumOfFriendsAvg = relationshipsAvg,
                NumOfUsers = users.Count,
                NumOfConnections = edges.Count,
            };
        }
    }
}