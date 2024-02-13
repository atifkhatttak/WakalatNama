using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Business.ViewModels
{
    public class CourtCaseVM
    {
        public long? CaseId { get; set; }
        public long CitizenId { get; set; }
        public long LawyerId { get; set; }
        public long RedundantLawyerId { get; set; }
        public string CaseNumber { get; set; }

        public int? PartyId { get; set; }

        public int? CategoryId { get; set; }
        public string? CaseDescription { get; set; }

        public int? CaseJurisdictionId { get; set; }
        public int CourtId { get; set; }
        public int? CasePlacingId { get; set; }
        public IFormFileCollection Documents { get; set; } = null;
        public DateTime? CreatedDate { get; set; }

        public long? CreatedUser { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public long? ModifiedUser { get; set; }
        public bool? IsDeleted { get; set; }
        public string? CaseTitle { get; internal set; }
    }
}
