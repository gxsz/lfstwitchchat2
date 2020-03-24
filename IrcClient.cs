using System;
using System.IO;
using System.Net.Sockets;

namespace LFSTwitchChat
{
    class IrcClient
    {
        private string username;
        private string channel;

        private TcpClient tcpClient;
        private StreamReader inputStream;
        private StreamWriter outputStream;

        public IrcClient(string ip, int port, string username, string password, string channel)
        {
            this.username = username;
            this.channel = channel;

            tcpClient = new TcpClient(ip, port);
            inputStream = new StreamReader(tcpClient.GetStream());
            outputStream = new StreamWriter(tcpClient.GetStream());

            outputStream.WriteLine($"PASS {password}");
            outputStream.WriteLine($"NICK {username}");
            outputStream.WriteLine($"USER {username} 8 * :{username}");
            outputStream.WriteLine($"JOIN #{channel}");
            outputStream.Flush();
        }

        public void SendIrcMessage(string message)
        {
            outputStream.WriteLine(message);
            outputStream.Flush();
        }

        public string ReadMessage()
        {
            return inputStream.ReadLine();
        }

        public void SendChatMessage(string message)
        {
            SendIrcMessage($":{username}!{username}@{username}.tmim.twitch.tv PRIVMSG #{channel} :{message}");
        }
    }
}
