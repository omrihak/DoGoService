using DoGoService.Algorithm;
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
    public class AlgorithmController : ApiController
    {

        // POST: api/Algorithm
        [ResponseType(typeof(AlgorithmAnswer))]
        public IHttpActionResult PostAlgorithm(AlgorithmData dogData)
        {
            dogData.DogWalks[0].DogUserId = 1;
            dogData.DogWalks[0].TimeOfWalk = 45;
            dogData.DogWalks[1].DogUserId = 2;
            dogData.DogWalks[1].TimeOfWalk = 45;
            return Ok(AlgorithmManager.DoAlgorithm(dogData.HomeLocation, dogData.DogWalks));
        }
    }
}
