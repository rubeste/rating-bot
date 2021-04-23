using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatingBot.Configs
{
    public class RatingConfig
    {
        public ulong[] ChannelIds { get; set; }
        public List<string> EmojiNames { get; set; }
        public string Environment { get; set; }
    }
}
