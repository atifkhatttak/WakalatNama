using Business.BusinessLogic;
using Business.Services;
using Data.DomainModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WKLNAMA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : BaseController<AppRole>
    {
        private readonly IRoleRepository roleRepository;

        public RoleController(IRoleRepository roleRepository, IHttpContextAccessor httpContextAccessor) :base(roleRepository,httpContextAccessor)
        {
            this.roleRepository = roleRepository;
        }

        //[HttpGet]
        //public   ActionResult<List<AppRole>> Get()
        //{
        //   var data= roleRepository.GetAll();
        //     return Ok(data);
        //}


        //[HttpPost]
        //public ActionResult<AppRole> Create(AppRole _model)
        //{
        //      roleRepository.Insert(_model);
        //    return Ok(data);
        //}
    }
}
