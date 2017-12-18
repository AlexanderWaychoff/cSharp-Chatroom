using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Client
    {
        private NetworkStream stream;
        TcpClient client;
        public string UserId;
        public string userName = "";
        private string disconnected = " disconnected.";
        private bool isReceiving = false;
        Object ReceiveLock = new Object();
        public Dictionary<int, Client> userInfo = new Dictionary<int, Client>();

        public NetworkStream Stream
        {
            get
            {
                return stream;
            }
            set
            {
                stream = value;
            }
        } 
        public Client()
        {
            
        }
        public Client(NetworkStream Streamy, TcpClient Client)
        {
            this.Stream = Streamy;
            client = Client;
            this.userName = Receive();
            UserId = "495933b6-1762-47a1-b655-483510072e73";
        }
        public void Send(string Message)
        { 
            byte[] message = Encoding.ASCII.GetBytes(Message);
            Stream.Write(message, 0, message.Count());
        }
        public string Receive()
        {
            if (!isReceiving)
            {
                lock (ReceiveLock)
                {
                    try
                    {
                        isReceiving = true;
                        byte[] receivedMessage = new byte[256];
                        Stream.Read(receivedMessage, 0, receivedMessage.Length);
                        receivedMessage = TrimEnd(receivedMessage);
                        string recievedMessageString = this.userName + ": " + Encoding.ASCII.GetString(receivedMessage);
                        Console.WriteLine(recievedMessageString);
                        isReceiving = false;
                        return recievedMessageString;
                    }
                    catch
                    {
                        isReceiving = false;
                        return disconnected;
                    }
                }
            }
            return null;
        }
        public static byte[] TrimEnd(byte[] array)
        {
            int lastIndex = Array.FindLastIndex(array, b => b != 0);

            Array.Resize(ref array, lastIndex + 1);

            return array;
        }
    }        
}
