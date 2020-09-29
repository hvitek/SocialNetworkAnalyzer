using SocialNetworkAnalyzer.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace SocialNetworkAnalyzer.Core.DbContext
{
    public class DbContextProvider : IDbContextProvider
    {
        public DbContextProvider(SocialNetworkAnalyzerDbContext socialNetworkAnalyzerDbContext)
        {
            this.socialNetworkAnalyzerDbContext = socialNetworkAnalyzerDbContext;
        }

        private readonly SocialNetworkAnalyzerDbContext socialNetworkAnalyzerDbContext;

        public SocialNetworkAnalyzerDbContext Get()
        {
            return socialNetworkAnalyzerDbContext;
        }
    }
}