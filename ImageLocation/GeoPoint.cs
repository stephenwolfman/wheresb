using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;

namespace ShowPictureLocationLib
{
    /// <summary>
    /// Wrapper class for Longitude and Latitude
    /// </summary>
    public class GeoPoint
    {
        public Coord Longitude { set; get; }
        public Coord Latitude { set; get; }
        // TODO: Altitude?

        /// <summary>
        /// Will return true when both Longitude and Latitude are set, else false
        /// </summary>
        public bool IsValid
        {
            get
            {
                return (Longitude != null && Longitude.IsValid && Latitude != null && Latitude.IsValid);
            }
        }

        // Overloaded to make debugging easier
        public override string ToString()
        {
            List<string> txt = new List<string>();
            if(Longitude != null && Longitude.IsValid)
            {
                txt.Add(String.Format("Longitude: {0}", Longitude.ToString()));
            }
            if (Latitude != null && Latitude.IsValid)
            {
                txt.Add(String.Format("Latitude: {0}", Latitude.ToString()));
            }
            return String.Join(", ", txt);
        }

        /// <summary>
        /// Get Longitude and Latitude from an image file
        /// </summary>
        /// <param name="fully_qualified_path_to_image">File system path for the file</param>
        /// <returns>GeoPoint, IsValid will be false if points are not set properly.</returns>
        public static GeoPoint GetFromImageFile(string fully_qualified_path_to_image)
        {
            if (String.IsNullOrEmpty(fully_qualified_path_to_image))
            {
                throw new ArgumentException("Expected fully qualified path to an valid image file, but no path was passed in.", "fully_qualified_path_to_image");
            }

            if (!File.Exists(fully_qualified_path_to_image))
            {
                throw new FileNotFoundException(String.Format("Expected fully qualified path to an valid image file, but image does not exist at path specified: \"{0}\".", fully_qualified_path_to_image), fully_qualified_path_to_image);
            }

            Image img = null;

            try
            {
                img = Image.FromFile(fully_qualified_path_to_image);
            }
            catch (Exception ex)
            {
                throw new Exception("Error loading image file.  Is this a valid image file (that Microsoft .NET knows about)?", ex);
            }

            if (img == null)
            {
                throw new Exception("Error loading image file.  Is this a valid image file (that Microsoft .NET knows about)?");
            }

            return GetFromImage(img);
        }

        /// <summary>
        /// Get Longitude and Latitude from an Image
        /// </summary>
        /// <param name="img">Valid image with GPS metadata</param>
        /// <returns>GeoPoint, or exceptil will be thrown if image is not valid.</returns>
        public static GeoPoint GetFromImage(Image img)
        {
            GeoPoint ret = new GeoPoint();

            try
            {
                ret.Latitude = Coord.ExifGpsToLatitude(img);
                ret.Longitude = Coord.ExifGpsToLongitude(img);
            }
            catch (Exception ex)
            {
                throw new Exception("Error reading image properties.  Does this image have proper GPS details?", ex);
            }

            if (!ret.IsValid)
            {
                throw new Exception("Error reading image properties.  Does this image have proper GPS details?");
            }

            return ret;
        }
    }
}
