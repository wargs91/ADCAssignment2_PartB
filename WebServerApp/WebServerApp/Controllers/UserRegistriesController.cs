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
    public class UserRegistriesController : ApiController
    {
        private WebServerDbEntities db = new WebServerDbEntities();

        // GET: api/UserRegistries
        public IQueryable<UserRegistry> GetUserRegistries()
        {
            return db.UserRegistries;
        }

        // GET: api/UserRegistries/5
        [ResponseType(typeof(UserRegistry))]
        public IHttpActionResult GetUserRegistry(int id)
        {
            UserRegistry userRegistry = db.UserRegistries.Find(id);
            if (userRegistry == null)
            {
                return NotFound();
            }

            return Ok(userRegistry);
        }

        // PUT: api/UserRegistries/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutUserRegistry(int id, UserRegistry userRegistry)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != userRegistry.Id)
            {
                return BadRequest();
            }

            db.Entry(userRegistry).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserRegistryExists(id))
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

        // POST: api/UserRegistries
        [ResponseType(typeof(UserRegistry))]
        public IHttpActionResult PostUserRegistry(UserRegistry userRegistry)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.UserRegistries.Add(userRegistry);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (UserRegistryExists(userRegistry.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = userRegistry.Id }, userRegistry);
        }

        // DELETE: api/UserRegistries/5
        [ResponseType(typeof(UserRegistry))]
        public IHttpActionResult DeleteUserRegistry(int id)
        {
            UserRegistry userRegistry = db.UserRegistries.Find(id);
            if (userRegistry == null)
            {
                return NotFound();
            }

            db.UserRegistries.Remove(userRegistry);
            db.SaveChanges();

            return Ok(userRegistry);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool UserRegistryExists(int id)
        {
            return db.UserRegistries.Count(e => e.Id == id) > 0;
        }
    }
}