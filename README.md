# Rating-Bot
This is a simple rating bot that you can use to add reactions to any file, embedded content.

## Known Problems

- The bot adds reactions slowly.
  
  Currently there isn't support adding a lot of reactions,
  and as each requests takes about 1 second it can take a bit.

## Questions/Feedback?
You can use the issues tab to submit issues. Bear in mind that this is a small project i did for fun so it might take some time.

## How to Use
1. download the Docker-Compose file
2. Change image for the proper system
  
    For x86 architectures:

        services:
          ratingbot:
            image: rubeste/ratingbot:latest

    For ARM architectures:

        services:
          ratingbot:
            image: rubeste/ratingbot:latest-arm
    
3. Configure Environment values
4. Run docker-compose up -d

## Environment value structure
This application uses .NET Core configuration with environment variables.
it implements two Prefixes.
- `DOTNET_`
  
    For .NET Core Specific configurations like `ENVIRONMENT`

  - `ENVIRONMENT`

      If this value is Development it will add verbose console output.

- `RatingBot_`

    For Rating Bot Specific configurations like `prefix`

  - `prefix`

    A char to use as a command prefix.

  - `token`

    Your bot token.

  - `RatingConfig__ChannelIds_{X}`

    A list of ulongs representing the channel ids you want the bot to listen to.

  - `RatingConfig__EmojiName__{X}`

    A list of strings representing unicode emoji's or discord emotes. The first entry in the list has a value of 0, the next 1 and so on. This value is used to calculate the avarage rating.

    So if you have 4 emoji's this would be your rating: `X/3`.
    
    If the value is an unicode emoji you just have to enter that emoji into the environment variable.
    Otherwhise you have to give the full discord emote like this: `<:your_emote:000000000000000000> `


Example of each environment variable:

    DOTNET_ENVIRONMENT=Development
    RatingBot_prefix=!
    RatingBot_token=YOUR_TOKEN
    RatingBot_RatingConfig__ChannelIds__0=000000000000000000
    RatingBot_RatingConfig__EmojiNames__0=0️⃣
    RatingBot_RatingConfig__EmojiNames__1=1️⃣
    RatingBot_RatingConfig__EmojiNames__2=2️⃣
    RatingBot_RatingConfig__EmojiNames__3=3️⃣
    RatingBot_RatingConfig__EmojiNames__4=4️⃣
    RatingBot_RatingConfig__EmojiNames__5=5️⃣
    RatingBot_RatingConfig__EmojiNames__6=6️⃣
    RatingBot_RatingConfig__EmojiNames__7=7️⃣
    RatingBot_RatingConfig__EmojiNames__8=8️⃣
    RatingBot_RatingConfig__EmojiNames__9=9️⃣
    RatingBot_RatingConfig__EmojiNames__10=🔟
