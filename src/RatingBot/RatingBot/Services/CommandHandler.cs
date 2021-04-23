using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace RatingBot.Services
{
    public class CommandHandler
    {
        public static IServiceProvider Provider;
        public static DiscordSocketClient Discord;
        public static CommandService Commands;
        public static IOptions<RatingConfig> Config;
        private readonly RatingService _rating;

        public CommandHandler(DiscordSocketClient discord, CommandService commands, IOptions<RatingConfig> config, RatingService rating,
            IServiceProvider provider)
        {
            Provider = provider;
            Discord = discord;
            Commands = commands;
            Config = config;
            _rating = rating;

            Discord.Ready += OnReady;
            Discord.MessageReceived += OnMessageReceived;
        }

        private async Task OnMessageReceived(SocketMessage arg)
        {
            var msg = arg as SocketUserMessage;
            if (msg == null || msg.Author.IsBot || !msg.Channel.Id.Equals(Convert.ToUInt64(Config.Value.ChannelId))) return;
            if (msg.Attachments.Any() || msg.Embeds.Any())
            {
                await _rating.ProcessPictureMessage(msg);
                return;
            }
            var context = new SocketCommandContext(Discord, msg);
            int pos = 0;
            if (msg.HasStringPrefix("!", ref pos) || msg.HasMentionPrefix(Discord.CurrentUser, ref pos))
            {
                var result = await Commands.ExecuteAsync(context, pos, Provider);

                if (!result.IsSuccess)
                {
                    var reason = result.ErrorReason;

                    await context.Channel.SendMessageAsync($"The following error occurred: {Environment.NewLine}{reason}");
                    Console.WriteLine(reason);
                }
            }

        }

        private Task OnReady()
        {
            Console.WriteLine($"Connected as {Discord.CurrentUser.Username}${Discord.CurrentUser.Discriminator}");
            return Task.CompletedTask;
        }


    }
}
