using Business.ViewModels;
using Data.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services
{
    public interface IAccountRepository: IBaseRepository<AppUser>
    {
        public Task<RegisterViewModel> Register(RegisterViewModel registerViewModel);
        public Task<string> SignInUser(LoginViewModel registerViewModel);

        public Task<AppUserVm> ForgotPassword(string email);

        public Task<AppUserVm> VerifyOTP(string email,int otpCode);

        public Task<bool> ResetPassword(AppUserVm userVm);

        public Task<UserClaimVM> GetClaims(LoginViewModel loginModel);


    } 
} 
