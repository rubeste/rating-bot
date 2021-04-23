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
                    var logLevel = !string.IsNullOrEmpty(context.Configuration["Environment"]) && context.Configuration["Environment"].Equals("Development")
                        ? LogLevel.Debug
                        : LogLevel.Information;
                    x.SetMinimumLevel(logLevel);
                })
                .ConfigureDiscordHost<DiscordSocketClient>((context, config) =>
                {
                    var logSeverity = !string.IsNullOrEmpty(context.Configuration["Environment"]) && context.Configuration["Environment"].Equals("Development")
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
                        throw new Exception("No token given.");
                    }
                    config.Token = context.Configuration["token"];
                })
                .UseCommandService((context, config) =>
                {
                    var logSeverity = !string.IsNullOrEmpty(context.Configuration["Environment"]) && context.Configuration["Environment"].Equals("Development")
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
                        throw new Exception("Could not obtain RatingConfig values");
                    }
                    else
                    {
                        var ratingConf = context.Configuration.GetSection(nameof(RatingConfig)).Get<RatingConfig>();
                        var idsExist = ratingConf.ChannelIds != null && ratingConf.ChannelIds.Any();
                        var emojiExist = ratingConf.EmojiNames != null && ratingConf.EmojiNames.Count > 1;
                        if (!idsExist && !emojiExist)
                        {
                            throw new Exception("Not enough chanel Id's & not enough emoji's");
                        }
                        else if (!idsExist)
                        {
                            throw new Exception("Not enough chanel Id's");
                        }
                        else if (!emojiExist)
                        {
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