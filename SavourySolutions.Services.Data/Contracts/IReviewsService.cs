﻿namespace SavourySolutions.Services.Data.Contracts
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using SavourySolutions.Models.ViewModels;
    using SavourySolutions.Models.ViewModels.Reviews;

    public interface IReviewsService : IBaseDataService
    {
        public Task<ResponseModel> CreateAsync(CreateReviewInputModel createReviewInputModel);

        public Task<IEnumerable<TViewModel>> GetAll<TViewModel>(int recipeId);

        public Task<IEnumerable<TViewModel>> GetTopReviews<TViewModel>();
    }
}
