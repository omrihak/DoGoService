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
    public class PathsController : ApiController
    {

        // POST: api/Algorithm
        [ResponseType(typeof(WalkerPath))]
        public IHttpActionResult PostAlgorithm(WalkRequest dogData)
        {
            dogData.DogWalks[0].UserId = 1;
            dogData.DogWalks[0].Duration = 45;
            dogData.DogWalks[1].UserId = 2;
            dogData.DogWalks[1].Duration = 45;
            return Ok(AlgorithmManager.DoAlgorithm(dogData.StartingLocation, dogData.DogWalks));
        }
    }
}
