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
            TextLogger textLogger = new TextLogger();
            new Server(textLogger).Run();
            Console.ReadLine();
        }
    }
}
