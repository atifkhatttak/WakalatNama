﻿using Business.BusinessLogic;
using Business.Services;
using Business.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjWakalatnama.DataLayer.Models;

namespace WKLNAMA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles ="Citizen")]
    public class CategoryController : BaseController<CaseCategory>
    {
        private readonly ICategoryRepository categoryRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CategoryController(ICategoryRepository categoryRepository, IHttpContextAccessor httpContextAccessor) : base(categoryRepository, httpContextAccessor)
        {
            this.categoryRepository = categoryRepository;
            this._httpContextAccessor = httpContextAccessor;
        }

        //[AllowAnonymous]
        //[HttpGet("GetCategories")]
        //public async Task<ActionResult> Get()
        //{
        //    var d = await categoryRepository.GetAll();

        //    return Ok(Task.FromResult(d));
        //}



    }
}
