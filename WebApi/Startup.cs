using System.Net;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using YukiDrive.Contexts;
using YukiDrive.Services;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using YukiDrive.Models;

namespace YukiDrive
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            //首次使用，添加管理员账户
            using (SettingService settingService = new SettingService(new SettingContext()))
            {
                if (settingService.Get("isInit") == null)
                {
                    using (UserService userService = new UserService(new UserContext()))
                    {
                        User adminUser = new User()
                        {
                            Username = YukiDrive.Configuration.AdminName
                        };
                        userService.Create(adminUser, YukiDrive.Configuration.AdminPassword);
                    }
                    settingService.Set("isInit","true").Start();
                }
            }
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            //不要被 CG 采用单一实例
            services.AddSingleton<IDriveAccountService, DriveAccountService>(provider => new DriveAccountService(new SiteContext()));
            services.AddDbContext<SiteContext>(ServiceLifetime.Transient);
            services.AddDbContext<DriveContext>(ServiceLifetime.Transient);
            services.AddTransient<IDriveService, DriveService>();
            services.AddDbContext<UserContext>(ServiceLifetime.Scoped);
            services.AddScoped<IUserService, UserService>();
            //设置
            services.AddDbContext<SettingContext>();
            services.AddScoped<SettingService>();
            //配置身份验证
            var key = Encoding.ASCII.GetBytes(YukiDrive.Configuration.Secret);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        var userService = context.HttpContext.RequestServices.GetRequiredService<IUserService>();
                        var userId = int.Parse(context.Principal.Identity.Name);
                        var user = userService.GetById(userId);
                        if (user == null)
                        {
                            // return unauthorized if user no longer exists
                            context.Fail("Authentication Failed");
                        }
                        return Task.CompletedTask;
                    },
                    OnAuthenticationFailed = context =>
                    {
                        context.Fail("Authentication Failed");
                        return Task.CompletedTask;
                    },
                    OnChallenge = async context =>
                    {
                        await context.HttpContext.Response.BodyWriter.WriteAsync(new ReadOnlyMemory<byte>(Encoding.UTF8.GetBytes("Unauthrized")));
                    },
                    OnForbidden = context =>
                    {
                        context.Fail("Forbidden");
                        return Task.CompletedTask;
                    }
                };
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            // configure DI for application services
            services.AddScoped<IUserService, UserService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // global cors policy
            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseHttpsRedirection();
            app.UseRouting();
            //验证
            app.UseAuthentication();
            //授权
            app.UseAuthorization();
            //spa
            app.UseSpa(config =>
            {
                config.Options.SourcePath = "wwwroot";
            });
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
