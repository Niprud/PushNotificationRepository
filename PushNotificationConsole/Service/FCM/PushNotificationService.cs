using Newtonsoft.Json;
using PushNotificationConsole.Entities.FcmModel;
using System;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PushNotificationConsole.Service.FCM
{
    public class PushNotificationService : IDisposable
    {
        private readonly string FireBasePushNotificationURL = ConfigurationManager.AppSettings["url"].ToString(); //new Uri("https://fcm.googleapis.com/fcm/send");
        private readonly string SenderID = ConfigurationManager.AppSettings["SenderID"].ToString();
        private readonly string ServerKey = ConfigurationManager.AppSettings["ServerKey"].ToString();
        private readonly Lazy<HttpClient> lazyHttp = new Lazy<HttpClient>();
        public async Task<bool> SendPushNotification(string[] deviceTokens, string title, string body, object data)
        {
            bool sent = true;

            if (deviceTokens.Count() > 0)
            {
                var messageInformation = new Message()
                {
                    Notification = new Notification()
                    {
                        title = title,
                        text = body
                    },
                    data = data,
                    registration_ids = deviceTokens
                };

                string jsonMessage = JsonConvert.SerializeObject(messageInformation);

                var request = new HttpRequestMessage(HttpMethod.Post, FireBasePushNotificationURL);

                request.Headers.TryAddWithoutValidation("Authorization", "key=" + ServerKey);
                request.Headers.TryAddWithoutValidation("Sender", "id=" + SenderID);
                request.Content = new StringContent(jsonMessage, Encoding.UTF8, "application/json");

                //HttpResponseMessage result;

                //alternative
                using (var response = await lazyHttp.Value.SendAsync(request))
                {
                    response.EnsureSuccessStatusCode();
                }

                //using (var client = new HttpClient())
                //{
                //    result = await client.SendAsync(request);
                //    sent = sent && result.IsSuccessStatusCode;
                //}
            }
            return sent;
        }

        public void Dispose()
        {
            if (lazyHttp.IsValueCreated)
            {
                lazyHttp.Value.Dispose();
            }
        }
    }
}
