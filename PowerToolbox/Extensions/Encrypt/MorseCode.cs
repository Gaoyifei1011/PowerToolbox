using System.Collections.Generic;
using System.Linq;

namespace PowerToolbox.Extensions.Encrypt
{
    /// <summary>
    /// 摩尔斯密码加密算法实现
    /// </summary>
    internal static class MorseCode
    {
        private static readonly Dictionary<char, string> morseCharDict = new()
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

        private static readonly Dictionary<string, char> morseToChar = morseCharDict.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);

        /// <summary>
        /// 加密
        /// </summary>
        internal static string MorseEncode(string encodeText)
        {
            if (string.IsNullOrEmpty(encodeText))
            {
                return default;
            }

            string encoded = string.Empty;
            foreach (char c in encodeText)
            {
                if (morseCharDict.TryGetValue(c, out string value))
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
        /// 解密
        /// </summary>
        internal static string MorseDecode(string morseCode)
        {
            if (string.IsNullOrEmpty(morseCode))
            {
                return default;
            }

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
