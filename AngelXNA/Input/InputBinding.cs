using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AngelXNA.Infrastructure.Console;
using AngelXNA.Messaging;

namespace AngelXNA.Input
{
    public class InputBinding
    {
        // The console message that will be executed when the corresponding input is pressed
        public string PressMessage { get; set; }
        
        // The console message that will be executed when the corresponding input is released
        public string ReleaseMessage { get; set; }

        public void OnPress()
        {
            if (PressMessage == null)
            {
                return;
            }
            //DeveloperConsole.Instance.ExecuteInConsole(PressMessage);
            Switchboard.Instance.Broadcast(new Message(PressMessage));
        }

        public void OnRelease()
        {
            if (ReleaseMessage == null)
            {
                return;
            }
            //DeveloperConsole.Instance.ExecuteInConsole(ReleaseMessage);
            Switchboard.Instance.Broadcast(new Message(PressMessage));
        }
    }
}
