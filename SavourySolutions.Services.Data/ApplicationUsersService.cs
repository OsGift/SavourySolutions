namespace SavourySolutions.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using SavourySolutions.Data.Common.Repositories;
    using SavourySolutions.Data.Models;
    using SavourySolutions.Services.Data.Common;
    using SavourySolutions.Services.Data.Contracts;
    using SavourySolutions.Services.Mapping;

    using Microsoft.EntityFrameworkCore;

    public class ApplicationUsersService : IApplicationUsersService
    {
        private readonly IDeletableEntityRepository<ApplicationUser> ApplicationUsersRepository;

        public ApplicationUsersService(IDeletableEntityRepository<ApplicationUser> ApplicationUsersRepository)
        {
            this.ApplicationUsersRepository = ApplicationUsersRepository;
        }

        public async Task BanByIdAsync(string id)
        {
            var ApplicationUser = await this.ApplicationUsersRepository
                .All()
                .FirstOrDefaultAsync(u => u.Id == id);

            if (ApplicationUser == null)
            {
                throw new NullReferenceException(
                    string.Format(ExceptionMessages.ApplicationUserNotFound, id));
            }

            this.ApplicationUsersRepository.Delete(ApplicationUser);
            await this.ApplicationUsersRepository.SaveChangesAsync();
        }

        public async Task UnbanByIdAsync(string id)
        {
            var ApplicationUser = await this.ApplicationUsersRepository
                .AllWithDeleted()
                .FirstOrDefaultAsync(u => u.Id == id);

            if (ApplicationUser == null)
            {
                throw new NullReferenceException(
                    string.Format(ExceptionMessages.ApplicationUserNotFound, id));
            }

            this.ApplicationUsersRepository.Undelete(ApplicationUser);
            await this.ApplicationUsersRepository.SaveChangesAsync();
        }

        public async Task<IEnumerable<TViewModel>> GetAllApplicationUsersAsync<TViewModel>()
        {
            var users = await this.ApplicationUsersRepository
              .AllWithDeleted()
              .To<TViewModel>()
              .ToListAsync();

            return users;
        }

        public async Task<TViewModel> GetViewModelByIdAsync<TViewModel>(string id)
        {
            var ApplicationUserViewModel = await this.ApplicationUsersRepository
                .AllWithDeleted()
                .Where(u => u.Id == id)
                .To<TViewModel>()
                .FirstOrDefaultAsync();

            if (ApplicationUserViewModel == null)
            {
                throw new NullReferenceException(string.Format(ExceptionMessages.ApplicationUserNotFound, id));
            }

            return ApplicationUserViewModel;
        }
    }
}
