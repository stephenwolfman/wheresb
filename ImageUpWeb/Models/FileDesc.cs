using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace ImageUpWeb.Models
{
    [DataContract]
    public class FileDesc
    {
        [DataMember]
        public int MapImageId { get; set; }

        [DataMember]
        public string name { get; set; }

        [DataMember]
        public string fileName { get; set; }

        [DataMember]
        public string path { get; set; }

        [DataMember]
        public string imageUrl { get; set; }

        [DataMember]
        public long size { get; set; }

        [DataMember]
        public double Lat { get; set; }

        [DataMember]
        public double Long { get; set; }

        public FileDesc(string n, string p, long s, string u, double l, double m,string f)
        {
            name = n;
            path = p;
            size = s;
            imageUrl = u;
            Lat = l;
            Long = m;
            fileName = f;
        }
    }
}