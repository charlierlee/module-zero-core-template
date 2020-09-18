using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Abp;
using Abp.Extensions;
using Abp.Notifications;
using Abp.Timing;
using AbpCompanyName.AbpProjectName.Controllers;
using Abp.AspNetCore.Mvc.Authorization;
using System.IO;
using System;
using Abp.UI;
using Abp.Runtime.Caching;
using AbpCompanyName.AbpProjectName.Net.MimeTypes;
using System.Net;

namespace AbpCompanyName.AbpProjectName.Web.Host.Controllers
{
    public class FileController : AbpProjectNameControllerBase
    {
        private readonly ICacheManager cacheManager;
 
        public FileController(ICacheManager cacheManager)
        {
            this.cacheManager = cacheManager;
        }
        public ActionResult DownloadDbFile(string token)
        {
            try
            {
                // Loading file from cache.
                MemoryStream ms = (MemoryStream) this.cacheManager.GetCache("db").GetOrDefault(token);
                if(ms == null){
                    return StatusCode(404);
                }
                // Set the proper HTTP response content type
                FileContentResult fileContentResult =
                        File(ms.ToArray(), MimeTypeNames.ApplicationOctetStream, token);
                return fileContentResult;
            }
            catch (Exception e)
            {
                throw new UserFriendlyException("Error", "Error downloading file. Please try again.", e);
            }
            finally
            {
                this.cacheManager.GetCache("db").Remove(token);
            }
        }
    }
}