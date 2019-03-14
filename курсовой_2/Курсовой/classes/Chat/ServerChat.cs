using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;


namespace Курсовой.classes
{
    public class ServerChat: ChatTcp
    {
        private TcpListener TcpListener;
        private List<ClientChat> ClientsList;
        public Cipher Cipher;
        public Dictionary<int, SendMethod> SendMethods;
        private Thread ListenThread;



        public ServerChat(int publicKey, int privatKey, int port)
        {
            this.Cipher = new Cipher(privatKey, publicKey);
            this.ClientsList = new List<ClientChat>();
            this.TcpListener = new TcpListener(IPAddress.Any, port);
            SendMethods = new Dictionary<int, SendMethod>()
            {
                { Const.SERVER_MESSAGE, this.ServerMessage },
                { Const.PRIVATE_MESSAGE, this.PrivateMessage },
                { Const.BROADCASTING_MESSAGE, this.BroadcastingMessage },
                { Const.AUDIO_MESSAGE, this.AudioMessage },
                { Const.SET_LOGIN, this.SetLogin },
                { Const.SET_PASSWORD, this.SetPassword },
                { Const.CHECK_LOGIN, this.CheckLogin },
                { Const.CHECK_PASSWORD, this.CheckPassword },
                { Const.SEND_OPEN_KEY, this.NewOpenKey }
            };
            this.ListenThread = new Thread(new ThreadStart(this.Listening));
            ListenThread.Start();
        }

        public void Listening()
        {
            try
            {
                this.TcpListener.Start();
                while (true)
                {
                    TcpClient client = this.TcpListener.AcceptTcpClient();
                    this.ClientsList.Add(new ClientChat(client, (byte)(this.ClientsList.Count()+1), this));

                    Thread clientThread = new Thread(new ThreadStart(this.ClientsList[ClientsList.Count - 1].Listen));
                    clientThread.Start();
                }
            }
            catch(Exception ex)
            {
                ///this.ErrorCallback["listening"](ex.Message);
            }
        }


        public bool ClassifierCommand(byte[] message, int id)
        {
            //byte[] EncodMessage = this.ClientsList[id-1].Cipher.decode(message);
            return this.SendMethods[(int)(message[Const.COMMAND_BYTE_POSITION])](message, id);
        }


        public bool Close()
        {
            try
            {
                for (int i = 0; i < this.ClientsList.Count; i++)
                {
                    try
                    {
                        this.ClientsList[i].Close();

                    } catch(Exception ex)
                    {
                        
                    }
                }
                
                if (this.TcpListener != null)
                {
                    this.TcpListener.Stop();
                }
                if (this.ListenThread != null)
                {
                    ListenThread.Abort();
                }
            } catch(Exception ex)
            {
                return false;
            }
            return true;
        }





/////////////////////////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////// SERVER METHODS ////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////////////////////
        public bool ServerMessage(byte[] message, int id)
        {
            message[Const.COMMAND_BYTE_POSITION] = Const.SERVER_MESSAGE;
            message[Const.CONTROL_NUMBER_POSITION] = Const.CONTROL_NUMBER;
            message = Cipher.code(message);
            this.ClientsList.Find(el => el.Id == id).Stream.Write(message, 0, message.Length);
            return true;
        }

        public bool ServerMessage(string mes, int id)
        {
            byte[] message = Encoding.Unicode.GetBytes(mes);
            int len = message.Length;
            Array.Resize(ref message, Const.MESSAGE_SIZE);
            byte[] lenArray = BitConverter.GetBytes(len);
            message[Const.MESSAGE_LENGTH_POSITION] = lenArray[0];
            message[Const.MESSAGE_LENGTH_POSITION + 1] = lenArray[1];
            return ServerMessage(message, id);
        }

        public bool PrivateMessage(byte[] message, int id)
        {
            if (!ClientsList[id-1].isAuthorised)
            {
                return false;
            }
            try
            {
                if (isValidId(message[Const.RECIEVER_ID_POSITION]))
                {
                    ClientsList[(int)message[Const.RECIEVER_ID_POSITION] - 1].Stream.Write(message, 0, message.Length);
                }
            } catch(Exception ex)
            {
                /// message
                return false;
            }
            return true;
        }

