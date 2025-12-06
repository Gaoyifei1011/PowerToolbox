namespace PowerToolbox.Extensions.Encrypt
{
    /// <summary>
    /// 凯撒密码对称加密实现
    /// </summary>
    public static class CaesarCipher
    {
        /// <summary>
        /// 凯撒密码加密
        /// </summary>
        public static string CaesarEncrypt(string input, int offset)
        {
            char[] charArray = input.ToCharArray();
            for (int i = 0; i < charArray.Length; i++)
            {
                char c = charArray[i];
                if (char.IsLetter(c))
                {
                    char offsetBase = char.IsUpper(c) ? 'A' : 'a';
                    charArray[i] = (char)((((c + offset) - offsetBase) % 26) + offsetBase);
                }
            }
            return new string(charArray);
        }

        /// <summary>
        /// 凯撒密码解密
        /// </summary>
        public static string CaesarDecrypt(string input, int offset)
        {
            return CaesarEncrypt(input, 26 - (offset % 26));
        }
    }
}
