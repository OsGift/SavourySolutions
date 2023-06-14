namespace SavourySolutions.Models.ViewModels.Privacy
{
    using System.ComponentModel.DataAnnotations;
    using Ganss.XSS;

    using SavourySolutions.Data.Models;
    using SavourySolutions.Services.Mapping;

    using static SavourySolutions.Models.Common.ModelValidation.PrivacyValidation;

    public class PrivacyDetailsViewModel : IMapFrom<Privacy>
    {
        public int Id { get; set; }

        [Display(Name = PageContentDisplayName)]
        public string PageContent { get; set; } 

        public string SanitizedPageContent => new HtmlSanitizer().Sanitize(this.PageContent);
    }
}