        public bool BroadcastingMessage(byte[] message, int id)
        {
            if (!ClientsList[id-1].isAuthorised)
            {
                return false;
            }
            for (int i = 0; i < this.ClientsList.Count(); i++)
            {
                if (this.ClientsList[i].Id == id)
                {
                    continue;
                }
                ClientsList[i].Stream.Write(message, 0, message.Length);
            }
            return true;
        }

        public bool AudioMessage(byte[] data, int id)
        {
            return PrivateMessage(data, id);
        }

        public bool SetLogin(byte[] data, int id)
        {
            byte[] oldLogin = this.ClientsList[id-1].Login;
            byte[] DecodeData = this.ClientsList[id-1].Cipher.decode(data);
            if (DecodeData[Const.CONTROL_NUMBER_POSITION] != Const.CONTROL_NUMBER)
            {
                this.ServerMessage("подпись невалидна", id);
                return false;
            }
            byte[] newLogin = getData(DecodeData);

            if(this.ClientsList[id-1].isAuthorised)
            {
                if(this.isUniqLogin(newLogin))
                {
                    this.ClientsList[id-1].Login = newLogin;
                    this.ServerMessage("логин изменен", id);
                    BroadcastAboutNewUser(this.ClientsList[id - 1].Id, this.ClientsList[id - 1].Login,
                        this.ClientsList[id - 1].Cipher.publicKey);
                    MessageAboutUsers(id);
                    byte[] Password = FileWork.GetClientPassword(Encoding.Unicode.GetString(oldLogin));
                    FileWork.Deletefile(Encoding.Unicode.GetString(oldLogin));
                    if (!FileWork.SaveClientInfo(Encoding.Unicode.GetString(newLogin), Password))
                    {
                        this.ServerMessage("не удалось вас зарегистрировать", id);
                        return false;
                    }
                }
            } else
            {
                if(!this.isUniqLogin(newLogin))
                {
                    this.ClientsList[id-1].Login = newLogin;
                    this.ServerMessage("логин принят", id);

                    this.ClientsList[id-1].Password = FileWork.GetClientPassword(Encoding.Unicode.GetString(newLogin));
                    BroadcastAboutNewUser(this.ClientsList[id - 1].Id, this.ClientsList[id - 1].Login,
                        this.ClientsList[id - 1].Cipher.publicKey);
                    MessageAboutUsers(id);
                    ///////// чтение пароля из файла
                }
                else
                {
                    this.ClientsList[id - 1].Login = newLogin;
                    this.ClientsList[id-1].Password = new byte[Const.PASSWORD_SIZE];
                    if (!FileWork.SaveClientInfo(Encoding.Unicode.GetString(newLogin), this.ClientsList[id-1].Password))
                    {
                        this.ServerMessage("не удалось вас зарегистрировать", id);
                        return false;
                    }
                    /// запись логина пароля в файл
                    this.ClientsList[id-1].isAuthorised = true;
                    this.ClientsList[id-1].isValidLogin = true;
                    this.ServerMessage("вы зарегистрированы: ", id);
                    BroadcastAboutNewUser(this.ClientsList[id - 1].Id, this.ClientsList[id - 1].Login,
                        this.ClientsList[id - 1].Cipher.publicKey);
                    MessageAboutUsers(id);
                }
            }
            return false;
        }

        

        public bool SetPassword(byte[] data, int id)
        {
            byte[] DecodeData = this.ClientsList[id-1].Cipher.decode(data);
            if (DecodeData[Const.CONTROL_NUMBER_POSITION] != Const.CONTROL_NUMBER)
            {
                this.ServerMessage("подпись невалидна", id);
                return false;
            }
            byte[] newPassword = getData(DecodeData);
            
            if(this.ClientsList[id-1].isAuthorised)
            {
                this.ClientsList[id-1].Password = FileWork.getSHA1(newPassword);
                if (!FileWork.SaveClientInfo(Encoding.Unicode.GetString(this.ClientsList[id-1].Login),
                    this.ClientsList[id-1].Password))
                {
                    this.ServerMessage("не удалось поменять пароль", id);
                    return false;
                }
                ///////////////////////////////////////////////////////////SaveToFile

                this.ServerMessage("пароль изменен", id);
                return true;
            }
            else
            {
                if(FileWork.getSHA1(newPassword) == this.ClientsList[id-1].Password)
                {
                    this.ClientsList[id-1].isAuthorised = true;
                    ServerMessage("клиент авторизирован", id);
                    BroadcastAboutNewUser(this.ClientsList[id - 1].Id, this.ClientsList[id - 1].Login,
                        this.ClientsList[id - 1].Cipher.publicKey);
                }
            }
            
            return false;
        }


