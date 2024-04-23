using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Data.DomainModels;

namespace ProjWakalatnama.DataLayer.Models
{
    public class Favourite : BaseModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long FavouriteId { get; set; }

        [Required]
        public long UserId { get; set; }

        [Required]
        public long LawyerId { get; set; }
        [DefaultValue(false)]
        public bool IsFavourite { get; set; }       
    }
}
