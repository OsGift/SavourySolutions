using System.Reflection;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SavourySolutions.Common.Attributes;
using SavourySolutions.Data;
using SavourySolutions.Data.Common.Repositories;
using SavourySolutions.Data.Models;
using SavourySolutions.Data.Repositories;
using SavourySolutions.Data.Seeding;
using SavourySolutions.Models.ViewModels;
using SavourySolutions.Services.Data;
using SavourySolutions.Services.Data.Contracts;
using SavourySolutions.Services.Mapping;
using SavourySolutions.Services.Messaging;
using SavourySolutions.Web.Hubs;
using SavourySolutions.Web.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.Configure<CookiePolicyOptions>(
    options =>
    {
        options.CheckConsentNeeded = context => true;
        options.MinimumSameSitePolicy = SameSiteMode.None;
    });

builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "X-CSRF-TOKEN";
});

builder.Services.AddControllersWithViews(
    options =>
    {
        options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
    }).AddRazorRuntimeCompilation();
builder.Services.AddRazorPages();

builder.Services.ConfigureInfraStructure(builder.Configuration);

var app = builder.Build();


AutoMapperConfig.RegisterMappings(typeof(ErrorViewModel).GetTypeInfo().Assembly);

if (!app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseDatabaseErrorPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStatusCodePagesWithRedirects("/Home/HttpError?statusCode={0}");
app.UseStaticFiles();
app.UseCookiePolicy();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseAdminMiddleware();

// Configure the HTTP request pipeline.
app.UseEndpoints(
    endpoints =>
    {
        endpoints.MapControllerRoute("areaRoute", "{area:exists}/{controller=Home}/{action=Index}/{id?}");
        endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
        endpoints.MapControllerRoute("subscription", "{controller=Home}/{action=ThankYouSubscription}/{email?}");
        endpoints.MapRazorPages();
        endpoints.MapHub<ChatHub>("/chat");
    });

app.Run();
