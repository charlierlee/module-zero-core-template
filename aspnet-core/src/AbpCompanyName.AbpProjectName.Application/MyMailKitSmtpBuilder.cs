using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;
using AbpCompanyName.AbpProjectName.Authorization;
using Abp.MailKit;
using MailKit.Net.Smtp;
using Abp.Net.Mail.Smtp;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using AbpCompanyName.AbpProjectName.Configuration;
using Microsoft.Extensions.Logging;
using System;
namespace AbpCompanyName.AbpProjectName
{
    public class MyMailKitSmtpBuilder : DefaultMailKitSmtpBuilder
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;
        public MyMailKitSmtpBuilder(
            ILoggerFactory logger,
            IConfiguration configuration,
            ISmtpEmailSenderConfiguration smtpEmailSenderConfiguration,
            IAbpMailKitConfiguration abpMailKitConfiguration
        )
            : base(smtpEmailSenderConfiguration,abpMailKitConfiguration)
        {
            _configuration = configuration;
            _logger = logger.CreateLogger("MyMailKitSmtpBuilder");
        }

        protected override void ConfigureClient(SmtpClient client)
        {
            if(string.IsNullOrEmpty(_configuration.GetSection("Mail:Smtp.Host").Value)){
                _logger.LogError("Mail:Smtp.Host is not defined");
            }
            else if(string.IsNullOrEmpty(_configuration.GetSection("Mail:Smtp.Port").Value)){
                _logger.LogError("Mail:Smtp.Port is not defined");
            }
            else if(string.IsNullOrEmpty(_configuration.GetSection("Mail:Smtp.UserName").Value)){
                _logger.LogError("Mail:Smtp.UserName is not defined");
            }
            else if(string.IsNullOrEmpty(_configuration.GetSection("Mail:Smtp.Password").Value)){
                _logger.LogError("Mail:Smtp.Password is not defined");
            }
            client.ServerCertificateValidationCallback = (sender, certificate, chain, errors) => true;
            client.Connect (_configuration.GetSection("Mail:Smtp.Host").Value, int.Parse(_configuration.GetSection("Mail:Smtp.Port").Value), false);
			client.Authenticate (_configuration.GetSection("Mail:Smtp.UserName").Value, _configuration.GetSection("Mail:Smtp.Password").Value);
            //base.ConfigureClient(client); otherwise will get error about already connected
        }
    }
}