        public bool ClientShutDown(byte[] data, int id)
        {
            byte[] DecodeData = this.ClientsList[id-1].Cipher.decode(data);
            if (DecodeData[Const.CONTROL_NUMBER_POSITION] != Const.CONTROL_NUMBER)
            {
                this.ServerMessage("подпись невалидна", id);
                return false;
            }

            if (!ClientsList[id-1].isAuthorised)
            {
                return false;
            }
            byte[] message = new byte[Const.MESSAGE_SIZE];
            for (int i = 0; i < this.ClientsList.Count(); i++)
            {
                try
                {
                    if (this.ClientsList[i].Id == id)
                    {
                        continue;
                    }
                    message[Const.USER_ID_POSITION] = this.ClientsList[i].Id;
                    message[Const.COMMAND_BYTE_POSITION] = Const.CLIENT_SHUTDOWN;
                    this.ClientsList[i].Stream.Write(message, 0, message.Length);
                }
                catch (Exception ex)
                {
                    /// error message  
                    continue;
                }
            }
            return true;
        }

        public bool NewUserInfo(byte[] data, int id)
        {
           
            return true;
        }

        public byte[] GetMessageAboutUser(int id, byte[] Login, int publicKey)
        {
            byte[] message = new byte[Const.MESSAGE_SIZE];
            message[0] = (byte)id; int j;
            byte[] publicK = BitConverter.GetBytes(publicKey);

            for (j = 0; j < publicK.Length; j++)
            {
                message[j + 1] = publicK[j];
            }

            for (int i = 0; i < Login.Length; i++)
            {
                message[i + j + 1] = Login[i];
            }

            byte[] lengthArray = BitConverter.GetBytes(Login.Length + 5);
            message[Const.MESSAGE_LENGTH_POSITION] = lengthArray[0];
            message[Const.MESSAGE_LENGTH_POSITION + 1] = lengthArray[1];
            message[Const.MESSAGE_LENGTH_POSITION + 2] = lengthArray[2];
            message[Const.MESSAGE_LENGTH_POSITION + 3] = lengthArray[3];

            message[Const.CONTROL_NUMBER_POSITION] = Const.CONTROL_NUMBER;
            message = Cipher.code(message);

            message[Const.COMMAND_BYTE_POSITION] = Const.NEW_USER_INFO;
            return message;
        }

        public bool BroadcastAboutNewUser(int id, byte[] Login, int publicKey)
        {
            if (Login == null) { return false; }
            byte[] message = GetMessageAboutUser(id, Login, publicKey);
            /*byte[] message = new byte[Const.MESSAGE_SIZE];
            message[0] = (byte)id;  int j;
            byte[] publicK = BitConverter.GetBytes(publicKey);

            for (j = 0; j < publicK.Length; j++)
            {
                message[j + 1] = publicK[j];
            }

            for (int i = 0; i < Login.Length; i++)
            {
                message[i + j + 1] = Login[i];
            }
       
            byte[] lengthArray = BitConverter.GetBytes(Login.Length + 5);
            message[Const.MESSAGE_LENGTH_POSITION] = lengthArray[0];
            message[Const.MESSAGE_LENGTH_POSITION + 1] = lengthArray[1];
            message[Const.MESSAGE_LENGTH_POSITION + 2] = lengthArray[2];
            message[Const.MESSAGE_LENGTH_POSITION + 3] = lengthArray[3];

            message[Const.CONTROL_NUMBER_POSITION] = Const.CONTROL_NUMBER;
            message = Cipher.code(message);
            
            message[Const.COMMAND_BYTE_POSITION] = Const.NEW_USER_INFO;*/
            for (int i = 0; i < this.ClientsList.Count(); i++)
            {
                ClientsList[i].Stream.Write(message, 0, message.Length);
            }
            return true;
        }

        public bool MessageAboutUsers(int id)
        {
            for(int i = 0; i < this.ClientsList.Count; i++)
            {
                if (this.ClientsList[i].Id == id) { continue; }
                byte[] message = GetMessageAboutUser(ClientsList[i].Id, ClientsList[i].Login, ClientsList[i].Cipher.publicKey);
                ClientsList.Find(el => el.Id == id).Stream.Write(message, 0, message.Length);
            }
            return true;
        }

