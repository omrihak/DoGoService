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
using Newtonsoft.Json;

namespace DoGoService.Controllers
{
    [AllowAnonymous]
    public class WalkersController : ApiController
    {
        private DogoDbEntities db = new DogoDbEntities();

        // GET: api/Some
        public IEnumerable<string> Get()
        {
            return QueryDate().Select(walker => JsonConvert.SerializeObject(walker));
        }

        private IEnumerable<User> QueryDate()
        {
            var parameters = Request.GetQueryNameValuePairs().ToList();
            var walkers = db.Users.Where(user => user.isWalker).ToList();

            if (parameters.Any(par => par.Key == "userName"))
            {
                walkers = walkers.Where(walker => walker.userName == parameters.First(par => par.Key == "userName").Value).ToList();
            }

            return walkers;
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
            var walker = JsonConvert.DeserializeObject<User>(value);
            walker.id = db.Users.Max(user => user.id) + 1;

            walker.walker.userId = walker.id;
            db.Walkers.Add(walker.walker);

            walker.availabilityTimes.ToList().ForEach(time => time.userId = walker.id);
            db.AvailabilityTimes.AddRange(walker.availabilityTimes);
            db.Users.Add(walker);
        }

        // PUT: api/Some/5
        public void Put(int id, [FromBody]string value)
        {
            var postedWalker = JsonConvert.DeserializeObject<User>(value);

            db.Walkers.Remove(db.Walkers.First(walker => walker.userId == id));
            db.Walkers.Add(postedWalker.walker);

            db.AvailabilityTimes.RemoveRange(db.AvailabilityTimes.Where(times => times.userId == id));
            db.AvailabilityTimes.AddRange(postedWalker.availabilityTimes);

            db.Users.Remove(db.Users.First(walker => walker.id == id));
            db.Users.Add(postedWalker);
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