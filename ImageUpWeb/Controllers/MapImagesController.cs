using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ImageUp.DAC;
using ImageUp.Models;
using System.Configuration;

using ImageLocation;

using System.Diagnostics;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using ImageUpWeb.Models;
using System.IO;

namespace ImageUpWeb.Controllers
{
    public class MapImagesController : ApiController
    {
        private string ConnectionString { get { return ConfigurationManager.ConnectionStrings["MapImage"].ConnectionString; } }
        // GET api/mapimages
        public IEnumerable<MapImage> Get()
        {
            IEnumerable<MapImage> mapImages = null;
            MapImageDac mapImageDac = new MapImageDac();
            mapImageDac.connectionString = ConnectionString;
            mapImages = mapImageDac.GetMapImageList();

            return mapImages;
        }

        // GET api/mapimages/5
        public IEnumerable<MapImage> Get(int id)
        {
            MapImageDac mapImageDac = new MapImageDac();
            mapImageDac.connectionString = ConnectionString;

            return mapImageDac.GetMapImage(id);
        }

        // POST api/mapimages
        public void Post([FromBody]string value)
        {
        }
        

        // PUT api/mapimages/5
        public void Put(int id, MapImage mapImage)
        {
            MapImageDac mapImageDac = new MapImageDac();
            mapImageDac.connectionString = ConnectionString;

            mapImageDac.UpdateMapImage(mapImage);


        }

        // DELETE api/mapimages/5
        public void Delete(int id)
        {
        }
    }

    public class CustomMultipartFormDataStreamProvider : MultipartFormDataStreamProvider
    {
        public CustomMultipartFormDataStreamProvider(string path)
            : base(path)
        { }

        public override string GetLocalFileName(System.Net.Http.Headers.HttpContentHeaders headers)
        {
            var name = !string.IsNullOrWhiteSpace(headers.ContentDisposition.FileName) ? headers.ContentDisposition.FileName : "NoName";
            return name.Replace("\"", string.Empty); //this is here because Chrome submits files in quotation marks which get treated as part of the filename and get escaped
        }
    }

    public class MyMultipartFormDataStreamProvider : MultipartFormDataStreamProvider
    {
        public MyMultipartFormDataStreamProvider(string path)
            : base(path)
        {

        }

        public override string GetLocalFileName(System.Net.Http.Headers.HttpContentHeaders headers)
        {
            string fileName;
            if (!string.IsNullOrWhiteSpace(headers.ContentDisposition.FileName))
            {
                fileName = headers.ContentDisposition.FileName;
            }
            else
            {
                fileName = Guid.NewGuid().ToString() + ".data";
            }
            return fileName.Replace("\"", string.Empty);
        }
    }
}
