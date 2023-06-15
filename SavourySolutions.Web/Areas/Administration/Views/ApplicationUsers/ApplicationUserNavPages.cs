namespace SavourySolutions.Web.Areas.Administration.Views.ApplicationUsers
{
    using SavourySolutions.Web.Areas.Administration.Views.Shared;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class ApplicationUserNavPages : AdminNavPages
    {
        public static string GetAll => "GetAll";

        public static string GetAllNavClass(ViewContext viewContext) => PageNavClass(viewContext, GetAll);
    }
}
