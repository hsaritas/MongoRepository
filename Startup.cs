using MongoRepository.DataAccess.Abstract;
using MongoRepository.DataAccess.Concrete;
using MongoRepository.Services.Abstract;
using MongoRepository.Services.Concrete;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System.IO;
using System.Reflection;

namespace MongoRepository
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "DS.MongoRepository.API", Version = "v1" });
                var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                foreach (var file in Directory.GetFiles(dir, "*.xml"))
                {
                    c.IncludeXmlComments(file, includeControllerXmlComments: true);
                }
            });

            services.AddCors(options => options.AddPolicy("CorsPolicy",
            builder =>
            {
                builder.WithOrigins(Configuration.GetSection("AppSettings:AllowedOrigins").Get<string[]>()).AllowAnyMethod().AllowAnyHeader();
            }));

            services.AddHealthChecks();
            services.AddControllers();
            services.AddScoped<IMongodbDal, MongoDbModelDal>();
            services.AddScoped<IMongoModelRepositoryService, MongoModelRepositoryService>();
            services.AddScoped<MongoDbBsonDal, MongoDbBsonDal>();
            services.AddScoped<IMongoBsonRepositoryService, MongoBsonRepositoryService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "DS.MongoRepository.API V1");
                c.SupportedSubmitMethods(new Swashbuckle.AspNetCore.SwaggerUI.SubmitMethod[] { });
            });
            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseCors("CorsPolicy");

            app.UseAuthorization();

            //Handle Unhandled Exceptions
            app.UseExceptionHandler(c => c.Run(async context =>
            {
                var exception = context.Features
                    .Get<IExceptionHandlerPathFeature>()
                    .Error;
                
                var result = JsonConvert.SerializeObject(new { error = exception.Message });
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(result);
            }));


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/health");
                endpoints.MapControllers();
            });
        }
    }
}
