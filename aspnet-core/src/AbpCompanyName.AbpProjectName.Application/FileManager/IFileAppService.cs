using System.Threading.Tasks;
using Abp.Application.Services;
using AbpCompanyName.AbpProjectName.Sessions.Dto;
using Microsoft.AspNetCore.Http;

namespace AbpCompanyName.AbpProjectName.FileManager
{
    public interface IFileAppService : IApplicationService
    {
        Task Upload(IFormFile file);
        Task<FileOutput> GenerateDb();
    }
}
