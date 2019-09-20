namespace PushNotificationConsole.Entities.FcmModel
{
    public class Message
    {
        public string[] registration_ids { get; set; }
        public Notification Notification { get; set; }
        public object data { get; set; }
    }
}
