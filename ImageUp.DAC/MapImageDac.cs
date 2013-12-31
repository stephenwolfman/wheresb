using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ImageUp.Models;
using Dapper;
using DapperExtensions;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;

namespace ImageUp.DAC
{
    /// <summary>
    /// Summary description for MapImageDac
    /// </summary>
    public class MapImageDac : BaseDAC
    {
        public MapImageDac()
        {
            
        }

        public IEnumerable<MapImage> GetMapImageList()
        {
            IEnumerable<MapImage> mapIMageList = null;
            try
            {
                using (var connection = new SqlConnection(this.connectionString))
                {

                    mapIMageList = connection.Query<MapImage>("select MapImageId, Lat, Long, [Desc], SentBy,Comment,ImageUrl,VideoURL from dbo.MapImage");
                }
            }
            catch (Exception ex)
            {

            }
            return mapIMageList;
        }

        public IEnumerable<MapImage> GetMapImage(int mapImageId)
        {
            IEnumerable<MapImage> mapImageList = null;
            try
            {
                using (var connection = new SqlConnection(this.connectionString))
                {

                    mapImageList = connection.Query<MapImage>("select MapImageId, Lat, Long, [Desc], SentBy,Comment,ImageUrl,VideoURL from dbo.MapImage WHERE MapImageId = @MapImageId", new { MapImageId = mapImageId });
                }
            }
            catch (Exception ex)
            {

            }
            return mapImageList;
        }

        public void UpdateMapImage(MapImage mapImage)
        {
            try
            {
                using (var connection = new SqlConnection(this.connectionString))
                {
                    connection.Execute("IF EXISTS(SELECT * FROM dbo.MapImage WHERE ImageUrl = @ImageUrl) BEGIN UPDATE MapImage SET Lat = @Lat, Long = @Long, [Desc] = @Desc, SentBy = @SentBy, Comment = @Comment WHERE ImageUrl = @ImageUrl END ELSE BEGIN INSERT MapImage (Lat, Long, [Desc], SentBy, Comment, ImageUrl) VALUES (@Lat, @Long, @Desc, @SentBy, @Comment, @ImageUrl) END", new { Lat = mapImage.Lat, Long = mapImage.Long, Desc = mapImage.Desc, Comment = mapImage.Comment, SentBy = mapImage.SentBy, ImageUrl = mapImage.ImageUrl });
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}