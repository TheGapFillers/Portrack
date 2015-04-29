using System;
using System.Configuration;
using Microsoft.Owin.Hosting;
using TheGapFillers.AuthService.WebApi;

namespace TheGapFillers.AuthService.Hosts.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            string host = ConfigurationManager.AppSettings["Host"];
            Console.WriteLine("Starting on {0}", host);


            // Start OWIN host.
            using (WebApp.Start<Startup>(host))
            {
                Console.WriteLine("Press enter to quit.");
                Console.ReadLine();
            }
        }
    }
}
