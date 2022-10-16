﻿using Microsoft.AspNetCore.Mvc;
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

            RestClient restClient = new RestClient("http://localhost:49901/");
            RestRequest restRequest = new RestRequest("api/UserRegistries", Method.Get);
            RestResponse restResponse = restClient.Get(restRequest);

            List<UserRegistry> userRegistries = JsonConvert.DeserializeObject<List<UserRegistry>>(restResponse.Content);

            return View(userRegistries);
        }
        [HttpPost]
        public IActionResult SubmitUser([FromBody] UserRegistry newUser)
        {
            RestClient restClient = new RestClient("http://localhost:49901/");
            RestRequest restRequest = new RestRequest("api/UserRegistries", Method.Post);
            restRequest.AddJsonBody(JsonConvert.SerializeObject(newUser));
            RestResponse restResponse = restClient.Execute(restRequest);

            UserRegistry returnUser = JsonConvert.DeserializeObject<UserRegistry>(restResponse.Content);
            if (returnUser != null)
            {
                return Ok(returnUser);
            }
            else
            {
                return BadRequest();
            }
        }
    }
}