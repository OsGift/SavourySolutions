﻿namespace SavourySolutions.Models.ViewModels.Reviews
{
    using System;

    using SavourySolutions.Data.Models;
    using SavourySolutions.Models.ViewModels.Recipes;
    using SavourySolutions.Services.Mapping;

    public class ReviewDetailsViewModel : IMapFrom<Review>
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public int Rate { get; set; }

        public int RecipeId { get; set; }

        public RecipeDetailsViewModel Recipe { get; set; }

        public string UserId { get; set; }

        public string UserUsername { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
