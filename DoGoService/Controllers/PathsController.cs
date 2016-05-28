using DoGoService.Paths;
using DoGoService.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace DoGoService.Controllers
{
    [AllowAnonymous]
    public class PathsController : ApiController
    {

        // POST: api/Algorithm
        [ResponseType(typeof(WalkerPath))]
        public IHttpActionResult PostPath(WalkRequest dogData)
        {
            return Ok(AlgorithmManager.DoAlgorithm(dogData.StartingLocation, dogData.DogWalks));
        }
    }
}
