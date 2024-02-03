using Business.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjWakalatnama.DataLayer.Models;

namespace WKLNAMA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CaseJurisdictionController : BaseController<CaseJurisdiction>
    {
        private readonly ICaseJurisdictionRepository caseJurisdictionRepository;

        public CaseJurisdictionController(ICaseJurisdictionRepository caseJurisdiction, IHttpContextAccessor httpContextAccessor) : base(caseJurisdiction, httpContextAccessor)
        {
            this.caseJurisdictionRepository = caseJurisdiction;

        }
    }
}
