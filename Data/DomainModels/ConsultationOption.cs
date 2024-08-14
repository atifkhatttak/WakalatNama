using Data.DomainModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProjWakalatnama.DataLayer.Models
{
    public class ConsultationOption : BaseModel
    {
        [Key]
        public int ID { get; set; }

        [Required]
        [StringLength(50)]
        public string ConsultationType { get; set; }
    }
}
