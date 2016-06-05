using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DoGoService.Controllers
{
    public class FileTextController : ApiController
    {
        // GET: api/FileText
        public IEnumerable<string> Get()
        {
            //return new string[] { "value1", "value2" };
            var C = "032010 1";
            var G = "320003 1";
            var Am = "002210 1";
            var F = "133211 1";

            return new[] { C, G, Am, F };
        }
    }
}
