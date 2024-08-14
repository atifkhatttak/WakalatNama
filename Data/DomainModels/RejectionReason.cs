using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.DomainModels
{
    public  class RejectionReason
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int RejectionId { get; set; }
        public string? RejectionNote { get; set; }
        public string? Description { get; set; }
    }
}
