using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ImageUp.Models{
/// <summary>
/// Summary description for MapImage
/// </summary>
public class MapImage
{

        public int MapImageId { get; set; }
        public double Lat { get; set; }
        public double Long { get; set; }
        public string Desc {get;set;}
        public string SentBy  {get;set;}
        public string Comment  {get;set;}
        public string ImageUrl  {get;set;}
        
	public MapImage()
	{

	}
}

}