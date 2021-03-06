﻿using Newtonsoft.Json;

namespace SlackLibrary.Connections.Sockets.Messages.Outbound
{
    internal class TypingIndicatorMessage : BaseMessage
    {
        public TypingIndicatorMessage()
        {
            Type = "typing";
        }

        [JsonProperty("channel")]
        public string Channel { get; set; } 
    }
}