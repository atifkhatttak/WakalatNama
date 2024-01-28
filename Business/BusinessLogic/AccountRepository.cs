using Business.Services;
using Business.ViewModels;
using Data.Context;
using Data.DomainModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Business.BusinessLogic
{
    public class AccountRepository : BaseRepository<AppUser>,IAccountRepository
    {
        private readonly WKNNAMADBCtx ctx;
        private readonly UserManager<AppUser> userManager;
        private readonly RoleManager<AppRole> roleManager;
        private readonly IConfiguration config;

        public AccountRepository(WKNNAMADBCtx ctx, UserManager<AppUser> userManager, RoleManager<AppRole> roleManager, IConfiguration config) : base(ctx)
        {
            this.ctx = ctx;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.config = config;
        }

        public async  Task<RegisterViewModel> Register(RegisterViewModel registerViewModel)
        {
            try
            {
                AppUser _user = new AppUser
                {
                    FirstName = registerViewModel.FirstName,
                    LastName = registerViewModel.LastName,
                    Email = registerViewModel.Email,
                    UserName = registerViewModel.UserName

                };

              var isUserExist= ctx.Users.Any(x => x.UserName == _user.UserName || x.Email == _user.Email);

                if (!isUserExist)
                {
                    var _userCreated = await userManager.CreateAsync(_user);

                    if (_userCreated.Succeeded)
                    {
                        var _passwordCreated = await userManager.AddPasswordAsync(_user, registerViewModel.Password);

                        if (_passwordCreated.Succeeded)
                        {
                            var _role = await roleManager.FindByNameAsync(registerViewModel.RoleName);

                            if (_role != null)
                            {
                                var roleresult = await userManager.AddToRoleAsync(_user, _role.Name ?? "Citizen");
                            };
                        }
                    }
                }
               
            }
            catch (Exception ex) {
                throw ex;

            }
            return registerViewModel;
        }

        public async Task<RegisterViewModel> UpdateUser(RegisterViewModel registerViewModel)
        {
            try
            {
                AppUser _user = new AppUser
                {
                    FirstName = registerViewModel.FirstName,
                    LastName = registerViewModel.LastName,
                    Email = registerViewModel.Email,
                    UserName = registerViewModel.UserName

                };

                var _userCreated = await userManager.UpdateAsync(_user);

                if (_userCreated.Succeeded)
                {
                    var _passwordCreated = await userManager.AddPasswordAsync(_user, registerViewModel.Password);

                    if (_passwordCreated.Succeeded)
                    {
                        var _role = await roleManager.FindByNameAsync(_user.UserName);

                        if (_role != null)
                        {
                            var roleresult = await userManager.AddToRoleAsync(_user, _role.Name ?? "Citizen");
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;

            }
            return registerViewModel;
        }

        public async Task<string> SignInUser(LoginViewModel loginModel)
        {
            var _user = await userManager.FindByEmailAsync(loginModel.UserName);

            if (_user != null)
            {
                var _passwordVerified = await userManager.CheckPasswordAsync(_user, loginModel.Password);

                if (_passwordVerified)
                {
                   var roles=  await userManager.GetRolesAsync(_user);

                    List<Claim> claims = [new Claim("UserName", loginModel.UserName),
                                          new Claim(ClaimTypes.Email, loginModel.UserName),
                                          new Claim("FirstName", _user.FirstName),
                                          new Claim("LastName", _user.LastName)
                                          ];
                    foreach (var role  in roles)
                    {
                        claims.Add( new Claim( ClaimTypes.Role, roles.FirstOrDefault()));
                    }
                  return  GenerateToken(claims);
                }
            }
           return string.Empty;
        }
 
        private string GenerateToken(List<Claim> claims=null)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var Sectoken = new JwtSecurityToken(config["Jwt:Issuer"],
              config["Jwt:Issuer"],
              claims,
              expires: DateTime.Now.AddMinutes(120),
              signingCredentials: credentials);

            var token = new JwtSecurityTokenHandler().WriteToken(Sectoken);
            return token;
        }
    }
}
