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
        private bool isServerOpen;
        private Object QueueLock = new Object();
        int UserId;
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
                        string message = client.Recieve();                        
                        Respond(message);                        
                    }
                    catch
                    {

                    }
                }
            });
        }
        private void AcceptClient()
        {
            TcpClient clientSocket = default(TcpClient);
            clientSocket = listener.AcceptTcpClient();
            Console.WriteLine("Connection Initiated");
            NetworkStream stream = clientSocket.GetStream();
            client = new Client(stream, clientSocket);
        }
        private void Respond(string body)
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
