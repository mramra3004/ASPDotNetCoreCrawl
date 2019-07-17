using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace ASPDotNetCore3Crawl
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
	    string port=Environment.GetEnvironmentVariable("PORT");
	    if (port == null || port="") port=5000;

	    Console.WriteLine("Port:"+port);
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>()
			.UseUrls("http://*:"+port+");
                });
    }
}
