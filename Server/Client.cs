using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Client
    {
        NetworkStream stream;
        TcpClient client;
        //Server server = new Server();
        public string UserId;
        public string userName;
        Object ReceiveLock = new Object();
        public Dictionary<int, Client> userInfo = new Dictionary<int, Client>();
        //public List<IObserver<Client>> subscribers = new List<IObserver<Client>>();

        public Client()
        {
            //this.server = server;
        }
        public Client(NetworkStream Stream, TcpClient Client)
        {
            stream = Stream;
            client = Client;
            this.userName = Receive();
            UserId = "495933b6-1762-47a1-b655-483510072e73";
        }
        public void Send(string Message)
        { 
            byte[] message = Encoding.ASCII.GetBytes(Message);
            stream.Write(message, 0, message.Count());
        }
        public string Receive() //string
        {
            lock (ReceiveLock)
            {
                try
                {

                    byte[] recievedMessage = new byte[256];
                    stream.Read(recievedMessage, 0, recievedMessage.Length);
                    string recievedMessageString = Encoding.ASCII.GetString(recievedMessage);
                    Console.WriteLine(recievedMessageString);

                    //Task.Run(() => server.Broadcast(recievedMessageString));
                    return recievedMessageString;
                }
                catch
                {
                    return null;
                }
            }
                    //catch
                    //{
                    //    return "";
                    //}
//}
        }        
    }
}
