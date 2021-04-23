using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord.Commands;
using Newtonsoft.Json;

namespace RatingBot
{
    public class Program
    {
        public static async Task Main(string[] args)
			=> await Startup.RunAsync(args);
    }
}
