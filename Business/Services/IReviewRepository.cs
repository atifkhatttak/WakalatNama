using Business.BusinessLogic;
using Business.ViewModels;
using Data.DomainModels;
using ProjWakalatnama.DataLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services
{
    public interface IReviewRepository : IBaseRepository<Review>
    {
        Task<List<Review>>  GetUserReviews(int? userId);
        Task MarkFaourite(ReviewFavouriteVM favouriteVM);
        Task<List<LawyerVM>> GetFavouriteLawyers(long? userId);
    }
}
