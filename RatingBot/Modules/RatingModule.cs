using System.Data;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RatingBot.Configs;
using RatingBot.Services;

namespace RatingBot.Modules
{
    public class RatingModule : ModuleBase<SocketCommandContext>
    {
        private readonly RatingService _ratingService;
        private readonly ILogger<RatingModule> _logger;
        private readonly bool _isDevelopment;

        public RatingModule(RatingService ratingService, ILogger<RatingModule> logger, IHostEnvironment env)
        {
            _ratingService = ratingService;
            _logger = logger;
            _isDevelopment = env.IsDevelopment();
        }

        [Command("stats")]
        public async Task Stats(string arg = null)
        {
            if (arg != null && arg.Equals("Dicks"))
            {
                await Context.Channel.SendMessageAsync("Invalid command. Maybe use group 1 instead of 0.");
                _logger.LogDebug("Rubeste was again being dumb with regex");
                return;
            }
            await _ratingService.GenReport(Context.Channel);
        }

        [Command("test")]
        public async Task Test(string text)
        {
            if (_isDevelopment)
            {
                var newMessage = await Context.Channel.SendMessageAsync(text);
                _ratingService.ProcessChannelMessage(newMessage);
                return;
            }
            await Context.Channel.SendMessageAsync("Command disabled in production.");
        }
    }
}