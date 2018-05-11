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
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace GUIClient.Views
{
    public class MainWindow : Window
    {
        private static Button btnConsumerMerchantsSeedRequestsGet;
        private static Button btnConsumerPut;
        private static Button btnConsumerRequestsHistoryViewGet;
        private static Button btnConsumerGet;
        private static Button btnConsumerRequestsHistoryGet;
        private static Button btnConsumerRequestsGet;
        private static Button btnConsumerMerchantsGet;
        private static Button btnConsumerRequestsHistoryViewPut;
        private static Button btnConsumerRequestsIdAcceptGet;
        private static Button btnConsumerRequestsIdAcceptDelete;
        private static Button btnConsumerRequestsIdGet;
        private static Button btnConsumerRequestsHistoryPut;

        private static TextBlock outputText;
        private static TextBlock timeText;

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

            btnConsumerMerchantsSeedRequestsGet = this.FindControl<Button>("btnConsumerMerchantsSeedRequestsGet");
            btnConsumerPut = this.FindControl<Button>("btnConsumerPut");
            btnConsumerRequestsHistoryViewGet = this.FindControl<Button>("btnConsumerRequestsHistoryViewGet");
            btnConsumerGet = this.FindControl<Button>("btnConsumerGet");
            btnConsumerRequestsHistoryGet = this.FindControl<Button>("btnConsumerRequestsHistoryGet");
            btnConsumerRequestsGet = this.FindControl<Button>("btnConsumerRequestsGet");
            btnConsumerMerchantsGet = this.FindControl<Button>("btnConsumerMerchantsGet");
            btnConsumerRequestsHistoryViewPut = this.FindControl<Button>("btnConsumerRequestsHistoryViewPut");
            btnConsumerRequestsIdAcceptGet = this.FindControl<Button>("btnConsumerRequestsIdAcceptGet");
            btnConsumerRequestsIdAcceptDelete = this.FindControl<Button>("btnConsumerRequestsIdAcceptDelete");
            btnConsumerRequestsIdGet = this.FindControl<Button>("btnConsumerRequestsIdGet");
            btnConsumerRequestsHistoryPut = this.FindControl<Button>("btnConsumerRequestsHistoryPut");

            outputText = this.FindControl<TextBlock>("outputText");
            timeText = this.FindControl<TextBlock>("timeText");

            ConfigureSignalR();

            btnConsumerMerchantsSeedRequestsGet.Click += BtnConsumerMerchantsSeedRequestsGet_Click;
            btnConsumerPut.Click += BtnConsumerPut_Click;
            btnConsumerRequestsHistoryViewGet.Click += BtnConsumerRequestsHistoryViewGet_Click;
            btnConsumerGet.Click += BtnConsumerGet_Click;
            btnConsumerRequestsHistoryGet.Click += BtnConsumerRequestsHistoryGet_Click;
            btnConsumerRequestsGet.Click += BtnConsumerRequestsGet_Click;
            btnConsumerMerchantsGet.Click += BtnConsumerMerchantsGet_Click;
            btnConsumerRequestsHistoryViewPut.Click += BtnConsumerRequestsHistoryViewPut_Click;
            btnConsumerRequestsIdAcceptGet.Click += BtnConsumerRequestsIdAcceptGet_Click;
            btnConsumerRequestsIdAcceptDelete.Click += BtnConsumerRequestsIdAcceptDelete_Click;
            btnConsumerRequestsIdGet.Click += BtnConsumerRequestsIdGet_Click;
            btnConsumerRequestsHistoryPut.Click += BtnConsumerRequestsHistoryPut_Click;
        }

        private async void BtnConsumerRequestsHistoryPut_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                outputText.Text = string.Empty;
                timeText.Text = string.Empty;
                btnConsumerRequestsHistoryPut.IsEnabled = false;

            });

            Stopwatch sw = new Stopwatch();

            hubConnection.On<String>("ConsumerRequestsHistoryPut", async (data) =>
            {

                sw.Stop();

                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    outputText.Text = JToken.Parse(data).ToString(Formatting.Indented);
                    timeText.Text = $"Time: {sw.ElapsedMilliseconds} ms";
                    btnConsumerRequestsHistoryPut.IsEnabled = true;

                });

            });

            sw.Start();

            await hubConnection.InvokeAsync("ConsumerRequestsHistoryPut", new { RequestIds = new string[] { "6fd27f1c-88d4-432d-919a-76a4e8cb773b" } });
        }

        private async void BtnConsumerRequestsIdGet_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                outputText.Text = string.Empty;
                timeText.Text = string.Empty;
                btnConsumerRequestsIdGet.IsEnabled = false;

            });

            Stopwatch sw = new Stopwatch();

            hubConnection.On<String>("ConsumerRequestsIdGet", async (data) =>
            {

                sw.Stop();

                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    outputText.Text = JToken.Parse(data).ToString(Formatting.Indented);
                    timeText.Text = $"Time: {sw.ElapsedMilliseconds} ms";
                    btnConsumerRequestsIdGet.IsEnabled = true;

                });

            });

            sw.Start();

            await hubConnection.InvokeAsync("ConsumerRequestsIdGet", "6fd27f1c-88d4-432d-919a-76a4e8cb773b", "37d82b6d-8e7a-437b-bd1f-c0d2b2b56e75");
        }

        private async void BtnConsumerRequestsIdAcceptDelete_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                outputText.Text = string.Empty;
                timeText.Text = string.Empty;
                btnConsumerRequestsIdAcceptDelete.IsEnabled = false;

            });

            Stopwatch sw = new Stopwatch();

            hubConnection.On<String>("ConsumerRequestsIdAcceptDelete", async (data) =>
            {

                sw.Stop();

                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    outputText.Text = JToken.Parse(data).ToString(Formatting.Indented);
                    timeText.Text = $"Time: {sw.ElapsedMilliseconds} ms";
                    btnConsumerRequestsIdAcceptDelete.IsEnabled = true;

                });

            });

            sw.Start();

            await hubConnection.InvokeAsync("ConsumerRequestsIdAcceptDelete", "6fd27f1c-88d4-432d-919a-76a4e8cb773b");
        }

        private async void BtnConsumerRequestsIdAcceptGet_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                outputText.Text = string.Empty;
                timeText.Text = string.Empty;
                btnConsumerRequestsIdAcceptGet.IsEnabled = false;

            });

            Stopwatch sw = new Stopwatch();

            hubConnection.On<String>("ConsumerRequestsIdAcceptGet", async (data) =>
            {

                sw.Stop();

                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    outputText.Text = JToken.Parse(data).ToString(Formatting.Indented);
                    timeText.Text = $"Time: {sw.ElapsedMilliseconds} ms";
                    btnConsumerRequestsIdAcceptGet.IsEnabled = true;

                });

            });

            sw.Start();

            await hubConnection.InvokeAsync("ConsumerRequestsIdAcceptGet", "6fd27f1c-88d4-432d-919a-76a4e8cb773b");
        }

        private async void BtnConsumerRequestsHistoryViewPut_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                outputText.Text = string.Empty;
                timeText.Text = string.Empty;
                btnConsumerRequestsHistoryViewPut.IsEnabled = false;

            });

            Stopwatch sw = new Stopwatch();

            hubConnection.On<String>("ConsumerRequestsHistoryViewPut", async (data) =>
            {

                sw.Stop();

                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    outputText.Text = JToken.Parse(data).ToString(Formatting.Indented);
                    timeText.Text = $"Time: {sw.ElapsedMilliseconds} ms";
                    btnConsumerRequestsHistoryViewPut.IsEnabled = true;

                });

            });

            sw.Start();

            await hubConnection.InvokeAsync("ConsumerRequestsHistoryViewPut");
        }

        private async void BtnConsumerMerchantsGet_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                outputText.Text = string.Empty;
                timeText.Text = string.Empty;
                btnConsumerMerchantsGet.IsEnabled = false;

            });

            Stopwatch sw = new Stopwatch();

            hubConnection.On<String>("ConsumerMerchantsGet", async (data) =>
            {

                sw.Stop();

                await Dispatcher.UIThread.InvokeAsync(() =>
               {
                   outputText.Text = JToken.Parse(data).ToString(Formatting.Indented);
                   timeText.Text = $"Time: {sw.ElapsedMilliseconds} ms";
                   btnConsumerMerchantsGet.IsEnabled = true;

               });

            });

            sw.Start();

            await hubConnection.InvokeAsync("ConsumerMerchantsGet");
        }

        private async void BtnConsumerRequestsGet_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                outputText.Text = string.Empty;
                timeText.Text = string.Empty;
                btnConsumerRequestsGet.IsEnabled = false;

            });

            Stopwatch sw = new Stopwatch();

            hubConnection.On<String>("ConsumerRequestsGet", async (data) =>
            {

                sw.Stop();

                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    outputText.Text = JToken.Parse(data).ToString(Formatting.Indented);
                    timeText.Text = $"Time: {sw.ElapsedMilliseconds} ms";
                    btnConsumerRequestsGet.IsEnabled = true;

                });

            });

            sw.Start();

            await hubConnection.InvokeAsync("ConsumerRequestsGet", "Active", 1, 10);
        }

        private async void BtnConsumerRequestsHistoryGet_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                outputText.Text = string.Empty;
                timeText.Text = string.Empty;
                btnConsumerRequestsHistoryGet.IsEnabled = false;

            });

            Stopwatch sw = new Stopwatch();

            hubConnection.On<String>("ConsumerRequestsHistoryGet", async (data) =>
            {

                sw.Stop();

                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    outputText.Text = JToken.Parse(data).ToString(Formatting.Indented);
                    timeText.Text = $"Time: {sw.ElapsedMilliseconds} ms";
                    btnConsumerRequestsHistoryGet.IsEnabled = true;

                });

            });

            sw.Start();

            await hubConnection.InvokeAsync("ConsumerRequestsHistoryGet", 1, 10);
        }

        private async void BtnConsumerGet_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                outputText.Text = string.Empty;
                timeText.Text = string.Empty;
                btnConsumerGet.IsEnabled = false;

            });

            Stopwatch sw = new Stopwatch();

            hubConnection.On<String>("ConsumerGet", async (data) =>
            {

                sw.Stop();

                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    outputText.Text = JToken.Parse(data).ToString(Formatting.Indented);
                    timeText.Text = $"Time: {sw.ElapsedMilliseconds} ms";
                    btnConsumerGet.IsEnabled = true;

                });

            });

            sw.Start();

            await hubConnection.InvokeAsync("ConsumerGet");
        }

        private async void BtnConsumerRequestsHistoryViewGet_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                outputText.Text = string.Empty;
                timeText.Text = string.Empty;
                btnConsumerRequestsHistoryViewGet.IsEnabled = false;

            });

            Stopwatch sw = new Stopwatch();

            hubConnection.On<String>("ConsumerRequestsHistoryViewGet", async (data) =>
            {
                var bytes = System.Convert.FromBase64String(data);
                data = _dataEncryptor.Decrypt(bytes, clientEncryptorKey);

                sw.Stop();

                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    outputText.Text = JToken.Parse(data).ToString(Formatting.Indented);
                    timeText.Text = $"Time: {sw.ElapsedMilliseconds} ms";
                    btnConsumerRequestsHistoryViewGet.IsEnabled = true;

                });

            });

            sw.Start();

            await hubConnection.InvokeAsync("ConsumerRequestsHistoryViewGet");
        }

        private async void BtnConsumerPut_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                outputText.Text = string.Empty;
                timeText.Text = string.Empty;
                btnConsumerPut.IsEnabled = false;

            });

            Stopwatch sw = new Stopwatch();

            hubConnection.On<String>("ConsumerPut", async (data) =>
            {
                var bytes = System.Convert.FromBase64String(data);
                data = _dataEncryptor.Decrypt(bytes, clientEncryptorKey);

                sw.Stop();

                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    outputText.Text = JToken.Parse(data).ToString(Formatting.Indented);
                    timeText.Text = $"Time: {sw.ElapsedMilliseconds} ms";
                    btnConsumerPut.IsEnabled = true;

                });

            });

            sw.Start();
            await hubConnection.InvokeAsync("ConsumerPut", EncodeData(new { Email = "fharkinsoe@linkedin.com", Firstname = "Vasya" }));
        }

        private async void BtnConsumerMerchantsSeedRequestsGet_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                outputText.Text = string.Empty;
                timeText.Text = string.Empty;
                btnConsumerMerchantsSeedRequestsGet.IsEnabled = false;

            });

            Stopwatch sw = new Stopwatch();

            hubConnection.On<String>("ConsumerMerchantsSeedRequestsGet", async (data) =>
           {
               var bytes = System.Convert.FromBase64String(data);
               data = _dataEncryptor.Decrypt(bytes, clientEncryptorKey);

               sw.Stop();

               await Dispatcher.UIThread.InvokeAsync(() =>
              {
                  outputText.Text = JToken.Parse(data).ToString(Formatting.Indented);
                  timeText.Text = $"Time: {sw.ElapsedMilliseconds} ms";
                  btnConsumerMerchantsSeedRequestsGet.IsEnabled = true;

              });

           });

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

            var url = "http://www.private.covrsecurity.io";
            //var url = "http://localhost:8959";


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

            //var methodsWithoutEncryption = new List<string>()
            //    {
            //        "ConsumerGet",
            //        "ConsumerRequestsHistoryGet",
            //        "ConsumerRequestsGet",
            //        "ConsumerMerchantsGet",
            //        "ConsumerRequestsHistoryViewPut",
            //        "ConsumerRequestsHistoryViewPut",
            //        "ConsumerRequestsIdAcceptGet",
            //        "ConsumerRequestsIdAcceptDelete",
            //        "ConsumerRequestsIdGet",
            //        "ConsumerRequestsHistoryPut"

            //    };

            //var methodsWithEncryption = new List<string>()
            //    {
            //        "ConsumerRequestsHistoryViewGet",
            //        "ConsumerPut",
            //        "ConsumerMerchantsSeedRequestsGet"
            //    };

            //methodsWithoutEncryption.ForEach(method =>
            //{
            //    hubConnection.On<String>(method, (data) =>
            //    {
            //        outputText.Text = JToken.Parse(data).ToString(Formatting.Indented);
            //    });
            //});
            //var window = this;
            //methodsWithEncryption.ForEach(method =>
            //{
            //    hubConnection.On<String>(method, async (data) =>
            //    {
            //        var bytes = System.Convert.FromBase64String(data);
            //        data = _dataEncryptor.Decrypt(bytes, clientEncryptorKey);
            //        sw.Stop();
            //        await Dispatcher.UIThread.InvokeAsync(() => {
            //            outputText.Text = JToken.Parse(data).ToString(Formatting.Indented);
            //            timeText.Text = $"Time: {sw.ElapsedMilliseconds} ms";
            //            btnConsumerGet.IsEnabled = true;

            //        }, DispatcherPriority.Render);
            //    });
            //});


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
