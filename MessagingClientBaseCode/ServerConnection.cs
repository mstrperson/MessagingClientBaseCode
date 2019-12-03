using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace MessagingClientBaseCode
{
    public class ServerConnection
    {
        private Socket serverConnection;

        private Thread ReadThread;

        private bool EndProcess = false;

        private List<string> RecievedMessages;

        public bool HasMessages => RecievedMessages.Count > 0;

        public string GetNextMessage()
        {
            if (!this.HasMessages) return "";

            string message = RecievedMessages[0];
            RecievedMessages.RemoveAt(0);

            return message;
        }

        public ServerConnection(IPAddress serverIP, int port, string name)
        {
            serverConnection = new Socket(serverIP.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            serverConnection.Connect(new IPEndPoint(serverIP, port));
            serverConnection.Send(Encoding.ASCII.GetBytes(string.Format("{0}{1}", name, Message.EOM)));

            RecievedMessages = new List<string>();

            ReadThread = new Thread(new ThreadStart(ReadRun));
            ReadThread.Start();
        }

        public void Send(Message message)
        {
            serverConnection.Send(message.RawMessage);
        }

        private void ReadRun()
        {
            while (!EndProcess)
            {
                byte[] buffer = new byte[1024];

                string readStr = "";
                while (serverConnection.Connected)
                {
                    try
                    {
                        int byteCount = serverConnection.Receive(buffer);

                        readStr += Encoding.ASCII.GetString(buffer, 0, byteCount);
                        if (readStr.EndsWith(Message.EOM))
                        {
                            readStr = readStr.Replace(Message.EOM, "");
                            readStr = readStr.Replace(Message.Separator, ":  ");
                            RecievedMessages.Add(readStr);
                            break;
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("I seem to have lost connection...");
                        Console.WriteLine(e.Message);
                    }
                }
            }
        }
    }
}
