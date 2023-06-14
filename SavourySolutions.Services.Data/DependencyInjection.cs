using CloudinaryDotNet;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SavourySolutions.Common.Attributes;
using SavourySolutions.Data;
using SavourySolutions.Data.Common.Repositories;
using SavourySolutions.Data.Models;
using SavourySolutions.Data.Repositories;
using SavourySolutions.Data.Seeding;
using SavourySolutions.Services.Data.Contracts;
using SavourySolutions.Services.Messaging;

namespace SavourySolutions.Services.Data;

public static class DependencyInjection
{
    public static void ConfigureInfraStructure(this IServiceCollection services, IConfiguration configuration)
    {
        var constr = configuration.GetConnectionString("ConnStr");
        services.AddDbContext<ApplicationDbContext>(
               options =>
               {
                   options.UseSqlServer(configuration.GetConnectionString("ConnStr"));
               });
        
        services.AddDefaultIdentity<ApplicationUser>(IdentityOptionsProvider.GetIdentityOptions)
            .AddRoles<ApplicationRole>().AddEntityFrameworkStores<ApplicationDbContext>();

        //services.Configure<CookiePolicyOptions>(
        //    options =>
        //    {
        //        options.CheckConsentNeeded = context => true;
        //        options.MinimumSameSitePolicy = SameSiteMode.None;
        //    });

        services.AddAntiforgery(options =>
        {
            options.HeaderName = "X-CSRF-TOKEN";
        });

        //services.AddControllersWithViews(
        //    options =>
        //    {
        //        options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
        //    }).AddRazorRuntimeCompilation();
        //services.AddRazorPages();

        services.AddScoped<PasswordExpirationCheckAttribute>();

        services.AddSignalR();

        //services.AddSingleton(this.configuration);

        // Data repositories
        services.AddScoped(typeof(IDeletableEntityRepository<>), typeof(EfDeletableEntityRepository<>));
        services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));

        // Application services
        services.AddTransient<IEmailSender>(
            serviceProvider => new SendGridEmailSender(configuration["SendGridSavourySolutions:ApiKey"]));
        services.AddTransient<ICloudinaryService, CloudinaryService>();
        services.AddTransient<IContactsService, ContactsService>();
        services.AddTransient<IPrivacyService, PrivacyService>();
        services.AddTransient<IFaqService, FaqService>();
        services.AddTransient<ICategoriesService, CategoriesService>();
        services.AddTransient<IArticlesService, ArticlesService>();
        services.AddTransient<IRecipesService, RecipesService>();
        services.AddTransient<IArticleCommentsService, ArticleCommentsService>();
        services.AddTransient<IReviewsService, ReviewsService>();
        services.AddTransient<IApplicationUsersService, ApplicationUsersService>();
        services.AddTransient<IChatService, ChatService>();

        // External login providers
        services.AddAuthentication()
            .AddFacebook(facebookOptions =>
            {
                facebookOptions.AppId = configuration["Authentication:Facebook:AppId"];
                facebookOptions.AppSecret = configuration["Authentication:Facebook:AppSecret"];
            });

        var account = new Account(
            configuration["Cloudinary:AppName"],
            configuration["Cloudinary:AppKey"],
            configuration["Cloudinary:AppSecret"]);      

        Cloudinary cloudinary = new Cloudinary(account);

        services.AddSingleton(cloudinary);

        // Seed the data
        using (var serviceScope = services.BuildServiceProvider().CreateScope())
        {
            var dbContext = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            dbContext.Database.Migrate();
            new ApplicationDbContextSeeder().SeedAsync(dbContext, serviceScope.ServiceProvider).GetAwaiter().GetResult();
        }
    }
}