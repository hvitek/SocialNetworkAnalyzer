using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SocialNetworkAnalyzer.Core.Datasets;
using SocialNetworkAnalyzer.Core.Relationships;
using SocialNetworkAnalyzer.Core.Users;

namespace SocialNetworkAnalyzer.EntityFrameworkCore
{
    public interface ISocialNetworkAnalyzerDbContext
    {
        IConfiguration Configuration { get; }
        DbSet<Dataset> Datasets { get; set; }
        DbSet<Relationship> Relationships { get; set; }
        DbSet<User> Users { get; set; }
    }
}