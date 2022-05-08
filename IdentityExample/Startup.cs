using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using IdentityExample.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using IdentityExample.CustomPolicies;
using IdentityExample.ModelBinders;
using IdentityExample.Services;
using Hangfire;

namespace IdentityExample
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            string connStr = Configuration.GetConnectionString("shopWithAuth");
            services.AddDbContext<ShopDbContext>(options => options.UseSqlServer(connStr));
            services.AddIdentity<User, IdentityRole>().AddEntityFrameworkStores<ShopDbContext>();
            services.AddIdentityCore<User>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ShopDbContext>()
                .AddTokenProvider<DataProtectorTokenProvider<User>>(TokenOptions.DefaultProvider);
            services.AddTransient<IAuthorizationHandler, AllowUserHandler>();
            services.AddTransient<IEmailService, EmailService>();

            services.AddHangfire(x => x.UseSqlServerStorage(connStr));
            services.AddHangfireServer();

            services.AddAuthentication()
                .AddGoogle(googleOptions =>
                {
                    IConfigurationSection googleAuthSection = Configuration.GetSection("Authentication:Google");
                    googleOptions.ClientId = googleAuthSection.GetSection("ClientId").Value;
                    googleOptions.ClientSecret = googleAuthSection.GetSection("ClientSecret").Value;
                    googleOptions.SignInScheme = IdentityConstants.ExternalScheme;
                })
                .AddFacebook(fbOptions =>
                {
                    IConfigurationSection fbAuthSection = Configuration.GetSection("Authentication:Facebook");
                    fbOptions.AppId = fbAuthSection.GetSection("AppId").Value;
                    fbOptions.AppSecret = fbAuthSection.GetSection("AppSecret").Value;
                });
            services.AddAuthorization(options => {
                options.AddPolicy("AspManager", policy=> {
                    policy.RequireRole("manager", "admin");
                    policy.RequireClaim("PrefferedFramework", "ASP.NET Core");
                });
                options.AddPolicy("LocalUsers", policy => {
                    policy.AddRequirements(new AllowUserPolicy("Denys", "Serhii"));
                });
                options.AddPolicy("PrivatePolicy", policy =>
                {
                    policy.AddRequirements(new AllowPrivatePolicy());
                });
            });
            services.AddDistributedMemoryCache();
            services.AddSession();
            services.AddControllersWithViews(options => {
                options.ModelBinderProviders.Insert(0, new CartModelBinderProvider());
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseHangfireDashboard("/dashboard");
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSession();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "root",
                    pattern: "/",
                    defaults: new { controller = "Home", action = "Index", category = (string)null });
                endpoints.MapControllerRoute(
                    name: "pages",
                    pattern: "page{page}",
                    defaults: new { controller = "Home", action = "Index", category = (string)null },
                    constraints: new { page = @"\d+" });
                endpoints.MapControllerRoute(
                    name: "cart",
                    pattern: "cart/{action=Index}",
                    defaults: new {controller = "Cart"}
                    );
                //endpoints.MapControllerRoute(
                //    name: "categoryContr",
                //    pattern: "categories",
                //    defaults: new { controller = "Categories", action = "Index" });
                endpoints.MapControllerRoute(
                    name: "categories",
                    pattern: "{category}",
                    defaults: new { controller = "Home", action = "Index", page = 1 });

                endpoints.MapControllerRoute(
                    name: "standart",
                    pattern: "{category}/page{page}",
                    defaults: new { controller = "Home", action = "Index" },
                    constraints: new { page = @"\d+" });


                endpoints.MapControllerRoute(
                    name: "Categories",
                    pattern: "products/details/{id?}",
                    defaults: new { controller = "Products", action = "Details" });


/*                endpoints.MapControllerRoute(
                    name: "GetCat",
                    pattern: "admin/Categories/GetCategories/{title?}",
                    defaults: new { controller = "Categories", action = "GetCategories" });*/


                endpoints.MapControllerRoute(
                    name: "productDetails",
                    pattern: "products/details/{id?}",
                    defaults: new { controller = "Products", action = "Details" });
                endpoints.MapControllerRoute(
                    name: "productGetComments",
                    pattern: "products/comments/{id?}"
                   // defaults: new { controller = "Products", action = "GetComments" }
                   );
                endpoints.MapControllerRoute(
                    name: "productGetPhotos",
                    pattern: "products/getPhoto/{id?}"
                    //defaults: new { controller = "Products", action = "GetPhoto" }
                    );


                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "admin/{controller=Home}/{action=Index}/{id?}");

                endpoints.MapControllerRoute(
                    name: "Login",
                    pattern: "admin/{controller=LogIn}/{action=Login}");
            });
        }
    }
}
