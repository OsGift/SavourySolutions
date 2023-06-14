﻿namespace SavourySolutions.Services.Data
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using SavourySolutions.Data.Common.Repositories;
    using SavourySolutions.Data.Models;
    using SavourySolutions.Models.InputModels.AdministratorInputModels.Privacy;
    using SavourySolutions.Models.ViewModels.Privacy;
    using SavourySolutions.Services.Data.Common;
    using SavourySolutions.Services.Data.Contracts;
    using SavourySolutions.Services.Mapping;

    using Microsoft.EntityFrameworkCore;

    public class PrivacyService : IPrivacyService
    {
        private readonly IDeletableEntityRepository<Privacy> privacyRepository;

        public PrivacyService(IDeletableEntityRepository<Privacy> privacyRepository)
        {
            this.privacyRepository = privacyRepository;
        }

        public async Task<PrivacyDetailsViewModel> CreateAsync(PrivacyCreateInputModel privacyCreateInputModel)
        {
            var privacy = new Privacy
            {
                PageContent = privacyCreateInputModel.PageContent,
            };

            bool doesPrivacyExist = await this.privacyRepository.All().AnyAsync(x => x.PageContent == privacy.PageContent);
            if (doesPrivacyExist)
            {
                throw new ArgumentException(
                    string.Format(ExceptionMessages.PrivacyAlreadyExists, privacy.PageContent));
            }

            await this.privacyRepository.AddAsync(privacy);
            await this.privacyRepository.SaveChangesAsync();

            var viewModel = await this.GetViewModelByIdAsync<PrivacyDetailsViewModel>(privacy.Id);

            return viewModel;
        }

        public async Task DeleteByIdAsync(int id)
        {
            var privacy = await this.privacyRepository.All().FirstOrDefaultAsync(p => p.Id == id);

            if (privacy == null)
            {
                throw new NullReferenceException(
                    string.Format(ExceptionMessages.PrivacyNotFound, id));
            }

            this.privacyRepository.Delete(privacy);
            await this.privacyRepository.SaveChangesAsync();
        }

        public async Task EditAsync(PrivacyEditViewModel privacyEditViewModel)
        {
            var privacy = await this.privacyRepository.All().FirstOrDefaultAsync(p => p.Id == privacyEditViewModel.Id);

            if (privacy == null)
            {
                throw new NullReferenceException(
                    string.Format(ExceptionMessages.PrivacyNotFound, privacyEditViewModel.Id));
            }

            privacy.PageContent = privacyEditViewModel.PageContent;

            this.privacyRepository.Update(privacy);
            await this.privacyRepository.SaveChangesAsync();
        }

        public async Task<TViewModel> GetViewModelAsync<TViewModel>()
        {
            var privacyViewModel = await this.privacyRepository
               .All()
               .To<TViewModel>()
               .FirstOrDefaultAsync();

            return privacyViewModel;
        }

        public async Task<TViewModel> GetViewModelByIdAsync<TViewModel>(int id)
        {
            var privacy = await this.privacyRepository
                .All()
                .Where(p => p.Id == id)
                .To<TViewModel>()
                .FirstOrDefaultAsync();

            if (privacy == null)
            {
                throw new NullReferenceException(string.Format(ExceptionMessages.PrivacyNotFound, id));
            }

            return privacy;
        }
    }
}
