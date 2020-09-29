using SocialNetworkAnalyzer.EntityFrameworkCore;

namespace SocialNetworkAnalyzer.Core.DbContext
{
    public interface IDbContextProvider
    {
        SocialNetworkAnalyzerDbContext Get();
    }
}