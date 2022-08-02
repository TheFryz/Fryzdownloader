 using DiscordRPC;
using DiscordRPC.Logging;
using System;

namespace Fryzdownloader
{


    class Discord
    {
        public DiscordRpcClient client;
        
        void Initialize()
        {
            client = new DiscordRpcClient("971859979478900836");

            //Set the logger
            client.Logger = new ConsoleLogger() { Level = LogLevel.Warning };

            //Subscribe to events
            client.OnReady += (sender, e) =>
            {
                Console.WriteLine("Received Ready from user {0}", e.User.Username);
            };

            client.OnPresenceUpdate += (sender, e) =>
            {
                Console.WriteLine("Received Update! {0}", e.Presence);
            };

            void Update()
            {
                //Invoke all the events, such as OnPresenceUpdate
                client.Invoke();
            };

            //Connect to the RPC
            client.Initialize();

            //Set the rich presence
            //Call this as many times as you want and anywhere in your code.
            client.SetPresence(new RichPresence()
            {
                Details = "Example Project",
                State = "csharp example",
                Assets = new Assets()
                {
                    LargeImageKey = "image_large",
                    LargeImageText = "Lachee's Discord IPC Library",
                    SmallImageKey = "image_small"
                }
            });
        }
    }
}
