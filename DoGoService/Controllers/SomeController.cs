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
    public class SomeController : ApiController
    {
        DogoDbEntities db = new DogoDbEntities();

        // GET: api/Some
        public IEnumerable<string> Get()
        {
            return new[] { JsonConvert.SerializeObject(db.Users.ToList()) };
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
            try
            {
                var user = JsonConvert.DeserializeObject<User>(value);
                if (user.availabilityTimes != null)
                {
                    db.AvailabilityTimes.AddRange(user.availabilityTimes);
                }

                if (user.dogs != null)
                {
                    db.Dogs.AddRange(user.dogs);
                }

                if (user.walker != null)
                {
                    db.Walkers.Add(user.walker);
                }
            }
            catch (Exception)
            {
            }
        }

        // PUT: api/Some/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Some/5
        public void Delete(int id)
        {
        }
    }
}
