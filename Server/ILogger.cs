using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class ILogger : ILog
    {
        private Object LogKey = new object();
        string path = "log.txt";        

        public void LogMessage(string message)
        {
            lock (LogKey)
            {
                using (StreamWriter write = File.AppendText(path))
                {
                    write.WriteLine(message);
                    write.WriteLine("{0} {1}", DateTime.Now.ToLongTimeString());
                }
            }
        }
        
        public void JoinChat(string userName)
        {
            lock(LogKey)
            {
                using (StreamWriter write = File.AppendText(path))
                {
                    write.WriteLine(userName + "has joined the chat");
                    write.WriteLine("{0} {1}", DateTime.Now.ToLongTimeString());
                }
            }
        }
        
        public void LeaveChat(string userName)
        {
            lock(LogKey)
            {
                using (StreamWriter write = File.AppendText(path))
                {
                    write.WriteLine(userName + "has left the chat");
                    write.WriteLine("{0} {1}", DateTime.Now.ToLongTimeString());
                }
            }
        }

        void ILog.LogMessage()
        {
            throw new NotImplementedException();
        }

        void ILog.JoinChat()
        {
            throw new NotImplementedException();
        }

        void ILog.LeaveChat()
        {
            throw new NotImplementedException();
        }
    }
}

