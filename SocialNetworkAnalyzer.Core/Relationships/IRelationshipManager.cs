using System.Collections.Generic;
using System.Threading.Tasks;
using SocialNetworkAnalyzer.EntityFrameworkCore.Entities;

namespace SocialNetworkAnalyzer.Core.Relationships
{
    public interface IRelationshipManager
    {
        Task CreateBulk(List<CreateNewRelationshipDto> dtos, bool isTransactional = true);

        Task<List<Relationship>> GetAllRelationshipsOfUser(int userId);
    }
}