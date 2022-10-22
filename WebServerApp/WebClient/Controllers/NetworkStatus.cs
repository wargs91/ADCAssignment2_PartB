using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebClient.Models;

namespace WebClient.Controllers
{
    public class NetworkStatus : Controller
    {
        
        // GET: NetworkStatus
        public ActionResult Index()
        {

            return View();
        }

        // GET: NetworkStatus/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: NetworkStatus/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Home/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(NetworkStatus networkStatus)
        {
            NetworkStatusClass newNetworkStatus = new NetworkStatusClass();
            newNetworkStatus.clientIP = "123";
            newNetworkStatus.clientPort = "123456";
            newNetworkStatus.status = "Currently Working on job";
            newNetworkStatus.jobComplete = 5;


            //newNetworkStatus = networkStatus;
            ViewBag.Message = newNetworkStatus;
            return View(); 
        }

        // GET: NetworkStatus/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: NetworkStatus/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: NetworkStatus/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: NetworkStatus/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
