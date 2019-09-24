using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using PushSharp.Apple;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PushNotificationConsole.Service2
{
    public class ApnSender
    {
        public void SendAppleNotification(string message)
        {
            List<string> MY_DEVICE_TOKENS = new List<string> { };

            try
            {
                string path = "~/Files/Certificate/IOS/Production_Certificate.p12";
                //Get Certificate
                var appleCert = File.ReadAllBytes(path);

                var config = new ApnsConfiguration(ApnsConfiguration.ApnsServerEnvironment.Production, appleCert, "password");

                // Create a new broker
                var apnsBroker = new ApnsServiceBroker(config);

                // Wire up events
                apnsBroker.OnNotificationFailed += (notification, aggregateEx) =>
                {

                    aggregateEx.Handle(ex =>
                    {

                        // See what kind of exception it was to further diagnose
                        if (ex is ApnsNotificationException)
                        {
                            var notificationException = (ApnsNotificationException)ex;

                            var apnsNotification = notificationException.Notification;
                            var statusCode = notificationException.ErrorStatusCode;
                            string desc = $"Apple Notification Failed: ID={apnsNotification.Identifier}, Code={statusCode}";
                            Console.WriteLine(desc);

                        }
                        else
                        {
                            string desc = $"Apple Notification Failed for some unknown reason : {ex.InnerException}";
                            // Inner exception might hold more useful information like an ApnsConnectionException			
                            Console.WriteLine(desc);

                        }
                        // Mark it as handled
                        return true;
                    });
                };

                apnsBroker.OnNotificationSucceeded += (notification) =>
                {
                    Console.WriteLine("Apple Notification Sent successfully!");
                };
               
                // Start Proccess 
                apnsBroker.Start();

                foreach(var deviceToken in MY_DEVICE_TOKENS)
                {
                    //Queue a notification to send
                    apnsBroker.QueueNotification(new ApnsNotification
                    {
                        DeviceToken = deviceToken,
                        Payload = JObject.Parse(("{\"aps\":{\"badge\":1,\"sound\":\"oven.caf\",\"alert\":\"" + (message + "\"}}")))
                    });
                }

                apnsBroker.Stop();

            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
