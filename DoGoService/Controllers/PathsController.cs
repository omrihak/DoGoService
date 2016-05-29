using DoGoService.Paths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

namespace DoGoService.Controllers
{
    [AllowAnonymous]
    public class PathsController : ApiController
    {
        // POST: api/Paths
        [ResponseType(typeof(WalkerPath))]
        public IHttpActionResult PostPath(WalkRequest request)
        {
            return Ok(AlgorithmManager.DoAlgorithm(request.StartingLocation, request.DogWalks));
        }
    }
}