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
        private readonly PathsCalculator _calculator;

        public PathsController()
        {
            _calculator = new PathsCalculator();
        }

        // POST: api/Paths
        [ResponseType(typeof(WalkerPath))]
        public IHttpActionResult PostPath(WalkRequest request)
        {
            return Ok(_calculator.DoAlgorithm(request.OwnerId, request.DogWalks));
        }
    }
}