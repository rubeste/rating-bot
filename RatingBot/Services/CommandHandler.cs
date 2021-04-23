using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Addons.Hosting;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RatingBot.Configs;

namespace RatingBot.Services
{
    public class CommandHandler : InitializedService
    {
        private readonly IServiceProvider _provider;
        private readonly DiscordSocketClient _client;
        private readonly CommandService _service;
        private readonly IConfiguration _config;
        private readonly IOptions<RatingConfig> _ratingConf;
        private readonly RatingService _rating;
        private readonly ILogger<CommandHandler> _logger;

        public CommandHandler(
            IServiceProvider provider, 
            DiscordSocketClient client, 
            CommandService service, 
            IConfiguration config, 
            IOptions<RatingConfig> ratingConf,
            RatingService rating,
            ILogger<CommandHandler> logger)
        {
            _provider = provider;
            _client = client;
            _service = service;
            _config = config;
            _ratingConf = ratingConf;
            _rating = rating;
            _logger = logger;
        }

        public override async Task InitializeAsync(CancellationToken cancellationToken)
        {
            _client.MessageReceived += OnMessageReceived;
            _service.CommandExecuted += OnCommandExecuted;
            await _service.AddModulesAsync(Assembly.GetEntryAssembly(), _provider);
        }

        private async Task OnMessageReceived(SocketMessage arg)
        {
            if (!(arg is SocketUserMessage message)) return;
            if (message.Source != MessageSource.User) return;
            if (!_ratingConf.Value.ChannelIds.Contains(message.Channel.Id)) return;
            if (message.Attachments.Any() || message.Embeds.Any())
            {
                await _rating.ProcessPictureMessage(message);
                _logger.LogDebug("Processing animal picture");
                return;
            }

            var argPos = 0;
            if (!message.HasStringPrefix(_config["prefix"], ref argPos) && !message.HasMentionPrefix(_client.CurrentUser, ref argPos)) return;

            var context = new SocketCommandContext(_client, message);
            await _service.ExecuteAsync(context, argPos, _provider);
        }

        private async Task OnCommandExecuted(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            if (command.IsSpecified && !result.IsSuccess) await context.Channel.SendMessageAsync($"Error: {result}");
        }
    }
}