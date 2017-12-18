using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class TextLogger : ILog
    {
        private Object LogKey = new object();
        string path = "log.txt";        

        public void LogMessage(string message)
        {
            lock (LogKey)
            {
                using (StreamWriter write = File.AppendText(path))
                {
                    write.WriteLine(message + " " + DateTime.Now.ToLongTimeString());                    
                }
            }
        }
        
        public void JoinChat(string userName)
        {
            lock(LogKey)
            {
                using (StreamWriter write = File.AppendText(path))
                {
                    write.WriteLine(userName + " has joined the chat " + DateTime.Now.ToLongTimeString());                    
                }
            }
        }
        
        public void LeaveChat(string userName)
        {
            lock(LogKey)
            {
                using (StreamWriter write = File.AppendText(path))
                {
                    write.WriteLine(userName + " has left the chat " + DateTime.Now.ToLongTimeString());                    
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

