using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;
using AbpCompanyName.AbpProjectName.Authorization;
using Abp.MailKit;
using Abp.Configuration.Startup;
using Abp.Dependency;
namespace AbpCompanyName.AbpProjectName
{
    [DependsOn(
        typeof(AbpMailKitModule),
        typeof(AbpProjectNameCoreModule), 
        typeof(AbpAutoMapperModule))]
    public class AbpProjectNameApplicationModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Authorization.Providers.Add<AbpProjectNameAuthorizationProvider>();
            Configuration.ReplaceService<IMailKitSmtpBuilder, MyMailKitSmtpBuilder>(DependencyLifeStyle.Transient);
        }

        public override void Initialize()
        {
            var thisAssembly = typeof(AbpProjectNameApplicationModule).GetAssembly();

            IocManager.RegisterAssemblyByConvention(thisAssembly);

            Configuration.Modules.AbpAutoMapper().Configurators.Add(
                // Scan the assembly for classes which inherit from AutoMapper.Profile
                cfg => cfg.AddMaps(thisAssembly)
            );
        }
    }
}
