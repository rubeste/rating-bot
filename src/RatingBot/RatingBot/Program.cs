using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RatingBot
{
    public class Program
    {
		private DiscordSocketClient _client;
        private Configuration _config;
        private RatingManager _rating;

		public static void Main(string[] args)
			=> new Program().MainAsync().GetAwaiter().GetResult();

		public async Task MainAsync()
        {
            _processingList = new List<ulong>();
            _config = LoadConfig();
            _rating = new RatingManager(_config);
			_client = new DiscordSocketClient();
			_client.Log += Log;
			_client.MessageReceived += HandleMessage;
			await _client.LoginAsync(TokenType.Bot, _config.Token);
			await _client.StartAsync();
            // Block this task until the program is closed.
			await Task.Delay(-1);
		}

        private Configuration LoadConfig()
        {
            Configuration config;
            using (StreamReader r = new StreamReader(Environment.CurrentDirectory + "\\config.json"))
            {
				config = JsonConvert.DeserializeObject<Configuration>(r.ReadToEnd());
            }
            if (config == null)
            {
                throw new Exception("Couldn't load file");
            }
            return config;
        }

		private Task Log(LogMessage msg)
		{
			Console.WriteLine(msg.ToString());
			return Task.CompletedTask;
		}

        private List<ulong> _processingList;

        private async Task HandleMessage(SocketMessage message)
        {
            if (!message.Channel.Id.Equals(_config.ChannelId) || message.Author.IsBot)
                return;
            //HandleNormalMessage
            if (message.Attachments.Any() || message.Embeds.Any() || Regex.IsMatch(message.Content, "^\\!test")) //TODO: Remove debug code
            {
                _processingList.Add(message.Id);
                await _rating.ProcessPictureMessage(message);
                _processingList.Remove(message.Id);
            }
            //Handel processing
            else if (_processingList.Any())
            {
                await message.Channel.SendMessageAsync("I am still busy processing some messages. Please be patient.");
            }
            //HandleCommand
            else if (Regex.IsMatch(message.Content, "^\\!"))
            {
                await HandleCommand(message);
            }
        }

        private async Task HandleCommand(SocketMessage message)
        {
            var match = Regex.Match(message.Content.Substring(1), "^(\\w+)\\s*");
            switch (match.Groups[1].Value)
            {
                case "stats":
                    //Easter Egg
                    if (Regex.IsMatch(message.Content.Substring(1).Replace(match.Groups[0].Value, ""), "^dicks$"))
                    {
                        await message.Channel.SendMessageAsync("Invalid command. Maybe use group 1 instead of 0.");
                        break;
                    }
                    await _rating.GetReport(message.Channel);
                    break;

                default:
                    await message.Channel.SendMessageAsync("Invalid command.");
                    break;
            }
        }
	}
}
