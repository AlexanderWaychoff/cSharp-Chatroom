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
        TcpListener listener;
        private Queue<Message> queueMessages;
        private Object QueueLock = new Object();                
        private Object DictionaryLock = new Object();
        private Object LimitClientActionLock = new Object();
        private ILog Logger;
        int UserId = 1;
        string message;
        private Object AcceptClientLock = new Object();
        private static bool isServerOpen;
        private static bool areUsersConnected = false;
        private static bool isListening = false;
        private static bool hasMessageToSend = false;
        List<int> Connections = new List<int>();
        static List<Thread> clientListeners = new List<Thread>();

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
            //{
                //Thread myThread;
                //myThread = new Thread(new ThreadStart(WaitToBroadcast));
                //myThread.Start();
                //clientListeners.Add(myThread);
                //isServerOpen = true;
                //if (!isListening)
                //{
                //    isListening = true;
                //    while (isServerOpen)
                //    {
                //        ListenForClients();
                //    }
                //}
                //while (isServerOpen)
                //{
                //    try
                //    {
                //        await WaitForMessage();
                //        //string message = client.Receive();
                //        //Respond(message);
                //        Broadcast(message);

                //        //await Task.Run(() => AcceptClient());
                //        //Logger.JoinChat();
                //        //AddToQueue(client.userName, client);
                //    }
                //    catch
                //    {

                //    }
                //    while (areUsersConnected)
                //    {
                //        await WaitForMessage();
                //        //string message = client.Receive();
                //        //Respond(message);
                //        Broadcast(message);
                //    }
                //    //while((i = Stream.Read(bytes, 0, bytes.Length)) != 0)
                //}
            //});
        }
        public void ListenForClients()
        {
            while (IsServerOpen)
            {
                try
                {
                    AcceptClient();
                    //Thread myThread;
                    //myThread = new Thread(new ThreadStart(HoldClientListeners));
                    //myThread.Start();
                    //clientListeners.Add(myThread);
                    message = clientCommands.Receive();
                    //Task.Run(() => clientCommands.Receive());
                    Task.Run(() => Broadcast(message));
                }
                catch
                {

                }
            }
        }
        //public async void HoldClientListeners()
        //{
        //    while(isServerOpen)
        //    {
        //        try
        //        {
        //            await WaitForMessage();
        //            lock (LimitClientActionLock)
        //            {
        //                hasMessageToSend = true;
        //                while(hasMessageToSend)
        //                {

        //                }
        //            }
        //            //string message = client.Receive();
        //            //Respond(message);
        //            //Broadcast(message);

        //            //await Task.Run(() => AcceptClient());
        //            //Logger.JoinChat();
        //            //AddToQueue(client.userName, client);
        //        }
        //        catch
        //        {

        //        }
        //    }
        //}
        //public Task WaitForMessage()
        //{
        //    return Task.Run(() =>
        //    {
        //        while (!hasMessageToSend)
        //        {
        //            lock (DictionaryLock)
        //            {
        //                foreach (KeyValuePair<int, Client> client in clientCommands.userInfo)
        //                {
        //                    message = client.Value.Receive();
        //                    if (message != null)
        //                    {
        //                        lock (LimitClientActionLock)
        //                        {
        //                            hasMessageToSend = true;
        //                            //Broadcast(client.Value.userName.ToString() + ": " + message);
        //                        }
        //                    }
        //                }
        //            }
        //        }

        //    });
        //}
        //private void WaitToBroadcast()
        //{
        //    while(isServerOpen)
        //    {
        //        if (hasMessageToSend)
        //        {
        //            Broadcast(message);
        //            hasMessageToSend = false;
        //        }
        //    }
        //}
        public void Broadcast(string message)
        {
            lock (DictionaryLock)
            {
                //Client filter;
                foreach (KeyValuePair<int, Client> user in clientCommands.userInfo)
                {
                    user.Value.Send(message);
                    
                }
                //hasMessageToSend = false;
            }
        }
        private void AcceptClient()
        {
            //return Task.Run(() =>
            //{
            while (isServerOpen)
            {
                lock (AcceptClientLock)
                {
                    TcpClient clientSocket = default(TcpClient);
                    clientSocket = listener.AcceptTcpClient();
                    Console.WriteLine("Connection Initiated");
                    NetworkStream stream = clientSocket.GetStream();
                    Client client = new Client(stream, clientSocket);
                    lock (DictionaryLock) clientCommands.userInfo.Add(UserId, client);
                    areUsersConnected = true;
                    //client.subscribers.Add(client);
                    UserId += 1;
                    Task.Run(() => Receive());
                    //Task.Run(() => Broadcast(message));

                }
            }
            //});
        }
        private void Receive()
        {
            while (isServerOpen)
            {
                message = clientCommands.Receive();
                Task.Run(() => Broadcast(message));
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
