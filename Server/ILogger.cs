using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class ILogger
    {
        private Object LogKey = new object();
        string path = "log.txt";
        Client logClient;

        public void LogMessage(string message)
        {
            lock (LogKey)
            {
                using (StreamWriter write = File.AppendText(path))
                {
                    write.WriteLine(message);
                }
            }
        }
        
        public void JoinChat()
        {
            lock(LogKey)
            {
                using (StreamWriter write = File.AppendText(path))
                {
                    write.WriteLine(logClient.userName + "has joined the chat");
                }
            }
        }
        
        public void LeaveChat()
        {
            lock(LogKey)
            {
                using (StreamWriter write = File.AppendText(path))
                {
                    write.WriteLine(logClient.userName + "has left the chat");
                }
            }
        }        
        

    }
}

