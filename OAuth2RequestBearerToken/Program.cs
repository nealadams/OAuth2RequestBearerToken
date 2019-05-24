using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace OAuth2RequestBearerToken
{
    class Program
    {
        private static IConfigurationRoot _configuration;
        private static readonly OAuth2Config _oAuth2Config = new OAuth2Config();
        
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            _configuration = builder.Build();
            _configuration.GetSection("OAuth2Configuration").Bind(_oAuth2Config);

            var app = new Program();
            app.GetBearerToken();
        }

        private async void GetBearerToken()
        {
            BearerTokenClient bearerTokenClient = new BearerTokenClient();
            var result = await bearerTokenClient.GetBearerToken(_oAuth2Config);
            Console.WriteLine("Bearer Token:" + Environment.NewLine + result.access_token);

            var now = DateTime.Now;
            var expires = now.AddSeconds(Convert.ToInt64(result.expires_in));
            
            Console.WriteLine(Environment.NewLine + "Expires:\n" + expires);
        }
    }
}