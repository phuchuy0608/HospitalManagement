using Hangfire;
using HangfireBasicAuthenticationFilter;
using HospitalManagement.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System;
using HospitalManagement.Extension;
using HospitalManagement.Helpers;
using HospitalManagement.Infrastructure;
using HospitalManagement.Interfaces;
using HospitalManagement.Models;

namespace HospitalManagement
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }



        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddDistributedMemoryCache(); // Đăng ký dịch vụ lưu cache trong bộ nhớ (Session sẽ sử dụng nó)

            services.AddSession(option => { option.IdleTimeout = TimeSpan.FromMinutes(120); });


            var jwtSettings = Configuration.GetSection("JWTSettings");
            services.AddAuthentication()
                .AddCookie(options => { options.SlidingExpiration = false; }
                )



                .AddGoogle(googleOptions =>
                {
                    googleOptions.ClientId = Configuration["Google:ClientId"];
                    googleOptions.ClientSecret = Configuration["Google:ClientSecret"];
                });


            services.AddControllersWithViews().AddNewtonsoftJson(x =>
                x.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);

            services.AddDefaultIdentity<NhanVienYte>(options => { options.SignIn.RequireConfirmedAccount = false; })
                .AddRoles<IdentityRole>().AddErrorDescriber<CustomErrorDescriber>()
                .AddEntityFrameworkStores<DataContext>();
            services
                .AddDatabase(Configuration)
                .AddRepositories();

            services.AddSignalR(options =>
            {
                options.KeepAliveInterval = TimeSpan.FromSeconds(15);
                options.EnableDetailedErrors = true;
            });

            services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
            {
                builder.AllowCredentials();
            }));


            services.AddHangfireServer();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Latest)
                .AddRazorPagesOptions(options =>
                {
                    options.Conventions.AuthorizeAreaFolder("Identity", "/Account/Manage");
                    options.Conventions.AuthorizeAreaPage("Identity", "/Account/Logout");
                }).AddRazorRuntimeCompilation();

            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.Name = ".AspNetCore.Identity.Application";

                options.LoginPath = "/Identity/Account/Login";
                options.LogoutPath = "/Identity/Account/Logout";
                options.AccessDeniedPath = "/Identity/Account/AccessDenied";
                options.ExpireTimeSpan = TimeSpan.FromHours(6);
                options.SlidingExpiration = false;
            });
        }


        //This method gets called by the runtime.Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,
            IRecurringJobManager recurringJobManager, IServiceProvider serviceProvider)
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

            //Xử lý lỗi 
            app.Use(async (context, next) =>
            {
                await next();
                if (context.Response.StatusCode == 403)
                {
                    context.Request.Path = "/Admin/NoneUser";
                    await next();
                }

                if (context.Response.StatusCode == 404)
                {
                    context.Request.Path = "/Error/Error400";
                    await next();
                }

                if (context.Response.StatusCode == 500)
                {
                    context.Request.Path = "/Error/Error500";
                    await next();
                }
            });

            app.UseExceptionHandler(c => c.Run(async context =>
            {
                var exception = context.Features
                    .Get<IExceptionHandlerPathFeature>()
                    .Error;
                var response = new { error = exception.Message };
                await context.Response.WriteAsJsonAsync(response);
            }));

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSession();
            app.UseRouting();
            app.UseCors("MyPolicy");

            app.UseAuthentication();

            app.UseAuthorization();
            app.UseWebSockets();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    "default",
                    "{controller=Home}/{action=Index}");
                endpoints.MapHub<SignalServer>("/signalServer");
                endpoints.MapHub<RealtimeHub>("/PhieuKham");
                endpoints.MapRazorPages();
            });
            //app.UseHangfireDashboard();
            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new[]
                {
                        new HangfireCustomBasicAuthenticationFilter{
                            User = Configuration.GetSection("HangfireSettings:UserName").Value,
                            Pass = Configuration.GetSection("HangfireSettings:Password").Value
                        }
                }

            });
            recurringJobManager.AddOrUpdate(
                "Run every minute",
                () => serviceProvider.GetService<IAutoBackground>().AutoDelete(),
                "00 19 * * *"
            );
        }
    }
}
