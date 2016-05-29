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
    public class DogOwnersController : ApiController
    {
        private DogoDbEntitiesConnection db = new DogoDbEntitiesConnection();

        // GET: api/DogOwners
        public IQueryable<DogOwner> GetDogOwners()
        {
            var result = db.DogOwners.ToList();
            if (Request.GetQueryNameValuePairs().Any(pair => pair.Key == "userName"))
            {
                var name = Request.GetQueryNameValuePairs().First(pair => pair.Key == "userName").Value;
                result = db.DogOwners.Where(owner => owner.userName == name).ToList();
            }

            if (Request.GetQueryNameValuePairs().Any(pair => pair.Key == "connectedTo"))
            {
                var walkerId = Request.GetQueryNameValuePairs().First(pair => pair.Key == "connectedTo").Value;
                result = db.DogOwners.Where(owner => db.UserRequests.Any(request => request.Status == "Accepted"
                                                                                 && ((request.RequestedUserId.ToString() == walkerId && request.RequestingUserId == owner.id)
                                                                                 || (request.RequestedUserId == owner.id && request.RequestingUserId.ToString() == walkerId)))).ToList();
            }

            return result.AsQueryable();
        }

        // GET: api/DogOwners/5
        [ResponseType(typeof(DogOwner))]
        public IHttpActionResult GetDogOwner(int id)
        {
            DogOwner dogOwner = db.DogOwners.Find(id);
            if (dogOwner == null)
            {
                return NotFound();
            }

            return Ok(dogOwner);
        }

        // PUT: api/DogOwners/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutDogOwner(int id, DogOwner dogOwner)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != dogOwner.id)
            {
                return BadRequest();
            }

            db.Entry(dogOwner).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DogOwnerExists(id))
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

        // POST: api/DogOwners
        [ResponseType(typeof(DogOwner))]
        public IHttpActionResult PostDogOwner(DogOwner dogOwner)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.DogOwners.Add(dogOwner);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (DogOwnerExists(dogOwner.id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = dogOwner.id }, dogOwner);
        }

        // DELETE: api/DogOwners/5
        [ResponseType(typeof(DogOwner))]
        public IHttpActionResult DeleteDogOwner(int id)
        {
            DogOwner dogOwner = db.DogOwners.Find(id);
            if (dogOwner == null)
            {
                return NotFound();
            }

            db.DogOwners.Remove(dogOwner);
            db.SaveChanges();

            return Ok(dogOwner);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool DogOwnerExists(int id)
        {
            return db.DogOwners.Count(e => e.id == id) > 0;
        }
    }
}