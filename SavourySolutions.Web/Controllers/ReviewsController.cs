﻿namespace SavourySolutions.Web.Controllers
{
    using System;
    using System.Threading.Tasks;

    using SavourySolutions.Data.Models;
    using SavourySolutions.Models.ViewModels.Recipes;
    using SavourySolutions.Services.Data.Contracts;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    public class ReviewsController : Controller
    {
        private readonly IReviewsService reviewsService;
        private readonly IRecipesService recipesService;
        private readonly UserManager<ApplicationUser> userManager;

        public ReviewsController(
            IReviewsService reviewsService,
            UserManager<ApplicationUser> userManager,
            IRecipesService recipesService)
        {
            this.reviewsService = reviewsService;
            this.recipesService = recipesService;
            this.userManager = userManager;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(RecipeDetailsPageViewModel input)
        {
            if (input.CreateReviewInputModel.Rate < 1) 
            {
                //ModelState.AddModelError("", "Kindly select a rating");
            }

            if (!this.ModelState.IsValid)
            {
                var recipe = await this.recipesService
                    .GetViewModelByIdAsync<RecipeDetailsViewModel>(input.CreateReviewInputModel.RecipeId);

                var model = new RecipeDetailsPageViewModel
                {
                    Recipe = recipe,
                    CreateReviewInputModel = input.CreateReviewInputModel,
                };

                return this.View("/Views/Recipes/Details.cshtml", model);
            }

            var userId = this.userManager.GetUserId(this.User);
            input.CreateReviewInputModel.UserId = userId;

            try
            {
                await this.reviewsService.CreateAsync(input.CreateReviewInputModel);
            }
            catch (ArgumentException aex)
            {
                return this.BadRequest(aex.Message);
            }

            return this.RedirectToAction("Details", "Recipes", new { id = input.CreateReviewInputModel.RecipeId });
        }

        public async Task<IActionResult> Remove(int reviewId, int recipeId)
        {
            await this.reviewsService.DeleteByIdAsync(reviewId);

            return this.RedirectToAction("Details", "Recipes", new { id = recipeId });
        }
    }
}
