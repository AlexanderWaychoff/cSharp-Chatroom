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
        Client clientCommands = new Client();
        Client client;
        TcpListener listener;
        private Queue<Message> queueMessages;
        private Object QueueLock = new Object();                
        private Object DictionaryLock = new Object();
        private Object LimitClientActionLock = new Object();
        private ILog Logger;
        int UserId = 0;
        string message;
        private Object AcceptClientLock = new Object();
        private static bool isServerOpen;
        private static bool areUsersConnected = false;
        private static bool isListening = false;
        private static bool hasMessageToSend = false;
        List<int> Connections = new List<int>();
        static List<Client> clientListeners = new List<Client>();

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
        public void Run()
        {
            IsServerOpen = true;
            Task.Run(() => AcceptClient());
        }
        public void Broadcast(string message)
        {
            //while (isServerOpen)
            //{
                //if (hasMessageToSend)
                //{
                //lock (DictionaryLock)
                //{
                Client filter;

                foreach (KeyValuePair<int, Client> user in clientCommands.userInfo)
                        {
                            try
                            {
                    filter = user.Value;
                        filter.Send(message);
                            }
                            catch
                            {
                                Console.WriteLine("Message failed to send");
                            }

                        }
                       // hasMessageToSend = false;
                    //}
                //}
            //}
        }
        private void AcceptClient()
        {
            //return Task.Run(() =>
            //{
            while (isServerOpen)
            {
                TcpClient clientSocket = default(TcpClient);
                clientSocket = listener.AcceptTcpClient();
                Console.WriteLine("Connection Initiated");
                NetworkStream stream = clientSocket.GetStream();
                client = new Client(stream, clientSocket);
                lock (DictionaryLock) clientCommands.userInfo.Add(UserId, client);
                clientListeners.Add(client);
                areUsersConnected = true;
                //client.subscribers.Add(client);
                UserId += 1;
                Task.Run(() => Receive());
                //Task.Run(() => Broadcast(message));
            }
            //});
        }
        private void Receive()
        {
            while (isServerOpen)
            {
                //for (int i = 0; i < clientListeners.Count; i++)
                //{
                    message = client.Receive();
                    //if (message != null)
                    //{
                        Task.Run(() => Broadcast(message));
                //    }
                //}
                //Task.Run(() => Broadcast(message));
                //if (!hasMessageToSend)
                //{
                //    lock (DictionaryLock)
                //    {
                //        foreach (KeyValuePair<int, Client> user in clientCommands.userInfo)
                //        {
                //            message = user.Value.Receive();
                //            if(message != null && !hasMessageToSend)
                //            {
                //                hasMessageToSend = true;
                //                Broadcast(message);    //Task.Run(() => 
                //            }
                //        }
                //    }
                //}
                //Task.Run(() => Broadcast(message));
            }
        }
        public void Respond(string body)
        {
             clientCommands.Send(body);            
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
