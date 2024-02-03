using Business.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjWakalatnama.DataLayer.Models;

namespace WKLNAMA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : BaseController<UserProfile>
    {
        private readonly IUserRepository userRepository;

        public UsersController(IUserRepository userRepository, IHttpContextAccessor httpContextAccessor) : base(userRepository, httpContextAccessor)
        {
            this.userRepository = userRepository;

        }
    }
}
