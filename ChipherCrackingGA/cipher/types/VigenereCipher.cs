using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChipherCrackingGA.Cipher.Types
{
    /// <summary>
    /// An implementation of the well known polyalphabetic <see href="https://en.wikipedia.org/wiki/Vigen%C3%A8re_cipher">Vigenere Cipher</see>.
    /// </summary>
    public class VigenereCipher : ICipher
    {
        //
        public string PlainText { get; set; }
        public string CipherText { get; set; }
        public string AttemptedDecipherText { get; set; }

        public VigenereCipher(string plainText)
        {
            this.PlainText = plainText;
        }

        public string Encipher(string key)
        {
            this.CipherText = Cipher(PlainText, key, true);
            return this.CipherText;
        }

        public string Decipher(string key)
        {
            this.AttemptedDecipherText = Cipher(CipherText, key, false);
            return this.AttemptedDecipherText;
        }

        private int Mod(int a, int b)
        {
            return (a % b + b) % b;
        }

        private string Cipher(string input, string key, bool encipher)
        {
            for (int i = 0; i < key.Length; ++i)
                if (!char.IsLetter(key[i]))
                    return null; // Error

            string output = string.Empty;
            int nonAlphaCharCount = 0;

            for (int i = 0; i < input.Length; ++i)
            {
                if (char.IsLetter(input[i]))
                {
                    bool cIsUpper = char.IsUpper(input[i]);
                    char offset = cIsUpper ? 'A' : 'a';
                    int keyIndex = (i - nonAlphaCharCount) % key.Length;
                    int k = (cIsUpper ? char.ToUpper(key[keyIndex]) : char.ToLower(key[keyIndex])) - offset;
                    k = encipher ? k : -k;
                    char ch = (char)((Mod(((input[i] + k) - offset), 26)) + offset);
                    output += ch;
                }
                else
                {
                    output += input[i];
                    ++nonAlphaCharCount;
                }
            }
            return output;
        }
    }
}
