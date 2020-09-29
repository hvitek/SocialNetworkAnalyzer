using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SocialNetworkAnalyzer.Core.DataService;
using SocialNetworkAnalyzer.Core.Datasets;
using SocialNetworkAnalyzer.Core.DbContext;
using SocialNetworkAnalyzer.Core.Relationships;
using SocialNetworkAnalyzer.Core.Users;
using SocialNetworkAnalyzer.EntityFrameworkCore;

namespace SocialNetworkAnalyzer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            using (var scope = app.ApplicationServices.CreateScope())
            {
                using (var context = scope.ServiceProvider.GetService<SocialNetworkAnalyzerDbContext>())
                {
                    context.Database.EnsureCreated();
                    context.Database.Migrate();
                }
            }
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var dbName = Configuration.GetConnectionString(("DatabaseName"));
            var connectionString = String.Format("Data Source={0}{1}", AppContext.BaseDirectory, dbName);
            services.AddDbContext<SocialNetworkAnalyzerDbContext>(options => options.UseSqlite(connectionString));

            services.AddControllersWithViews();
            services.AddTransient<IInputDataProcessor, InputDataProcessor>();
            services.AddTransient<IDatasetManager, DatasetManager>();
            services.AddTransient<IDbContextProvider, DbContextProvider>();
            services.AddTransient<IRelationshipManager, RelationshipManager>();
            services.AddTransient<IUserManager, UserManager>();
        }
    }
}