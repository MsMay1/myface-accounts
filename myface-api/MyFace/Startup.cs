using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MyFace.Repositories;
using MyFace.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace MyFace
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        private static string CORS_POLICY_NAME = "_myfaceCorsPolicy";

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)  
           .AddJwtBearer(options =>  
           {  
               options.TokenValidationParameters = new TokenValidationParameters  
               {  
                   ValidateIssuer = true,  
                   ValidateAudience = true,  
                   ValidateLifetime = true,  
                   ValidateIssuerSigningKey = true,  
  
                   ValidIssuer = "https://localhost:5001",  
                   ValidAudience = "https://localhost:5001",  
                   IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("KeyForSignInSecret@1234"))  
               };  
           });  
  
            services.AddDbContext<MyFaceDbContext>(options =>
            {
                options.UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole()));
                options.UseSqlite("Data Source=myface.db");
            });

            services.AddCors(options =>
            {
                options.AddPolicy(CORS_POLICY_NAME, builder =>
                    builder
                        .WithOrigins("http://localhost:3000")
                        .AllowAnyMethod()
                        .AllowAnyHeader());
            });

            services.AddControllers();

            services.AddTransient<IInteractionsRepo, InteractionsRepo>();
            services.AddTransient<IPostsRepo, PostsRepo>();
            services.AddTransient<IUsersRepo, UsersRepo>();
            services.AddTransient<AuthService>();
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
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication(); 
            
            app.UseAuthorization();  

            app.UseCors(CORS_POLICY_NAME);

            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}
