using System.Web.Http;
using System.Web.Http.Description;

namespace WebServerApp.Controllers
{
    public class NetworkStatusController : ApiController
    {
        NetworkStatusClass networkStatus = new NetworkStatusClass();
        // POST: api/NetworkStatus
        [ResponseType(typeof(NetworkStatusClass))]
        public IHttpActionResult PostStatus(NetworkStatusClass postedStatus)
        {
            networkStatus = postedStatus;
            return Ok(postedStatus);
        }
        //Get
        [ResponseType(typeof(NetworkStatusClass))]
        public IHttpActionResult GetUserRegistry(int id)
        {
            return Ok(networkStatus);
        }

    }
}
