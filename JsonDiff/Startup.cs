using JsonDiff.Core;
using JsonDiff.Formatters;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace JsonDiff
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
            services.AddControllers(o=>
            {
                o.InputFormatters.Add(new CustomInputFormatter());
                o.OutputFormatters.Add(new CustomOutputFormatter());
            });
            services.AddApiVersioning(o =>
            {
                o.DefaultApiVersion = new ApiVersion(1, 0);
                o.ApiVersionReader = ApiVersionReader.Combine(
                    new QueryStringApiVersionReader("api-version"));
            });
            services.AddSwaggerGen(opt =>
            {
                opt.IncludeXmlComments(@"XmlComments.xml");
            });
            
            //Only Singletons, since they are all stateless.
            services.AddSingleton<IContentComparer, ContentComparer>();
            services.AddSingleton<IFilesDb, FilesDb>(sp => 
                new FilesDb(Configuration.GetConnectionString("JsonDiff")));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSwagger();
            app.UseSwaggerUI();
        }
    }
}
