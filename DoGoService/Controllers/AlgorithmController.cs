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
            return Ok(AlgorithmManager.DoAlgorithm(dogData.HomeLocation, dogData.DogWalks));
        }
    }
}
