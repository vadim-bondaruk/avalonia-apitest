using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using DataEncryptionLibrary.Interfaces;
using DataEncryptionLibrary.Models;
using Microsoft.AspNetCore.Http.Connections.Client;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.SignalR.Protocol;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace GUIClient.Views
{
    public class MainWindow : Window
    {
        private static Button btnConsumerGet;
        private static TextBlock outputText;
        private static TextBlock timeText;
        private static Stopwatch sw = new Stopwatch();
        private static EncryptorKey clientEncryptorKey = new EncryptorKey();
        private static IDataEncryptor _dataEncryptor = new DataEncryptionLibrary.Helpers.DataEncryptor();
        private static HubConnection hubConnection;
        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            //this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            btnConsumerGet = this.FindControl<Button>("btnConsumerGet");
            outputText = this.FindControl<TextBlock>("outputText");
            timeText = this.FindControl<TextBlock>("timeText");
            ConfigureSignalR();
            btnConsumerGet.Click += BtnConsumerGet_Click;
            
        }

        private async void BtnConsumerGet_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {

            btnConsumerGet.IsEnabled = false;

            sw.Start();
            
            await hubConnection.InvokeAsync("ConsumerMerchantsSeedRequestsGet", EncodeData(new { CompanyId = "37d82b6d-8e7a-437b-bd1f-c0d2b2b56e75" }));
        }

        public static string GetToken(string url)
        {
            var requestUri = url + "/api/auth";
            var content = new StringContent("{\"id\":\"mobile_client\", \"secret\":\"secret\"}", Encoding.UTF8, "application/json");
            HttpClient client = new HttpClient();
            var data = JsonConvert.DeserializeObject(client.PostAsync(requestUri, content).Result.Content.ReadAsStringAsync().Result, typeof(TokenResponse));
            return (data as TokenResponse)?.AccessToken;
        }

        public static string EncodeData(object data)
        {
            var stringData = JsonConvert.SerializeObject(data);

            var encryptedData = _dataEncryptor.EncryptDataByAes256Gcm(stringData, clientEncryptorKey);

            return System.Convert.ToBase64String(encryptedData);
        }

        public async void ConfigureSignalR()
        {
            // Client keys pair
            var client_x25519key = _dataEncryptor.CreateX25519EncryptorKey().Key;

            // Client public key bytes array
            var clientPublicKeyBytes = _dataEncryptor.GetX25519EncryptorPublicKeyBytes(client_x25519key);

            //var url = "http://www.private.covrsecurity.io";
            var url = "http://localhost:8959";


            var token = GetToken(url);


            // SignalR hub url
            var stringUri = $"{url}/userNotification";



            var options = new HttpConnectionOptions()
            {
                AccessTokenProvider = () => Task.FromResult(token),
                Url = new Uri(stringUri),
            };

            var factory = new HttpConnectionFactory(Options.Create(options), new LoggerFactory());

            hubConnection = new HubConnection(factory, new JsonHubProtocol(), null);

            var methodsWithoutEncryption = new List<string>()
                {
                    "ConsumerGet",
                    "ConsumerRequestsHistoryGet",
                    "ConsumerRequestsGet",
                    "ConsumerMerchantsGet",
                    "ConsumerRequestsHistoryViewPut",
                    "ConsumerRequestsHistoryViewPut",
                    "ConsumerRequestsIdAcceptGet",
                    "ConsumerRequestsIdAcceptDelete",
                    "ConsumerRequestsIdGet",
                    "ConsumerRequestsHistoryPut"

                };

            var methodsWithEncryption = new List<string>()
                {
                    "ConsumerRequestsHistoryViewGet",
                    "ConsumerPut",
                    "ConsumerMerchantsSeedRequestsGet"
                };

            methodsWithoutEncryption.ForEach(method =>
            {
                hubConnection.On<String>(method, (data) =>
                {
                    outputText.Text = JToken.Parse(data).ToString(Formatting.Indented);
                });
            });
            var window = this;
            methodsWithEncryption.ForEach(method =>
            {
                hubConnection.On<String>(method, async (data) =>
                {
                    var bytes = System.Convert.FromBase64String(data);
                    data = _dataEncryptor.Decrypt(bytes, clientEncryptorKey);
                    sw.Stop();
                    await Dispatcher.UIThread.InvokeAsync(() => {
                        outputText.Text = JToken.Parse(data).ToString(Formatting.Indented);
                        timeText.Text = $"Time: {sw.ElapsedMilliseconds} ms";
                        btnConsumerGet.IsEnabled = true;
                        
                    }, DispatcherPriority.Render);
                });
            });


            hubConnection.On<string>("PublicKeyExchange", keyDTOstring =>
            {
                // Server public key and nonce bytes
                var keyDto = JsonConvert.DeserializeObject<ServerPublicKeyDto>(keyDTOstring);

                ReadOnlySpan<byte> nonce = System.Convert.FromBase64String(keyDto.Nonce);

                // Create nonce from server nonce bytes
                clientEncryptorKey.Nonce = new NSec.Cryptography.Nonce(nonce.Slice(0, 4), nonce.Slice(4));

                clientEncryptorKey.Key = _dataEncryptor.CreateAes256GcmSymmetricKey(System.Convert.FromBase64String(keyDto.PublicKey), client_x25519key);

            });

            await hubConnection.StartAsync();

            await hubConnection.InvokeAsync("PublicKeyExchange", System.Convert.ToBase64String(clientPublicKeyBytes));
        }
    }
}
