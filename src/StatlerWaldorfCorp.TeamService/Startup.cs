using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StatlerWaldorfCorp.TeamService.Models;
using StatlerWaldorfCorp.TeamService.Persistence;
using StatlerWaldorfCorp.TeamService.LocationClient;
using System;
using Microsoft.EntityFrameworkCore;

namespace StatlerWaldorfCorp.TeamService {
  public class Startup
  {
    public static string[] Args {get; set;} = new string[] { };
    private ILogger logger;
    private ILoggerFactory loggerFactory;
    public Startup(IConfiguration configuration, ILoggerFactory loggerFactory)
    {
        Configuration = configuration;

        this.loggerFactory = loggerFactory;
        this.loggerFactory.AddConsole(LogLevel.Information);
        this.loggerFactory.AddDebug();

        this.logger = this.loggerFactory.CreateLogger("StartUp");
    }

    public IConfiguration Configuration { get; } 

    public void ConfigureServices (IServiceCollection services)
    {
      var transient = true;
      if (Configuration.GetSection("transient") != null) {
          transient = Boolean.Parse(Configuration.GetSection("transient").Value);
      }
      
      if (transient) {
          logger.LogInformation("Using transient team record repository.");
          services.AddScoped<ITeamRepository, MemoryTeamRepository>();
      } else {            
          // string connectionString = Configuration.GetSection("sqldb:cstr").Value;

          string sqlServer = Environment.GetEnvironmentVariable("SQL_SERVER"); // "totiputeamsvcdb.database.windows.net";
          string databaseName = Environment.GetEnvironmentVariable("SQL_DATABASE"); // "TeamServiceDB";
          string userId = Environment.GetEnvironmentVariable("SQL_USERID"); // "totipu";
          string password = Environment.GetEnvironmentVariable("SQL_PASSWORD"); // "industrija2!";

          string connectionString = $"Server=tcp:{sqlServer},1433;Initial Catalog={databaseName};Persist Security Info=False;User ID={userId};Password={password};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

          services.AddEntityFrameworkSqlServer()
              .AddDbContext<TeamDbContext> (options => options.UseSqlServer(connectionString));

          logger.LogInformation("Using '{0}' for DB connection string.", connectionString);

          services.AddScoped<ITeamRepository, TeamRecordRepository>();
      }

      services.AddMvc();
      
      var locationUrl = Configuration.GetSection("location:url").Value;
      logger.LogInformation("Using {0} for location service URL.", locationUrl);
      services.AddSingleton<ILocationClient>(new HttpLocationClient(locationUrl));
    }       
    
    public void Configure(IApplicationBuilder app)
    {
      app.UseMvc();
    }
  }   
}
