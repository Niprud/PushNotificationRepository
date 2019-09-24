using PushNotificationConsole.Helpers;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Configuration;

namespace PushNotificationConsole.Service.APNS
{
    public class ApnService : IDisposable
    {
        private static readonly Dictionary<ApnServerType, string> servers = new Dictionary<ApnServerType, string>
        {
            {ApnServerType.Development, "https://api.sandbox.push.apple.com:443" },   //https://api.development.push.apple.com:443
            {ApnServerType.Production, "https://api.push.apple.com:443" }
        };

        private readonly string apnIdHeader = ConfigurationManager.AppSettings["apnIdHeader"].ToString();   //apns-id(UUID)

        private readonly string p8privateKey;
        private readonly string p8privateKeyId;
        private readonly string teamId;
        private readonly string appBundleIdentifier;
        private readonly ApnServerType server;
        private readonly Lazy<string> jwtToken;
        private readonly Lazy<HttpClient> http;
        private readonly Lazy<WinHttpHandler> handler;

        public ApnService(string p8privateKey, string p8privateKeyId, string teamId, string appBundleIdentifier,ApnServerType server)
        {
            this.p8privateKey = p8privateKey;
            this.p8privateKeyId = p8privateKeyId;
            this.teamId = teamId;
            this.server = server;
            this.appBundleIdentifier = appBundleIdentifier;
            this.jwtToken = new Lazy<string>(() => CreateJwtToken());
            this.handler = new Lazy<WinHttpHandler>(() => new WinHttpHandler());
            this.http = new Lazy<HttpClient>(() => new HttpClient(handler.Value));
        }

        public async Task ApnSender(object notification, string deviceToken, string apnsId = null, int apnsExpiration = 0, int apnsPriority = 10)
        {
            var path = $"/3/device/{deviceToken}";
            var json = JsonHelper.Serialize(notification);

            var request = new HttpRequestMessage(HttpMethod.Post, new Uri(servers[server] + path))
            {
                Version = new Version(2, 0),
                Content = new StringContent(json)
            };

            request.Headers.Authorization = new AuthenticationHeaderValue("bearer", jwtToken.Value);
            request.Headers.TryAddWithoutValidation(":method", "POST");
            request.Headers.TryAddWithoutValidation(":path", path);
            request.Headers.Add("apns-topic", appBundleIdentifier);   //com.example.MyApp
            request.Headers.Add("apns-expiration", apnsExpiration.ToString());
            request.Headers.Add("apns-priority", apnsPriority.ToString());

            if (!string.IsNullOrWhiteSpace(apnsId))
            {
                request.Headers.Add(apnIdHeader, apnsId);
            }

            using (var response = await http.Value.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
            }
        }

        public void Dispose()
        {
            if (http.IsValueCreated)
            {
                handler.Value.Dispose();
                http.Value.Dispose();
            }
        }

        private string CreateJwtToken()
        {
            var header = JsonHelper.Serialize(new { alg = "ES256", kid = p8privateKeyId });
            var payload = JsonHelper.Serialize(new { iss = teamId, iat = ToEpoch(DateTime.UtcNow) });        
            var key = CngKey.Import(Convert.FromBase64String(p8privateKey), CngKeyBlobFormat.Pkcs8PrivateBlob);

            using (var dsa = new ECDsaCng(key))
            {
                dsa.HashAlgorithm = CngAlgorithm.Sha256;
                var headerBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(header));
                var payloadBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(payload));
                var unsignedJwtData = $"{headerBase64}.{payloadBase64}";
                var signature = dsa.SignData(Encoding.UTF8.GetBytes(unsignedJwtData));
                return $"{unsignedJwtData}.{Convert.ToBase64String(signature)}";
            }
        }

        private static int ToEpoch(DateTime time)
        {
            var span = time - new DateTime(1970, 1, 1);
            return Convert.ToInt32(span.TotalSeconds);
        }
    }
}
