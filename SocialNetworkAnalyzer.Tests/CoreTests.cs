using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SocialNetworkAnalyzer.Core.DataService;
using SocialNetworkAnalyzer.Core.Datasets;
using SocialNetworkAnalyzer.Core.DbContext;
using SocialNetworkAnalyzer.Core.Relationships;
using SocialNetworkAnalyzer.Core.Users;
using SocialNetworkAnalyzer.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace SocialNetworkAnalyzer.Tests
{
    public class CoreTests
    {
        public CoreTests()
        {
            services = new ServiceCollection();
            services.AddDbContext<SocialNetworkAnalyzerDbContext>(options => options.UseSqlite("Filename=:memory:"));
            services.AddTransient<IDatasetManager, DatasetManager>();
            services.AddTransient<IUserManager, UserManager>();
            services.AddSingleton<IDbContextProvider, FakeContextProvider>();
            services.AddSingleton<IInputDataProcessor, InputDataProcessor>();
            services.AddTransient<IRelationshipManager, RelationshipManager>();
            serviceProvider = services.BuildServiceProvider();
            datasetManager = serviceProvider.GetRequiredService<IDatasetManager>();
            userManager = serviceProvider.GetRequiredService<IUserManager>();
            relationshipManager = serviceProvider.GetRequiredService<IRelationshipManager>();
            inputDataProcessor = serviceProvider.GetRequiredService<IInputDataProcessor>();
        }

        private IDatasetManager datasetManager;

        private IInputDataProcessor inputDataProcessor;

        private IRelationshipManager relationshipManager;

        private IServiceProvider serviceProvider;

        private IServiceCollection services;

        private IUserManager userManager;

        [Fact]
        public async Task CheckCreateDataset_ShoudCreatedAndEqual()
        {
            using (var db = new SocialNetworkAnalyzerDbContext(Utilities.TestDbContextOptions()))
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

                var id1 = await datasetManager.CreateDatasetAndGetId(new Entities.Dataset { Description = "", Name = "" });
                var id2 = await datasetManager.CreateDatasetAndGetId(new Entities.Dataset { Description = "", Name = "" });

                var result = await datasetManager.GetAll();

                Assert.Equal(2, result.Count);
            }
        }

        [Fact]
        public async Task CheckDataset()
        {
            using (var context = new SocialNetworkAnalyzerDbContext(Utilities.TestDbContextOptions()))
            {
                await PrepareData(context);

                var result = await context.Datasets.FirstOrDefaultAsync();
                // Assert
                Assert.NotNull(result);
                Assert.Equal("test.description", result.Description);
                Assert.Equal("test.name", result.Name);
            }
        }

        [Fact]
        public async Task CheckDatasetInfo_ShoudBeRighType()
        {
            using (var db = new SocialNetworkAnalyzerDbContext(Utilities.TestDbContextOptions()))
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

                var datasetId = await datasetManager.CreateDatasetAndGetId(new Entities.Dataset { Description = "", Name = "" });
                var userId1 = await userManager.Create(new EntityFrameworkCore.Entities.User { DatasetId = datasetId, RemoteId = 5 });
                var userId2 = await userManager.Create(new EntityFrameworkCore.Entities.User { DatasetId = datasetId, RemoteId = 6 });

                var list = new List<CreateNewRelationshipDto>();
                list.Add(new CreateNewRelationshipDto { User1Id = userId1.Id, User2Id = userId2.Id });
                await relationshipManager.CreateBulk(list);
                var datasetInfo = await datasetManager.GetWithInfo(datasetId);

                Assert.IsType<DatasetInfo>(datasetInfo);
            }
        }

        [Theory]
        [InlineData("sgsdg 1")]
        [InlineData("1.6 5")]
        public async Task CheckInputFileValidator_ShoudBeFalse(string value)
        {
            bool isValid = inputDataProcessor.TryParseInputLine(value, out int a, out int b);
            Assert.False(isValid);
        }

        [Theory]
        [InlineData("1 0", 1, 0)]
        [InlineData("5 2", 5, 2)]
        [InlineData("8 3", 8, 3)]
        [InlineData("91 11", 91, 11)]
        public async Task CheckInputFileValidator_ShoudBeValid(string value, int originalA, int originalB)
        {
            bool isValid = inputDataProcessor.TryParseInputLine(value, out int a, out int b);

            Assert.True(isValid);
            Assert.Equal(originalA, a);
            Assert.Equal(originalB, b);
        }

        [Fact]
        public async Task CheckNodesAndEdges_ShoudBeCreatedAndEqual()
        {
            using (var db = new SocialNetworkAnalyzerDbContext(Utilities.TestDbContextOptions()))
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

                var datasetId = await datasetManager.CreateDatasetAndGetId(new Entities.Dataset { Description = "", Name = "" });
                var userId1 = await userManager.Create(new EntityFrameworkCore.Entities.User { DatasetId = datasetId, RemoteId = 5 });
                var userId2 = await userManager.Create(new EntityFrameworkCore.Entities.User { DatasetId = datasetId, RemoteId = 6 });

                var list = new List<CreateNewRelationshipDto>();
                list.Add(new CreateNewRelationshipDto { User1Id = userId1.Id, User2Id = userId2.Id });
                await relationshipManager.CreateBulk(list);
                var edges = await datasetManager.GetEdges(datasetId);
                var nodes = await datasetManager.GetNodes(datasetId);

                Assert.Single(edges);
                Assert.Equal(2, nodes.Count);
            }
        }

        [Fact]
        public async Task CheckRelationships()
        {
            using (var context = new SocialNetworkAnalyzerDbContext(Utilities.TestDbContextOptions()))
            {
                await PrepareData(context);

                var user = await context.Users.FirstOrDefaultAsync();
                var result = await context.Relationships.ToListAsync();
                // Assert
                Assert.NotEmpty(result);
                Assert.Single(result);
            }
        }

        [Fact]
        public async Task CheckRelationshipsGetAll()
        {
            using (var context = new SocialNetworkAnalyzerDbContext(Utilities.TestDbContextOptions()))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
                var datasetId = await datasetManager.CreateDatasetAndGetId(new Entities.Dataset { Description = "", Name = "" });
                var user1 = await userManager.Create(new EntityFrameworkCore.Entities.User { RemoteId = 0, DatasetId = datasetId });
                var user2 = await userManager.Create(new EntityFrameworkCore.Entities.User { RemoteId = 0, DatasetId = datasetId });
                var list = new List<CreateNewRelationshipDto>();
                list.Add(new CreateNewRelationshipDto { User1Id = user1.Id, User2Id = user2.Id });
                await relationshipManager.CreateBulk(list);

                var relationshipOfUser = await relationshipManager.GetAllRelationshipsOfUser(user1.Id);

                // Assert
                Assert.Single(relationshipOfUser);
            }
        }

        [Fact]
        public async Task CheckUsers()
        {
            using (var context = new SocialNetworkAnalyzerDbContext(Utilities.TestDbContextOptions()))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                var datasetId = await datasetManager.CreateDatasetAndGetId(new Entities.Dataset { Description = "", Name = "" });
                var user1 = await userManager.Create(new EntityFrameworkCore.Entities.User { DatasetId = datasetId, RemoteId = 5 });
                var user2 = await userManager.Create(new EntityFrameworkCore.Entities.User { DatasetId = datasetId, RemoteId = 6 });

                var usersWithDatasetFilter = await userManager.GetAll(datasetId);
                var usersWithoutDatasetFilter = await userManager.GetAll();

                var listOfUsers = new List<int>();
                listOfUsers.Add(5);
                listOfUsers.Add(6);

                var remoteIds = await userManager.GetUserByRemoteIds(listOfUsers, datasetId);

                // Assert
                Assert.Equal(2, usersWithDatasetFilter.Count());
                Assert.Equal(2, usersWithoutDatasetFilter.Count());
                Assert.Equal(2, remoteIds.Count());
            }
        }

        [Fact]
        public async Task CheckUsers_ShouldBeAddedToDbContext()
        {
            using (var context = new SocialNetworkAnalyzerDbContext(Utilities.TestDbContextOptions()))
            {
                await PrepareData(context);

                var result = await context.Users.ToListAsync();
                // Assert
                Assert.NotEmpty(result);
                Assert.Equal(2, result.Count);
            }
        }

        private async Task PrepareData(SocialNetworkAnalyzerDbContext context)
        {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            var dataset = context.Datasets.Add(new Entities.Dataset
            {
                Description = "test.description",
                Name = "test.name"
            });
            context.SaveChanges();

            context.Relationships.Add(new EntityFrameworkCore.Entities.Relationship
            {
                User1 = new EntityFrameworkCore.Entities.User
                {
                    RemoteId = 98487,
                    Dataset = await context.Datasets.FirstOrDefaultAsync()
                },
                User2 = new EntityFrameworkCore.Entities.User
                {
                    RemoteId = 18615,
                    Dataset = await context.Datasets.FirstOrDefaultAsync()
                }
            });
            context.SaveChanges();
        }
    }
}