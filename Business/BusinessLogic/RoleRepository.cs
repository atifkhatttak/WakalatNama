using Business.Services;
using Data.Context;
using Data.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.BusinessLogic
{
    public class RoleRepository :BaseRepository<AppRole>, IRoleRepository
    {
        public RoleRepository(WKNNAMADBCtx ctx): base(ctx)
        {
                
        }
    }
}
