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
    public class TripsController : ApiController
    {
        private DogoDbEntitiesConnection db = new DogoDbEntitiesConnection();

        // GET: api/Trips
        public IQueryable<Trip> GetTrips()
        {
            var result = db.Trips.ToList();
            if (Request.GetQueryNameValuePairs().Any(pair => pair.Key == "walkerId"))
            {
                var id = Request.GetQueryNameValuePairs().First(pair => pair.Key == "walkerId").Value;
                result = db.Trips.Where(trip => trip.dogWalkerId.ToString() == id).ToList();
            }

            if (Request.GetQueryNameValuePairs().Any(pair => pair.Key == "ownerId"))
            {
                var id = Request.GetQueryNameValuePairs().First(pair => pair.Key == "ownerId").Value;
                result = db.Trips.Where(trip => trip.dogOwnerId.ToString() == id).ToList();
            }

            return result.AsQueryable();
        }

        // GET: api/Trips/5
        [ResponseType(typeof(Trip))]
        public IHttpActionResult GetTrip(int id)
        {
            Trip trip = db.Trips.Find(id);
            if (trip == null)
            {
                return NotFound();
            }

            return Ok(trip);
        }

        // PUT: api/Trips/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutTrip(int id, Trip trip)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != trip.id)
            {
                return BadRequest();
            }

            db.Entry(trip).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TripExists(id))
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

        // POST: api/Trips
        [ResponseType(typeof(Trip))]
        public IHttpActionResult PostTrip(Trip trip)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Trips.Add(trip);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (TripExists(trip.id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = trip.id }, trip);
        }

        // DELETE: api/Trips/5
        [ResponseType(typeof(Trip))]
        public IHttpActionResult DeleteTrip(int id)
        {
            Trip trip = db.Trips.Find(id);
            if (trip == null)
            {
                return NotFound();
            }

            db.Trips.Remove(trip);
            db.SaveChanges();

            return Ok(trip);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool TripExists(int id)
        {
            return db.Trips.Count(e => e.id == id) > 0;
        }
    }
}