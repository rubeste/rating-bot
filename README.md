# Rating-Bot
This is a simple rating bot that you can use to add reactions to any file, embedded content.

## How to install
1. Install dotnet 5.0
2. Download the zip file in the releases.
3. Configure the appsettings.json file
4. Run this command

## Configuration file structure
    {
      "prefix": "!", // Prefix used to create commands.
      "Environment": "Development", // If you set this as Development you get more verbosity in the output. Plus access to the !test command
      "token": "YOUR TOKEN HERE", // Your discord bot token
      "RatingConfig": {
        "ChannelIds": [
          000000000000000000 // ulong channel id for the bot to use
        ],
        "EmojiNames": [ // Emoji's to use as the rating. first entry is allways the lowest and last is the highest.
          "\u0030\uFE0F\u20E3", //0️⃣
          "\u0031\uFE0F\u20E3", //1️⃣
          "\u0032\uFE0F\u20E3", //2️⃣
          "\u0033\uFE0F\u20E3", //3️⃣
          "\u0034\uFE0F\u20E3", //4️⃣
          "\u0035\uFE0F\u20E3", //5️⃣
          "\u0036\uFE0F\u20E3", //6️⃣
          "\u0037\uFE0F\u20E3", //7️⃣
          "\u0038\uFE0F\u20E3", //8️⃣
          "\u0039\uFE0F\u20E3", //9️⃣
          "\uD83D\uDD1F"        //🔟
        ]
      }
    }
