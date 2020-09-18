using System.Threading.Tasks;
using Abp.Configuration;
using Abp.Zero.Configuration;
using AbpCompanyName.AbpProjectName.Authorization.Accounts.Dto;
using AbpCompanyName.AbpProjectName.Authorization.Users;
using Microsoft.Extensions.Configuration;
using Abp.Net.Mail;
using System.Net;
using System;
using System.Linq;
using Microsoft.Extensions.Logging;
namespace AbpCompanyName.AbpProjectName.Authorization.Accounts
{
    public class AccountAppService : AbpProjectNameAppServiceBase, IAccountAppService
    {
        // from: http://regexlib.com/REDetails.aspx?regexp_id=1923
        public const string PasswordRegex = "(?=^.{8,}$)(?=.*\\d)(?=.*[a-z])(?=.*[A-Z])(?!.*\\s)[0-9a-zA-Z!@#$%^&*()]*$";

        private readonly UserRegistrationManager _userRegistrationManager;
        private readonly UserManager _userManager;
        private readonly IEmailSender _emailSender;
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;

        public AccountAppService(
            ILoggerFactory logger,
            UserRegistrationManager userRegistrationManager,
            UserManager userManager,
            IEmailSender emailSender,
            IConfiguration configuration)
        {
            _logger = logger.CreateLogger("AccountAppService");
            _userRegistrationManager = userRegistrationManager;
            _userManager = userManager;
            _emailSender = emailSender;
            _configuration = configuration;
        }

        public async Task<IsTenantAvailableOutput> IsTenantAvailable(IsTenantAvailableInput input)
        {
            var tenant = await TenantManager.FindByTenancyNameAsync(input.TenancyName);
            if (tenant == null)
            {
                return new IsTenantAvailableOutput(TenantAvailabilityState.NotFound);
            }

            if (!tenant.IsActive)
            {
                return new IsTenantAvailableOutput(TenantAvailabilityState.InActive);
            }

            return new IsTenantAvailableOutput(TenantAvailabilityState.Available, tenant.Id);
        }
        public async Task SendPasswordResetEmail(string userNameOrEmailAddress)
        {
            try{
                var user = await _userManager.FindByNameOrEmailAsync(AbpSession.TenantId, userNameOrEmailAddress);
                if (user == null)
                {
                    /*
                    TODO
                    You can return abp user friendly exception if email was not found. Because user can make typo. And remove all unnecessary comments.
                    @vdurante
                    vdurante on Mar 28, 2019 Author Contributor
                    @alirizaadiyahsi I honestly think it is a security flaw to inform if the e-mail exists or not. Nowadays it is common to inform that the user will receive an e-mail if it is registered on the website. If he made a typo, he can try again.
                    */
                    return;
                }

                user.SetNewPasswordResetCode();
                var passwordResetCode = user.PasswordResetCode;

                var email = this.L(
                    "PasswordResetEmailBody",
                    _configuration.GetSection("App:ClientRootAddress").Value.TrimEnd('/'),
                    user.TenantId ?? 0,
                    user.Id,
                    WebUtility.UrlEncode(passwordResetCode));

                _emailSender.Send(
                    from: (await SettingManager.GetSettingValueAsync(EmailSettingNames.DefaultFromAddress)),
                    to: user.EmailAddress,
                    subject: this.L("PasswordResetEmailSubject"),
                    body: email,
                    isBodyHtml: true
                );
            }
            catch(Exception e){
                _logger.LogError(e.ToString());
            }
        }

        public async Task<bool> VerifyPasswordResetCode(int? tenantId, long userId, string passwordResetCode)
        {
            if(tenantId == 0)
                tenantId = null;
            using (CurrentUnitOfWork.SetTenantId(tenantId))
            {
                var user = await _userManager.GetUserByIdAsync(userId);

                return await _userManager.VerifyPasswordResetCodeAsync(user, passwordResetCode);
            }
        }

        public async Task ResetPassword(int? tenantId, long userId, string token, string newPassword)
        {
            if(tenantId == 0)
                tenantId = null;
            using (CurrentUnitOfWork.SetTenantId(tenantId))
            {
                var user = await _userManager.GetUserByIdAsync(userId);

                var result = await _userManager.BasicResetPasswordAsync(user, token, newPassword);

                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.FirstOrDefault()?.ToString());
                }
            }
        }

        public async Task<RegisterOutput> Register(RegisterInput input)
        {
            var user = await _userRegistrationManager.RegisterAsync(
                input.Name,
                input.Surname,
                input.EmailAddress,
                input.UserName,
                input.Password,
                true // Assumed email address is always confirmed. Change this if you want to implement email confirmation.
            );

            var isEmailConfirmationRequiredForLogin = await SettingManager.GetSettingValueAsync<bool>(AbpZeroSettingNames.UserManagement.IsEmailConfirmationRequiredForLogin);

            return new RegisterOutput
            {
                CanLogin = user.IsActive && (user.IsEmailConfirmed || !isEmailConfirmationRequiredForLogin)
            };
        }
    }
}
