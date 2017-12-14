using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    class Server
    {
        public static Client client;
        TcpListener listener;

        private Queue<Message> queueMessages; 

        private Object QueueLock = new Object();
        int UserId = 1;
        string userName;
     


        private Object AcceptClientLock = new Object();
        private static bool isServerOpen;
        public static bool IsServerOpen
        {
            get
            {
                return isServerOpen;
            }
            set
            {
                isServerOpen = value;
            }
        }
        public Server()

        {
            queueMessages = new Queue<Message>();
            int port = 9999;            
            listener = new TcpListener(IPAddress.Any, port); //Parse("127.0.0.1")
            listener.Start();
        }
        public Task Run()
        {
            return Task.Run(() =>
            {
                isServerOpen = true;
                while (isServerOpen)
                {
                    try
                    {
                        AcceptClient();
                        string message = client.Receive();
                        Respond(message);
                    }
                    catch
                    {

                    }
                }
            });
        }
        private Task AcceptClient()
        {
            return Task.Run(() =>
            {
                lock (AcceptClientLock)
                {
                    TcpClient clientSocket = default(TcpClient);
                    clientSocket = listener.AcceptTcpClient();
                    Console.WriteLine("Connection Initiated");
                    NetworkStream stream = clientSocket.GetStream();
                    client = new Client(stream, clientSocket);
                    userName = client.Receive();
                    client.userInfo.Add(UserId, userName);
                    UserId += 1;
                    Respond(userName + " has joined.");
                }
            });
        }
        public void Respond(string body)
        {
             client.Send(body);            
        }

        private void AddToQueue(string message, Client client)
        {
            lock(QueueLock)
            {
                Message clientMessage = new Message(client, message);
                queueMessages.Enqueue(clientMessage);
            }                      
        }

        private Message RemoveFromQueue()
        {
            return queueMessages.Dequeue();
        }
    }
}
