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
            });
        }
        public Task Receive()
        {

            return Task.Run(() =>
            {
                try
                {
                    byte[] recievedMessage = new byte[256];
                    stream.Read(recievedMessage, 0, recievedMessage.Length);
                    UI.DisplayMessage(Encoding.ASCII.GetString(recievedMessage));
                }
                catch
                {
                    Console.WriteLine("Connection was lost!");
                    IsConnected = false;
                }
            });
        }

        public void GetUsername()
        {
            Console.WriteLine("Please enter your username");
            username = Console.ReadLine();
        }

        public Task Chat()
        {
            return Task.Run(() =>
            {
                Parallel.Invoke(() =>
                    {
                        Send();
                    },
                    () =>
                    {
                        Receive();
                    }
                );
            });    
        }
        
    }
}
