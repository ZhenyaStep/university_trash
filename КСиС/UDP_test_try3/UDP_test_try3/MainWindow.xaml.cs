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

namespace UDP_test_try3
{
  /// <summary>
  /// Логика взаимодействия для MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    //Порт для работы приложения
    int localPort = 11000;
    //локальный IP адрес
    IPAddress localIP;
    //Экземпляр для прослушивания входящих данных
    ListenNetwork listen;

    /// <summary>
    /// Инициализация главного окна
    /// </summary>
    public MainWindow()
    {
      InitializeComponent();

      //Опреденение локального IP адреса
      IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
      localIP = ipHostInfo.AddressList.LastOrDefault(i => i.AddressFamily == AddressFamily.InterNetwork);

      //Запуск прослушивания входящих данных
      listen = new ListenNetwork(localPort, localIP);
      listen.StartListening();
    }

    /// <summary>
    /// Обработка события нажатия на кнопку "Start test"
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void button_Click(object sender, RoutedEventArgs e)
    {
      IPAddress remoteIP;
      //Обработка введённого удалённого IP адреса
      if (!IPAddress.TryParse(IPAddrTB.Text, out remoteIP))
      {
        MessageBox.Show("Check IP address", "Error! 6");
        return;
      }

      //Запуск тестирования сети
      DoneLB.Items.Add("Test start " + remoteIP);
      TestNetwork test = new TestNetwork(listen, new IPEndPoint(remoteIP, localPort));
      DoneLB.Items.Add(test.StartTest());
    }

    /// <summary>
    /// Обработка события закрытия окна
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
      //Остановка прослушивания
      listen.StopListening();
    }
  }
}
