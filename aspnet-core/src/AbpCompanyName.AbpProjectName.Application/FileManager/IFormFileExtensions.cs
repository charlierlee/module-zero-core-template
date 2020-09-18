using Microsoft.AspNetCore.Http;
using System.IO;
using System.Net.Http.Headers;

namespace AbpCompanyName.AbpProjectName.FileManager
{
    public static class IFormFileExtensions
    {
        public static string GetFileName(this IFormFile file)
        {
            var fileContent = ContentDispositionHeaderValue.Parse(file.ContentDisposition);

            // Some browsers send file names with full path.
            // We are only interested in the file name.
            return Path.GetFileName(fileContent.FileName.Trim('"'));
        }
    }
}