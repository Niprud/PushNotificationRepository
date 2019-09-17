using System;
using System.Collections.Generic;
using System.Text;

namespace PushNotificationConsole.Entities
{
    public class Message
    {
        public string[] registration_ids { get; set; }
        public Notification Notification { get; set; }
        public object data { get; set; }
    }
}


{
   "aps" : {
      "alert" : {
         "title" : "Hello World",
         "subtitle" : "This is awesome"
         "body” : “Even more Content",
         "thread_identifier": "Master-Thread"
      },
     "badge": 12,
     "sound": "customSound.caf"
   },
   "custom-data" : {
     "custom-element": "custom-value"
   }
}