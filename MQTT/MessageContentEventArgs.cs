using System;

namespace MQTT
{
    public class MessageContentEventArgs : EventArgs
    {
        public string MessageContent { get; set; }

        public MessageContentEventArgs(string messageContent)
        {
            MessageContent = messageContent;
        }
    }
}
