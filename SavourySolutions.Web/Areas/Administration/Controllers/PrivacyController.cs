namespace SavourySolutions.Web.Areas.Administration.Controllers
{
    using System.Threading.Tasks;

    using SavourySolutions.Models.InputModels.AdministratorInputModels.Privacy;
    using SavourySolutions.Models.ViewModels.Privacy;
    using SavourySolutions.Services.Data.Contracts;

    using Microsoft.AspNetCore.Mvc;

    public class PrivacyController : AdministrationController
    {
        private readonly IPrivacyService privacyService;

        public PrivacyController(IPrivacyService privacyService)
        {
            this.privacyService = privacyService;
        }

        public IActionResult Index()
        {
            return this.View();
        }

        public IActionResult Create()
        {
            return this.View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(PrivacyCreateInputModel privacyCreateInputModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(privacyCreateInputModel);
            }

            await this.privacyService.CreateAsync(privacyCreateInputModel);
            return this.RedirectToAction("Privacy", "Home", new { area = string.Empty });
        }

        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var privacyToEdit = await this.privacyService
                        .GetViewModelByIdAsync<PrivacyEditViewModel>(id);

                if (privacyToEdit == null)
                {
                    return this.RedirectToAction("Privacy", "Create", new { area = string.Empty });
                }

                return this.View(privacyToEdit);
            }
            catch (Exception ex)
            {
                return this.RedirectToAction("Create", "Privacy");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(PrivacyEditViewModel privacyEditViewModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(privacyEditViewModel);
            }

            await this.privacyService.EditAsync(privacyEditViewModel);
            return this.RedirectToAction("Privacy", "Home", new { area = string.Empty });
        }

        public async Task<IActionResult> Remove(int id)
        {
            try
            {
                var privacyToDelete = await this.privacyService.GetViewModelByIdAsync<PrivacyDetailsViewModel>(id);

                return this.View(privacyToDelete);
            }
            catch (Exception)
            {
                return this.RedirectToAction("Create", "Privacy");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Remove(PrivacyDetailsViewModel privacyDetailsViewModel)
        {
            await this.privacyService.DeleteByIdAsync(privacyDetailsViewModel.Id);

            return this.RedirectToAction("Privacy", "Home", new { area = string.Empty });
        }
    }
}
