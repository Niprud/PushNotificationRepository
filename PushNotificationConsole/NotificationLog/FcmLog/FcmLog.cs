using PushSharp.Common;
using PushSharp.Google;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text;

namespace PushNotificationConsole.NotificationLog.FcmLog
{
    public static class FcmLog
    {
        static string fcmErrorLogPath = ConfigurationManager.AppSettings["fcmErrorLogPath"].ToString();
        static string fcmSuccessLogPath = ConfigurationManager.AppSettings["fcmSuccessLogPath"].ToString();

        public static void WriteErrorLog(Exception ex)
        {
            if (!File.Exists(fcmErrorLogPath))
            {
                File.Create(fcmErrorLogPath).Dispose();
            }
            try
            {
                using (StreamWriter writer = File.AppendText(fcmErrorLogPath))
                {
                    writer.WriteLine("=============Error Logging :Fcm Notification Failed ===========");
                    writer.WriteLine("===========Start============= " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt"));
                    if (ex is GcmNotificationException)
                    {
                        var notificationException = (GcmNotificationException)ex;

                        writer.WriteLine("Message ID: " + notificationException.Notification.MessageId);
                        writer.WriteLine("Device Token: " + notificationException.Notification.RegistrationIds);
                        writer.WriteLine("Expiration Date: " + notificationException.Notification.TimeToLive);
                        writer.WriteLine("Is device registration id valid: " + notificationException.Notification.IsDeviceRegistrationIdValid());
                        writer.WriteLine("Code: " + notificationException.Description);
                        writer.WriteLine("Error Message: " + ex.Message);
                        writer.WriteLine("Error Type: " + ex.GetType().FullName);
                        writer.WriteLine("Inner Exception: " + ex.InnerException.Message);
                        writer.WriteLine("Stack Trace: " + ex.StackTrace);
                        writer.WriteLine("Source: {0}", ex.Source);
                    }
                    else if (ex is GcmMulticastResultException)
                    {
                        var multicastException = (GcmMulticastResultException)ex;

                        foreach (var failedKvp in multicastException.Failed)
                        {
                            writer.WriteLine("===========Start============= " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt"));
                            var n = failedKvp.Key;
                            var e = failedKvp.Value;
                            Console.WriteLine($"Android Notification Failed: ID = {n.MessageId}, Description = {e.Message}");
                            writer.WriteLine("Error Message: " + ex.Message);
                            writer.WriteLine("Error Type: " + ex.GetType().FullName);
                            writer.WriteLine("Inner Exception: " + ex.InnerException.Message);
                            writer.WriteLine("Stack Trace: " + ex.StackTrace);
                            writer.WriteLine("Source: {0}", ex.Source);
                            writer.WriteLine("===========End============= " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt"));
                            writer.WriteLine(Environment.NewLine);
                        }
                    }
                    else if (ex is DeviceSubscriptionExpiredException)
                    {
                        var notificationException = (DeviceSubscriptionExpiredException)ex;

                        var oldID = notificationException.OldSubscriptionId;
                        var newID = notificationException.NewSubscriptionId;

                        writer.WriteLine("Device RegistrationID expired with ID:  " + oldID);
                        if (!string.IsNullOrEmpty(newID))
                        {
                            writer.WriteLine("New Device RegistrationID: " + newID);
                        }
                        writer.WriteLine("Expiration Date: " + notificationException.ExpiredAt);
                        writer.WriteLine("Is device registration id valid: " + notificationException.Notification.IsDeviceRegistrationIdValid());
                        writer.WriteLine("Error Message: " + ex.Message);
                        writer.WriteLine("Error Type: " + ex.GetType().FullName);
                        writer.WriteLine("Inner Exception: " + ex.InnerException.Message);
                        writer.WriteLine("Stack Trace: " + ex.StackTrace);
                        writer.WriteLine("Source: {0}", ex.Source);
                    }
                    else if (ex is RetryAfterException)
                    {
                        var notificationException = (RetryAfterException)ex;
                        writer.WriteLine("GCM rate limited, don't send more until after: " + notificationException.RetryAfterUtc);
                        writer.WriteLine("Is device registration id valid: " + notificationException.Notification.IsDeviceRegistrationIdValid());
                        writer.WriteLine("Error Message: " + ex.Message);
                        writer.WriteLine("Error Type: " + ex.GetType().FullName);
                        writer.WriteLine("Inner Exception: " + ex.InnerException.Message);
                        writer.WriteLine("Stack Trace: " + ex.StackTrace);
                        writer.WriteLine("Source: {0}", ex.Source);
                    }
                    else
                    {
                        writer.WriteLine("GCM notification failed for some unknown reason");
                        writer.WriteLine("Error Message: " + ex.Message);
                        writer.WriteLine("Error Type: " + ex.GetType().FullName);
                        writer.WriteLine("Inner Exception: " + ex.InnerException.Message);
                        writer.WriteLine("Stack Trace: " + ex.StackTrace);
                        writer.WriteLine("Source: {0}", ex.Source);
                    }

                    writer.WriteLine("===========End============= " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt"));
                    writer.WriteLine(Environment.NewLine);
                    writer.WriteLine(Environment.NewLine);

                }
            }
            catch (Exception e)
            {
                //Do not do anything 
            }
        }

        public static void WriteSuccessLog(GcmNotification notification)
        {
            if (!File.Exists(fcmSuccessLogPath))
            {
                File.Create(fcmSuccessLogPath).Dispose();
            }
            try
            {
                using (StreamWriter writer = File.AppendText(fcmSuccessLogPath))
                {
                    writer.WriteLine("===========Start============= " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt"));
                    writer.WriteLine($"Notification sent to Devices with Registration IDs : {0} at Time : {2} with Data : {3}",
                        notification.RegistrationIds, DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt"), notification.Data);
                    writer.WriteLine("===========End============= " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt"));
                    writer.WriteLine(Environment.NewLine);

                    writer.Flush();
                    writer.Close();
                }
            }
            catch (Exception e)
            {
                //Do not do anything 
            }
        }

        public static void WriteSuccessLog(GcmMulticastResultException multicastException)
        {
            if (!File.Exists(fcmSuccessLogPath))
            {
                File.Create(fcmSuccessLogPath).Dispose();
            }
            try
            {
                using (StreamWriter writer = File.AppendText(fcmSuccessLogPath))
                {
                    writer.WriteLine("===========Start============= " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt"));

                    foreach (var succeededNotification in multicastException.Succeeded)
                    {
                        writer.WriteLine($"Notification sent to Devices with Message ID : {0} at Time : {2} with Data : {3}",
                        succeededNotification.MessageId, DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt"), succeededNotification.Data);
                    }
                    writer.WriteLine("===========End============= " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt"));
                    writer.WriteLine(Environment.NewLine);

                    writer.Flush();
                    writer.Close();
                }
            }
            catch (Exception e)
            {
                //Do not do anything 
            }
        }
    }
}
