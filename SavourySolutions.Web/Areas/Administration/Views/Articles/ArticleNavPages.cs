﻿namespace SavourySolutions.Web.Areas.Administration.Views.Articles
{
    using SavourySolutions.Web.Areas.Administration.Views.Shared;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class ArticleNavPages : AdminNavPages
    {
        public static string CreateArticle => "CreateArticle";

        public static string GetAll => "GetAll";

        public static string CreateArticleNavClass(ViewContext viewContext) => PageNavClass(viewContext, CreateArticle);

        public static string GetAllNavClass(ViewContext viewContext) => PageNavClass(viewContext, GetAll);
    }
}
