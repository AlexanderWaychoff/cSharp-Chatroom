using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{   
        public interface ILog
        {
            void LogMessage();
            void JoinChat();
            void LeaveChat();
        }
    
}
