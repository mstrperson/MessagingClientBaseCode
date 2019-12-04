using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace MessagingClientBaseCode
{
    class Program
    {
        static Thread RecieveThread;
        static ServerConnection serverConnection;

        static void Main(string[] args)
        {
            Console.InputEncoding = Encoding.UTF8;
            Console.OutputEncoding = Encoding.UTF8;

            serverConnection = new ServerConnection(new IPAddress(new byte[] { 10, 0, 4, 86 }), 12345, "pc");

            RecieveThread = new Thread(new ThreadStart(DisplayMessages));
            RecieveThread.Start();
            string input = "";

            while(!input.Equals("quit"))
            {
                Console.Write(">>  ");
                input = Console.ReadLine();
                if (input.Contains("|"))
                {
                    string[] parts = input.Split('|');
                    Message message = new Message(parts[0].Split(',').ToList(), parts[1]);
                    serverConnection.Send(message);
                }
            }

            serverConnection.Dispose();
        }

        static void DisplayMessages()
        {
            while(serverConnection.StillConnected)
            {
                if(serverConnection.HasMessages)
                {
                    Console.WriteLine(serverConnection.GetNextMessage());
                }

                Thread.Sleep(250);
            }
        }
    }
}
