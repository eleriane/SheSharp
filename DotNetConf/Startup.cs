using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using DotNetConf.Data;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Hosting;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Swashbuckle.AspNetCore.SwaggerUI;
using Newtonsoft.Json;
using Microsoft.OpenApi.Models;

namespace DotNetConf
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
            services.AddMvcCore()
                    .AddApiExplorer()
                    .AddNewtonsoftJson(options =>
                            options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore)
                    .SetCompatibilityVersion(CompatibilityVersion.Latest);

            services.AddDbContext<DotNetConfContext>();
            services.AddTransient<DotNetConfSeeder>();

            services.AddVersionedApiExplorer(p =>
            {
                p.GroupNameFormat = "'v'VVV";
                p.SubstituteApiVersionInUrl = true;
            });

            services.AddApiVersioning(cfg =>
            {
                cfg.DefaultApiVersion = new ApiVersion(1, 1);
                cfg.AssumeDefaultVersionWhenUnspecified = true;
                cfg.ReportApiVersions = true;

                //cfg.ApiVersionReader = new QueryStringApiVersionReader("v");

                cfg.ApiVersionReader = ApiVersionReader.Combine(
                  new HeaderApiVersionReader("X-Version"),
                new QueryStringApiVersionReader("v"));
            });

            //services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Developers Women Conference API", Version = "v1", Description = "Esta API é um exemplo de documentação" });
                c.SwaggerDoc("v2", new OpenApiInfo { Title = "Developers Women Conference API", Version = "v2", Description = "Esta API é um exemplo de documentação" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            //Ativa o Swagger
            app.UseSwagger();

            // Ativa o Swagger UI
            app.UseSwaggerUI(opt =>
            {
                //foreach (var description in provider.ApiVersionDescriptions)
                //{
                //    opt.SwaggerEndpoint(
                //    $"/swagger/{description.GroupName}/swagger.json",
                //    description.GroupName.ToUpperInvariant());
                //}
                opt.SwaggerEndpoint("/swagger/v1/swagger.json", "V1");
                opt.SwaggerEndpoint("/swagger/v2/swagger.json", "V2");

                opt.DocExpansion(DocExpansion.List);
            });

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseEndpoints(cfg =>
            {
                cfg.MapControllers();
            });
        }
    }
}
