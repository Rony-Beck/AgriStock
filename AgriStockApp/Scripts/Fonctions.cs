using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace AgriStockApp.Scripts
{
    public static class Fonctions
    {
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
    }
}
