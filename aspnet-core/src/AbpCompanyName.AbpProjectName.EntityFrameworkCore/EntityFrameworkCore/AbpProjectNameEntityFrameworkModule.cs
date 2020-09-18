using Abp.EntityFrameworkCore.Configuration;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.Zero.EntityFrameworkCore;
using AbpCompanyName.AbpProjectName.EntityFrameworkCore.Seed;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;

namespace AbpCompanyName.AbpProjectName.EntityFrameworkCore
{
    [DependsOn(
        typeof(AbpProjectNameCoreModule), 
        typeof(AbpZeroCoreEntityFrameworkCoreModule))]
    public class AbpProjectNameEntityFrameworkModule : AbpModule
    {
        /* Used it tests to skip dbcontext registration, in order to use in-memory database of EF Core */
        public bool SkipDbContextRegistration { get; set; }

        public bool SkipDbSeed { get; set; }
        private readonly IHostingEnvironment _env;
        private readonly ILogger _logger;
        public AbpProjectNameEntityFrameworkModule(IHostingEnvironment env, ILoggerFactory logger)
        {
            _env = env;
            _logger = logger.CreateLogger("AbpProjectNameEntityFrameworkModule");
        }
        public override void PreInitialize()
        {
            var basePath = System.IO.Path.Combine(_env.ContentRootPath, "App_Data");
            var fullPath = System.IO.Path.Combine(basePath, "AbpCompanyName.AbpProjectName.db");
            var connectionString = "Filename=" + fullPath;
            if (!SkipDbContextRegistration)
            {
                Configuration.Modules.AbpEfCore().AddDbContext<AbpProjectNameDbContext>(options =>
                {
                    if (options.ExistingConnection != null)
                    {
                        AbpProjectNameDbContextConfigurer.Configure(options.DbContextOptions, options.ExistingConnection);
                    }
                    else
                    {
                        AbpProjectNameDbContextConfigurer.Configure(options.DbContextOptions, connectionString);
                        //AbpProjectNameDbContextConfigurer.Configure(options.DbContextOptions, options.ConnectionString);
                    }
                });
            }
            //specific to sqlite. remove if sql server
            Configuration.UnitOfWork.IsTransactional = false;
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(AbpProjectNameEntityFrameworkModule).GetAssembly());
        }

        public override void PostInitialize()
        {
            if (!SkipDbSeed)
            {
                SeedHelper.SeedHostDb(IocManager);
            }
        }
    }
}
