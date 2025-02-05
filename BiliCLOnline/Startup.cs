using BiliCLOnline.IServices;
using BiliCLOnline.Services;
using BiliCLOnline.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;

namespace BiliCLOnline
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
            services.AddSingleton(Configuration);

            services.AddSingleton<WebHelper>();
            services.AddSingleton<Helper>();
            services.AddScoped<IBearerInfo, BearerInfo>();
            services.AddScoped<IReplyResult, ReplyResult>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "BiliCLOnline", Version = "v1" });
            });

            services.AddQueuePolicy(option =>
            {
                option.MaxConcurrentRequests = 10;
                option.RequestQueueLimit = 50;
            });

            services.AddCors(options => 
            {
                options.AddPolicy("cors", p => 
                { 
                    p.WithOrigins(Configuration.GetValue<string>("CorsTarget"))
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
                });
            });

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseConcurrencyLimiter();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "HttpReplApi v1");
                });
            }
            app.UseCors("cors");

            app.UseMiddleware<HCaptchaVerifyingMiddleware>();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
