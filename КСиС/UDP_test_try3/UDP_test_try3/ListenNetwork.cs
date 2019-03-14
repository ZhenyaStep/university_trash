using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Windows;
using System.Timers;

namespace UDP_test_try3
{
  /// <summary>
  /// Класс для создания прослушивания данных
  /// </summary>
  public class ListenNetwork
  {
    //Сокет для прослушивания
    public Socket listeningSocket { private set; get; }

    //Порт и IP адрес для работы сокета
    private int localPort;
    private IPAddress localIP;

    //Поток для прослушивания
    private Thread listeningThread;

    //Флаг для указания начала тестирования сети
    private bool IsTestStart = false;
    private object locker = new object();

    //Данные тестирования сети
    private long ReceiveTime = 0;
    private int CountReceivePacks = 0;

    /// <summary>
    /// Инициализирует экземпляр класса ListenNetwork
    /// </summary>
    /// <param name="localPort">Порт для работы сокета</param>
    /// <param name="localIP">Локальный IP адрес</param>
    public ListenNetwork(int localPort, IPAddress localIP)
    {
      this.localPort = localPort;
      this.localIP = localIP;
    }

    /// <summary>
    /// Запуск прослушивания данных
    /// </summary>
    /// <returns>Слушающий сокет</returns>
    public Socket StartListening()
    {
      try
      {
        //Создание сокета для прослушивания
        listeningSocket = new Socket(localIP.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
        listeningSocket.Bind(new IPEndPoint(localIP, localPort));

        //Создание параллельного потока для прослушивания
        listeningThread = new Thread(listenUDP);
        listeningThread.Start();
        return listeningSocket;
      }
      catch (Exception ex)
      {
        MessageBox.Show(ex.Message, "Error! 1");
        return null;
      }
    }

    /// <summary>
    /// Прослушивание данных
    /// </summary>
    private void listenUDP()
    {
      try
      {
        while (true)
        {
          //Начальное значение для данных тестирования
          int CountPacks = 0;
          long ReceiveTime = 0;
          bool IsTest = false;

          int bytes = 0;
          byte[] data = new byte[PackInfo.PACK_SIZE];

          EndPoint remoteEP = new IPEndPoint(localIP, 0);
          IPEndPoint FullRemoteEP = null;

          //Ожидание поступления данных
          while (true)
          {
            lock (locker)
            {
              if (listeningSocket == null)
              {
                return;
              }
              if (listeningSocket.Available > 0)
              {
                break;
              }
            }
          }

          //Чтение данных
          do
          {
            bytes = listeningSocket.ReceiveFrom(data, ref remoteEP);
            if (bytes != 0)
            {
              lock (locker)
              {
                IsTest = IsTestStart;
              }
              //Если данные теста
              if (IsTest)
              {
                if (CountPacks == 0 && ReceiveTime == 0)
                {
                  //Замер времени прибытия первой порции данных
                  ReceiveTime = DateTime.Now.Ticks;
                }
                CountPacks++;
              }
              else
              {
                //Отправка данных на приславшую их удалённую точку
                if (FullRemoteEP == null)
                {
                  FullRemoteEP = remoteEP as IPEndPoint;
                  FullRemoteEP.Port = localPort;
                }
                listeningSocket.SendTo(data, FullRemoteEP);
              }
            }
            //Задержка для получения всех данных
            if (listeningSocket.Available == 0)
            {
              Thread.Sleep(4000);
            }
          }
          while (listeningSocket.Available > 0);

          //Формирование данных тестирования
          if (IsTest)
          {
            lock (locker)
            {
              this.ReceiveTime = ReceiveTime;
              CountReceivePacks = CountPacks;
              IsTestStart = false;
            }
          }
        }
      }
      catch (Exception ex)
      {
        MessageBox.Show(ex.Message, "Error! 2");
      }
    }

    /// <summary>
    /// Остановка слушающего потока
    /// </summary>
    public void StopListening()
    {
      try
      {
        if (listeningSocket != null)
        {
          //Закрытие сокета
          lock (locker)
          {
            listeningSocket.Shutdown(SocketShutdown.Both);
            listeningSocket.Close();
            listeningSocket = null;
          }
        }
        if (listeningThread != null && listeningThread.IsAlive)
        {
          listeningThread.Abort();
          listeningThread = null;
        }
      }
      catch (Exception ex)
      {
        MessageBox.Show(ex.Message, "Error! 3");
      }
    }

    /// <summary>
    /// Установка флага начала тестирования
    /// </summary>
    public void SetTestStart()
    {
      lock (locker)
      {
        IsTestStart = true;
      }
    }

    //Флаг окончания времени ожидания
    private bool IsTimeOver = false;
    private object TimeLocker = new object();

    /// <summary>
    /// Сбор информации о тестировании
    /// </summary>
    /// <returns>Время получения первой порции в тактах и количество полученных порций</returns>
    public Tuple<long, int> GetTestInfo()
    {
      //Запуск таймера для устранения бесконечного ожидания
      var timer = new System.Timers.Timer();
      timer.AutoReset = true;
      timer.Interval = 40000;
      timer.Elapsed += OnTimedEvent;
      timer.Enabled = true;

      bool IsTestOver = false;
      bool IsTimeOver = false;
      while (!IsTimeOver)
      {
        //Проверка флага окончания тестирования
        lock (locker)
        {
          IsTestOver = !IsTestStart;
        }

        //Возврат данных о тестировании
        if (IsTestOver)
        {
          timer.Stop();
          timer.Close();
          lock (TimeLocker)
          {
            this.IsTimeOver = false;
          }
          return new Tuple<long, int>(ReceiveTime, CountReceivePacks);
        }

        lock (TimeLocker)
        {
          IsTimeOver = this.IsTimeOver;
        }
      }
      timer.Close();
      lock (TimeLocker)
      {
        this.IsTimeOver = false;
      }
      return null;
    }

    /// <summary>
    /// Обработка срабатывания таймера
    /// </summary>
    /// <param name="source"></param>
    /// <param name="e"></param>
    private void OnTimedEvent(object source, ElapsedEventArgs e)
    {
      lock (TimeLocker)
      {
        IsTimeOver = true;
      }
    }
  }
}
