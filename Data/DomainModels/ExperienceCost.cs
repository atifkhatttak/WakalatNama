using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProjWakalatnama.DataLayer.Models
{
    public class ExperienceCost
    {
        [Key]
        public int ID { get; set; }

        public int? ExperienceMin { get; set; }

        public int? ExperienceMax { get; set; }

        public decimal? CostMin { get; set; }

        public decimal? CostMax { get; set; }

        [MaxLength]
        public string Description { get; set; }
    }
}
