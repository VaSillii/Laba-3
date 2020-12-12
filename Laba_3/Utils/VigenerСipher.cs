using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Laba_3.Utils
{
    public class VigenerСipher
    {
        private static char[] alphabet; 

        static VigenerСipher()
        {
            alphabet = new char[] {
                'А','Б','В','Г','Д','Е','Ё','Ж',
                'З','И','Й','К','Л','М','Н','О',
                'П','Р','С','Т','У','Ф','Х','Ц',
                'Ч','Ш','Щ','Ъ','Ы','Ь','Э','Ю','Я'};
        }

        private static string VigenerRealization(string text, string key, bool encryptingFlag = true)
        {
            key = key.ToUpper();

            string message = "";
            int indexKey = 0;

            foreach (var item in text)
            {
                char upp_char = Convert.ToChar(item.ToString().ToUpper());
                if (!Array.Exists(alphabet, elm => elm == upp_char))
                {
                    message += item;
                    continue;
                }

                int indexAlphabet;
                bool flagRegistr = Char.IsLower(item);

                if (encryptingFlag)
                {
                    indexAlphabet = (Array.IndexOf(alphabet, upp_char) + alphabet.Length -
                        Array.IndexOf(alphabet, key[indexKey])) % alphabet.Length;
                }
                else
                {
                    indexAlphabet = (Array.IndexOf(alphabet, upp_char) +
                        Array.IndexOf(alphabet, key[indexKey])) % alphabet.Length;
                }

                message += flagRegistr ? alphabet[indexAlphabet].ToString().ToLower() : alphabet[indexAlphabet].ToString();
                indexKey += 1;

                if (indexKey == key.Length)
                {
                    indexKey = 0;
                }
            }
            return message;
        }

        private static string Encrypt(string text, string key) => VigenerRealization(text, key);
        private static string Decrypt(string text, string key) => VigenerRealization(text, key, false);

        public static string GetDataCipher(bool flagAction, string text, string key)
        {
            if (flagAction)
            {
                return VigenerСipher.Encrypt(text, key);
            }
            else
            {
                return VigenerСipher.Decrypt(text, key);
            }
        }
    }
}
