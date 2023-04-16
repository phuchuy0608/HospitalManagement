


using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using HospitalManagement.Interfaces;
using HospitalManagement.Models;
using HospitalManagement.Services;
using HospitalManagement.Service;

namespace HospitalManagement.Extension
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.KnownProxies.Add(IPAddress.Parse("10.0.0.100"));
            });

            //Token tồn tại trong 2 tiếng
            services.Configure<DataProtectionTokenProviderOptions>(opt =>
                opt.TokenLifespan = TimeSpan.FromHours(2));

            services.AddResponseCompression(opts =>
            {
                opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                    new[] { "application/octet-stream" });
            });
            // Configure DbContext with Scoped lifetime
            services.AddDbContext<DataContext>(options =>
                {
                    options.UseSqlServer(configuration.GetConnectionString("DataContextConnection"));
                    
                }
            );

            services.AddHangfire(config =>
             config.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                 .UseSimpleAssemblyNameTypeSerializer()
                 .UseDefaultTypeSerializer()
                 .UseSqlServerStorage(configuration.GetConnectionString("DataContextConnection"))
         );

            return services;
        }

        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            return services
                .AddScoped<IBenh, Benhsvc>()
                .AddScoped<IDichVu, DichVusvc>()
                .AddScoped<IThuoc, Thuocsvc>()
                .AddScoped<ICustomer, CustomerSvc>()
                .AddScoped<IValidate, ValidateSvc>()
                .AddScoped<IChuyenKhoa, ChuyenKhoasvc>()
                .AddScoped<INhanVienYte, NhanVienYtesvc>()
                .AddScoped<INguoiDung, NguoiDungsvc>()
                .AddScoped<ITiepNhan, TiepNhanSvc>()
                .AddScoped<IKhamBenh, KhamBenhsvc>()
                .AddScoped<ITheLoai, TheLoaisvc>()
                .AddScoped<ITinTuc, TinTucsvc>()
                .AddScoped<ITienIch, TienIchsvc>()
                .AddScoped<ICacheBase,CacheBase>()
                .AddScoped<IAutoBackground, AutoBackgroundSvc>()
                .AddScoped<IReport, ReportSvc>()
                .AddScoped<OTPCLASS>()
                .AddScoped<IDuocSi, DuocSisvc>();


                



        }

      
    }
}
