using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SocialNetworkAnalyzer.Entities;
using SocialNetworkAnalyzer.EntityFrameworkCore.Entities;
using System;

namespace SocialNetworkAnalyzer.EntityFrameworkCore
{
    public class SocialNetworkAnalyzerDbContext : DbContext
    {
        public SocialNetworkAnalyzerDbContext(DbContextOptions<SocialNetworkAnalyzerDbContext> options) : base(options)
        {
        }

        public SocialNetworkAnalyzerDbContext()
        {
        }

        public DbSet<Dataset> Datasets { get; set; }

        public DbSet<Relationship> Relationships { get; set; }

        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            base.OnConfiguring(builder);
        }
    }
}