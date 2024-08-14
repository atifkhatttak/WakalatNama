using Business.Chat_Hub;
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
        private readonly IMessageRepository messageRepository;
        private readonly RoleManager<AppRole> roleManager;
        private readonly IEmailService _emailService;
        private readonly INotificationRepository _notification;
        private readonly IConfiguration config;
        private readonly ChatHub chatHub;

        public AccountRepository(WKNNAMADBCtx ctx, UserManager<AppUser> userManager, RoleManager<AppRole> roleManager, IConfiguration config,
            ChatHub chatHub,IMessageRepository messageRepository,IEmailService emailService,INotificationRepository notification) : base(ctx)
        {
            this.ctx = ctx;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.config = config;
            this.chatHub = chatHub;
            this.messageRepository = messageRepository;
            this._emailService = emailService;
            this._notification = notification;
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

              var isUserExist= ctx.Users.Any(x => x.UserName == _user.UserName || x.Email == _user.Email && !x.IsDeleted);

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
                                RoleId = _role.Id;
                                var roleresult = await userManager.AddToRoleAsync(_user, _role.Name ?? "Citizen");
                            }
                        }
                    }

                    //Insert User Profile 
                    var insertedUser = ctx.Users.Where(x => x.Email == _user.Email).FirstOrDefault();
                    if (insertedUser != null)
                    {
                        UserProfile userProfile = new UserProfile()
                        {
                            UserId = insertedUser.Id,
                            RoleId = RoleId,
                            UserName = insertedUser.UserName,
                            FullName = insertedUser.FirstName + " " + insertedUser.LastName,
                            Email = insertedUser.Email ?? "",
                            ContactNumber = insertedUser.PhoneNumber ?? "",
                            IsOverseas = registerViewModel.IsOverseas,
                            CityId = registerViewModel.CityId,
                        };
                        if (RoleId == (int)Roles.Lawyer)
                        {
                            //Add lawyer Id
                            var c=ctx.Cities.Where(c=>c.Id==registerViewModel.CityId).FirstOrDefault();

                            userProfile.LawyerCode = $"{config["AppConfig:PrefixLawyerCode"]}{c?.ShortName}{insertedUser?.Id + 1000}";
                            userProfile.IsActive = false;
                            userProfile.IsAgreed = false;
                            userProfile.IsVerified = false;
                        }
                        else
                        {
                            userProfile.IsActive = true;
                            userProfile.IsAgreed = true;
                            userProfile.IsVerified = true;
                        }

                        await ctx.AddAsync(userProfile);
                        await ctx.SaveChangesAsync();
                        registerViewModel.UserProfileId = userProfile.ProfileId!;

                        //Create message pipline with Admin
                        var getAdmin = await ctx.UserProfiles.FirstOrDefaultAsync(x => x.RoleId == (int)Roles.Admin);
                        if (RoleId == (int)Roles.Lawyer || RoleId == (int)Roles.Citizen)
                        {                            
                            var res = messageRepository.Create(new MessageVm()
                            {
                                FromUserId = getAdmin?.UserId??0,
                                ToUserId = insertedUser.Id,
                                Content = config["AppConfig:AdminGreetingMsg"].ToString(),
                                IsRead = false,
                                ParentId = -1
                            }).Result;
                             chatHub.DirectMessage(res);
                        }

                        #region Case DateNotification

                        if (RoleId==(int)Roles.Lawyer)
                        {                            
                            try
                            {
                                //email
                                string body = $"New lawyer {insertedUser.Email} requested for approval";
                                await _emailService.SendMailTrapEmail($"{insertedUser.Email} Account Request", body, "asimrahim777@gmail.com");

                                //Send Notification
                                NotificationVm vm = new NotificationVm();
                                vm.NotificationType = (int)NotificationType.CaseDate;
                                vm.ToUserId = getAdmin.UserId;
                                vm.FromUserId = insertedUser.Id;
                                vm.Content = body;
                                vm.ImageUrl = "";

                                await _notification.BroadCastAndSaveNotification(vm);
                            }
                            catch (Exception)
                            {
                            }
                        }

                        #endregion
                    }

                    registerViewModel.UserId= insertedUser?.Id??0;
                   
                }
                else
                {
                    registerViewModel = null;
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
            AppUserVm _appUser = null;

            var _dbUser =await userManager.FindByEmailAsync(email);
            
            if (_dbUser != null) 
            {
                 _appUser = new AppUserVm
                {
                    Id = _dbUser.Id,
                    FullName = _dbUser.FirstName + " " + _dbUser.LastName,
                    Email = _dbUser.Email!,
                    UserName = _dbUser.UserName!,
                };

             var _passwordToken = await  userManager.GeneratePasswordResetTokenAsync(_dbUser);

                
                _dbUser!.OTPCode = Convert.ToInt32(GenerateOTP(4));
                _appUser.OTPCode =(int)_dbUser!.OTPCode;
              await SaveAsync();
                    
            }
            return _appUser!;

        }
        private string GenerateOTP(int noOfDigits)
        {
            Random rnd = new Random();
            string otpDigit = string.Empty;

            for(int i = 0; i < noOfDigits; i++)
            {
                int digit = rnd.Next(0, 9); // creates a number between 0 and 9
                otpDigit += digit;
            }
            return otpDigit;          
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
               await userManager.RemovePasswordAsync(_user);
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

                       var userProfile =await ctx.UserProfiles.Where(x => x.UserId == _user.Id && !x.IsDeleted).FirstOrDefaultAsync();
                        if (userProfile!=null)
                        {
                            userClaimVM.UserName = userProfile.FullName; //primarily Full name will be shown
                            userClaimVM.CityId=userProfile.CityId;
                            userClaimVM.ProfilePic = userProfile.ProfilePicUrl;
                        }

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

        public async Task<List<AppUserVm>> GetCitizenLaywer(long citizenId)
        {
            var _user = await (from u in ctx.Users
                                join c in ctx.CourtCases
                                on u.Id equals c.CitizenId
                                where u.IsDeleted != false && c.IsDeleted != true && u.Id == citizenId
                               select new AppUserVm
                                {
                                    UserName = u.UserName,
                                    FullName = u.FirstName + " " + u.LastName,
                                    Id = u.Id,
                                    Email = u.Email,
                                })
                               .Union(
                               from u1 in ctx.Users
                               where u1.IsDeleted != true && u1.UserName == "cservice"
                               select new AppUserVm
                               {
                                   UserName = u1.UserName,
                                   FullName = u1.FirstName + " " + u1.LastName,
                                   Id = u1.Id,
                                   Email = u1.Email,
                               }
                              ).ToListAsync();

            return _user;
        }

        public async Task<List<AppUserVm>> GetLaywerCitizen(long lawyerId)
        {
            var _user = await (from u in ctx.Users
                               join c in ctx.CourtCases
                               on u.Id equals c.LawyerId
                               where u.IsDeleted != false && c.IsDeleted != true && u.Id== lawyerId
                               select new AppUserVm
                               {
                                   UserName = u.UserName,
                                   FullName = u.FirstName + " " + u.LastName,
                                   Id = u.Id,
                                   Email = u.Email,
                               })
                               .Union(
                               from u1 in ctx.Users
                               where u1.IsDeleted != true && u1.UserName == "cservice"
                               select new AppUserVm
                               {
                                   UserName = u1.UserName,
                                   FullName = u1.FirstName + " " + u1.LastName,
                                   Id = u1.Id,
                                   Email = u1.Email,
                               }
                              ).ToListAsync();

            return _user;
        }
        public async Task<List<AppUserVm>> GetAdminUsers(long adminId)
        {
            var _user = await (from u in ctx.Users
                               where u.IsDeleted != false && u.Id!=adminId
                               select new AppUserVm
                               {
                                   UserName = u.UserName,
                                   FullName = u.FirstName + " " + u.LastName,
                                   Id = u.Id,
                                   Email = u.Email,
                               }).ToListAsync();

            return _user;
        }

        public async Task<List<AppUserVm>> GetCustomerSerice()
        {

            var _user = await (from u in ctx.Users
                               where u.IsDeleted != false && u.UserName== "cservice"
                               select new AppUserVm
                               {
                                   UserName = u.UserName,
                                   FullName = u.FirstName + " " + u.LastName,
                                   Id = u.Id,
                                   Email = u.Email,
                               }).ToListAsync();
            return _user;
        }

        public async Task<List<AppUserVm>> GetChatUser(long userId, string roleName)
        {
            List<AppUserVm> _users = new List<AppUserVm>();

            if (roleName == Roles.Citizen.ToString())
                _users.AddRange(await GetCitizenLaywer(userId));
            else if (roleName == Roles.Lawyer.ToString())
                _users.AddRange(await GetLaywerCitizen(userId));
            else
                _users.AddRange(await GetAdminUsers(userId));

            return _users;
        }

        public async Task<bool> UpdateToken(long userId, string token)
        {
            return !string.IsNullOrEmpty(token) &&
            await ctx.UserProfiles.Where(x => x.UserId == userId && x.IsActive == true && !x.IsDeleted)
             .ExecuteUpdateAsync(x => x.SetProperty(p => p.DeviceToken, token)) > 0;
        }
    }
}
