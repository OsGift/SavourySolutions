namespace SavourySolutions.Web.Areas.Administration.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;

    using SavourySolutions.Common;
    using SavourySolutions.Data.Models;
    using SavourySolutions.Models.ViewModels.ApplicationUsers;
    using SavourySolutions.Services.Data.Contracts;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class ApplicationUsersController : AdministrationController
    {
        private readonly UserManager<ApplicationUser> ApplicationUserManager;
        private readonly IApplicationUsersService ApplicationUsersService;
        private readonly RoleManager<ApplicationRole> roleManager;

        public ApplicationUsersController(
            UserManager<ApplicationUser> ApplicationUserManager,
            IApplicationUsersService ApplicationUsersService,
            RoleManager<ApplicationRole> roleManager)
        {
            this.ApplicationUserManager = ApplicationUserManager;
            this.ApplicationUsersService = ApplicationUsersService;
            this.roleManager = roleManager;
        }

        public IActionResult Index()
        {
            return this.View();
        }

        public async Task<IActionResult> GetAll()
        {
            var users = await this.ApplicationUsersService
                .GetAllApplicationUsersAsync<ApplicationUserDetailsViewModel>();

            return this.View(users);
        }

        public async Task<IActionResult> Edit(string id)
        {
            var user = await this.ApplicationUserManager.FindByIdAsync(id);
            var isAdmin = await this.ApplicationUserManager.IsInRoleAsync(user, GlobalConstants.AdministratorRoleName);
            var isUser = await this.ApplicationUserManager.IsInRoleAsync(user, GlobalConstants.UserRoleName);

            var currUserRole = user.Roles.FirstOrDefault(x => x.UserId == id);
            var currUserRoleName = await this.roleManager.FindByIdAsync(currUserRole.RoleId);

            var ApplicationUserEditViewModel = new ApplicationUserEditViewModel
            {
                RoleId = currUserRole.RoleId,
                RoleName = currUserRoleName.Name,
            };

            ApplicationUserEditViewModel.RolesList = this.roleManager.Roles
                .Select(x => new SelectListItem
                {
                    Text = x.Name,
                })
                .ToList();

            if (currUserRoleName.Name == GlobalConstants.AdministratorRoleName && isAdmin == true)
            {
                ApplicationUserEditViewModel.RolesList
                    .Find(x => x.Text == GlobalConstants.AdministratorRoleName).Selected = true;
            }

            if (currUserRoleName.Name == GlobalConstants.UserRoleName && isUser == true)
            {
                ApplicationUserEditViewModel.RolesList
                    .Find(x => x.Text == GlobalConstants.UserRoleName).Selected = true;
            }

            return this.View(ApplicationUserEditViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ApplicationUserEditViewModel model, string id)
        {
            if (!this.ModelState.IsValid)
            {
                model.RolesList = this.roleManager.Roles
                .Select(x => new SelectListItem
                {
                    Text = x.Name,
                })
                .ToList();

                return this.View(model);
            }

            if (model.NewRole == model.RoleName)
            {
                model.RolesList = this.roleManager.Roles
                .Select(x => new SelectListItem
                {
                    Text = x.Name,
                })
                .ToList();

                return this.View(model);
            }

            var user = await this.ApplicationUserManager.FindByIdAsync(id);

            await this.ApplicationUserManager.RemoveFromRoleAsync(user, model.RoleName);

            await this.ApplicationUserManager.AddToRoleAsync(
                user,
                model.NewRole);

            return this.RedirectToAction("GetAll", "ApplicationUsers", new { area = "Administration" });
        }

        public async Task<IActionResult> Ban(string id)
        {
            var ApplicationUserToBan = await this.ApplicationUsersService
                .GetViewModelByIdAsync<ApplicationUserDetailsViewModel>(id);

            return this.View(ApplicationUserToBan);
        }

        [HttpPost]
        public async Task<IActionResult> Ban(ApplicationUserDetailsViewModel ApplicationUserDetailsViewModel)
        {
            await this.ApplicationUsersService.BanByIdAsync(ApplicationUserDetailsViewModel.Id);

            return this.RedirectToAction("GetAll", "ApplicationUsers", new { area = "Administration" });
        }

        public async Task<IActionResult> Unban(string id)
        {
            var ApplicationUserToUnban = await this.ApplicationUsersService
                .GetViewModelByIdAsync<ApplicationUserDetailsViewModel>(id);

            return this.View(ApplicationUserToUnban);
        }

        [HttpPost]
        public async Task<IActionResult> Unban(ApplicationUserDetailsViewModel ApplicationUserDetailsViewModel)
        {
            await this.ApplicationUsersService.UnbanByIdAsync(ApplicationUserDetailsViewModel.Id);

            return this.RedirectToAction("GetAll", "ApplicationUsers", new { area = "Administration" });
        }
    }
}