        /*public bool MessageAboutUsers(int id)
        {
            int index = 0, j;
            byte[] message = new byte[Const.MESSAGE_SIZE];
            for(int i = 0; i < this.ClientsList.Count(); i++)
            {
                if(this.ClientsList[i].Id != id)
                {
                    message[index++] = (byte)ClientsList[i].Id;
                    for(j = 0; j < ClientsList[i].Login.Length; j++)
                    {
                        message[index++] = ClientsList[i].Login[j];
                    }
                    byte[] Key = BitConverter.GetBytes(ClientsList[i].Cipher.publicKey);
                    message[index++] = Key[0];
                    message[index++] = Key[1];
                    message[index++] = Key[2];
                    message[index++] = Key[3];
                }
            }

            message = Cipher.code(message);
            message[Const.COMMAND_BYTE_POSITION] = Const.NEW_USER_INFO;
            ClientsList[id-1].Stream.Write(message, 0, message.Length);
            return true;
        }*/

        public bool NewOpenKey(byte[] data, int id)
        {
            //byte[] DecodeData = this.ClientsList[id-1].Cipher.decode(data);
            if (data[Const.CONTROL_NUMBER_POSITION] != Const.CONTROL_NUMBER)
            {
                this.ServerMessage("подпись невалидна", id);
                return false;
            }
            string num = Encoding.Unicode.GetString(getData(data));
            int key;
            if(!Int32.TryParse(num, out key))
            {
                this.ServerMessage("ключ должен быть числом", id);
            }
            this.ClientsList.Find(el => el.Id == id).Cipher.publicKey = key;
            this.ServerMessage("public key изменен", id);
            BroadcastAboutNewUser(this.ClientsList[id - 1].Id, this.ClientsList[id - 1].Login,
                        this.ClientsList[id - 1].Cipher.publicKey);
            return true;
        }


        //////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////// DATA PREPARE //////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////////////
        /*
        private bool isUniqLogin(byte[] login)
        {
            int j;
            for (int i = 0; i < this.ClientsList.Count(); i++)
            {
                bool isDublicate = true;
                j = 0;
                if (this.ClientsList[i].Login == null) { continue; }
                while ((j < login.Length) && (j < this.ClientsList[i].Login.Length))
                {
                    if ((login[j] != this.ClientsList[i].Login[j]))
                    {
                        isDublicate = false;
                        break;
                    }
                    j++;
                }
                if (isDublicate) { return false; }
            }
            return true;
        }
        */
        private bool isUniqLogin(byte[] login)
        {
            foreach(ClientChat client in this.ClientsList)
            {
                if (client.Login == null) { continue; }
                if (Enumerable.SequenceEqual(client.Login, login))
                {
                    return false;
                }
            }
            return true;
        }


        private bool ChangePassword(byte[] Password, int id)
        {
            string loginString = Encoding.Unicode.GetString(ClientsList[id-1].Login);
            //this.ClientsList[id-1].Password = Password;
            this.ClientsList[id-1].Password = FileWork.getSHA1(Password);
            /// write to file
            return FileWork.SaveClientInfo(loginString, Password);
        }

        public bool CheckLogin(byte[] data, int id)
        {
            byte[] Login = new byte[Const.LOGIN_SIZE];
            for (int i = 0; i < Const.LOGIN_SIZE; i++)
            {
                Login[i] = data[i];
            }
            if (FileWork.CheckLoginExistence(Encoding.Unicode.GetString(Login)))
            {
                ClientsList[id-1].isValidLogin = true;
                ClientsList[id-1].Login = Login;
                ClientsList[id-1].Password = FileWork.GetClientPassword(Encoding.Unicode.GetString(Login));
                return true;
            }
            return false;
        }

        public bool CheckPassword(byte[] data, int id)
        {
            byte[] Password = new byte[Const.PASSWORD_SIZE];
            byte[] HashPassword = FileWork.getSHA1(Password);
            for (int i = 0; i < HashPassword.Length; i++)
            {
                if (HashPassword[i] != ClientsList[id-1].Password[i])
                {
                    return false;
                }
            }
            ClientsList[id-1].isAuthorised = true;
            return true;
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









        private bool isValidId(byte id)
        {
            if(id > this.ClientsList.Count())
            {
                return false;
            }
            return true;
        }
    }
}
