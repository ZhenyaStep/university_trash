using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.IO;
using System.Security.Cryptography;




namespace ConsoleApplication3
{
    class Program
    {
        static void Main(string[] args)
        {
            byte[] Text = new byte[]{12,12,12, 23,34,35,34,35,34,34,35,43,14,14,35,13,2,24,22,214,2,24,24
            ,24,24,24,24,24,4,4,4,4,4,4,4,4,4,4,24,24,24,
            24,24,24,24,24,4,4,4,4,4,4,4,4,4,4,24,24,24,24,24,4,4,4,4,4,4,4,4,4,4,
            24,24,24,24,24,4,4,4,4,4,4,4,4,4,4,
            24,24,24,24,24,4,4,4,4,4,4,4,4,4,4,24,24,24,24,24,4,4,4,4,4,4,4,4,4,4,
            24,24,4,4,4,4,4,4,4,4,4,4,24,24,24,24,24,4,4,4,4,4,4,4,4,4,4};
            byte[] HASH = CSHA.getHash(Text);

            byte[] TRUEHASH = SHA1.Create().ComputeHash(Text);

        }
    }


    


    class CSHA
    {
        private const int BytesPerBlock = 64;
        /*
        private const UInt32 A = 1732584193;
        private const UInt32 B = 4023233417;

        private const UInt32 C = 2562383102;
        private const UInt32 D = 271733878;
        private const UInt32 E = 3285377520;
        private const UInt32 k1 = 1518500249;
        private const UInt32 k2 = 1859775393;
        private const UInt32 k3 = 2400959708;
        private const UInt32 k4 = 3395469782;
         * */

        private const UInt32 A = 0x67452301;
        private const UInt32 B = 0xEFCDAB89;
        private const UInt32 C = 0x98BADCFE;
        private const UInt32 D = 0x10325476;
        private const UInt32 E = 0xC3D2E1F0;

        private const UInt32 k1 = 0x5A827999;
        private const UInt32 k2 = 0x6ED9EBA1;
        private const UInt32 k3 = 0x8F1BBCDC;
        private const UInt32 k4 = 0xCA62C1D6;

        private static BigInteger N_32 = (BigInteger)Math.Pow(2, 32);
        private static int N_16 = (int)Math.Pow(2, 16);
        private static int N_8 = (int)Math.Pow(2, 8);

        private static UInt32 F(UInt32 X, UInt32 Y, UInt32 Z, int t, out UInt32 K)
        {
            K = 0;
            if ((t >= 0) && (t <= 19)) { K = k1; return (X & Y) | ((~X) & Z); }
            if ((t >= 20) && (t <= 39)) { K = k2; return X ^ Y ^ Z; }
            if ((t >= 40) && (t <= 59)) { K = k3; return (X & Y) | (X & Z) | (Y & Z); }
            if ((t >= 60) && (t <= 79)) { K = k4; return X ^ Y ^ Z; }
            return 0;
        }

        private static UInt32 getW(int t, ref UInt32[] Data)
        {
            if ((t >= 0) && (t <= 15)) { return Data[t]; }
            Array.Resize(ref Data, Data.Length + 1);
            Data[t] = ShiftLeft((Data[t - 3] ^ Data[t - 8] ^ Data[t - 14] ^ Data[t - 16]), 1);
            //Data[t] = ShiftLeft((AddMod2In32(AddMod2In32(AddMod2In32(Data[t - 3], Data[t - 8]), Data[t - 14]), Data[t - 16])), 1);
            return Data[t];
        }

        private static UInt32 AddMod2In32(UInt32 A, UInt32 B)
        {
            return (UInt32)(((BigInteger)(A) + (BigInteger)(B)) % N_32);

        }


        private static UInt32 ShiftLeft(UInt32 A, int n)
        {
            uint Answer = A;
            return Answer <<= n;
        }


        private static UInt32[] ShaStep(UInt32[] Data, UInt32 A, UInt32 B, UInt32 C, UInt32 D, UInt32 E)
        {
            UInt32 temp, K;
            UInt32 a = A, b = B, c = C, d = D, e = E;
            for (int t = 0; t < 80; t++)
            {
                temp = AddMod2In32(AddMod2In32(AddMod2In32(AddMod2In32(
                    (ShiftLeft(a, 5)), F(b, c, d, t, out K)), e), getW(t, ref Data)), K);
                e = d;
                d = c;
                c = ShiftLeft(b, 30);
                b = a;
                a = temp;

            }

            a = AddMod2In32(a, A);
            b = AddMod2In32(b, B);
            c = AddMod2In32(c, C);
            d = AddMod2In32(d, D);
            e = AddMod2In32(e, E);
            return new UInt32[] { a, b, c, d, e };
        }

