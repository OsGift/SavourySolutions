namespace SavourySolutions.Services.Data.Tests
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    using SavourySolutions.Data;
    using SavourySolutions.Data.Models;
    using SavourySolutions.Data.Models.Enumerations;
    using SavourySolutions.Data.Repositories;
    using SavourySolutions.Models.ViewModels.ApplicationUsers;
    using SavourySolutions.Services.Data.Common;
    using SavourySolutions.Services.Data.Contracts;
    using SavourySolutions.Services.Mapping;

    using Microsoft.Data.Sqlite;
    using Microsoft.EntityFrameworkCore;

    using Newtonsoft.Json;
    using Xunit;

    public class ApplicationUsersServiceTests : IAsyncDisposable
    {
        private readonly IApplicationUsersService ApplicationUsersService;
        private EfDeletableEntityRepository<ApplicationUser> ApplicationUsersRepository;
        private SqliteConnection connection;

        private ApplicationUser firstApplicationUser;

        public ApplicationUsersServiceTests()
        {
            this.InitializeMapper();
            this.InitializeDatabaseAndRepositories();
            this.InitializeFields();

            this.ApplicationUsersService = new ApplicationUsersService(this.ApplicationUsersRepository);
        }

        [Fact]
        public async Task CheckIfBanByIdAsyncWorksCorrectly()
        {
            this.SeedDatabase();

            await this.ApplicationUsersService.BanByIdAsync(this.firstApplicationUser.Id);

            var count = await this.ApplicationUsersRepository.All().CountAsync();

            Assert.Equal(0, count);
        }

        [Fact]
        public async Task CheckIfBanByIdAsyncThrowsNullReferenceException()
        {
            this.SeedDatabase();

            var exception = await Assert
                .ThrowsAsync<NullReferenceException>(async () => await this.ApplicationUsersService.BanByIdAsync("3"));

            Assert.Equal(string.Format(ExceptionMessages.ApplicationUserNotFound, 3), exception.Message);
        }

        [Fact]
        public async Task CheckIfUnbanByIdAsyncWorksCorrectly()
        {
            this.SeedDatabase();

            await this.ApplicationUsersService.UnbanByIdAsync(this.firstApplicationUser.Id);

            var count = await this.ApplicationUsersRepository.All().CountAsync();

            Assert.Equal(1, count);
        }

        [Fact]
        public async Task CheckIfUnbanByIdAsyncThrowsNullReferenceException()
        {
            this.SeedDatabase();

            var exception = await Assert
                .ThrowsAsync<NullReferenceException>(async () => await this.ApplicationUsersService.UnbanByIdAsync("2"));

            Assert.Equal(string.Format(ExceptionMessages.ApplicationUserNotFound, 2), exception.Message);
        }

        [Fact]
        public async Task CheckIfGetAllApplicationUsersAsyncWorksCorrectly()
        {
            this.SeedDatabase();

            var result = await this.ApplicationUsersService.GetAllApplicationUsersAsync<ApplicationUserDetailsViewModel>();

            var count = result.Count();

            Assert.Equal(1, count);
        }

        [Fact]
        public async Task CheckIfGetViewModelByIdAsyncWorksCorrectly()
        {
            this.SeedDatabase();

            var expectedModel = new ApplicationUserDetailsViewModel
            {
                Id = this.firstApplicationUser.Id,
                Username = this.firstApplicationUser.UserName,
                FullName = this.firstApplicationUser.FullName,
                CreatedOn = this.firstApplicationUser.CreatedOn,
                isDeleted = this.firstApplicationUser.IsDeleted,
                Gender = this.firstApplicationUser.Gender,
            };

            var viewModel = await this.ApplicationUsersService
                .GetViewModelByIdAsync<ApplicationUserDetailsViewModel>(this.firstApplicationUser.Id);

            var expectedObj = JsonConvert.SerializeObject(expectedModel);
            var actualResultObj = JsonConvert.SerializeObject(viewModel);

            Assert.Equal(expectedObj, actualResultObj);
        }

        [Fact]
        public async Task CheckIfGetViewModelByIdAsyncThrowsNullReferenceException()
        {
            this.SeedDatabase();

            var exception = await Assert
                .ThrowsAsync<NullReferenceException>(
                async () => await this.ApplicationUsersService.GetViewModelByIdAsync<ApplicationUserDetailsViewModel>("3"));

            Assert.Equal(string.Format(ExceptionMessages.ApplicationUserNotFound, "3"), exception.Message);
        }

        public async ValueTask DisposeAsync()
        {
            await this.connection.CloseAsync();
            await this.connection.DisposeAsync();
        }

        private void InitializeDatabaseAndRepositories()
        {
            this.connection = new SqliteConnection("DataSource=:memory:");
            this.connection.Open();
            var options = new DbContextOptionsBuilder<ApplicationDbContext>().UseSqlite(this.connection);
            var dbContext = new ApplicationDbContext(options.Options);

            dbContext.Database.EnsureCreated();

            this.ApplicationUsersRepository = new EfDeletableEntityRepository<ApplicationUser>(dbContext);
        }

        private void InitializeFields()
        {
            this.firstApplicationUser = new ApplicationUser
            {
                Id = "1",
                FullName = "Kiril Petrov",
                UserName = "Kiril789",
                Gender = Gender.Male,
                CreatedOn = DateTime.Parse("2020-02-10"),
            };
        }

        private async void SeedDatabase()
        {
            await this.SeedUsers();
        }

        private async Task SeedUsers()
        {
            await this.ApplicationUsersRepository.AddAsync(this.firstApplicationUser);

            await this.ApplicationUsersRepository.SaveChangesAsync();
        }

        private void InitializeMapper() => AutoMapperConfig.
            RegisterMappings(Assembly.Load("SavourySolutions.Models.ViewModels"));
    }
}
