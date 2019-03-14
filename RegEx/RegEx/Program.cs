using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegEx
{
    class Program
    {
        private static string alphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private static string symbols = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ_0123456789";
        private static string nv = "@#$";
        private static string[][] transp = new string[][] {new string[] {nv, alphabet, nv,  nv,  nv,       nv,  nv,       nv,  nv},
                                                           new string[] {nv, symbols,  "=:", nv,  nv,       nv,  nv,       nv,  nv},
                                                           new string[] {nv, nv,       nv,  "(", nv,       nv,  nv,       nv,  nv},
                                                           new string[] {nv, nv,       nv,  nv,  alphabet, nv,  nv,       nv,  nv},
                                                           new string[] {nv, nv,       nv,  nv,  symbols,  ",", nv,       nv,  nv},
                                                           new string[] {nv, nv,       nv,  nv,  nv,       nv,  alphabet, nv,  nv},
                                                           new string[] {nv, nv,       nv,  nv,  nv,       ",", symbols,  ")", nv},
                                                           new string[] {nv, nv,       nv,  nv,  nv,       nv,  nv,       nv,  ";"}};

        private static bool makeTransp (ref int step, char symb)
        {
            bool result = true;
            string[] forCheck = transp[step];
            for (int i = 0; i < forCheck.Length; i++)
                if (forCheck[i].Contains(symb))
                {
                    step = i;
                    if (step != 8)
                        result = false;
                    break;
                }
            return result;
        }

        static void Main(string[] args)
        {
            string userStr;
            int condition = 0;
            bool stop = false;
            int typeIdent = 1;

            Console.WriteLine("Введите строку объявления типа(или переменной типа) перечисления\nПример: TMyType = (first, second, third);\n\tMyVar:(first,second,third);");
            userStr = Console.ReadLine();
            userStr = userStr.Replace(" ", "");

            for (int i = 0; (i < userStr.Length) && (!stop); i++)
            {
                if (userStr[i] == '=')

                {
                    typeIdent = 0;
                }
                stop = makeTransp(ref condition, userStr[i]);
            }

            if (condition == 8)
            {
                Console.WriteLine("Объявление составлено верно!");
                if (typeIdent == 0)
                {
                    Console.WriteLine("Вы объявили тип перечисление.");
                }
                else
                {
                    Console.WriteLine("Вы объявили переменную типа перечисление.");
                }
            }
            else
            {
                Console.WriteLine("Объявление составлено неверно!");
            }

            Console.ReadKey();
        }
    }
}
