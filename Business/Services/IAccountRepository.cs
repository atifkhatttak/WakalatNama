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
    } 
} 
