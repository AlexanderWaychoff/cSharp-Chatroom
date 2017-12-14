using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            new Server().Run();
            while (Server.IsServerOpen)
            {
                string message = Server.client.Receive();
                //Server.Respond(message);
            }
            Console.ReadLine();
        }
    }
}
