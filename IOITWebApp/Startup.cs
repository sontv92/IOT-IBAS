using System;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.IO.Compression;
using System.Linq;
using System.Text;
using IOITWebApp.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.Swagger;

namespace IOITWebApp
{
    public class Startup
    {
        //Scaffold-DbContext "Server=210.86.231.82,5000;Database=IOITEcommerce;user id=cnttweb;password=cntt@2018;" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models\EF -Force
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDetection();

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => false;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            string domain = Configuration["AppSettings:JwtIssuer"];

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            })
                .AddJwtBearer(cfg =>
                {
                    cfg.RequireHttpsMetadata = false;
                    cfg.SaveToken = true;
                    cfg.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuer = domain,
                        ValidAudience = domain,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["AppSettings:JwtKey"])),
                        ClockSkew = TimeSpan.Zero // remove delay of token when expire
                    };
                });

            //var cultureInfo = new CultureInfo("en-US");
            //CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            //CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "wwwroot/cms";
            });

            services.AddDistributedMemoryCache(); // Adds a default in-memory implementation of IDistributedCache
            services.AddSession(options => {
                options.IdleTimeout = TimeSpan.FromMinutes(600);//You can set Time  
                options.Cookie.HttpOnly = true;
            });

            services.AddWebOptimizer(pipeline =>
            {
                //CMS
                pipeline.AddCssBundle("/cms/main.css", "cms/styles.*.css");
                pipeline.AddJavaScriptBundle("/cms/main.js",
                    "cms/runtime.*.js",
                    "cms/polyfills.*.js",
                    "cms/scripts.*.js",
                    "cms/main.*.js"
                    );

                // Creates a CSS and a JS bundle. Globbing patterns supported.
                pipeline.AddCssBundle("/css/main.css", "css/*.css");

                pipeline.AddJavaScriptBundle("/js/main.js",
                    "js/js/jquery.min.js",
                    "js/js/bootstrap.min.js",
                    "js/js/asidebar.jquery.js",
                    "js/js/wow.min.js",
                    "js/js/owl.carousel.min.js");

                pipeline.AddJavaScriptBundle("/js/app.js",
                    "js/angular/angular.min.js",
                    "js/angular/angular-animate.min.js",
                    "js/angular/angular-aria.min.js",
                    "js/angular/angular-material.min.js",
                    "js/angular/loading-bar.min.js",
                    "js/app/app.js",
                    "js/app/customer.js",
                    "js/app/search.js");

                // AddFiles/AddBundle allow for custom pipelines
                pipeline.AddBundle("/text.txt", "text/plain", "random/*.txt")
                        .AdjustRelativePaths()
                        .Concatenate()
                        .FingerprintUrls()
                        .MinifyCss();
            });

            services.AddResponseCaching();
            services.AddResponseCompression(options =>
            {
                options.Providers.Add<GzipCompressionProvider>();
                options.MimeTypes =
                    ResponseCompressionDefaults.MimeTypes.Concat(
                        new[] { "image/svg+xml" });
            });

            services.Configure<GzipCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.Fastest;
            });
            services.AddMemoryCache();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
            .AddJsonOptions(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver())
            .AddWebApiConventions();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseResponseCaching();

            //app.Use(async (context, next) =>
            //{
            //    string path = context.Request.Path;

            //    if (path.EndsWith(".css") || path.EndsWith(".js"))
            //    {

            //        //Set css and js files to be cached for 30 days
            //        TimeSpan maxAge = new TimeSpan(30, 0, 0, 0);     //7 days
            //        context.Response.Headers.Append("Cache-Control", "max-age=" + maxAge.TotalSeconds.ToString("0"));

            //    }
            //    else if (path.EndsWith(".gif") || path.EndsWith(".jpg") || path.EndsWith(".jpeg")
            //    || path.EndsWith(".png") || path.EndsWith(".webp"))
            //    {
            //        //custom headers for images goes here if needed
            //        TimeSpan maxAge = new TimeSpan(30, 0, 0, 0);     //30days
            //        context.Response.Headers.Append("Cache-Control", "max-age=" + maxAge.TotalSeconds.ToString("0"));
            //    }
            //    else
            //    {
            //        //Request for views fall here.
            //        context.Response.Headers.Append("Cache-Control", "no-cache");
            //        context.Response.Headers.Append("Cache-Control", "private, no-store");

            //    }
            //    await next();
            //});

            app.UseResponseCompression();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            ////Enable middleware to serve generated Swagger as a JSON endpoint.
            //app.UseSwagger();

            //// Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            //// specifying the Swagger JSON endpoint.
            //app.UseSwaggerUI(c =>
            //{
            //    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            //});
            var provider = new FileExtensionContentTypeProvider();
            provider.Mappings[".cache"] = "image/jpg";
            provider.Mappings[".woff"] = "font/woff";

            app.UseStaticFiles(new StaticFileOptions
            {
                ContentTypeProvider = provider,
                OnPrepareResponse = ctx =>
                {
                    const int durationInSeconds = 60 * 60 * 24 * 7;
                    ctx.Context.Response.Headers[Microsoft.Net.Http.Headers.HeaderNames.CacheControl] =
                        "public,max-age=" + durationInSeconds;
                }
            });

            app.Use(async (context, next) =>
            {
                var culture = new CultureInfo("en-US")
                {
                    DateTimeFormat =
                    {
                        ShortDatePattern = "dd/MM/yyyy",
                        LongDatePattern = "dd/MM/yyyy hh:mm:ss tt",
                        ShortTimePattern = "hh:mm:ss tt",
                        LongTimePattern = "hh:mm:ss tt"
                    }
                };

                CultureInfo.CurrentCulture = culture;
                CultureInfo.CurrentUICulture = culture;

                await next();
            });

            app.UseWebOptimizer();
            app.UseHttpsRedirection();
            //app.UseStaticFiles();
            app.UseSpaStaticFiles();
            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseSession();
            app.UseMvc(routes =>
            {
               
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");


                //routes.MapSpaFallbackRoute("spa-fallback", new { controller = "Home", action = "CMS" });
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "AppCMS";

                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });

        }
    }
}
