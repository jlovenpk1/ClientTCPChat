using System;
using ServerTCPChat.Client;

namespace ServerTCPChat
{
    class Program
    {
        static void Main(string[] args)
        {
            new Client.Client().Start();
        }
    }
}
