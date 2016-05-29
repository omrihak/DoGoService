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
    public class DogWalkersController : ApiController
    {
        private DogoDbEntitiesConnection db = new DogoDbEntitiesConnection();

        // GET: api/DogWalkers
        public IQueryable<DogWalker> GetDogWalkers()
        {
            var result = db.DogWalkers.ToList();
            if (Request.GetQueryNameValuePairs().Any(pair => pair.Key == "userName"))
            {
                var name = Request.GetQueryNameValuePairs().First(pair => pair.Key == "userName").Value;
                result = db.DogWalkers.Where(walker => walker.userName == name).ToList();
            }

            return result.AsQueryable();
        }

        // GET: api/DogWalkers/5
        [ResponseType(typeof(DogWalker))]
        public IHttpActionResult GetDogWalker(int id)
        {
            DogWalker dogWalker = db.DogWalkers.Find(id);
            if (dogWalker == null)
            {
                return NotFound();
            }

            return Ok(dogWalker);
        }

        // PUT: api/DogWalkers/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutDogWalker(int id, DogWalker dogWalker)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != dogWalker.id)
            {
                return BadRequest();
            }

            db.Entry(dogWalker).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DogWalkerExists(id))
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

        // POST: api/DogWalkers
        [ResponseType(typeof(DogWalker))]
        public IHttpActionResult PostDogWalker(DogWalker dogWalker)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.DogWalkers.Add(dogWalker);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (DogWalkerExists(dogWalker.id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = dogWalker.id }, dogWalker);
        }

        // DELETE: api/DogWalkers/5
        [ResponseType(typeof(DogWalker))]
        public IHttpActionResult DeleteDogWalker(int id)
        {
            DogWalker dogWalker = db.DogWalkers.Find(id);
            if (dogWalker == null)
            {
                return NotFound();
            }

            db.DogWalkers.Remove(dogWalker);
            db.SaveChanges();

            return Ok(dogWalker);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool DogWalkerExists(int id)
        {
            return db.DogWalkers.Count(e => e.id == id) > 0;
        }
    }
}