        private static byte[][] PrepareBlocks(byte[] Text)
        {
            int k = 0;
            int j;
            Int64 len = Text.Length * 8;  /// длинна всего массива Byte
            byte[][] Answer = new byte[Text.Length / BytesPerBlock][];
            for (int i = 0; i < Answer.Length; i++) { Array.Resize(ref Answer[i], BytesPerBlock); for (j = 0; j < BytesPerBlock; j++) { Answer[i][j] = (Text[k++]); } }
            if (Text.Length % BytesPerBlock != 0)
            {
                j = 0;
                Array.Resize(ref Answer, Answer.Length + 1);
                Array.Resize(ref Answer[Answer.Length - 1], BytesPerBlock);
                while (k < Text.Length) { Answer[Answer.Length - 1][j++] = (Text[k++]); }
                if (j < BytesPerBlock)
                {
                    Answer[Answer.Length - 1][j++] = 128;
                    if (j+8 >= BytesPerBlock) 
                    {
                        Array.Resize(ref Answer, Answer.Length + 1);
                        Array.Resize(ref Answer[Answer.Length - 1], BytesPerBlock);
                    }
                }
                else
                {
                    Array.Resize(ref Answer, Answer.Length + 1);
                    Array.Resize(ref Answer[Answer.Length - 1], BytesPerBlock);
                    Answer[Answer.Length - 1][0] = 128;
                }

                Answer[Answer.Length - 1][BytesPerBlock - 8] = (byte)(((len / N_32) / N_16) / N_8);
                Answer[Answer.Length - 1][BytesPerBlock - 7] = (byte)(((len / N_32) / N_16) % N_8);
                Answer[Answer.Length - 1][BytesPerBlock - 6] = (byte)(((len / N_32) % N_16) / N_8);
                Answer[Answer.Length - 1][BytesPerBlock - 5] = (byte)(((len / N_32) % N_16) % N_8);
                Answer[Answer.Length - 1][BytesPerBlock - 4] = (byte)(((len % N_32) / N_16) / N_8);
                Answer[Answer.Length - 1][BytesPerBlock - 3] = (byte)(((len % N_32) / N_16) % N_8);
                Answer[Answer.Length - 1][BytesPerBlock - 2] = (byte)(((len % N_32) % N_16) / N_8);
                Answer[Answer.Length - 1][BytesPerBlock - 1] = (byte)(((len % N_32) % N_16) % N_8);
            }

            return Answer;
        }

        private static UInt32[] convBlockToUint32(byte[] Block)
        {
            UInt32[] Answer = new UInt32[16];
            int pos = 0;
            byte[] temp = new byte[4];

            for (int i = 0; i < Answer.Length; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    temp[3 - j] = Block[pos + j];
                }
                Answer[i] = BitConverter.ToUInt32(temp, 0);
                pos += 4;
            }
            return Answer;
        }

