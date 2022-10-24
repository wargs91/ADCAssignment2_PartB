using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using WebServerApp.Models;

namespace WebServerApp.Controllers
{
    public class NetworkStatusTablesController : ApiController
    {
        private WebServerDbEntities1 db = new WebServerDbEntities1();

        // GET: api/NetworkStatusTables
        public IQueryable<NetworkStatusTable> GetNetworkStatusTables()
        {
            return db.NetworkStatusTables;
        }

        // GET: api/NetworkStatusTables/5
        [ResponseType(typeof(NetworkStatusTable))]
        public IHttpActionResult GetNetworkStatusTable(int id)
        {
            NetworkStatusTable networkStatusTable = db.NetworkStatusTables.Find(id);
            if (networkStatusTable == null)
            {
                return NotFound();
            }

            return Ok(networkStatusTable);
        }

        // PUT: api/NetworkStatusTables/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutNetworkStatusTable(int id, NetworkStatusTable networkStatusTable)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != networkStatusTable.Id)
            {
                return BadRequest();
            }

            db.Entry(networkStatusTable).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NetworkStatusTableExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/NetworkStatusTables
        [ResponseType(typeof(NetworkStatusTable))]
        public IHttpActionResult PostNetworkStatusTable(NetworkStatusTable networkStatusTable)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.NetworkStatusTables.Add(networkStatusTable);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (NetworkStatusTableExists(networkStatusTable.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = networkStatusTable.Id }, networkStatusTable);
        }

        // DELETE: api/NetworkStatusTables/5
        [ResponseType(typeof(NetworkStatusTable))]
        public IHttpActionResult DeleteNetworkStatusTable(int id)
        {
            NetworkStatusTable networkStatusTable = db.NetworkStatusTables.Find(id);
            if (networkStatusTable == null)
            {
                return NotFound();
            }

            db.NetworkStatusTables.Remove(networkStatusTable);
            db.SaveChanges();

            return Ok(networkStatusTable);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool NetworkStatusTableExists(int id)
        {
            return db.NetworkStatusTables.Count(e => e.Id == id) > 0;
        }
    }
}