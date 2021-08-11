using System;
using DiscordRPC;

namespace VKAlpha.Helpers
{
    public class DiscordPresence
    {
        private readonly DiscordRpcClient client;
        private int discordPipe = -1;

        private static readonly Assets defaultAssets = new Assets { LargeImageKey = "alpha", LargeImageText = "VK Alpha" };

        private static readonly RichPresence defaultPresence = new RichPresence
        {
            Details = "LISTENING TO VK ALPHA",
            Timestamps = new Timestamps { Start = DateTime.UtcNow },
            State = "Chillin...",
            Assets = defaultAssets
        };

        public DiscordPresence(int pipe = -1)
        {
            if (pipe != -1) discordPipe = pipe;

            client = new DiscordRpcClient("708756041567043635", discordPipe, autoEvents: true);
        //    client.Logger = new ConsoleLogger(LogLevel.Trace, true);
            setDefaultPresence();
            client.Initialize();
        }

        public void updatePresence(string artist, string title, TimeSpan trackEnd)
        {
            try
            {
                client.SetPresence(new RichPresence
                {
                    Details = artist,
                    Timestamps = new Timestamps(DateTime.UtcNow, DateTime.UtcNow.AddSeconds(trackEnd.TotalSeconds)),
                    State = title,
                    Assets = defaultAssets
                });
            }
            catch (Exception) { }
        }

        public void setDefaultPresence()
        {
            try
            {
                client.SetPresence(defaultPresence);
            }
            catch(Exception) { }
        }

        public void destroy()
        {
            client.Dispose();
        }
    }
}
