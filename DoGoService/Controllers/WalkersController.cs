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
using DoGoService.DataObjects;

namespace DoGoService.Controllers
{
    [AllowAnonymous]
    public class WalkersController : ApiController
    {
        private DogoDbEntities db = new DogoDbEntities();

        // GET: api/Walkers
        public IQueryable<Walker> GetWalkers()
        {
            return db.Walkers;
        }

        // GET: api/Walkers/5
        [ResponseType(typeof(Walker))]
        public IHttpActionResult GetWalker(int id)
        {
            Walker walker = db.Walkers.Find(id);
            if (walker == null)
            {
                return NotFound();
            }

            return Ok(walker);
        }

        // PUT: api/Walkers/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutWalker(int id, Walker walker)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != walker.UserId)
            {
                return BadRequest();
            }

            db.Entry(walker).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WalkerExists(id))
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

        // POST: api/Walkers
        [ResponseType(typeof(Walker))]
        public IHttpActionResult PostWalker(Walker walker)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Walkers.Add(walker);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (WalkerExists(walker.UserId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = walker.UserId }, walker);
        }

        // DELETE: api/Walkers/5
        [ResponseType(typeof(Walker))]
        public IHttpActionResult DeleteWalker(int id)
        {
            Walker walker = db.Walkers.Find(id);
            if (walker == null)
            {
                return NotFound();
            }

            db.Walkers.Remove(walker);
            db.SaveChanges();

            return Ok(walker);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool WalkerExists(int id)
        {
            return db.Walkers.Count(e => e.UserId == id) > 0;
        }
    }
}