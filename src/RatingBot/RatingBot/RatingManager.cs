﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Humanizer;

namespace RatingBot
{
    public class RatingManager
    {
        public List<IMessage> ListedMessages { get; set; }
        private List<IEmote> _emotes;
        private readonly Configuration _config;

        public RatingManager(Configuration config)
        {
            _config = config;
            _emotes = new List<IEmote>();
            ListedMessages = new List<IMessage>();
            foreach (var emoji in _config.EmojiNames)
            {
                if (emoji.StartsWith("<"))
                {
                    _emotes.Add(Emote.Parse(emoji));
                }
                else
                {
                    _emotes.Add(new Emoji(emoji));
                }
            }
        }

        public async Task GetReport(ISocketMessageChannel channel)
        {
            if (!ListedMessages.Any())
            {
                await channel.SendMessageAsync("No messages found");
                return;
            }
            var ratings = new List<MessageRating>();
            foreach (var message in ListedMessages)
            {
                ratings.Add(await _getRatingOfMessage(message, channel));
            }
            ratings.Sort();
            ratings.Reverse();
            var best = ratings.GetRange(0, ratings.Count > 3 ? 3 : ratings.Count);
            var last = ratings.Last();
            await channel.SendMessageAsync($"The results of {last.Message.Timestamp.Month}/{last.Message.Timestamp.Year}:");
            for (int i = 1; i <= best.Count; i++)
            {
                await channel.SendMessageAsync($"In {i.ToOrdinalWords()} place with a rating of: {best[i-1].Rating.ToString(CultureInfo.CurrentCulture)}", messageReference: new MessageReference(best[i - 1].Message.Id, channel.Id));
            }
            if (!best.Any(b => b.Message.Id.Equals(last.Message.Id)))
            {
                await channel.SendMessageAsync($"In last place with a rating of: {last.Rating.ToString(CultureInfo.CurrentCulture)}", messageReference: new MessageReference(last.Message.Id, channel.Id));
            }

        }

        private async Task<MessageRating> _getRatingOfMessage(IMessage message, ISocketMessageChannel channel)
        {
            var total = 0;
            var votes = 0;
            message = await channel.GetMessageAsync(message.Id);
            var reactions = message.Reactions;
            for (int i = 0; i < _emotes.Count; i++)
            {
                total += i * (reactions[_emotes[i]].ReactionCount - 1);
                votes += message.Reactions[_emotes[i]].ReactionCount - 1;
            }
            if (votes == 0)
            {
                return new MessageRating
                {
                    Message = message,
                    Rating = Convert.ToDouble(Math.Round(Convert.ToDouble(5.0), 1))
                };
            }
            return new MessageRating
            {
                Message = message,
                Rating = Convert.ToDouble(Math.Round(Convert.ToDouble(total) / Convert.ToDouble(votes), 1))
            };
        }

        private void _addMessageToList(IMessage message)
        {
            if (ListedMessages.Any() && ListedMessages.First().Timestamp.Month != DateTime.Now.Month)
            {
                ListedMessages = new List<IMessage>();
            }
            ListedMessages.Add(message);
        }

        public async Task ProcessPictureMessage(IMessage message)
        {
            foreach (var emote in _emotes)
            {
                await message.AddReactionAsync(emote);
            }
            _addMessageToList(message);
        }
    }

    class MessageRating : IComparable<MessageRating>
    {
        public IMessage Message { get; set; }
        public double Rating { get; set; }

        public int CompareTo(MessageRating other)
        {
            if (other == null)
            {
                return 1;
            }
            return Rating.CompareTo(other.Rating);
        }
    }
}