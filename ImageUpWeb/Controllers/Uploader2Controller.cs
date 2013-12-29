using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ImageUpWeb.Controllers
{
    public class Uploader2Controller : ApiController
    {
        // GET api/uploader2
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/uploader2/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/uploader2
        public void Post([FromBody]string value)
        {
        }

        // PUT api/uploader2/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/uploader2/5
        public void Delete(int id)
        {
        }
    }
}
