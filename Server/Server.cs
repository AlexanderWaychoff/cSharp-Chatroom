using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    class Server : ILogger
    {
        public static Client client;
        TcpListener listener;
        private Queue<Message> queueMessages;
        private Object QueueLock = new Object();                
        private Object DictionaryLock = new Object();
        private ILog Logger;
        int UserId = 1;
        private Object AcceptClientLock = new Object();
        private static bool isServerOpen;
        private static bool areUsersConnected = false;
        List<int> Connections = new List<int>();

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
                        Logger.JoinChat();
                        AddToQueue(client.userName, client);
                    }
                    catch
                    {

                    }
                    while (areUsersConnected)
                    {
                        string message = client.Receive();
                        Respond(message);
                        Broadcast(message);
                    }
                    //while((i = Stream.Read(bytes, 0, bytes.Length)) != 0)
                }
            });
        }
        private void Broadcast(string message)
        {
            lock (DictionaryLock)
            {
                foreach (var user in client.userInfo)
                {
                    user.Value.Send(message);
                    Console.WriteLine(user.ToString());
                }
            }
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
                    lock (DictionaryLock) client.userInfo.Add(UserId, client);
                    areUsersConnected = true;
                    //client.subscribers.Add(client);
                    UserId += 1;
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

       // public void SendToAll(socket, string message)
        //{
           // Connections.Add(socket)
           // for(int i = 0; i < Connections.Count; i++);
           // {                
               // Socket tempSocket = 
                //client.Send(clientMessage);
           // }
        //}
}
}
