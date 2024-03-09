using Business.Enums;
using Business.Services;
using Business.ViewModels;
using Data.Context;
using Data.DomainModels;
using Google.Apis.Drive.v3.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using ProjWakalatnama.DataLayer.Models;
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
            int RoleId=0;
            try
            {
                AppUser _user = new AppUser
                {
                    FirstName = registerViewModel.FirstName,
                    LastName = registerViewModel.LastName,
                    Email = registerViewModel.Email,
                    UserName = registerViewModel.UserName,
                    PhoneNumber = registerViewModel.PhoneNumber

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
                                RoleId=_role.Id;
                                var roleresult = await userManager.AddToRoleAsync(_user, _role.Name ?? "Citizen");
                            };
                        }
                    }

                    //Insert User Profile 
                    var insertedUser = ctx.Users.Where(x=>x.Email == _user.Email).FirstOrDefault();
                    if (insertedUser != null)
                    {
                        UserProfile userProfile = new UserProfile()
                        {
                            UserId = insertedUser.Id,
                            RoleId = RoleId,
                            FullName = insertedUser.FirstName + " " + insertedUser.LastName,
                            Email=insertedUser.Email??"",
                            ContactNumber=insertedUser.PhoneNumber??"",
                            IsOverseas=registerViewModel.IsOverseas
                        };

                        await ctx.AddAsync(userProfile);
                       await ctx.SaveChangesAsync();
                    }

                    registerViewModel.UserId= insertedUser?.Id??0;
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
                                          new Claim("LastName", _user.LastName),
                                           new Claim("UserId", _user.Id.ToString())
                                          ];
                    foreach (var role  in roles)
                    {
                        claims.Add( new Claim( ClaimTypes.Role, roles.FirstOrDefault()));
                    }
                  return  GenerateToken(claims);
                }
                else
                {
                    return "Password is incorrect";
                }
            }
            else
            {
                return "User not found on this email";
            }
           return string.Empty;
        }
 
        private string GenerateToken(List<Claim>? claims)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var Sectoken = new JwtSecurityToken(config["Jwt:Issuer"],
              config["Jwt:Audience"],
              claims,
              expires: DateTime.Now.AddMinutes(120),
              signingCredentials: credentials);

            var token = new JwtSecurityTokenHandler().WriteToken(Sectoken);
            return token;
        }

        public async Task<AppUserVm> ForgotPassword(string email)
        {
           return await ctx.Users.Where(x => x.Email == email)
                .Select(x => new AppUserVm { 
                Id = x.Id,
                FullName = x.FirstName +" " + x.LastName,
                Email = x.Email!,
                UserName = x.UserName!
                })
                .FirstOrDefaultAsync();
        }

        public async Task PersistOTP(AppUserVm userVm,int otpCode)
        {
            var _user = await ctx.Users.Where(x => x.Email == userVm.Email && x.UserName == userVm.UserName).FirstOrDefaultAsync();

            if(_user != null )
            {
                _user.OTPCode = otpCode;
                await ctx.SaveChangesAsync();
            }
        }

        public async Task RemoveOTP(AppUserVm userVm) 
        {
            var _user = await ctx.Users.Where(x => x.Email == userVm.Email && x.UserName == userVm.UserName).FirstOrDefaultAsync();

            if (_user != null)
            {
                _user.OTPCode = null;
                await ctx.SaveChangesAsync();
            }
        }

        public async Task<AppUserVm> VerifyOTP(string email, int otpCode)
        {
           var _user = await ctx.Users.Where(x => x.Email == email && x.OTPCode == otpCode).Select(x=> new AppUserVm {
                          Id= x.Id,
                          FullName =x.FirstName + " " + x.LastName,
                          Email=x.Email!,
                          UserName=x.UserName!,
           }).FirstOrDefaultAsync();
           
            return _user;
        }

        public async Task<bool> ResetPassword(AppUserVm userVm)
        {
              var _user = await    userManager.FindByEmailAsync(userVm.Email);

            if(_user != null)
            {
               await userManager.AddPasswordAsync(_user,userVm.Password);

              await  ctx.SaveChangesAsync();

                return true;
            }

            return false;
        }

        public async Task<UserClaimVM> GetClaims(LoginViewModel loginModel)
        {
            UserClaimVM userClaimVM = new UserClaimVM();
            try
            {
            var _user = await userManager.FindByEmailAsync(loginModel.UserName);

            if (_user != null)
            {
                var _passwordVerified = await userManager.CheckPasswordAsync(_user, loginModel.Password);

                    if (_passwordVerified)
                    {
                        var roles = await userManager.GetRolesAsync(_user);

                        userClaimVM.Email = _user.Email;
                        userClaimVM.UserId = _user.Id;
                        userClaimVM.UserName = _user.UserName;
                        if (roles.Count > 0)
                        {
                            userClaimVM.Roles = new List<string>();
                            foreach (var role in roles)
                            {
                                userClaimVM.Roles.Add(role);
                            }
                        }
                   
                }
            }


            }
            catch (Exception ex)
            {
                throw ex;
            }
            return userClaimVM;
        }
    }
}
