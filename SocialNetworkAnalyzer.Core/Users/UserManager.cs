using Microsoft.EntityFrameworkCore;
using SocialNetworkAnalyzer.Core.DbContext;
using SocialNetworkAnalyzer.EntityFrameworkCore;
using SocialNetworkAnalyzer.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialNetworkAnalyzer.Core.Users
{
    public class UserManager : IUserManager
    {
        public UserManager(IDbContextProvider contextProvider)
        {
            this.contextProvider = contextProvider;
        }

        private readonly IDbContextProvider contextProvider;

        public async Task<User> Create(User entity, bool isTransactional = true)
        {
            var entityEntry = await contextProvider.Get().Users.AddAsync(entity);
            if (isTransactional)
            {
                await contextProvider.Get().SaveChangesAsync();
            }

            return entityEntry.Entity;
        }

        public async Task<List<User>> GetAll(int? datasetId = null)
        {
            if (datasetId.HasValue)
            {
                return await contextProvider.Get().Users
                    .Include(x => x.Dataset)
                    .Where(x => x.Dataset.Id == datasetId)
                    .ToListAsync();
            }

            return await contextProvider.Get().Users.ToListAsync();
        }

        public async Task<List<User>> GetUserByRemoteIds(List<int> ids, int datasetId)
        {
            var entities = await contextProvider.Get().Users.Where(x => ids.Contains(x.RemoteId) && x.DatasetId == datasetId).ToListAsync();
            return entities;
        }
    }
}