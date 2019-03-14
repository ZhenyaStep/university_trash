using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Numerics;

namespace Курсовой.classes
{
    public class Cipher
    {
        public int privateKey { get; set; }
        public int publicKey { get; set; }
        private int n;
        private int e;
        private int d;

        public Cipher()
        {

        }

        public Cipher(int privateKey, int publicKey)
        {
            this.privateKey = privateKey;
            this.publicKey = publicKey;
            byte[] keyBytes = BitConverter.GetBytes(privateKey);
            byte[] publicKeyBytes = BitConverter.GetBytes(publicKey);
            byte[] nBytes = new byte[4];
            byte[] eBytes = new byte[4];
            byte[] dBytes = new byte[4];

            eBytes[0] = keyBytes[2];
            eBytes[1] = keyBytes[3];

            nBytes[0] = keyBytes[0];
            nBytes[1] = keyBytes[1];

            dBytes[0] = publicKeyBytes[2];
            dBytes[1] = publicKeyBytes[3];

            this.n = BitConverter.ToInt32(nBytes, 0);
            this.e = BitConverter.ToInt32(eBytes, 0); ;
            this.d = BitConverter.ToInt32(dBytes, 0);
        }

        public Cipher(int publicKey)
        {
            this.publicKey = publicKey;
            byte[] keyBytes = BitConverter.GetBytes(publicKey);
            byte[] nBytes = new byte[4];
            byte[] dBytes = new byte[4];


            nBytes[0] = keyBytes[0];
            nBytes[1] = keyBytes[1];

            dBytes[0] = keyBytes[2];
            dBytes[1] = keyBytes[3];


            this.n = BitConverter.ToInt32(nBytes, 0);
            this.d = BitConverter.ToInt32(dBytes, 0);

        }


        public List<byte> code(List<byte> data)
        {
            List<byte> Answer = new List<byte>();

            return Answer;
        }

        

        public byte[] code(byte[] data)
        {
            byte[] Answer = new byte[data.Length];
            try
            {
                for (int i = 0; i < data.Length; i += 2)
                {
                    byte[] toConvert = new byte[2];
                    toConvert[0] = data[i];
                    toConvert[1] = data[i + 1];


                    Int16 mes = BitConverter.ToInt16(toConvert, 0);

                    byte[] CipherBytes = BigInteger.ModPow(mes, this.e, this.n).ToByteArray();
                    Answer[i] = CipherBytes[0];
                    Answer[i + 1] = CipherBytes[1];
                }
            }
            catch (Exception ex)
            {

            }
            return data;
          
        }

        public byte[] codePublic(byte[] data)
        {
            byte[] Answer = new byte[data.Length];
            try
            {
                for (int i = 0; i < data.Length; i += 2)
                {
                    byte[] toConvert = new byte[2];
                    toConvert[0] = data[i];
                    toConvert[1] = data[i + 1];
                    Int16 mes = BitConverter.ToInt16(toConvert, 0);
                    byte[] CipherBytes = BigInteger.ModPow(mes, this.d, this.n).ToByteArray();
                    Answer[i] = CipherBytes[0];
                    Answer[i + 1] = CipherBytes[1];
                }
            }
            catch(Exception ex)
            {

            }
            return data;
        }

        public byte[] decodeSecret(byte[] data)
        {
            byte[] Answer = new byte[data.Length];
            try
            {
                for (int i = 0; i < data.Length; i += 2)
                {
                    byte[] toConvert = new byte[2];
                    toConvert[0] = data[i];
                    toConvert[1] = data[i + 1];
                    Int16 mes = BitConverter.ToInt16(toConvert, 0);
                    byte[] DecipherBytes = BigInteger.ModPow(mes, this.e, this.n).ToByteArray();
                    Answer[i] = DecipherBytes[0];
                    Answer[i + 1] = DecipherBytes[1];
                }
            } catch(Exception ex)
            {

            }
            return data;
        }

        public byte[] decode(byte[] data)
        {
            byte[] Answer = new byte[data.Length];
            try
            {
                for (int i = 0; i < data.Length; i += 2)
                {
                    byte[] toConvert = new byte[2];
                    toConvert[0] = data[i];
                    toConvert[1] = data[i + 1];
                    Int16 mes = BitConverter.ToInt16(toConvert, 0);
                    byte[] DecipherBytes = BigInteger.ModPow(mes, this.d, this.n).ToByteArray();
                    Answer[i] = DecipherBytes[0];
                    Answer[i + 1] = DecipherBytes[1];
                }
            } catch(Exception ex)
            {

            }
            return data;
        }

        public string decode(string data)
        {
            string Answer = "";

            return data;
        }
    }
}
