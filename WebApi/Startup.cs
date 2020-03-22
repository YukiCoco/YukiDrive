using System.Net;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
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
using YukiDrive.Helpers;

namespace YukiDrive
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
            //SecretKey
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
                    ValidateAudience = false,
                    ValidateIssuer = false
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
            //app.UseHttpsRedirection();
            app.UseRouting();
            //spa应用
            app.UseStaticFiles();
            app.UseSpa(config =>
            {
                config.Options.SourcePath = "wwwroot";
                // if (env.IsDevelopment())
                // {
                //     config.UseProxyToSpaDevelopmentServer("http://localhost:8001");
                // }
            });
            //验证
            app.UseAuthentication();
            //授权
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
