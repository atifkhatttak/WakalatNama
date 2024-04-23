using Data.DomainModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.ViewModels
{
    public class ReviewFavouriteVM
    {
        public long UserId { get; set; }
        public long LawyerId { get; set; }
        public bool IsFavourite { get; set; }
    }
    public class FavouriteVM
    {
        public long FavouriteId { get; set; }
        public long UserId { get; set; }
        public long LawyerId { get; set; }
        public bool IsFavourite { get; set; }
    }
    public class ReviewVM
    {
        public int ReviewId { get; set; }
        public int CommitById { get; set; }
        public int CommitOnId { get; set; }
        public float? Rating { get; set; }
        public string? ReviewContent { get; set; }
        public string FullName { get; set; }
        public string DocPath { get; set; }
        public DateTime UpdateDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public long CreatedBy { get; set; }
        public long UpdatedBy { get; set; }
        public bool IsDeleted { get; set; } = false;
    }

}
