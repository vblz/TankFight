using System;
using System.Runtime.InteropServices;
using Docker.DotNet;
using FightServer.Services.Implementations;
using FightServer.Services.Interfaces;
using FightServer.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;

namespace FightServer
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
      services
        .AddCors();

      services
        .Configure<BattleSettings>(this.Configuration.GetSection("BattleSettings"))
        .Configure<ContainerSettings>(this.Configuration.GetSection("ContainerSettings"));

      var dockerUrl = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
        ? new Uri("npipe://./pipe/docker_engine")
        : new Uri("unix:/var/run/docker.sock");

      services
        .AddSingleton<IDockerClient>(
          new DockerClientConfiguration(dockerUrl)
            .CreateClient());

      services
        .AddSingleton<IBattleService, BattleService>()
        .AddSingleton<IDockerService, DockerService>();

      services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

      // Register the Swagger generator, defining 1 or more Swagger documents
      services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new Info { Title = "Fight service", Version = "v1" }); });
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
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Fight Service API V1");
        c.RoutePrefix = string.Empty;
      });

      app.UseMvc();
    }
  }
}