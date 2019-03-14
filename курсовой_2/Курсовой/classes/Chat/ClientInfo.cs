using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Курсовой.classes
{
    public class ClientInfo
    {
        public int publicKey;
        public string Login;
        public byte id;
        public Cipher Cipher;

        public ClientInfo(int publicKey, string Login, byte id)
        {
            this.publicKey = publicKey;
            this.Login = Login;
            this.id = id;
            this.Cipher = new Cipher(publicKey);
        }

    }
}
