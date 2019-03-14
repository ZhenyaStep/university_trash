using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;

namespace Курсовой.classes
{
    public static class FileWork
    {
        public static string path = Directory.GetCurrentDirectory();

        public static bool SaveClientInfo(string login, byte[] Password)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(Path.Combine(path, login + ".cfg"), false, Encoding.Unicode))
                {
                    writer.WriteLine(login);
                    byte[] HashPassword = getSHA1(Password);
                    writer.WriteLine(Encoding.Unicode.GetString(HashPassword));
                }
            }
            catch(Exception ex)
            {
                return false;
            }
            return true;
        }

        public static bool Deletefile(string fileName)
        {
            try
            {
                if (!File.Exists(fileName + ".cfg"))
                {
                    return false;
                }
                File.Delete(fileName + ".cfg");
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        public static byte[] GetClientPassword(string login)
        {
            byte[] Password = new byte[SHA1.Create().HashSize];
            try
            {
                using (StreamReader reader = new StreamReader(Path.Combine(path, login + ".cfg"), Encoding.Unicode))
                {
                    reader.ReadLine();
                    return Password = Encoding.Unicode.GetBytes(reader.ReadLine());
                }
            }
            catch(Exception ex)
            {
                return Password;
            }
        }

        public static bool CheckLoginExistence(string Login)
        {
            return File.Exists(Path.Combine(path, Login + ".cfg"));
        }


        public static byte[] getSHA1(byte[] Password)
        {
            SHA1 sha = SHA1.Create();
            byte[] HashPassword = sha.ComputeHash(Password);
            return HashPassword;
        }


    }
}
