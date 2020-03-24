using InSimDotNet;
using InSimDotNet.Packets;
using System;
using System.IO;
using System.Reflection;

namespace LFSTwitchChat
{
    class Program
    {
        static string[] chatMessages = { "1", "2", "3", "4", "5", "6", "7" };

        static void Main(string[] args)
        {
            StreamReader configReader = new StreamReader("config.cfg");

            string line = String.Empty;

            string username = "user", password = "pass", channel = "channel";

            while ((line = configReader.ReadLine()) != null)
            {
                if (line.StartsWith("user")) username = line.Substring(line.IndexOf("=") + 1).Trim();
                else if (line.StartsWith("pass")) password = line.Substring(line.IndexOf("=") + 1).Trim();
                else if (line.StartsWith("channel")) channel = line.Substring(line.IndexOf("=") + 1).Trim();
            }

            IrcClient client = new IrcClient("irc.twitch.tv", 6667, username, password, channel);

            InSim insim = new InSim();

            insim.Initialize(new InSimSettings
            {
                Host = "127.0.0.1",
                Port = 29999,
                Admin = String.Empty,
                Flags = InSimFlags.ISF_LOCAL,
                IName = "Twitch Chat",
                Interval = 250
            });

            insim.Bind<IS_MSO>(InSimMessageHandler);

            while (true)
            {
                var message = client.ReadMessage();

                if (!message.StartsWith("PING"))
                {
                    var formattedMessage = message;
                    try { formattedMessage = message.Substring(1, message.IndexOf("!") - 1) + " : " + message.Substring(message.IndexOf(":", 1) + 1); } catch { }

                    Console.WriteLine($"{message}");

                    string[] tempArray = chatMessages;

                    tempArray[6] = tempArray[5];
                    tempArray[5] = tempArray[4];
                    tempArray[4] = tempArray[3];
                    tempArray[3] = tempArray[2];
                    tempArray[2] = tempArray[1];
                    tempArray[1] = tempArray[0];

                    tempArray[0] = formattedMessage;

                    for (int index = 0; index < tempArray.Length; index++)
                    {
                        insim.Send(new IS_BTN
                        {
                            ClickID = (byte)(index + 1),
                            ReqI = 1,
                            BStyle = ButtonStyles.ISB_DARK,
                            Text = tempArray[index],
                            H = 6,
                            L = 0,
                            T = (byte)(120 - index * 6),
                            W = 50,
                            UCID = 0
                        });
                    }

                    insim.Send(new IS_BTN
                    {
                        ClickID = 8,
                        ReqI = 1,
                        Text = "© gxszu.co.uk",
                        H = 4,
                        L = 0,
                        T = 126,
                        W = 10,
                        UCID = 0,
                        BStyle = ButtonStyles.ISB_LEFT | ButtonStyles.ISB_DARK
                    });

                    chatMessages = tempArray;
                }
            }
        }

        private static void InSimMessageHandler(InSim insim, IS_MSO packet)
        {
            if (packet.UserType == UserType.MSO_O && packet.Msg == "clear")
            {
                var fileName = Assembly.GetExecutingAssembly().Location;
                System.Diagnostics.Process.Start(fileName);
                Environment.Exit(0);
            }
        }
    }
}