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
}
