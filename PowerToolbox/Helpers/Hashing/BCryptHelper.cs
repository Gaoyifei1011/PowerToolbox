using PowerToolbox.Services.Root;
using PowerToolbox.WindowsAPI.PInvoke.Bcrypt;
using System;
using System.Diagnostics;
using System.IO;

// 抑制 CA1806 警告
#pragma warning disable CA1806,CA2022

namespace PowerToolbox.Helpers.Hashing
{
    public static class BCryptHelper
    {
        private const string BCRYPT_MD2_ALGORITHM = "MD2";
        private const string BCRYPT_MD4_ALGORITHM = "MD4";

        /// <summary>
        /// 计算字符串内容的 MD2 哈希值
        /// </summary>
        public static string ComputeMD2Hash(byte[] data)
        {
            string md2Hash = string.Empty;

            try
            {
                if (BcryptLibrary.BCryptOpenAlgorithmProvider(out nint algorithmHandle, BCRYPT_MD2_ALGORITHM, null, 0) is 0)
                {
                    if (BcryptLibrary.BCryptCreateHash(algorithmHandle, out nint hashHandle, 0, 0, 0, 0, 0) is 0)
                    {
                        if (BcryptLibrary.BCryptHashData(hashHandle, data, data.Length, 0) is 0)
                        {
                            byte[] hash = new byte[16];
                            if (BcryptLibrary.BCryptFinishHash(hashHandle, hash, hash.Length, 0) is 0)
                            {
                                md2Hash = BitConverter.ToString(hash).Replace("-", "").ToLower();
                            }
                        }
                    }
                    BcryptLibrary.BCryptDestroyHash(hashHandle);
                }
                BcryptLibrary.BCryptCloseAlgorithmProvider(algorithmHandle, 0);
            }
            catch (Exception e)
            {
                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(BCryptHelper), nameof(ComputeMD2Hash), 1, e);
            }

            return md2Hash;
        }

        /// <summary>
        /// 计算文件流的 MD2 哈希值
        /// </summary>
        public static string ComputeMD2Hash(Stream stream)
        {
            string md2Hash = string.Empty;

            try
            {
                if (BcryptLibrary.BCryptOpenAlgorithmProvider(out nint algorithmHandle, BCRYPT_MD2_ALGORITHM, null, 0) is 0)
                {
                    if (BcryptLibrary.BCryptCreateHash(algorithmHandle, out nint hashHandle, 0, 0, 0, 0, 0) is 0)
                    {
                        byte[] data = new byte[stream.Length];
                        stream.Read(data, 0, data.Length);
                        stream.Seek(0, SeekOrigin.Begin);
                        if (BcryptLibrary.BCryptHashData(hashHandle, data, data.Length, 0) is 0)
                        {
                            byte[] hash = new byte[16];
                            if (BcryptLibrary.BCryptFinishHash(hashHandle, hash, hash.Length, 0) is 0)
                            {
                                md2Hash = BitConverter.ToString(hash).Replace("-", "").ToLower();
                            }
                        }
                    }
                    BcryptLibrary.BCryptDestroyHash(hashHandle);
                }
                BcryptLibrary.BCryptCloseAlgorithmProvider(algorithmHandle, 0);
            }
            catch (Exception e)
            {
                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(BCryptHelper), nameof(ComputeMD2Hash), 2, e);
            }

            return md2Hash;
        }

        /// <summary>
        /// 计算字符串内容的 MD4 哈希值
        /// </summary>
        public static string ComputeMD4Hash(byte[] data)
        {
            string md4Hash = string.Empty;

            try
            {
                if (BcryptLibrary.BCryptOpenAlgorithmProvider(out nint algorithmHandle, BCRYPT_MD4_ALGORITHM, null, 0) is 0)
                {
                    if (BcryptLibrary.BCryptCreateHash(algorithmHandle, out nint hashHandle, 0, 0, 0, 0, 0) is 0)
                    {
                        if (BcryptLibrary.BCryptHashData(hashHandle, data, data.Length, 0) is 0)
                        {
                            byte[] hash = new byte[16];
                            if (BcryptLibrary.BCryptFinishHash(hashHandle, hash, hash.Length, 0) is 0)
                            {
                                md4Hash = BitConverter.ToString(hash).Replace("-", "").ToLower();
                            }
                        }
                    }
                    BcryptLibrary.BCryptDestroyHash(hashHandle);
                }
                BcryptLibrary.BCryptCloseAlgorithmProvider(algorithmHandle, 0);
            }
            catch (Exception e)
            {
                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(BCryptHelper), nameof(ComputeMD4Hash), 1, e);
            }

            return md4Hash;
        }

        /// <summary>
        /// 计算文件流的 MD4 哈希值
        /// </summary>
        public static string ComputeMD4Hash(Stream stream)
        {
            string md4Hash = string.Empty;

            try
            {
                if (BcryptLibrary.BCryptOpenAlgorithmProvider(out nint algorithmHandle, BCRYPT_MD4_ALGORITHM, null, 0) is 0)
                {
                    if (BcryptLibrary.BCryptCreateHash(algorithmHandle, out nint hashHandle, 0, 0, 0, 0, 0) is 0)
                    {
                        byte[] data = new byte[stream.Length];
                        stream.Read(data, 0, data.Length);
                        stream.Seek(0, SeekOrigin.Begin);
                        if (BcryptLibrary.BCryptHashData(hashHandle, data, data.Length, 0) is 0)
                        {
                            byte[] hash = new byte[16];
                            if (BcryptLibrary.BCryptFinishHash(hashHandle, hash, hash.Length, 0) is 0)
                            {
                                md4Hash = BitConverter.ToString(hash).Replace("-", "").ToLower();
                            }
                        }
                    }
                    BcryptLibrary.BCryptDestroyHash(hashHandle);
                }
                BcryptLibrary.BCryptCloseAlgorithmProvider(algorithmHandle, 0);
            }
            catch (Exception e)
            {
                LogService.WriteLog(TraceEventType.Error, nameof(PowerToolbox), nameof(BCryptHelper), nameof(ComputeMD4Hash), 2, e);
            }

            return md4Hash;
        }
    }
}
