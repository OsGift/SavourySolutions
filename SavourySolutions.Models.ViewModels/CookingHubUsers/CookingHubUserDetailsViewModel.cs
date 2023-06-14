﻿namespace SavourySolutions.Models.ViewModels.ApplicationUsers
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using SavourySolutions.Services.Mapping;
    using SavourySolutions.Data.Models;
    using SavourySolutions.Data.Models.Enumerations;

    using static SavourySolutions.Models.Common.ModelValidation;
    using static SavourySolutions.Models.Common.ModelValidation.ApplicationUserValidation;

    public class ApplicationUserDetailsViewModel : IMapFrom<ApplicationUser>
    {
        [Display(Name = IdDisplayName)]
        public string Id { get; set; }

        public string Username { get; set; }

        public string FullName { get; set; }

        [Display(Name = CreatedOnDisplayName)]
        public DateTime CreatedOn { get; set; }

        public bool isDeleted { get; set; }

        public Gender Gender { get; set; }

        public IEnumerable<ApplicationRole> UserRoles { get; set; }
    }
}
