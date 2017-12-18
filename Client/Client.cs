using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class Client
    {
        TcpClient clientSocket;
        NetworkStream stream;
        private bool isConnected;
        private string username;

        public string Username
        {
            get
            {
                return username;
            }
        }

        public bool IsConnected
        {
            get
            {
                return isConnected;
            }
            set
            {
                isConnected = value;
            }
        }

        public Client(string IP, int port)
        {
            clientSocket = new TcpClient();
            try
            {
                GetUsername();
                Console.WriteLine("\nAttempting to connect.\n");
                clientSocket.Connect(IPAddress.Parse(IP), port);
                IsConnected = true;
                Console.WriteLine("Connection Successful!\n");
                stream = clientSocket.GetStream();
            }
            catch
            {
                isConnected = false;
            }
        }
        public void SendName()
        {
            string messageString = Username;
            byte[] message = Encoding.ASCII.GetBytes(messageString);
            stream.Write(message, 0, message.Count());
        }
        public Task Send()
        {
            return Task.Run(() =>
            {
                while (IsConnected)
                {
                    try
                    {
                        string messageString = UI.GetInput();
                        byte[] message = Encoding.ASCII.GetBytes(messageString);
                        stream.Write(message, 0, message.Count());
                    }
                    catch
                    {
                        IsConnected = false;
                    }
                }
            });
        }
        public void Receive()
        {
            while (IsConnected)
            {
                try
                {
                    byte[] receivedMessage = new byte[256];
                    stream.Read(receivedMessage, 0, receivedMessage.Length);
                receivedMessage = TrimEnd(receivedMessage);
                    UI.DisplayMessage(Encoding.ASCII.GetString(receivedMessage));
                }
                catch
                {
                    Console.WriteLine("Connection was lost!");
                    IsConnected = false;
                }
            }
        }
        public static byte[] TrimEnd(byte[] array)
        {
            int lastIndex = Array.FindLastIndex(array, b => b != 0);

            Array.Resize(ref array, lastIndex + 1);

            return array;
        }
        public void GetUsername()
        {
            Console.WriteLine("Please enter your username");
            username = Console.ReadLine();
        }

        public void Chat()
        {
            Task.Run(() => Receive());
            var t = Task.Run(() => Send());
            t.Wait();
        }
        
    }
}
