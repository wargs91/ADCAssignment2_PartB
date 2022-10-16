using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestSharp;
using System.ServiceModel;
using WebServerApp.Models;

namespace ClientGUI
{
    internal class Server
    {
        static void Main(string[] args)
        {

            RestClient restClient = new RestClient("http://localhost:49901/");
            RestRequest restRequest = new RestRequest("api/UserRegistries", Method.Get);
            RestResponse restResponse = restClient.Get(restRequest);
            List<UserRegistry> userRegistries = JsonConvert.DeserializeObject<List<UserRegistry>>(restResponse.Content);


            Random rnd = new Random();
            int length = userRegistries.Count;
            rnd = rnd.Next(maxValue: length);//how do I convert this to work?
            for (int i = rnd; i < userRegistries.Count; i++)
            {
                string IPAddress = userRegistries[i].IPAddress;
                string Port = userRegistries[i].Port;
                string url = "net.tcp:" + IPAddress + ":" + Port;
                ServiceHost host;
                //This represents a tcp/ip binding in the Windows network stack
                NetTcpBinding tcp = new NetTcpBinding();
                //Bind server to the implementation of DataServer
                host = new ServiceHost(typeof(PythonImplementation));
                host.AddServiceEndpoint(typeof(ServerInterface), tcp, url);
                //And open the host for business!
                host.Open();
            }
        }
    }
}
