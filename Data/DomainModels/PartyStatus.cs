using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProjWakalatnama.DataLayer.Models
{
    public class PartyStatus
    {
        [Key]
        public int ID { get; set; }

        [Required]
        [StringLength(50)]
        public string StatusName { get; set; }
    }
}
