using System.Data;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RatingBot.Configs;
using RatingBot.Services;

namespace RatingBot.Modules
{
    public class RatingModule : ModuleBase<SocketCommandContext>
    {
        private readonly RatingService _ratingService;
        private readonly IOptions<RatingConfig> _conf;
        private readonly ILogger<RatingModule> _logger;

        public RatingModule(RatingService ratingService, IOptions<RatingConfig> conf, ILogger<RatingModule> logger)
        {
            _ratingService = ratingService;
            _conf = conf;
            _logger = logger;
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
            if (_conf.Value.Environment.Equals("Development"))
            {
                var newMessage = await Context.Channel.SendMessageAsync(text);
                await _ratingService.ProcessPictureMessage(newMessage);
                return;
            }
            await Context.Channel.SendMessageAsync("Command disabled in production.");
        }
    }
}