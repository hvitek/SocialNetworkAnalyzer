using System.Collections.Generic;
using System.Threading.Tasks;
using SocialNetworkAnalyzer.EntityFrameworkCore.Entities;

namespace SocialNetworkAnalyzer.Core.Users
{
    public interface IUserManager
    {
        Task<User> Create(User entity, bool isTransactional = true);

        Task<List<User>> GetAll(int? datasetId = null);

        Task<List<User>> GetUserByRemoteIds(List<int> ids, int datasetId);
    }
}