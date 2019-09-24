using Newtonsoft.Json.Linq;
using PushSharp.Common;
using PushSharp.Google;
using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace PushNotificationConsole.Service2
{
    public class FcmSender
    {

        public static bool SendAndroidNotification(string message)
        {
            List<string> MY_REGISTRATION_IDS = new List<string> { };

            bool result = false;

            string senderID = ConfigurationManager.AppSettings["senderID"].ToString();

            string appID = ConfigurationManager.AppSettings["appID"].ToString();

            string packageName = ConfigurationManager.AppSettings["packageName"].ToString();

            var config = new GcmConfiguration(senderID, appID, packageName);
            config.GcmUrl = ConfigurationManager.AppSettings["url"].ToString();

            try
            {
                //create a new broker
                var gcmBroker = new GcmServiceBroker(config);

                gcmBroker.OnNotificationFailed += (notification, aggregateEx) =>
                {
                    aggregateEx.Handle(ex =>
                    {
                        // see what kind of exception it was to further diagnose
                        if (ex is GcmNotificationException)
                        {
                            var notificationException = (GcmNotificationException)ex;
                            var gcmNotification = notificationException.Notification;
                            var description = notificationException.Description;
                            string desc = $"Android Notification Failed: ID = {gcmNotification.MessageId}, Description = {description}";
                        }
                        else if (ex is GcmMulticastResultException)
                        {
                            var multicastException = (GcmMulticastResultException)ex;

                            foreach (var succeededNotification in multicastException.Succeeded)
                            {
                                Console.WriteLine($"Android Notification Succeeded: ID={succeededNotification.MessageId}");
                            }

                            foreach (var failedKvp in multicastException.Failed)
                            {
                                var n = failedKvp.Key;
                                var e = failedKvp.Value;

                                Console.WriteLine($"Android Notification Failed: ID = {n.MessageId}, Description = {e.Message}");
                            }
                        }
                        else if (ex is DeviceSubscriptionExpiredException)
                        {
                            var expiredException = (DeviceSubscriptionExpiredException)ex;

                            var oldID = expiredException.OldSubscriptionId;
                            var newID = expiredException.NewSubscriptionId;

                            Console.WriteLine($"Device RegistrationID expired with ID = {oldID}");

                            if (!string.IsNullOrEmpty(newID))
                            {
                                //Token has been updated, add it to the DB.
                                Console.WriteLine($"New Device RegistrationID = {newID}");
                            }
                        }
                        else if (ex is RetryAfterException)
                        {
                            var retryException = (RetryAfterException)ex;
                            //if you get rate limited, you should stop sending messages until after the Retry
                            Console.WriteLine($"GCM rate limited, don't send more until after {retryException.RetryAfterUtc}");

                        }
                        else
                        {
                            Console.WriteLine("GCM notification failed for some unknown reason");
                        }
                        // mark it as handled
                        return true;
                    });
                };

                gcmBroker.OnNotificationSucceeded += (notification) =>
                {
                    Console.WriteLine("GCM Notification Sent!");
                    result = true;
                };

                // start the broker
                gcmBroker.Start();

                foreach (var regID in MY_REGISTRATION_IDS)
                {
                    //Queue a notification to send
                    gcmBroker.QueueNotification(new GcmNotification
                    {
                        RegistrationIds = new List<string>
                        {
                            regID
                        },

                        Notification = JObject.Parse(
                        "{\"title\" : \"" + "Notifications" + "\"," +
                        "\"body\" : \"" + message + "\"," +
                        "\"icon\" : \"icon\"," +
                        "\"color\" : \"#ffffff\"," +
                        "\"sound\" : \"default\"}"
                        ),
                        Data = JObject.Parse("{\"AppNotificationMessage\" : \"" + message + "\"}")
                    });

                    //stop the broker, wait for it to finish
                    //this isn't done after every message, but after you are done with the broker
                    gcmBroker.Stop();
                }
            }
            catch (Exception)
            {

                throw;
            }

            return result;
        }
    }
}
