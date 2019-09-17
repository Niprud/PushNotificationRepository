using PushNotificationConsole.Service;
using System;

namespace PushNotificationConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            PushNotificationService pushNotificationService = new PushNotificationService();
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

            if(sendNotification == 1)
            {
                var pushSent = pushNotificationService.SendPushNotification(tokens, title, body, data);
                pushNotificationService.Dispose();
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
