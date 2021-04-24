using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Addons.Hosting;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RatingBot.Configs;
using RatingBot.Services;

namespace RatingBot
{
    class Program
    {
        static async Task Main()
        {
            var builder = new HostBuilder()
                .ConfigureHostConfiguration(x =>
                {
                    x.AddEnvironmentVariables("DOTNET_");
                })
                .ConfigureAppConfiguration(x =>
                {
                    var configuration = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", true, true)
                        .AddEnvironmentVariables("RatingBot_")
                        .Build();
                    x.AddConfiguration(configuration);
                })
                .ConfigureLogging((context, x) =>
                {
                    x.AddConsole();
                    var logLevel = context.HostingEnvironment.IsDevelopment()
                        ? LogLevel.Debug
                        : LogLevel.Information;
                    x.SetMinimumLevel(logLevel);
                })
                .ConfigureDiscordHost<DiscordSocketClient>((context, config) =>
                {
                    var logSeverity = context.HostingEnvironment.IsDevelopment()
                        ? LogSeverity.Verbose
                        : LogSeverity.Info;
                    config.SocketConfig = new DiscordSocketConfig
                    {
                        LogLevel = logSeverity, // Defines what kind of information should be logged from the API (e.g. Verbose, Info, Warning, Critical) adjust this to your liking
                        AlwaysDownloadUsers = true,
                        MessageCacheSize = 200,
                    };
                    if (string.IsNullOrEmpty(context.Configuration["token"]))
                    {
                        Console.WriteLine("No token given.");
                        throw new Exception("No token given.");
                    }
                    config.Token = context.Configuration["token"];
                })
                .UseCommandService((context, config) =>
                {
                    var logSeverity = context.HostingEnvironment.IsDevelopment()
                        ? LogSeverity.Verbose
                        : LogSeverity.Info;
                    config.CaseSensitiveCommands = false;
                    config.LogLevel = logSeverity;
                    config.DefaultRunMode = RunMode.Async;
                })
                .ConfigureServices((context, services) =>
                {
                    services.AddOptions();
                    if (!context.Configuration.GetSection(nameof(RatingConfig)).Exists())
                    {
                        Console.WriteLine("Could not obtain RatingConfig values");
                        throw new Exception("Could not obtain RatingConfig values");
                    }
                    else
                    {
                        var ratingConf = context.Configuration.GetSection(nameof(RatingConfig)).Get<RatingConfig>();
                        var idsExist = ratingConf.ChannelIds != null && ratingConf.ChannelIds.Any();
                        var emojiExist = ratingConf.EmojiNames != null && ratingConf.EmojiNames.Count > 1;
                        if (!idsExist && !emojiExist)
                        {
                            Console.WriteLine("Not enough chanel Id's & not enough emoji's");
                            throw new Exception("Not enough chanel Id's & not enough emoji's");
                        }
                        else if (!idsExist)
                        {
                            Console.WriteLine("Not enough chanel Id's");
                            throw new Exception("Not enough chanel Id's");
                        }
                        else if (!emojiExist)
                        {
                            Console.WriteLine("Not enough emoji's");
                            throw new Exception("Not enough emoji's");
                        }
                    }
                    services.Configure<RatingConfig>(context.Configuration.GetSection(nameof(RatingConfig)));
                    services.AddHostedService<CommandHandler>();
                    services.AddSingleton<RatingService>();
                })
                .UseConsoleLifetime();
            
            var host = builder.Build();
            using (host)
            {
                await host.RunAsync();
            }
        }
    }
}