using PushSharp.Apple;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text;

namespace PushNotificationConsole.NotificationLog.ApnLog
{
    public static class ApnLog
    {
        static string apnErrorLogPath = ConfigurationManager.AppSettings["apnErrorLogPath"].ToString();
        static string apnSuccessLogPath = ConfigurationManager.AppSettings["apnSuccessLogPath"].ToString();

        //logPath = HttpContext.Current.Server.MapPath("~");

        public static void WriteErrorLog(Exception exception)
        {
            var notificationException = (ApnsNotificationException)exception;
            if (!File.Exists(apnErrorLogPath))
            {
                File.Create(apnErrorLogPath).Dispose();
            }
            try
            {
                using (StreamWriter writer = File.AppendText(apnErrorLogPath))
                {
                    writer.WriteLine("=============Error Logging : Apple Notification Failed ===========");
                    writer.WriteLine("===========Start============= " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt"));
                    writer.WriteLine("Notification ID: " + notificationException.Notification.Identifier);
                    writer.WriteLine("Device Token: " + notificationException.Notification.DeviceToken);
                    writer.WriteLine("Expiration Date: " + notificationException.Notification.Expiration);
                    writer.WriteLine("Is device registration id valid: " + notificationException.Notification.IsDeviceRegistrationIdValid());
                    writer.WriteLine("Code: " + notificationException.ErrorStatusCode);
                    writer.WriteLine("Error Message: " + exception.Message);
                    writer.WriteLine("Error Type: " + exception.GetType().FullName);
                    writer.WriteLine("Inner Exception: " + exception.InnerException.Message);
                    writer.WriteLine("Stack Trace: " + exception.StackTrace);
                    writer.WriteLine("Source: {0}", exception.Source);
                    writer.WriteLine("===========End============= " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt"));
                    writer.WriteLine(Environment.NewLine);
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

        public static void WriteSuccessLog(ApnsNotification notification)
        {
            if (!File.Exists(apnSuccessLogPath))
            {
                File.Create(apnSuccessLogPath).Dispose();
            }
            try
            {
                using (StreamWriter writer = File.AppendText(apnSuccessLogPath))
                {
                    writer.WriteLine("===========Start============= " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt"));
                    writer.WriteLine($"Notification sent to Device with Token ID : {0}, Identifier : {1} at Time : {2} with Payload : {3}",
                        notification.DeviceToken, notification.Identifier, DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt"), notification.Payload);
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

        public static void ReadLog(string path)
        {
            string line;
            using (StreamReader sr = new StreamReader(path))
            {
                while ((line = sr.ReadLine()) != null)
                {
                    Console.WriteLine(line);
                }
            }
        }
    }
}
