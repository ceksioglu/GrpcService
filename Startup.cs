using Microsoft.EntityFrameworkCore;
using GrpcServiceDemo.Server.Services;
using GrpcServiceDemo.Server.Models;

namespace GrpcServiceDemo
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddGrpc();
            services.AddGrpcReflection();

            services.AddDbContext<GrpcUserServiceContext>(options =>
                options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<UserServiceImpl>();

                // Enable the reflection service
                if (env.IsDevelopment())
                {
                    endpoints.MapGrpcReflectionService();
                }

                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("gRPC endpoints require a gRPC client.");
                });
            });
        }
    }
}