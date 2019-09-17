using Newtonsoft.Json;
using PushNotificationConsole.Entities;
using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PushNotificationConsole.Service
{
    public class PushNotificationService : IDisposable
    {
        private readonly Uri FireBasePushNotificationURL = new Uri("https://fcm.googleapis.com/fcm/send");
        private readonly string SenderID = "1057831725262";
        private readonly string ServerKey = "AAAA9kuvKM4:APA91bG71WENNJDN0g9zcGrPuNfxoqAFCfJpNMPzOoWKSESSaDGkROdF-vuowKcizEJ3f1hmXY0WSeOw4YaXsrB2LaLztuZelB3RT-L0Fiz6s_Y04aImMld-SimAsM6dEH_iLdfIwSkQ";
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
