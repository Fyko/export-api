using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Calzolari.Grpc.AspNetCore.Validation;
using Microsoft.Extensions.Hosting;
using ExportAPI.Services;
using ExportAPI.Validators;

namespace ExportAPI
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

			services.AddGrpc(options =>
			{
				options.Interceptors.Add<ExceptionInterceptor>();
				options.EnableMessageValidation();
			});
			services.AddGrpcReflection();

			services.AddValidator<CreateExportRequestValidator>();
			services.AddGrpcValidation();
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
				app.UseHsts();
			}

			app.UseRouting();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapGrpcService<ExporterService>();
				endpoints.MapControllers();

				if (env.IsDevelopment())
				{
					endpoints.MapGrpcReflectionService();
				}
			});

			app.UseStaticFiles(new StaticFileOptions
			{
				FileProvider = new PhysicalFileProvider("/tmp/exports"),
				RequestPath = "/exports"
			});
		}
	}
}
