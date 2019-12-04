using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace MacVersion
{
    public class ServerConnection : IDisposable
    {
        private Socket serverConnection;

        public bool StillConnected => serverConnection.Connected;

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
            serverConnection.Send(Encoding.UTF8.GetBytes(string.Format("{0}{1}", name, Message.EOM)));

            RecievedMessages = new List<string>();

            ReadThread = new Thread(new ThreadStart(ReadRun));
            ReadThread.Start();
        }

        public void Send(Message message)
        {
            try
            {
                serverConnection.Send(message.RawMessage);
            }
            catch(Exception e)
            {
                Console.WriteLine("Failed to send...");
                Console.WriteLine(e.Message);
            }
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

                        readStr += Encoding.UTF8.GetString(buffer, 0, byteCount);
                        if (readStr.EndsWith(Message.EOM))
                        {
                            readStr = readStr.Replace(Message.EOM, "");
                            readStr = readStr.Replace(Message.SOT, ":  ");
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

        public void Dispose()
        {
            ReadThread.Abort();
            serverConnection.Close();
            serverConnection.Dispose();
        }
    }
}
