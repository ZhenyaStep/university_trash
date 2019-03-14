using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace Курсовой.classes
{
    public delegate bool delegateCallback(string Str);
    public delegate bool delegateAddUsers(List<ClientInfo> usersList);
    public delegate void SendCallback(byte[] data);
    public delegate bool SendMethod(byte[] data, int id);


    public abstract class ChatTcp
    {
        
    }
}
