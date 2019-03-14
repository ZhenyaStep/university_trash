using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.Windows;

namespace UDP_test_try3
{
  /// <summary>
  /// Класс для выполнения тестирования UDP соединения
  /// </summary>
  public class TestNetwork
  {
    //Экземпляр для прослушивания входящих данных
    private ListenNetwork listenNetwork;
    //Данные удалённой конечной точки
    private IPEndPoint remoteEP;

    /// <summary>
    /// Инициализирует экземпляр класса TestNetwork
    /// </summary>
    /// <param name="listen">Объект для прослушивания входящих данных</param>
    /// <param name="RemoteEP">Данные удалённой конечной точки</param>
    public TestNetwork(ListenNetwork listen, IPEndPoint RemoteEP)
    {
      listenNetwork = listen;
      remoteEP = RemoteEP;
    }

    /// <summary>
    /// Запуск тестирования UDP соединения
    /// </summary>
    /// <returns>Строка резуьтатов тестирования</returns>
    public string StartTest()
    {
      try
      {
        //Установка флага начала тестирования
        listenNetwork.SetTestStart();

        //Отправка данных
        byte[] buffer = new byte[PackInfo.PACK_SIZE];
        long SendTime = 0;

        for (int i = 0; i < PackInfo.COUNT_PACKS; i++)
        {
          if (SendTime == 0)
          {
            //Замер времени начала отправки
            SendTime = DateTime.Now.Ticks;
          }
          listenNetwork.listeningSocket.SendTo(buffer, remoteEP);
        }

        //Получение данных тестирования
        Tuple<long, int> ReceiveInfo = listenNetwork.GetTestInfo();
        string ret = "";
        //Формирование строки результатов
        if (ReceiveInfo != null)
        {
          ret = "Approximate round trip time in seconds: " + ((double)(ReceiveInfo.Item1 - SendTime) / (TimeSpan.FromSeconds(1).Ticks)).ToString("F3") + "\nReceive: " + (1 - (double)ReceiveInfo.Item2 / PackInfo.COUNT_PACKS).ToString("P2");
          MessageBox.Show(ret, "UDP Info");
        }
        else
        {
          ret = "Time is over!";
          MessageBox.Show(ret, "Error!");
        }
        return ret;
      }
      catch (Exception ex)
      {
        MessageBox.Show(ex.Message, "Error!");
        return ex.Message;
      }
    }
  }
}
