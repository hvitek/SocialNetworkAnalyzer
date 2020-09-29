using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SocialNetworkAnalyzer.Core.DbContext;
using SocialNetworkAnalyzer.EntityFrameworkCore;
using SocialNetworkAnalyzer.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialNetworkAnalyzer.Core.Relationships
{
    public class RelationshipManager : IRelationshipManager
    {
        public RelationshipManager(IDbContextProvider contextProvider)
        {
            this.contextProvider = contextProvider;
        }

        private readonly IDbContextProvider contextProvider;

        public async Task CreateBulk(List<CreateNewRelationshipDto> dtos, bool isTransactional = true)
        {
            var entities = dtos.Select(x => new Relationship
            {
                User1Id = x.User1Id,
                User2Id = x.User2Id
            });

            await contextProvider.Get().Relationships.AddRangeAsync(entities);
            if (isTransactional)
            {
                await contextProvider.Get().SaveChangesAsync();
            }
        }

        public async Task<List<Relationship>> GetAllRelationshipsOfUser(int userId)
        {
            return await contextProvider.Get().Relationships.Where(x => x.User1Id == userId || x.User2Id == userId).ToListAsync();
        }
    }
}