        public static byte[] getHash(byte[] Text)
        {
            UInt32[] temp;
            int pos = 0;
            byte[] ArrTemp;
            byte[][] Blocks = PrepareBlocks(Text);
            UInt32[] ABCDE = new UInt32[] { A, B, C, D, E };
            for (int i = 0; i < Blocks.Length; i++)
            {
                temp = convBlockToUint32(Blocks[i]);
                ABCDE = ShaStep(temp, ABCDE[0], ABCDE[1], ABCDE[2], ABCDE[3], ABCDE[4]);
            }
            byte[] Answer = new byte[ABCDE.Length * 4];
            for (int i = 0; i < ABCDE.Length; i++)
            {
                ArrTemp = BitConverter.GetBytes(ABCDE[i]);
                for (int j = 0; j < 4; j++) { Answer[pos++] = ArrTemp[j]; }
            }
            return Answer;
        }
    }




    class CRSA
    {
        private static BigInteger QuickPower(BigInteger A, BigInteger B, BigInteger M)
        {
            BigInteger Answer = 1;
            while (B != 0)
            {
                while (B % 2 == 0)
                {
                    B = B / 2;
                    A = A * A % M;
                }
                Answer = (Answer * A) % M;
                B = B - 1;
            }
            return Answer;
        }


        public static BigInteger Euclides(BigInteger A, BigInteger B, out BigInteger x1, out BigInteger d1)
        {
            BigInteger d0 = A;
            d1 = B;
            BigInteger x0 = 1;
            x1 = 0;
            BigInteger y0 = 0;
            BigInteger y1 = 1;
            BigInteger d2;
            BigInteger q;
            BigInteger x2 = 0;
            BigInteger y2;
            while (d1 > 1)
            {
                q = d0 / d1;
                d2 = d0 % d1;
                x2 = x0 - q * x1;
                y2 = y0 - q * y1;
                d0 = d1; d1 = d2;
                x0 = x1; x1 = x2;
                y0 = y1; y1 = y2;
            }
            if (y1 < 0) { y1 = y1 + A; }
            return y1;
        }

        public static byte[] Code(byte[] Text, BigInteger key, BigInteger N, out BigInteger[] Arr)
        {
            BigInteger[] Temp = new BigInteger[Text.Length];
            for (int i = 0; i < Text.Length; i++) { Temp[i] = (BigInteger)Text[i]; }
            Temp = CodeEncode(Temp, key, N);
            if (Temp.Length >= 100) { Arr = new BigInteger[100]; } else { Arr = new BigInteger[Temp.Length]; }
            for (int i = 0; i < Arr.Length; i++) { Arr[i] = Temp[i]; }
            return BigIntToBytes(Temp, N);
        }

        public static byte[] Decode(byte[] Text, BigInteger key, BigInteger N, out BigInteger[] Arr)
        {
            BigInteger[] temp = CodeEncode(blocksToBigInt(Text, N), key, N);
            byte[] Answer = new byte[temp.Length];
            for (int i = 0; i < Answer.Length; i++) { Answer[i] = (byte)(temp[i] % 256); }
            if (temp.Length >= 100) { Arr = new BigInteger[100]; } else { Arr = new BigInteger[temp.Length]; }
            for (int i = 0; i < Arr.Length; i++) { Arr[i] = temp[i]; }
            return Answer;
        }

        private static BigInteger[] CodeEncode(BigInteger[] Text, BigInteger key, BigInteger N)
        {
            BigInteger[] Answer = new BigInteger[Text.Length];
            for (int i = 0; i < Text.Length; i++)
            {
                Answer[i] = QuickPower(Text[i], key, N);
            }
            return Answer;
        }


        private static BigInteger BigIntFromArrBytes(byte[] Arr)
        {
            BigInteger Answer = 0;
            BigInteger q = 1;
            for (int i = Arr.Length - 1; i >= 0; i--)
            {
                Answer += Arr[i] * q;
                q *= 256;
            }
            return Answer;
        }

        private static byte[] add(byte[] A, byte B)
        {
            Array.Resize(ref A, A.Length + 1);
            A[A.Length - 1] = B;
            return A;
        }


        private static BigInteger[] blocksToBigInt(byte[] Text, BigInteger N)
        {
            BigInteger[] Answer = new BigInteger[100000];
            int ansPos = 0; int i = 0; int j = 0;
            byte[] temp;
            byte[] Arr_N = BigIntToBytes(N);
            while (i < Text.Length)
            {
                j = 0;
                temp = new byte[Arr_N.Length];
                while ((i < Text.Length) && (j < Arr_N.Length))
                {
                    temp[j++] = Text[i++];
                }
                if (ansPos >= Answer.Length) { Array.Resize(ref Answer, Answer.Length + 100000); }
                Answer[ansPos++] = BigIntFromArrBytes(temp);
            }
            Array.Resize(ref Answer, ansPos);
            return Answer;
        }



        private static byte[] BigIntToBytes(BigInteger A)
        {
            byte[] Answer = new byte[0];

            while (A != 0)
            {
                Array.Resize(ref Answer, Answer.Length + 1);
                Answer[Answer.Length - 1] = (byte)(A % 256);
                A = A / 256;
            }
            return Answer;
        }


        private static byte[] BigIntToBytes(BigInteger[] Text, BigInteger N)
        {
            byte[] N_Arr = BigIntToBytes(N);
            byte[] Answer = new byte[Text.Length * N_Arr.Length];
            byte[] temp;
            int tempPos;
            int pos = 0;
            for (int i = 0; i < Text.Length; i++)
            {
                tempPos = N_Arr.Length - 1;
                temp = new byte[N_Arr.Length];
                while (Text[i] != 0)
                {
                    temp[tempPos--] = (byte)(Text[i] % 256);
                    Text[i] /= 256;
                }
                concat(ref Answer, temp, ref pos);
            }
            return Answer;
        }


        private static void concat(ref byte[] A, byte[] B, ref int pos)
        {
            if (pos + B.Length > A.Length) { Array.Resize(ref A, A.Length + 100000); }
            for (int i = 0; i < B.Length; i++) { A[i + pos] = B[i]; }
            pos += B.Length;
            return;
        }

        private static byte[] reverse(byte[] A)
        {
            byte[] Answer = new byte[A.Length];
            for (int i = 0; i < A.Length; i++) { Answer[i] = A[A.Length - i - 1]; }
            return Answer;
        }

    }




    class IOFile
    {
        public static byte[] readFile(String Src)
        {
            return File.ReadAllBytes(Src);
        }

        public static void writeFile(String Src, byte[] Text)
        {
            File.WriteAllBytes(Src, Text);
            return;
        }

    }

}
