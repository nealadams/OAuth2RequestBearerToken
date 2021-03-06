﻿using System;
using System.IO;
using Microsoft.Extensions.Configuration;

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
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            _configuration = builder.Build();
            _configuration.GetSection("OAuth2Configuration").Bind(_oAuth2Config);

            var app = new Program();
            app.GetBearerToken();
        }

        private async void GetBearerToken()
        {
            BearerTokenClient bearerTokenClient = new BearerTokenClient();
            var result = await bearerTokenClient.GetBearerToken(_oAuth2Config);
            Console.WriteLine("Bearer Token: ");
            Console.Write(result.access_token);

            var expires = DateTime.Now.AddSeconds(Convert.ToInt64(result.expires_in)).ToString();
            
            Console.WriteLine(Environment.NewLine + "Expires:" + Environment.NewLine + expires);
            DotnetCoreClipboard.Clipboard.SetText(result.access_token);
        }
    }
}