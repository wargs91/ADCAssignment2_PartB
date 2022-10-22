using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;
using WebClient.Models;

namespace WebClient.Controllers
{
    public class HomeController : Controller
    {
        
        public IActionResult Index()
        {
            NetworkStatusClass newNetworkStatus = new NetworkStatusClass();
            newNetworkStatus.clientIP = "Wating for Client";
            newNetworkStatus.clientPort = "Waiting for Client";
            newNetworkStatus.status = "Waiting for Client";
            newNetworkStatus.jobComplete = 0;

            bool open = true;
            //newNetworkStatus = networkStatus;
            

            ViewBag.Title = "Ubiquity Network";

            RestClient restClient = new RestClient("http://localhost:49901/");
            RestRequest restRequest = new RestRequest("api/UserRegistries", Method.Get);
            RestResponse restResponse = restClient.Get(restRequest);

            List<UserRegistry> userRegistries = JsonConvert.DeserializeObject<List<UserRegistry>>(restResponse.Content);

            while (open == true)
            {
                RestClient statusRestClient = new RestClient("http://localhost:49901/");
                RestRequest statusRestRequest = new RestRequest("api/NetworkStatus", Method.Get);
                RestResponse statusRestResponse = restClient.Get(restRequest);

                NetworkStatusClass serverNetworkStatus = JsonConvert.DeserializeObject<NetworkStatusClass>(statusRestResponse.Content);

                if (serverNetworkStatus != null)
                    newNetworkStatus = serverNetworkStatus;
                Thread.Sleep(6000);
            }
            
            ViewBag.Message = newNetworkStatus;
            return View(userRegistries);

        }
        
    }
}
