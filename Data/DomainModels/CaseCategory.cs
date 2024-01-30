﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProjWakalatnama.DataLayer.Models
{
    public class CaseCategory
    {
        [Key]
        public int ID { get; set; }

        [Required]
        [StringLength(100)]
        public string CategoryName { get; set; }
    }
}
