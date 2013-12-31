﻿using System;
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
                    connection.Execute("IF EXISTS(SELECT * FROM dbo.MapImage WHERE MapImageId = @MapImageId) BEGIN UPDATE MapImage SET Lat = @Lat, Long = @Long, [Desc] = @Desc, SentBy = @SentBy, Comment = @Comment, ImageUrl = @ImageUrl, VideoURL = @VideoURL WHERE MapImageId = @MapImageId END ELSE BEGIN INSERT MapImage (Lat, Long, [Desc], SentBy, Comment, ImageUrl, VideoURL) VALUES (@Lat, @Long, @Desc, @SentBy, @Comment, @ImageUrl, @VideoURL) END", new {MapImageId = mapImage.MapImageId, Lat = mapImage.Lat, Long = mapImage.Long, Desc = mapImage.Desc, Comment = mapImage.Comment, SentBy = mapImage.SentBy, ImageUrl = mapImage.ImageUrl, VideoURL = mapImage.VideoURL});
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}