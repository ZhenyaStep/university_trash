using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Shapes;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Threading;


namespace Курсовой.classes.Chat
{
    //    public delegate void SendCallback(string str);

    public delegate bool methodDelegate(byte[] message);

    public class Client
    {
        public byte Id { private set; get; }
        public string login;
        public string password;
        public Cipher Cipher; // decode public Key
        public Cipher ServerCipher;
        public List<ClientInfo> ClientsInfo;
        private TcpClient TcpClient;
        private NetworkStream stream;
        private Random Random;
        private RichTextBox textbox;
        private ComboBox combobox;
        public delegateCallback delegateCallback;
        public delegateAddUsers addUserCallback;

        private Dictionary<int, methodDelegate> methods;


        public Client(ref RichTextBox textbox, ref ComboBox combobox, IPEndPoint ServerEP, 
            delegateCallback delegateCallback, delegateAddUsers addUserCallback)
        {
            this.delegateCallback = delegateCallback;
            this.addUserCallback = addUserCallback;
            this.textbox = textbox;
            this.combobox = combobox;
            this.ClientsInfo = new List<ClientInfo>();
            this.ServerCipher = new Cipher(123);
            this.TcpClient = new TcpClient();
            this.Cipher = new Cipher(345);
            this.ClientsInfo = new List<ClientInfo>();
            this.TcpClient.Connect(ServerEP.Address, ServerEP.Port);
            this.methods = new Dictionary<int, methodDelegate>()
            {
                { Const.AUDIO_MESSAGE, this.RecieveAudio},
                { Const.BROADCASTING_MESSAGE , this.RecieveBroadcasting},
                { Const.CLIENT_SHUTDOWN , this.RecieveClientShutDown},
                { Const.NEW_USER_INFO , this.RecieveNewClient},
                { Const.PRIVATE_MESSAGE , this.RecievePrivate},
                { Const.SERVER_MESSAGE, this.RecieveServerMessage}
            };
            this.stream = this.TcpClient.GetStream();
            Thread ListenThread = new Thread(new ThreadStart(this.Listen));
            ListenThread.Start();
        }


        public void Listen()
        {
            byte[] message = new byte[Const.MESSAGE_SIZE];
            
            while (true)
            {
                try
                {
                    while (this.stream.DataAvailable)
                    {
                        int len = this.stream.Read(message, 0, message.Length);
                        ClassifierCommand(message); 
                    }
                }
                catch (Exception ex)
                {
                    //this.textbox.AppendText("\n" + ex.Message + "\n");
                    this.delegateCallback("\n" + ex.Message + "\n");
                }
            }
        }

        public bool ClassifierCommand(byte[] message)
        {
            return this.methods[(int)message[Const.COMMAND_BYTE_POSITION]](message);
        }

        

        public bool RecievePrivate(byte[] message)
        {
            byte sender = message[Const.USER_ID_POSITION];
            if (sender != 0)
            {
                //message = this.ClientsInfo[(int)sender].Cipher.decode(message);
                message = this.ClientsInfo.Find(el => el.id == sender).Cipher.decode(message);
            }
            message = this.Cipher.decodeSecret(message);
            if (message[Const.CONTROL_NUMBER_POSITION] != Const.CONTROL_NUMBER)
            {
                //textbox.AppendText("unverified:\n" +
                // Encoding.Unicode.GetString(getData(message)) + "\n\n");
                this.delegateCallback("unverified:\n" +
                 Encoding.Unicode.GetString(getData(message)) + "\n\n");
                return false;
            }
            if(sender == 0)  // инкогнито сообщение
            {
                //textbox.AppendText("Anonymous private message:\n" + 
                //   Encoding.Unicode.GetString(getData(message)) + "\n\n");
                this.delegateCallback("Anonymous private message:\n" +
                   Encoding.Unicode.GetString(getData(message)) + "\n\n");
            }
            else 
            {
                ClientInfo info = ClientsInfo.Find(el => el.id == sender);
                string senderLogin = info.Login;

                // textbox.AppendText("Private message from: " + senderLogin + "\n" +
                //   Encoding.Unicode.GetString(getData(message)) + "\n\n");
                this.delegateCallback("Private message from: " + senderLogin + "\n" +
                   Encoding.Unicode.GetString(getData(message)) + "\n\n");
            }
            return true;
        }

        public bool RecieveBroadcasting(byte[] message)
        {
            byte sender = message[Const.USER_ID_POSITION];
            if (sender != 0)
            {
                //message = this.ClientsInfo[(int)sender].Cipher.decode(message);
                message = this.ClientsInfo.Find(el => el.id == sender).Cipher.decode(message);
            }
            
            if (message[Const.CONTROL_NUMBER_POSITION] != Const.CONTROL_NUMBER)
            {
                //textbox.AppendText("Unverified broadcasting message:\n" +
                //   Encoding.Unicode.GetString(getData(message)) + "\n\n");
                this.delegateCallback("Unverified broadcasting message:\n" +
                   Encoding.Unicode.GetString(getData(message)) + "\n\n");
                return false;
            }

            if (sender == 0) 
            {
                //textbox.AppendText("Anonymous broadcasting message:\n" +
                //   Encoding.Unicode.GetString(getData(message)) + "\n\n");
                this.delegateCallback("Anonymous broadcasting message:\n" +
                   Encoding.Unicode.GetString(getData(message)) + "\n\n");
            }
            else 
            {
                ClientInfo info = ClientsInfo.Find(el => el.id == sender);
                string senderLogin = info.Login;
                //textbox.AppendText("Broadcasting message from: " + senderLogin + "\n" +
                //   Encoding.Unicode.GetString(getData(message)) + "\n\n");
                this.delegateCallback("Broadcasting message from: " + senderLogin + "\n" +
                   Encoding.Unicode.GetString(getData(message)) + "\n\n");
            }
            return true;
        }

