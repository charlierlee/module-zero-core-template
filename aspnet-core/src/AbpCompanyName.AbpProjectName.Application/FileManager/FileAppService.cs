using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;
using Abp.Application.Services;
using System.Net.Http.Headers;
using AbpCompanyName.AbpProjectName.Sessions.Dto;
using System.Collections.Generic;
using Abp.Runtime.Caching;
using Abp.UI;
using Microsoft.AspNetCore.Hosting;

namespace AbpCompanyName.AbpProjectName.FileManager
{
    public class FileAppService : AbpProjectNameAppServiceBase, IFileAppService
    {
        // ABD cache dependency
        private readonly ICacheManager cacheManager;
        private readonly IHostingEnvironment _hostingEnvironment;
 
        // Inject other dependencies
        public FileAppService(IHostingEnvironment hostingEnvironment, ICacheManager cacheManager)
        {
            this._hostingEnvironment = hostingEnvironment;
            this.cacheManager = cacheManager;
        }
 
        public async Task<FileOutput> GenerateDb()
        {
            await CheckPermissions();
            // Generate unique token based in a timestamp
            string token = DateTime.Now.Ticks + ".db";
 
            MemoryStream ms = new MemoryStream();
            var baseDir = System.IO.Path.Combine(_hostingEnvironment.ContentRootPath, "App_Data");
            var filePath = Path.Combine(baseDir, "AbpCompanyName.AbpProjectName.db");
            if(!System.IO.File.Exists(filePath))
                filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AbpCompanyName.AbpProjectName.db");
            using (var fileStream = new FileStream(filePath, FileMode.Open)) {
                // Storing the stream in the cache.
                // The third param sets to delete the object after 1 minute
                byte[] bytes = new byte[fileStream.Length];
                fileStream.Read(bytes, 0, (int)fileStream.Length);
                ms.Write(bytes, 0, (int)fileStream.Length);
                cacheManager.GetCache("db").Set(
                        token, 
                        ms,
                        TimeSpan.FromMinutes(1));
            }
            return new FileOutput
            {
                File = new FileDto()
                {
                    Token = token,
                    Filename = "AbpCompanyName.AbpProjectName.db"
                }
            };
        }
        public virtual async Task Upload(IFormFile file){
            await CheckPermissions();
            var fileName = file.GetFileName();
            var fileExtension = Path.GetExtension(fileName);
            var baseDir = System.IO.Path.Combine(_hostingEnvironment.ContentRootPath, "App_Data");
            var filePath = Path.Combine(baseDir, file.FileName);
            //var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, file.FileName);
            using (var fileStream = new FileStream(filePath, FileMode.Create)) {
                await file.CopyToAsync(fileStream);
            }
        }
        private async Task<bool> CheckPermissions(){
            var output = new GetCurrentLoginInformationsOutput
            {
                Application = new ApplicationInfoDto
                {
                    Version = AppVersionHelper.Version,
                    ReleaseDate = AppVersionHelper.ReleaseDate,
                    Features = new Dictionary<string, bool>()
                }
            };

            if (AbpSession.TenantId.HasValue)
            {
                output.Tenant = ObjectMapper.Map<TenantLoginInfoDto>(await GetCurrentTenantAsync());
            }

            if (AbpSession.UserId.HasValue)
            {
                output.User = ObjectMapper.Map<UserLoginInfoDto>(await GetCurrentUserAsync());
            }
            if(output.User != null && output.User.UserName == "admin" && output.Tenant == null)
                return true;
            throw new UserFriendlyException("You do not have permissions to perform this task!");
        }
    }
}