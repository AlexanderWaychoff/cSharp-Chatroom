using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            
            Client client = new Client("127.0.0.1", 9999);
            client.SendName();      
            while (client.IsConnected)
<<<<<<< HEAD
            {                
                client.Send();                
                client.Receive();
=======
            {
                client.Chat();
>>>>>>> 6bfab24257d308766c1210db4a95c3f296d7f717
            }

            Console.ReadLine();
        }
    }
}
