using System.Collections.Generic;

namespace PowerToolbox.Extensions.Encrypt
{
    /// <summary>
    /// 摩尔斯密码加密算法实现
    /// </summary>
    public static class MorseCode
    {
        private static readonly Dictionary<char, string> charToMorse = new()
        {
            {'A', ".-"}, {'B', "-..."}, {'C', "-.-."}, {'D', "-.."}, {'E', "."},
            {'F', "..-."}, {'G', "--."}, {'H', "...."}, {'I', ".."}, {'J', ".---"},
            {'K', "-.-"}, {'L', ".-.."}, {'M', "--"}, {'N', "-."}, {'O', "---"},
            {'P', ".--."}, {'Q', "--.-"}, {'R', ".-."}, {'S', "..."}, {'T', "-"},
            {'U', "..-"}, {'V', "...-"}, {'W', ".--"}, {'X', "-..-"}, {'Y', "-.--"},
            {'Z', "--.."}, {'0', "-----"}, {'1', ".----"}, {'2', "..---"}, {'3', "...--"},
            {'4', "....-"}, {'5', "....."}, {'6', "-...."}, {'7', "--..."}, {'8', "---.."},
            {'9', "----."}
        };

        private static readonly Dictionary<string, char> morseToChar = [];

        static MorseCode()
        {
            foreach (KeyValuePair<char, string> pair in charToMorse)
            {
                morseToChar[pair.Value] = pair.Key;
            }
        }

        /// <summary>
        /// 摩尔斯密码加密
        /// </summary>
        public static string MorseEncode(string encodeText)
        {
            string encoded = string.Empty;
            foreach (char c in encodeText)
            {
                if (charToMorse.TryGetValue(c, out string value))
                {
                    encoded += value + " ";
                }
                else
                {
                    encoded += " ";
                }
            }
            return encoded.Trim();
        }

        /// <summary>
        /// 摩尔斯密码解密
        /// </summary>
        public static string MorseDecode(string morseCode)
        {
            string[] words = morseCode.Split(' ');
            string decoded = string.Empty;
            foreach (string word in words)
            {
                string[] letters = word.Split(' ');
                foreach (string letter in letters)
                {
                    if (morseToChar.TryGetValue(letter, out char value))
                    {
                        decoded += value;
                    }
                }
            }
            return decoded.Trim();
        }
    }
}