        public bool RecieveServerMessage(byte[] message)
        {
            message = this.ServerCipher.decode(message);
            if (message[Const.CONTROL_NUMBER_POSITION] != Const.CONTROL_NUMBER)
            {
                //textbox.AppendText("A message from an unverifyed server!\n\n");
                this.delegateCallback("A message from an unverifyed server!\n\n");
                return false;
            }
            //вывести сообщение
            // textbox.AppendText("Message from server:\n" +
            //         Encoding.Unicode.GetString(getData(message)) + "\n\n");
            this.delegateCallback("Message from server:\n" +
                     Encoding.Unicode.GetString(getData(message)) + "\n\n");

            return true;
        }

        public bool RecieveNewClient(byte[] message)
        {
            byte[] mes = this.Cipher.decode(message);
            mes = this.Cipher.decodeSecret(mes);
            if (mes[Const.CONTROL_NUMBER_POSITION] != Const.CONTROL_NUMBER)
            {
                ////вывод ошибки
                //  textbox.AppendText("An unverifyed user tried to connect!\n\n");
                this.delegateCallback("An unverifyed user tried to connect!\n\n");
                return false;
            }

            
            byte[] publicKey = new byte[Const.KEY_LENGTH];
            byte id;
            int index = 0;
            int i;

            byte[] lengthArray = new byte[4];
            lengthArray[0] = mes[Const.MESSAGE_LENGTH_POSITION];
            lengthArray[1] = mes[Const.MESSAGE_LENGTH_POSITION + 1];
            lengthArray[2] = mes[Const.MESSAGE_LENGTH_POSITION + 2];
            lengthArray[3] = mes[Const.MESSAGE_LENGTH_POSITION + 3];
            int messageLength = BitConverter.ToInt32(lengthArray, 0);
            byte[] Login = new byte[messageLength - 5];

            while (index < messageLength)
            {
                id = mes[index++];
                for (i = 0; i < Const.KEY_LENGTH; i++)
                {
                    publicKey[i] = mes[index + i];
                }
                index += i;

                for (i = 0; i < messageLength - 5; i++)
                {
                    Login[i] = mes[index + i];
                }
                index += i;

                string name = Encoding.Unicode.GetString(Login);
                if (name == this.login)
                {
                    this.Id = id;
                    continue;
                }

                if ((this.ClientsInfo.FindIndex(el => el.id == id) == -1) && (this.Id != id))
                {
                    this.ClientsInfo.Add(new ClientInfo(BitConverter.ToInt32(publicKey, 0), Encoding.Unicode.GetString(Login), id));
                }
                else
                {
                    if (id == this.Id)
                    {
                        this.login = Encoding.Unicode.GetString(Login);
                        this.Cipher.publicKey = BitConverter.ToInt32(publicKey, 0);
                    }
                    else
                    {
                        this.ClientsInfo.Find(el => el.id == id).Login = Encoding.Unicode.GetString(Login);
                        this.ClientsInfo.Find(el => el.id == id).Cipher.publicKey = BitConverter.ToInt32(publicKey, 0);
                    }
                }
                this.addUserCallback(this.ClientsInfo);
            }

            //this.addUserCallback(Encoding.Unicode.GetString(Login));
            return true;
        }

        public bool RecieveAudio(byte[] message)
        {
            byte[] audio = getData(message);
            byte id = message[Const.USER_ID_POSITION];
            if (id != 0)
            {
                message = this.ClientsInfo[id].Cipher.decode(message);
            }
            message = this.Cipher.decodeSecret(message);
            if (message[Const.CONTROL_NUMBER_POSITION] != Const.CONTROL_NUMBER)
            {
                ////вывод ошибки
                // textbox.AppendText("An unverifyed user tried to send you audio!\n\n");
                this.delegateCallback("An unverifyed user tried to send you audio!\n\n");
                return false;
            }
            //сохранить аудио
            return true;
        }

