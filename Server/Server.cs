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
    class Server : TextLogger
    {
        Client client;
        public Dictionary<int, Client> allSubscribers = new Dictionary<int, Client>();
        TcpListener listener;
        private Queue<string> queueMessages;
        private Object QueueLock = new Object();                
        private Object DictionaryLock = new Object();
        private Object LimitClientActionLock = new Object();
        private Object BroadcastLock = new Object();
        private Object ReceiveLock = new Object();
        private Object AcceptClientLock = new Object();
        ILog textLogger;
        int UserId = 0;
        int clientListenerIndexCounter = 0;
        private int saveClientIndexCounter = 0;
        private string disconnected = " disconnected.";
        private string saveUserName;
        string message = null;
        string previousMessage = null;
        private bool needThreads = false;
        private static bool isServerOpen;
        private bool areUsersConnected = false;
        List<Client> clientListeners = new List<Client>();
        List<Thread> threadReceiveListeners = new List<Thread>();
        List<Thread> threadConnectionListeners = new List<Thread>();




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
        public Server(ILog logger)

        {
            textLogger = logger;
            queueMessages = new Queue<string>();
            int port = 9999;            
            listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
        
        }
        public void Run()
        {
            IsServerOpen = true;
            Task.Run(() => AcceptClient());
        }
        public void Broadcast(string sendMessage)
        {
            lock (BroadcastLock)
            {
                for (int i = 0; i < clientListeners.Count; i++)
                {
                    try
                    {
                        RemoveFromQueue();                        
                        clientListeners[i].Send(sendMessage);
                        LogMessage(sendMessage);
                        
                    }
                    catch
                    {
                        Console.WriteLine("Message failed to send to " + clientListeners[i].userName);
                        clientListeners[i].Stream.Close();
                        allSubscribers.Remove(i);
                        clientListeners.RemoveAt(i);
                        i--;
                    }
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
                lock (DictionaryLock) allSubscribers.Add(UserId, client);
                clientListeners.Add(client);
                Task.Run(() => InformSubscribersOfNewUser(clientListeners[clientListeners.Count - 1].userName.ToString()));
                areUsersConnected = true;
                UserId += 1;
                if (!needThreads)
                {
                    needThreads = true;
                    Task.Run(() => CreateThreadsForReceive());
                }
            }
        }
        private void CreateThreadsForReceive()
        {
            int i = 0;
            while (isServerOpen)
            {
                try
                {
                    if (threadReceiveListeners.Count < clientListeners.Count)
                    {
                        for (i = threadReceiveListeners.Count; i < clientListeners.Count; i++)
                        {
                            Thread listener = new Thread(new ThreadStart(Receive));
                            threadReceiveListeners.Add(listener);
                            listener.Start();
                        }
                    }
                }
                catch
                {
                    
                }
            }
        }
        private void Receive()
        {
            try
            {
                if (message == null || message == previousMessage)
                {
                    clientListenerIndexCounter += 1;
                    if (clientListenerIndexCounter >= clientListeners.Count)
                    {
                        clientListenerIndexCounter = 0;
                    }
                    if (message != disconnected)
                    {
                        message = clientListeners[clientListenerIndexCounter].Receive();
                        AddToQueue(message);
                    }
                }
                lock (ReceiveLock)
                {
                    if (message == disconnected)
                    {
                        saveClientIndexCounter = clientListenerIndexCounter;
                        saveUserName = clientListeners[saveClientIndexCounter].userName;
                        Broadcast(saveUserName + disconnected);
                        message = null;
                        AddToQueue(message);
                        LeaveChat(saveUserName);
                    }
                }
                if (message != null)
                {
                    previousMessage = message;
                    lock (LimitClientActionLock)
                    {
                        AddToQueue(message);
                        Broadcast(message);                        
                    }
                    message = null;
                }
                threadReceiveListeners.RemoveAt(0);
            }
            catch
            {

            }
        }
        public void Respond(string body)
        {
             client.Send(body);            
        }
        public void InformSubscribersOfNewUser(string newUser)
        {
            for (int i = 0; i < clientListeners.Count; i++)
            {
                try
                {
                    clientListeners[i].Send(newUser + " has joined the chatroom.");
                    AddToQueue(newUser);
                    JoinChat(newUser);
                }
                catch
                {

                }
            }
        }
        private void AddToQueue(string message)
        {
            lock(QueueLock)
            {                
                queueMessages.Enqueue(message);
            }                      
        }

        private void RemoveFromQueue()
        {
            queueMessages.Dequeue();            
        }
    }
}
