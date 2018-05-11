using Avalonia;
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

namespace GUIClient.Views
{
    public class UserRegistration : Window
    {

        private static Button btnRegister;
        private static Button btnSendVerificationCode;
        private static Button btnCheckVerificationCode;
        private static Button btnMain;

        private static TextBlock outputText;
        private static TextBlock timeText;

        //private static string url = "http://www.private.covrsecurity.io";
        private static string url = "http://localhost:8959";

        private static EncryptorKey clientEncryptorKey = new EncryptorKey();
        private static IDataEncryptor _dataEncryptor = new DataEncryptionLibrary.Helpers.DataEncryptor();
        private static HubConnection hubConnection;

        public UserRegistration()
        {
            this.InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            btnRegister = this.FindControl<Button>("btnRegister");
            btnSendVerificationCode = this.FindControl<Button>("btnSendVerificationCode");
            btnCheckVerificationCode = this.FindControl<Button>("btnCheckVerificationCode");
            btnMain = this.FindControl<Button>("btnMain");

            timeText = this.FindControl<TextBlock>("timeText");
            outputText = this.FindControl<TextBlock>("outputText");

            ConfigureSignalR();

            btnRegister.Click += BtnRegister_Click;
            btnSendVerificationCode.Click += BtnSendVerificationCode_Click;
            btnCheckVerificationCode.Click += BtnCheckVerificationCode_Click;
            btnMain.Click += BtnMain_Click;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);


        }

        private void BtnMain_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            MainWindow main = new MainWindow();
            main.Show();
            this.Hide();
        }

        private async void BtnCheckVerificationCode_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                outputText.Text = string.Empty;
                timeText.Text = string.Empty;
                btnCheckVerificationCode.IsEnabled = false;

            });

            Stopwatch sw = new Stopwatch();

            hubConnection.On<String>("CheckVerificationCode", async (data) =>
            {

                sw.Stop();

                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    btnCheckVerificationCode.IsEnabled = true;
                    outputText.Text = JToken.Parse(data).ToString(Formatting.Indented);
                    timeText.Text = $"Time: {sw.ElapsedMilliseconds} ms, Host: {url}";


                });

            });

            sw.Start();

            await hubConnection.InvokeAsync("CheckVerificationCode", new
            {
                PhoneNumber = "+375292270283",
                Token = "956edf6a-0b93-46ef-b76d-d2c5a8695c4e",
                Pin = "8674"
            });
        }

        private async void BtnSendVerificationCode_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {

            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                outputText.Text = string.Empty;
                timeText.Text = string.Empty;
                btnSendVerificationCode.IsEnabled = false;
            });

            Stopwatch sw = new Stopwatch();

            hubConnection.On<String>("SendVerificationCode", async (data) =>
            {

                sw.Stop();

                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    outputText.Text = JToken.Parse(data).ToString(Formatting.Indented);
                    timeText.Text = $"Time: {sw.ElapsedMilliseconds} ms, Host: {url}";
                    btnSendVerificationCode.IsEnabled = true;

                });

            });

            sw.Start();

            await hubConnection.InvokeAsync("SendVerificationCode", new
            {
                PhoneNumber = "+375292270283"
            });
        }

        private async void BtnRegister_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                outputText.Text = string.Empty;
                timeText.Text = string.Empty;
                btnRegister.IsEnabled = false;

            });

            Stopwatch sw = new Stopwatch();

            hubConnection.On<String>("Register", async (data) =>
            {

                sw.Stop();

                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    outputText.Text = JToken.Parse(data).ToString(Formatting.Indented);
                    timeText.Text = $"Time: {sw.ElapsedMilliseconds} ms, Host: {url}";
                    btnRegister.IsEnabled = true;

                });

            });

            sw.Start();

            await hubConnection.InvokeAsync("Register", new
            {
                UserName = "916edf6a-0b93-46ef-b76d-d2c5a8695c4e",
                Password = "926edf6a-0b93-46ef-b76d-d2c5a8695c4e",
                PhoneNumber = "+375292270283",
                Token = "956edf6a-0b93-46ef-b76d-d2c5a8695c4e",
                DeviceType = "IOS"
            });

        }

        public async void ConfigureSignalR()
        {
            // Client keys pair
            var client_x25519key = _dataEncryptor.CreateX25519EncryptorKey().Key;

            // Client public key bytes array
            var clientPublicKeyBytes = _dataEncryptor.GetX25519EncryptorPublicKeyBytes(client_x25519key);

            // SignalR hub url
            var stringUri = $"{url}/userRegistration";

            var options = new HttpConnectionOptions()
            {

                Url = new System.Uri(stringUri),
            };

            var factory = new HttpConnectionFactory(Options.Create(options), new LoggerFactory());

            hubConnection = new HubConnection(factory, new JsonHubProtocol(), null);

            await hubConnection.StartAsync();


        }
    }
}
