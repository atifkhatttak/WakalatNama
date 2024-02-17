using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Data.DomainModels
{
    public class Review
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ReviewId { get; set; }
        [Required]
        public int CommitById { get; set; }
        [Required]
        public int CommitOnId { get; set; }
        public float? Rating { get; set; }
        public string? ReviewContent { get; set; }
        public DateTime? CreatedDate { get; set; }

        [DefaultValue(false)]
        public bool? IsDeleted { get; set; }
    }
}
