using RestSharp;
using System;
using System.IO;
using System.Security.Cryptography;

namespace AgriStockApp.Scripts
{
    public static class Fonctions
    {
        private const int BUF_SIZE = 65536;

        //Get Farming Simulator Server JSON Response
        public static string getServerData(string url)
        {
            var client = new RestClient(url);
            client.Timeout = 10000;
            var request = new RestRequest(Method.GET);
            request.AddHeader("cache-control", "no-cache");
            IRestResponse response = client.Execute(request);

            if ((int)response.StatusCode != 200) { return "error"; }
            return response.Content;
        }

        //Get Farming Simulator Server XML Response to JSON
        public static string getServerData_XML_to_JSON(string url)
        {
            var client = new RestClient(url);
            client.Timeout = 10000;
            var request = new RestRequest(Method.GET);
            request.AddHeader("cache-control", "no-cache");
            IRestResponse response = client.Execute(request);

            if ((int)response.StatusCode != 200) { return "error"; }

            return XMLConverter.XmlToJSON(response.Content.ToString());
        }

        //Get Local Mod Hash
        public static string GetModHash(string filePath)
        {
            byte[] dataBuffer = new byte[BUF_SIZE];
            byte[] dataBufferDummy = new byte[BUF_SIZE];
            int dataBytesRead = 0;
            string hashResult = string.Empty;
            HashAlgorithm hashAlg = null;
            FileStream fs = null;

            try
            {
                hashAlg = new MD5CryptoServiceProvider();
                fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.None, BUF_SIZE);
                do
                {
                    dataBytesRead = fs.Read(dataBuffer, 0, BUF_SIZE);
                    hashAlg.TransformBlock(dataBuffer, 0, dataBytesRead, dataBufferDummy, 0);
                }
                while (dataBytesRead != 0);

                byte[] nameBuffer = System.Text.Encoding.ASCII.GetBytes(Path.GetFileNameWithoutExtension(filePath));
                hashAlg.TransformFinalBlock(nameBuffer, 0, nameBuffer.Length);

                hashResult = BitConverter.ToString(hashAlg.Hash).Replace("-", "").ToLowerInvariant();
            }
            catch
            {
                hashResult = "0x";
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                    fs.Dispose();
                }
                if (hashAlg != null)
                {
                    hashAlg.Clear();
                }
            }
            return hashResult;
        }
    }
}
