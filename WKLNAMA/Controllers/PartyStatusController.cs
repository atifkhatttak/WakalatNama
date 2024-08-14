using Business.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjWakalatnama.DataLayer.Models;

namespace WKLNAMA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PartyStatusController : BaseController<PartyStatus>
    {
        private readonly IPartyStatusRepository partyStatusRepository;

        public PartyStatusController(IPartyStatusRepository partyStatusRepository, IHttpContextAccessor httpContextAccessor) : base(partyStatusRepository, httpContextAccessor)
        {
            this.partyStatusRepository = partyStatusRepository;

        }
    }
}
