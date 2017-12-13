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
                clientSocket.Connect(IPAddress.Parse(IP), port);                
                IsConnected = true;
                stream = clientSocket.GetStream();
            }
            catch
            {
                isConnected = false;
            }
        }
        public Task Send()
        {
            return Task.Run(() =>
            {
                string messageString = UI.GetInput();
                byte[] message = Encoding.ASCII.GetBytes(messageString);
                stream.Write(message, 0, message.Count());
            });
        }
        public void Recieve()
        {
            byte[] recievedMessage = new byte[256];
            stream.Read(recievedMessage, 0, recievedMessage.Length);
            UI.DisplayMessage(Encoding.ASCII.GetString(recievedMessage));
        }

        public string GetUsername()
        {
            Console.WriteLine("Please enter your username");
            return username = Console.ReadLine();
        }
    }
}
