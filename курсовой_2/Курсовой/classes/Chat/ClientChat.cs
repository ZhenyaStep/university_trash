using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace Курсовой.classes
{
    internal class ClientChat: ChatTcp
    {
        private TcpClient TcpClient;
        public bool isAuthorised;
        public bool isValidLogin;
        private ServerChat Server;
        public NetworkStream Stream;
        public byte Id { get; private set; }
        public byte[] Login;
        public Cipher Cipher;
        public byte[] Password;


        public ClientChat(TcpClient TcpClient, byte Id, ServerChat Server)
        {
            this.isAuthorised = false;
            this.isValidLogin = false;
            this.TcpClient = TcpClient;
            this.Server = Server;
            this.Id = Id;
            this.Cipher = new Cipher();
        }

       

        public void Listen()
        {
            try
            {
                this.Stream = this.TcpClient.GetStream();
                while (true)
                {
                    byte[] data = new byte[Const.MESSAGE_SIZE];
                    {
                        Int32 bytes = this.Stream.Read(data, 0, data.Length);
                        this.Server.ClassifierCommand(data, this.Id);
                    }
                }
            }
            catch (Exception ex)
            {
               
            }
        }    

        public bool Close()
        {
            try
            {
                if(Stream != null)
                {
                    this.Server.ServerMessage("сервер отключен", this.Id);
                    Stream.Close();
                }
                if(this.TcpClient != null)
                {
                    this.TcpClient.Close();
                }
            } catch(Exception ex)
            {
                //// message
                return false;
            }
            return true;
        }
    }
}
