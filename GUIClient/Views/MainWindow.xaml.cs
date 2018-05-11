using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace GUIClient.Views
{
    public class MainWindow : Window
    {
        private static Button btnUserNotification;
        private static Button btnUserRegistration;

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

            btnUserNotification = this.FindControl<Button>("btnUserNotification");
            btnUserRegistration = this.FindControl<Button>("btnUserRegistration");

            btnUserNotification.Click += BtnUserNotification_Click;
            btnUserRegistration.Click += BtnUserRegistration_Click;

        }

        private void BtnUserRegistration_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            UserRegistration reg = new UserRegistration();
            reg.Show();
            this.Hide();
        }

        private void BtnUserNotification_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            UserNotification notif = new UserNotification();
            notif.Show();
            this.Hide();
        }
    }
}
