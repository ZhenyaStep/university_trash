using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Курсовой.classes
{
    public static class Const
    {
        

        public const int MESSAGE_SIZE = 1032;
        public const int LOGIN_SIZE = 24;
        public const int PASSWORD_SIZE = 24;
        public const int KEY_LENGTH = 4;
        public const byte CONTROL_NUMBER = 91;

        public const int COMMAND_BYTE_POSITION = 1024;
        public const int MESSAGE_LENGTH_POSITION = 1025;
        public const int USER_ID_POSITION = 1029;
        public const int RECIEVER_ID_POSITION = 1030;
        public const int CONTROL_NUMBER_POSITION = 1031;
        

        public const byte SERVER_MESSAGE = 0;
        public const byte PRIVATE_MESSAGE = 1;
        public const byte BROADCASTING_MESSAGE = 2;
        public const byte AUDIO_MESSAGE = 3;
        public const byte SET_LOGIN = 4;
        public const byte SET_PASSWORD = 5;
        public const byte CHECK_LOGIN = 6;
        public const byte CHECK_PASSWORD = 7;
        public const byte CLIENT_SHUTDOWN = 8;
        public const byte NEW_USER_INFO = 9;
        public const byte SEND_OPEN_KEY = 10;
        
    }
}
