using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatingBot
{
    public class Configuration
    {
        public string Token { get; set; }
        public ulong ChannelId { get; set; }
        public List<string> EmojiNames { get; set; }
    }
}
