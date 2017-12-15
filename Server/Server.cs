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
        public Dictionary<int, Client> userInfo = new Dictionary<int, Client>();
        TcpListener listener;
        private Queue<Message> queueMessages;
        private Object QueueLock = new Object();                
        private Object DictionaryLock = new Object();
        private Object LimitClientActionLock = new Object();
        private ILog Logger;
        int UserId = 0;
        int clientListenerIndexCounter = 0;
        string message = null;
        string previousMessage = null;
        private Object AcceptClientLock = new Object();
        private bool needThreads = false;
        private static bool isServerOpen;
        private static bool areUsersConnected = false;
        private static bool isListening = false;
        private static bool hasMessageToSend = false;
        List<int> Connections = new List<int>();
        static List<Client> clientListeners = new List<Client>();
        static List<Thread> threadListeners = new List<Thread>();

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
        public void Broadcast(string sendMessage)
        {
            Client filter;
            foreach (KeyValuePair<int, Client> user in userInfo)
            {
                try
                {
                    filter = user.Value;
                    filter.Send(sendMessage);
                }
                catch
                {
                    Console.WriteLine("Message failed to send");
                }
            }
        }
        private void AcceptClient()
        {
            while (isServerOpen)
            {
                TcpClient clientSocket = default(TcpClient);
                clientSocket = listener.AcceptTcpClient();
                Console.WriteLine("Connection Initiated");
                NetworkStream stream = clientSocket.GetStream();
                client = new Client(stream, clientSocket);
                lock (DictionaryLock) userInfo.Add(UserId, client);
                clientListeners.Add(client);
                areUsersConnected = true;
                UserId += 1;
                if (!needThreads)
                {
                    needThreads = true;
                    Task.Run(() => CreateThreads());
                }
            }
        }
        private void CreateThreads()
        {
            int i = 0;
            while (isServerOpen)
            {
                for (i = 0; threadListeners.Count < clientListeners.Count; i++)
                {
                    Thread listener = new Thread(new ThreadStart(Receive));
                    threadListeners.Add(listener);
                    listener.Start();
                }
                threadListeners.Clear();
            }
        }
        private void Receive()
        {
            if (message == null || message == previousMessage)
                {
                clientListenerIndexCounter += 1;
                    if(clientListenerIndexCounter >= clientListeners.Count)
                    {
                        clientListenerIndexCounter = 0;
                    }
                message = clientListeners[clientListenerIndexCounter].Receive();
            }
            if (message != null)
            {
                previousMessage = message;
                lock (LimitClientActionLock)
                {
                    Broadcast(message);
                }
                message = null;
            }
            threadListeners.Clear();
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
