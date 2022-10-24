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

            ViewBag.Title = "Ubiquity Network";


            RestClient networkRestClient = new RestClient("http://localhost:49901/");
            RestRequest networkRestRequest = new RestRequest("api/NetworkStatusTables", Method.Get);
            RestResponse networkRestResponse = networkRestClient.Get(networkRestRequest);

            List<NetworkStatusTable> newNetworkStatus = JsonConvert.DeserializeObject<List<NetworkStatusTable>>(networkRestResponse.Content);

            
            return View(newNetworkStatus);

        }
        
    }
}
