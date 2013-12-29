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
    public class UploaderController : ApiController
    {
        private string ConnectionString { get { return ConfigurationManager.ConnectionStrings["MapImage"].ConnectionString; } }
        // GET api/uploader
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/uploader/5
        public string Get(int id)
        {
            return "value";
        }



        // POST api/uploader
        public Task<IEnumerable<FileDesc>> Post()
        {
            var folderName = "images";
            var PATH = HttpContext.Current.Server.MapPath("~/" + folderName);
            var rootUrl = Request.RequestUri.AbsoluteUri.Replace(Request.RequestUri.AbsolutePath, String.Empty);
            MapImageDac mapImageDac = new MapImageDac();
            mapImageDac.connectionString = this.ConnectionString;
            if (Request.Content.IsMimeMultipartContent())
            {
                var streamProvider = new CustomMultipartFormDataStreamProvider(PATH);
                var task = Request.Content.ReadAsMultipartAsync(streamProvider).ContinueWith<IEnumerable<FileDesc>>(t =>
                {

                    if (t.IsFaulted || t.IsCanceled)
                    {
                        throw new HttpResponseException(HttpStatusCode.InternalServerError);
                    }

                    var fileInfo = streamProvider.FileData.Select(i =>
                    {
                        var info = new FileInfo(i.LocalFileName);
                        string newFileName = PATH + "\\" + Guid.NewGuid() + info.Extension;
                        File.Move(i.LocalFileName, newFileName);
                        File.Delete(i.LocalFileName);
                        double Lat = 0;
                        double Long = 0;

                        try
                        {
                            ShowPictureLocationLib.GeoPoint gPoint = new ShowPictureLocationLib.GeoPoint();
                            gPoint = ShowPictureLocationLib.GeoPoint.GetFromImageFile(newFileName);

                            Lat = gPoint.Latitude.ToGeoCode();
                            Long = gPoint.Longitude.ToGeoCode();

                            gPoint = null;
                        }
                        catch
                        {

                        }

                        info = new FileInfo(newFileName);
                        FileDesc fileDesc = new FileDesc(info.Name, rootUrl + "/" + folderName + "/" + info.Name, info.Length / 1024, String.Format("{0}/{1}", folderName, info.Name), Lat, Long,info.Name.Replace(info.Extension,""));
                        //Insert into DB

                        MapImage mapImage = new MapImage();
                        mapImage.Lat = Lat;
                        mapImage.Long = Long;
                        mapImage.ImageUrl = fileDesc.imageUrl;

                        mapImageDac.UpdateMapImage(mapImage);
                        

                        info = null;
                        return fileDesc;
                        //TODO:
                        /*Find lat long coordinates, if any
                         * Insert imageUrl into table
                         * 
                         * */

                    });
                    streamProvider = null;
                    return fileInfo;
                });

                return task;
            }
            else
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotAcceptable, "This request is not properly formatted"));
            }
        }

        // PUT api/uploader/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/uploader/5
        public void Delete(string id)
        {
            var folderName = "images";
            var PATH = HttpContext.Current.Server.MapPath("~/" + folderName);
            var filePath = String.Format(PATH + "\\{0}", id);
            try
            {
                File.Delete(filePath);
            }
            catch (Exception ex)
            {

            }

        }
    }
}
