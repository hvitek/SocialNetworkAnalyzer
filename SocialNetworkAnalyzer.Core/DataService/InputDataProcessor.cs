using SocialNetworkAnalyzer.Core.Datasets;
using SocialNetworkAnalyzer.Core.DbContext;
using SocialNetworkAnalyzer.Core.Relationships;
using SocialNetworkAnalyzer.Core.Users;
using SocialNetworkAnalyzer.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialNetworkAnalyzer.Core.DataService
{
    public class InputDataProcessor : IInputDataProcessor
    {
        public InputDataProcessor(
            IUserManager userManager,
            IDatasetManager datasetManager,
            IRelationshipManager relationshipManager,
            IDbContextProvider contextProvider)
        {
            this.userManager = userManager;
            this.datasetManager = datasetManager;
            this.relationshipManager = relationshipManager;
            this.contextProvider = contextProvider;
        }

        private readonly IDbContextProvider contextProvider;

        private readonly IDatasetManager datasetManager;

        private readonly IRelationshipManager relationshipManager;

        private readonly IUserManager userManager;

        public async Task Proccess(List<NewEntryLine> newEntryLines, int? datasetId = null)
        {
            if (!newEntryLines.Any())
            {
                return;
            }

            await SetDataset(datasetId);
            var usersToCreate = await GetUsersToCreate(newEntryLines, datasetId.Value);
            await CreateNewUsers(usersToCreate, datasetId.Value);
            await CreateNewRelationship(newEntryLines, datasetId.Value);
        }

        public bool TryParseInputLine(string line, out int userId, out int user2Id)
        {
            userId = 0;
            user2Id = 0;
            var validEntry = false;
            validEntry = line.Split(" ").Length == 2;
            if (!validEntry)
            {
                return validEntry;
            }

            validEntry = Int32.TryParse(line.Split(" ")[0], out userId);
            if (!validEntry)
            {
                return validEntry;
            }

            validEntry = Int32.TryParse(line.Split(" ")[1], out user2Id);
            if (!validEntry)
            {
                return validEntry;
            }

            return true;
        }

        private async Task CreateNewRelationship(List<NewEntryLine> newEntryLines, int datasetId)
        {
            var userRemoteIds = newEntryLines
                .Select(x => x.User1RemoteId)
                    .Concat(newEntryLines.Select(x => x.User2RemoteId))
                .Distinct()
                .ToList();
            var users = await userManager.GetUserByRemoteIds(userRemoteIds, datasetId);

            foreach (var line in newEntryLines)
            {
                line.User1Id = users.Where(x => x.RemoteId == line.User1RemoteId).FirstOrDefault().Id;
                line.User2Id = users.Where(x => x.RemoteId == line.User2RemoteId).FirstOrDefault().Id;
            }

            var newRelationships = new List<CreateNewRelationshipDto>();
            foreach (var item in newEntryLines)
            {
                newRelationships.Add(new CreateNewRelationshipDto
                {
                    User1Id = item.User1Id,
                    User2Id = item.User2Id
                });
            };

            await relationshipManager.CreateBulk(newRelationships);
        }

        private async Task CreateNewUsers(List<UserToCreate> usersToCreate, int datasetId)
        {
            //create new user
            foreach (var user in usersToCreate)
            {
                var newUser = await userManager.Create(new EntityFrameworkCore.Entities.User
                {
                    DatasetId = datasetId,
                    RemoteId = user.UserRemoteId
                }, false);

                user.UserId = newUser.Id;
            }

            await contextProvider.Get().SaveChangesAsync();
        }

        private async Task<List<UserToCreate>> GetUsersToCreate(List<NewEntryLine> newEntryLines, int datasetId)
        {
            return newEntryLines
                .Select(x => x.User1RemoteId)
                .Concat(newEntryLines.Select(x => x.User2RemoteId))
                .Distinct()
                .Select(x => new UserToCreate { UserRemoteId = x }).ToList()
                .ToList();
        }

        private async Task SetDataset(int? datasetId = null)
        {
            if (!datasetId.HasValue)
            {
                datasetId = await datasetManager.CreateDatasetAndGetId(
                    new Entities.Dataset
                    {
                        Name = String.Format("Dataset - {0}", DateTime.UtcNow.ToShortTimeString())
                    });
            }
        }
    }
}