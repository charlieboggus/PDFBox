using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PDFBox.Api.Data;
using PDFBox.Api.Helpers;

namespace PDFBox.Api
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
            // Get AppSettings section from appsettings.json and store in strongly typed settings object
            services.Configure< AppSettings >(Configuration.GetSection("AppSettings"));

            // Create the Database Contexts
            //services.AddDbContext< PDFBoxContext >(options => options.UseInMemoryDatabase("pdfbox"));   // In memory database for debugging
            services.AddDbContext< PDFBoxContext >(options => options.UseNpgsql(Configuration.GetConnectionString("PDFBoxConnection")));    // PostgreSQL DB for prod

            // Configure JWT authentication for user accounts
            services.AddAuthentication(options => 
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options => 
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration.GetSection("AppSettings").Get< AppSettings >().JwtSecret)),
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                };
            });

            // Add CORS
            services.AddCors();

            // Enable MVC & set compatibility version 2.2
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment()) { app.UseDeveloperExceptionPage(); } else { app.UseHsts(); }

            //app.UseHttpsRedirection(); This is commented out because it causes problems when calling the API from angular
            
            app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod().AllowCredentials());
            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
