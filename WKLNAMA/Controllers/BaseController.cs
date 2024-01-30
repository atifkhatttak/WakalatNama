using Business.BusinessLogic;
using Business.Services;
using Data.DomainModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WKLNAMA.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController<TEntity> : ControllerBase where TEntity : class
    {
        protected IBaseRepository<TEntity> _baseRepository;
        private readonly IHttpContextAccessor httpContextAccessor;

        public BaseController(IBaseRepository<TEntity> baseRepository,IHttpContextAccessor httpContextAccessor)
        {
            _baseRepository = baseRepository;
            this.httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        public virtual async Task<ActionResult> GetAll()
        {
            var data = await _baseRepository.GetAll(); 
            return Ok(Task.FromResult(data));
        }
        [HttpPost]
        public async virtual Task<ActionResult> Post(TEntity _viewModel)
        {
            _baseRepository.Insert(_viewModel);
            await _baseRepository.SaveAsync();
            return Ok(Task.FromResult(_viewModel));
        }

        [HttpGet("Find")]
        public async virtual Task<ActionResult> GetById(object id)
        {
            var data = await _baseRepository.GetById(id);
            return Ok(Task.FromResult(data));
        }

        [HttpDelete]
        public async virtual Task<ActionResult> Delete(object id)
        {
            await _baseRepository.Delete(id);
           await _baseRepository.SaveAsync();

            return Ok(Task.FromResult("Recored Deleted"));
        }

        [HttpPut]
        public async virtual Task<ActionResult> Update(TEntity _object)
        {
              _baseRepository.Update(_object);
          await  _baseRepository.SaveAsync();

            return Ok(Task.FromResult(_object));
        }





    }
}
