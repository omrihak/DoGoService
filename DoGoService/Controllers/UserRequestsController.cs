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
    public class UserRequestsController : ApiController
    {
        private DogoDbEntities db = new DogoDbEntities();

        // GET: api/UserRequests
        public IQueryable<UserRequest> GetUserRequests()
        {
            return db.UserRequests;
        }

        // GET: api/UserRequests/5
        [ResponseType(typeof(UserRequest))]
        public IHttpActionResult GetUserRequest(int id)
        {
            UserRequest userRequest = db.UserRequests.Find(id);
            if (userRequest == null)
            {
                return NotFound();
            }

            return Ok(userRequest);
        }

        // PUT: api/UserRequests/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutUserRequest(int id, UserRequest userRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != userRequest.Id)
            {
                return BadRequest();
            }

            db.Entry(userRequest).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserRequestExists(id))
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

        // POST: api/UserRequests
        [ResponseType(typeof(UserRequest))]
        public IHttpActionResult PostUserRequest(UserRequest userRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.UserRequests.Add(userRequest);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (UserRequestExists(userRequest.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = userRequest.Id }, userRequest);
        }

        // DELETE: api/UserRequests/5
        [ResponseType(typeof(UserRequest))]
        public IHttpActionResult DeleteUserRequest(int id)
        {
            UserRequest userRequest = db.UserRequests.Find(id);
            if (userRequest == null)
            {
                return NotFound();
            }

            db.UserRequests.Remove(userRequest);
            db.SaveChanges();

            return Ok(userRequest);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool UserRequestExists(int id)
        {
            return db.UserRequests.Count(e => e.Id == id) > 0;
        }
    }
}