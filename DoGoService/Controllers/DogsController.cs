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
    public class DogsController : ApiController
    {
        private DogoDbEntitiesConnection db = new DogoDbEntitiesConnection();

        // GET: api/Dogs
        public IQueryable<Dog> GetDogs()
        {
            return db.Dogs;
        }

        // GET: api/Dogs/5
        [ResponseType(typeof(Dog))]
        public IHttpActionResult GetDog(int id)
        {
            Dog dog = db.Dogs.Find(id);
            if (dog == null)
            {
                return NotFound();
            }

            return Ok(dog);
        }

        // PUT: api/Dogs/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutDog(int id, Dog dog)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != dog.userId)
            {
                return BadRequest();
            }

            db.Entry(dog).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DogExists(id))
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

        // POST: api/Dogs
        [ResponseType(typeof(Dog))]
        public IHttpActionResult PostDog(Dog dog)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Dogs.Add(dog);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (DogExists(dog.userId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = dog.userId }, dog);
        }

        // DELETE: api/Dogs/5
        [ResponseType(typeof(Dog))]
        public IHttpActionResult DeleteDog(int id)
        {
            Dog dog = db.Dogs.Find(id);
            if (dog == null)
            {
                return NotFound();
            }

            db.Dogs.Remove(dog);
            db.SaveChanges();

            return Ok(dog);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool DogExists(int id)
        {
            return db.Dogs.Count(e => e.userId == id) > 0;
        }
    }
}