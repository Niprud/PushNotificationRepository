using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace PushNotificationConsole.Entities.ApnModel
{
    public class AppleNotificationModel
    {
        public class ApnsPayload
        {
            [JsonProperty("alert")]
            public AlertBody AlertBody { get; set; }
            [JsonProperty("badge")]
            public int badge { get; set; }
            [JsonProperty("sound")]
            public string sound { get; set; }
        }

        public class AlertBody
        {
            [JsonProperty("title")]
            public string title { get; set; }
            [JsonProperty("body")]
            public string body { get; set; }
            [JsonProperty("action-loc-key")]
            public string actionLocKey { get; set; }
        }

        [JsonProperty("aps")]
        public ApnsPayload Apns { get; set; }
        [JsonProperty("acme1")]
        public string acme1 { get; set; }
        [JsonProperty("acme2")]
        public string acme2 { get; set; }
    }
}
