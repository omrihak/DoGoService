using DoGoService.DataObjects;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DoGoService.Controllers
{
    [AllowAnonymous]
    public class OwnersController : ApiController
    {
        DogoDbEntities db = new DogoDbEntities();

        // GET: api/Some
        public IEnumerable<string> Get()
        {
            return QueryDate().Select(owner => JsonConvert.SerializeObject(owner));
        }

        private IEnumerable<User> QueryDate()
        {
            var parameters = Request.GetQueryNameValuePairs().ToList();
            var owners = db.Users.Where(user => !user.isWalker).ToList();

            if (parameters.Any(par => par.Key == "userName"))
            {
                owners = owners.Where(owner => owner.userName == parameters.First(par => par.Key == "userName").Value).ToList();
            }

            if (parameters.Any(par => par.Key == "connectedTo"))
            {
                owners = owners.Where(owner => db.UserRequests.Any(request => request.status == "Accepted"
                && (request.requestingUserId == owner.id || request.requestedUserId == owner.id))).ToList();
            }

            return owners;
        }

        // GET: api/Some/5
        public string Get(int id)
        {
            var user = db.Users.Find(id);
            if (user == null)
            {
                return JsonConvert.SerializeObject("ObjectNotFound");
            }

            return JsonConvert.SerializeObject(user);
        }

        // POST: api/Some
        public void Post([FromBody]string value)
        {
            var owner = JsonConvert.DeserializeObject<User>(value);
            owner.dog.id = db.Dogs.Max(dog => dog.id) + 1;
            owner.dogId = owner.dog.id;
            db.Dogs.Add(owner.dog);
            owner.id = db.Users.Max(user => user.id) + 1;
            owner.availabilityTimes.ToList().ForEach(time => time.userId = owner.id);
            db.AvailabilityTimes.AddRange(owner.availabilityTimes);
            db.Users.Add(owner);
        }

        // PUT: api/Some/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Some/5
        public void Delete(int id)
        {
            if (db.Users.Any(user => user.id == id))
            {
                db.Users.Remove(db.Users.First(user => user.id == id));
            }
        }
    }
}
