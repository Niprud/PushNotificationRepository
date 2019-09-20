using PushNotificationConsole.Entities.FcmModel;
using PushNotificationConsole.Service.APNS;
using PushNotificationConsole.Service.FCM;
using System;

namespace PushNotificationConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            //var decodedString = "--W3sic3RhcnRfdGltZSI6IjAiLCJwcm9kdWN0X2lkIjoiODQwMDMzMDQiLCJ1cmwiOiIifSx7InN0YXJ0X3RpbWUiOiI3OSIsInByb2R1Y3RfaWQiOiI4NDAzNjk2MSIsInVybCI6IiJ9LHsic3RhcnRfdGltZSI6IjgyIiwicHJvZHVjdF9pZCI6Ijg0MDAzMDIwIiwidXJsIjoiIn0seyJzdGFydF90aW1lIjoiMTA5IiwicHJvZHVjdF9pZCI6IiIsInVybCI6Imh0dHBzOi8vYmxvZy5sYXJlaW5lZHVzaG9wcGluZy5jYS8yMDE3LzAxL3RyYW5zZm9ybWVyLXNlcy12aWV1eC1nYW50cy1kZS1jdWlyLWVuLTUtbWludXRlcy8ifV0="
            //    .Replace("-", "");
            //var bytes = Convert.FromBase64String(decodedString);
            //var encodedString = Encoding.UTF8.GetString(bytes);
            //Console.WriteLine(encodedString);

            PushNotificationService pushNotificationService = new PushNotificationService();
            //ApnService apnService = new ApnService(decodedString, "asffpaerero343", "asffpaerero343", "asffpaerero343", ApnServerType.Development);

            string title = "";
            string body = "";
            var data = new { action = "Play", userID = 5 };

            Console.WriteLine("Hello Everyone!");
            Console.WriteLine("Let's send push notifications!!!");

            Console.WriteLine("Title of Notification: ");
            title = Console.ReadLine();

            Console.WriteLine();

            Console.WriteLine("Description of the notification: ");
            body = Console.ReadLine();

            Console.WriteLine();

            Console.Write("Number of devices that receive this notification? ");
            int.TryParse(Console.ReadLine(), out int deviceCount);
            var tokens = new string[deviceCount];

            Console.WriteLine();

            for (int i = 0; i < deviceCount; i++)
            {
                Console.WriteLine($"Token for device number {i + 1}: ");
                tokens[i] = Console.ReadLine();
                Console.WriteLine();
            }

            Console.WriteLine("Do you want to send notifications?");
            Console.WriteLine("1 - Yes!!!");
            Console.WriteLine("0 - No!!!");
            int.TryParse(Console.ReadLine(), out int sendNotification);

            if (sendNotification == 1)
            {
                var fcmPush = pushNotificationService.SendPushNotification(tokens, title, body, data);
                //var apnPush = apnService.ApnSender("sfdghjg", "3456fdgdsdawere");
                pushNotificationService.Dispose();
                //apnService.Dispose();
                Console.WriteLine($"Notification sent!");
            }
            else
            {
                Console.WriteLine("Failed to send notification!");
            }
            Console.ReadKey();
        }
    }
}
