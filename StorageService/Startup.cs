using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using StorageService.Models;
using StorageService.Services.Implementations;
using StorageService.Services.Interfaces;
using Swashbuckle.AspNetCore.Swagger;

namespace StorageService
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			this.Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services
				.AddCors();
			
			var mongoClient = new MongoClient(this.Configuration["Mongo:Connection"]);
			var db = mongoClient.GetDatabase(this.Configuration["Mongo:Database"]);

			services
				.AddSingleton(db.GetCollection<BattleInfo>(this.Configuration["Mongo:BattleCollection"]))
				.AddSingleton(db.GetCollection<Frame>(this.Configuration["Mongo:FrameCollection"]));
			
			services.AddSingleton<IBattleStorage, BattleStorage>();
			services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
			
			// Register the Swagger generator, defining 1 or more Swagger documents
			services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new Info { Title = "Storage Service", Version = "v1" });
			});
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseCors(builder => builder
				.AllowAnyOrigin()
				.AllowAnyHeader()
				.AllowAnyMethod());
			
			// Enable middleware to serve generated Swagger as a JSON endpoint.
			app.UseSwagger();

			// Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
			// specifying the Swagger JSON endpoint.
			app.UseSwaggerUI(c =>
			{
				c.SwaggerEndpoint("/swagger/v1/swagger.json", "Storage Service API V1");
				c.RoutePrefix = string.Empty;
			});

			app.UseMvc();
		}
	}
}