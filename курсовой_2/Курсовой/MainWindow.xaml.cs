using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;
using System.Net.Sockets;

namespace Курсовой
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public classes.Chat.Client Client;
        public classes.ServerChat Server;
        public bool isAnonimous;

        public MainWindow()
        {
            InitializeComponent();
        }

/// <summary>
/// CHAT
/// </summary>
/// <param name="sender"></param>
/// <param name="e"></param>
        private void ChatModeCB_Checked(object sender, RoutedEventArgs e)
        {
            if ((sender as CheckBox).IsChecked == true)
            {
                PortValueTB.IsEnabled = true;
                StartServerBtn.IsEnabled = true;
            }
            else
            {
                PortValueTB.IsEnabled = false;
                StartServerBtn.IsEnabled = false;
            }
        }


        private void StartServerBtn_Click(object sender, RoutedEventArgs e)
        {
            int Port;
            if ((!Int32.TryParse(PortValueTB.Text, out Port)) || (Port < 1000) || (Port > 20000))
            {
                MessageBox.Show("Port must be in range [1000; 20000]!");
                return;
            }
            int privateKey;
            if (!Int32.TryParse(SecretKeyTB.Text, out privateKey))
            {
                MessageBox.Show("Key must be an integer!");
                return;
            }

            int publicKey;
            if (!Int32.TryParse(OpenKeyTB.Text, out publicKey))
            {
                MessageBox.Show("Key must be an integer!");
                return;
            }
            this.Server = new classes.ServerChat(publicKey, privateKey, Port);

        }

        private void SendBtn_Click(object sender, RoutedEventArgs e)
        {
            /// text from input
            /// from curr combobox
            try
            {
                string text = new TextRange(InputRB.Document.ContentStart, InputRB.Document.ContentEnd).Text;
                char[] Arr = text.ToArray();
                Array.Resize(ref Arr, Arr.Length - 2);
                text = new string(Arr);
                this.Client.Send(text,
                    (AvailableUsersCB.SelectedItem as ComboBoxItem).Content.ToString(), isAnonimous,
                    (this.CommandsCB.SelectedItem as ComboBoxItem).Content.ToString());
                if ((this.CommandsCB.SelectedItem as ComboBoxItem).Content.ToString() == "Private message")
                {
                    printMes("You to " + (AvailableUsersCB.SelectedItem as ComboBoxItem).Content.ToString() + ":\n" + "    " + text + "\n\n");
                }
                if ((this.CommandsCB.SelectedItem as ComboBoxItem).Content.ToString() == "Broadcasting")
                {
                    printMes("You" + ":\n" + "    " + text + "\n\n");
                }
            }
            catch(Exception ex)
            {
                this.MessagesRB.AppendText("\n" + ex.Message + "\n");
                return;
            }
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            if ((LoginTB.Text == "") || (PasswordTB.Password == "") || (IPValueTB.Text == ""))
            {
                MessageBox.Show("Not all registration fields filled in!");
                return;
            }

            string[] infoEP = IPValueTB.Text.Split(':');
            if (infoEP.Length != 2)
            {
                MessageBox.Show("Not valid End Point values!");
                return;
            }
            IPAddress ip;
            if(!IPAddress.TryParse(infoEP[0], out ip))
            {
                MessageBox.Show("Not valid IP!");
                return;
            }
            int portServer;
            if (!Int32.TryParse(infoEP[1], out portServer))
            {
                MessageBox.Show("Not valid server Port!");
                return;
            }
            int publicKey;
            if (!Int32.TryParse(OpenKeyTB.Text, out publicKey))
            {
                MessageBox.Show("Open Key must be an integer!");
                return;
            }

            

            if (this.Client != null)
            {
                this.Client.Close();
            }

            this.Client = new classes.Chat.Client(ref MessagesRB, ref AvailableUsersCB, new IPEndPoint(ip, portServer), 
                this.printMes, this.addUsersToCombobox); 
            isAnonimous = (AnonymousCheckBox.IsChecked == true);// checkBox state

            this.Client.password = PasswordTB.Password;
            this.Client.login = LoginTB.Text;
            this.Client.Cipher.publicKey = publicKey;

            this.Client.Send(OpenKeyTB.Text, "None", isAnonimous, "Change Public Key");
            this.Client.Send(LoginTB.Text, "None", isAnonimous, "Change Login");
            this.Client.Send(PasswordTB.Password, "None", isAnonimous, "Change Password");
            
        }

        public bool printMes(string str)
        {
            this.Dispatcher.Invoke(() =>
            {
                this.MessagesRB.AppendText(str);
            });
            return true;
        }

        public bool addUsersToCombobox(List<classes.ClientInfo> clients)
        {
            this.Dispatcher.Invoke(() =>
            {
                this.AvailableUsersCB.Items.Clear();
                this.AvailableUsersCB.Items.Add(new ComboBoxItem() { Content = "None" });
                this.AvailableUsersCB.SelectedIndex = 0;
                foreach (classes.ClientInfo client in clients)
                {
                    this.AvailableUsersCB.Items.Add(new ComboBoxItem() { Content = client.Login });
                }
            });
            return true;
        }

        private void AnonymousCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            //MessagesRB.Background = new ImageBrush(new BitmapImage(new Uri("hazker.png", UriKind.Relative)));
            MessagesRB.Background = new SolidColorBrush(Color.FromArgb(255,0,0,0));
            MessagesRB.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
            InputRB.Background = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            InputRB.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
            isAnonimous = true;

        }

        private void AnonymousCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            MessagesRB.Background = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
            MessagesRB.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            InputRB.Background = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
            InputRB.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            isAnonimous = false;
        }

        public void AddText(string from, string message, ref RichTextBox reciever)
        {
            reciever.AppendText(from + ":");
            reciever.AppendText("    " + message + "\n");
        }

        private void ChangeSecretKeyBtn_Click(object sender, RoutedEventArgs e)
        {
            int SecretKey;
            if(!Int32.TryParse(SecretKeyTB.Text, out SecretKey))
            {
                MessageBox.Show("Check the value of the private key!");
                return;
            }
            this.Client.Cipher.privateKey = SecretKey;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if(this.Client != null)
            {
                this.Client.Close();
            }
            if(this.Server != null)
            {
                this.Server.Close();
            }
        }
    }
}
