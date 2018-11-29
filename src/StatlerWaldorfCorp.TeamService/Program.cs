using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;


namespace StatlerWaldorfCorp.TeamService
{
    class Program
    {
		
        public static void Main(string[] args)
        {
			//foreach(var v in Environment.GetEnvironmentVariables().Keys) {
			//	Console.WriteLine(v.ToString());
			//}

			BuildWebHost(args).Run();
		}

		public static IWebHost BuildWebHost(string[] args) =>
			WebHost.CreateDefaultBuilder(args)										
				.UseStartup<Startup>()		
				.ConfigureAppConfiguration((hostContext, config) =>
				{
					// delete all default configuration providers
					config.Sources.Clear();
					config.AddJsonFile("appsettings.json", optional:true);
					config.AddEnvironmentVariables();
				})		
				.Build();
    }
}