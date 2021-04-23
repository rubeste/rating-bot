using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RatingBot.Services;

namespace RatingBot.Modules
{
    public class Rating : ModuleBase
    {
        private readonly RatingService _ratingService;
        private readonly IOptions<RatingConfig> _conf;

        public Rating(RatingService ratingService, IOptions<RatingConfig> conf)
        {
            _ratingService = ratingService;
            _conf = conf;
        }

        [Command("stats")]
        public async Task Stats(string arg = null)
        {
            if (arg != null && arg.Equals("Dicks"))
            {
                await Context.Channel.SendMessageAsync("Invalid command. Maybe use group 1 instead of 0.");
                return;
            }
            await _ratingService.GenReport(Context.Channel as ISocketMessageChannel);
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