        public bool RecieveClientShutDown(byte[] message)
        {
            byte id = message[Const.USER_ID_POSITION];
            message = this.Cipher.decodeSecret(message);
            if (message[Const.CONTROL_NUMBER_POSITION] != Const.CONTROL_NUMBER)
            {
                // textbox.AppendText("An unverifyed user tried to quit!\n\n");
                this.delegateCallback("An unverifyed user tried to quit!\n\n");
                return false;
            }

            //textbox.AppendText("User: " + this.ClientsInfo.Find(el=>el.id==id).Login + " quit!\n\n");
            this.delegateCallback("User: " + this.ClientsInfo.Find(el => el.id == id).Login + " quit!\n\n");
            return true;
        }

      
        public bool Send(string message, string login, bool isAnonimous, string command)
        {
            if (command == "AUDIO")
            {
                return SendAudio();
            }
            byte[] mes = Encoding.Unicode.GetBytes(message);
            int len = mes.Length;
            byte[] lenArray = BitConverter.GetBytes(len);
        
            Array.Resize(ref mes, Const.MESSAGE_SIZE);
            mes[Const.CONTROL_NUMBER_POSITION] = Const.CONTROL_NUMBER;
            mes[Const.MESSAGE_LENGTH_POSITION] = lenArray[0];
            mes[Const.MESSAGE_LENGTH_POSITION + 1] = lenArray[1];
            byte senderId = 0;

            if((!isAnonimous) && (command != "Change Public Key"))
            { 
                senderId = this.Id;
                mes = Cipher.code(mes);
            }

            if (login != "None")
            {
                byte receiverId = this.ClientsInfo.Find(el => el.Login == login).id;
                mes[Const.RECIEVER_ID_POSITION] = receiverId;
            }

            mes[Const.USER_ID_POSITION] = senderId;
            
            switch (command){
                case "Private message":
                    mes[Const.COMMAND_BYTE_POSITION] = Const.PRIVATE_MESSAGE;
                    break;
                case "Broadcasting":
                    mes[Const.COMMAND_BYTE_POSITION] = Const.BROADCASTING_MESSAGE;
                    break;
                case "Change Login":
                    mes[Const.COMMAND_BYTE_POSITION] = Const.SET_LOGIN;
                    break;
                case "Change Password":
                    mes[Const.COMMAND_BYTE_POSITION] = Const.SET_PASSWORD;
                    break;
                case "Change Public Key":
                    mes[Const.COMMAND_BYTE_POSITION] = Const.SEND_OPEN_KEY;
                    break;
                case "shut down":
                    mes[Const.COMMAND_BYTE_POSITION] = Const.CLIENT_SHUTDOWN;
                    break;
            }
            try
            {
                this.stream.Write(mes, 0, mes.Length);
            } catch(Exception ex)
            {
                /// this.textbox.AppendText("\n" + ex.Message + "\n");
                 this.delegateCallback("\n" + ex.Message + "\n");
                return false;
            }
            return true;
        }

        public bool SendAudio()
        {
            return true;
        }

        public bool Close()
        {
            try
            {
                if (stream != null)
                {
                    byte[] message = new byte[Const.MESSAGE_SIZE];
                    message[Const.CONTROL_NUMBER_POSITION] = Const.CONTROL_NUMBER;
                    message = this.Cipher.code(message);
                    message[Const.COMMAND_BYTE_POSITION] = Const.CLIENT_SHUTDOWN;
                    this.stream.Write(message, 0, message.Length);
                    stream.Close();
                }
                if (TcpClient != null)
                {
                    TcpClient.Close();
                }
            }
            catch (Exception ex)
            {
                //this.textbox.AppendText("\n" + ex.Message + "\n");
                this.delegateCallback("\n" + ex.Message + "\n");
                return false;
            }
            return true;
        }


        ///////////////////////////////////// ПОБОЧКИ ////////////////////////////////////////////

        private byte[] ConfigMessage(string text, byte recieverId, byte senderId)
        {
            byte[] message = Encoding.Unicode.GetBytes(text);
            byte[] messageLength = BitConverter.GetBytes(message.Length);
            Array.Resize(ref message, Const.MESSAGE_SIZE);

            for (int i = 0; i < messageLength.Length; i++)
            {
                message[Const.MESSAGE_LENGTH_POSITION + i] = messageLength[i];
            }

            if (senderId != 0)
            {
                message = this.Cipher.code(message);
            }
            if (recieverId != 0)
            {
                message = this.ClientsInfo.Find(el => el.id == recieverId).Cipher.codePublic(message);
            }

            if (recieverId == 0)
            {
                message[Const.COMMAND_BYTE_POSITION] = Const.BROADCASTING_MESSAGE;
            }
            else
            {
                message[Const.COMMAND_BYTE_POSITION] = Const.PRIVATE_MESSAGE;
            }

            message[Const.USER_ID_POSITION] = senderId;

            return message;
        }


        private byte[] getData(byte[] message)
        {
            byte[] len = new byte[4];
            len[0] = message[Const.MESSAGE_LENGTH_POSITION];
            len[1] = message[Const.MESSAGE_LENGTH_POSITION + 1];
            len[2] = message[Const.MESSAGE_LENGTH_POSITION + 2];
            len[3] = message[Const.MESSAGE_LENGTH_POSITION + 3];

            int dataLen = BitConverter.ToInt32(len, 0);

            byte[] data = new byte[dataLen];
            for (int i = 0; i < dataLen; i++)
            {
                data[i] = message[i];
            }
            return data;
        }

    }
}
