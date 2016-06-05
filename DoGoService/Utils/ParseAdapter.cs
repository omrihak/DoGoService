using DoGoService.Models;
using Parse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DoGoService.Utils
{
    public class ParseAdapter
    {
        public ParseAdapter()
        {
            ParseClient.Initialize("ClCDxIalYQPR6IrVXUHtHQW99tazxTZOAFUnanLB", "XMg5TpHke8IRgNiEjY9kl7KbdRp6Ux9e6jPviy4x");
        }

        public IEnumerable<DogOwner> GetOwners()
        {
            var owners = ParseUser.Query.FindAsync().Result.ToList().Select(parse =>
            {
                return new DogOwner()
                {
                    address = parse["address"].ToString(),
                    city = parse["city"].ToString(),
                    userId = int.Parse(parse["userId"].ToString()),
                    isComfortable6To8 = bool.Parse(parse["isComfortable6To8"].ToString()),
                    isComfortable8To10 = bool.Parse(parse["isComfortable8To10"].ToString()),
                    isComfortable10To12 = bool.Parse(parse["isComfortable10To12"].ToString()),
                    isComfortable12To14 = bool.Parse(parse["isComfortable12To14"].ToString()),
                    isComfortable14To16 = bool.Parse(parse["isComfortable14To16"].ToString()),
                    isComfortable16To18 = bool.Parse(parse["isComfortable16To18"].ToString()),
                    isComfortable18To20 = bool.Parse(parse["isComfortable18To20"].ToString()),
                    isComfortable20To22 = bool.Parse(parse["isComfortable20To22"].ToString()),
                };
            });

            return owners;
        }

        public IEnumerable<DogWalker> GetWalkers()
        {
            var walkerUsers = (from user in ParseUser.Query
                               where user.Get<bool>("isDogWalker")
                               select user).FindAsync().Result.Select(user => new DogWalker()
                               {
                                   id = user.Get<int>("userId"),
                                   address = user.Get<string>("address")
                               }).ToList();

            return walkerUsers;
        }
    }
}