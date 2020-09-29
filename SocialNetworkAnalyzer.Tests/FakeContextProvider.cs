using SocialNetworkAnalyzer.Core.DbContext;
using SocialNetworkAnalyzer.EntityFrameworkCore;

namespace SocialNetworkAnalyzer.Tests
{
    internal class FakeContextProvider : IDbContextProvider
    {
        private SocialNetworkAnalyzerDbContext socialNetworkAnalyzerDbContext { get; set; }

        public SocialNetworkAnalyzerDbContext Get()
        {
            if (socialNetworkAnalyzerDbContext == null)
            {
                socialNetworkAnalyzerDbContext = new SocialNetworkAnalyzerDbContext(Utilities.TestDbContextOptions());
            }

            return socialNetworkAnalyzerDbContext;
        }
    }
}