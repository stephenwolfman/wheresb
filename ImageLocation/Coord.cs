using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Imaging;
using System.Drawing;


namespace ShowPictureLocationLib
{
    /// <summary>
    /// Represent a location on the globe in Degrees, Minutes and Seconds
    /// </summary>
    public class Coord
    {
        public const string DEFAULT_COORD_MASK = "{0}{1}° {2}\' {3}\"";

        private struct DirectionConst
        {
            public const char NORTH = 'N';
            public const char SOUTH = 'S';
            public const char EAST = 'E';
            public const char WEST = 'W';

            public const char INVALID = 'X';
        }

        public double? Degree { set; get; }
        public double? Minute { set; get; }
        public double? Second { set; get; }

        /// <summary>
        /// N(orth), S(outh), E(ast), or W(est).
        /// X = INVALID_DIRECTION
        /// TODO: Maybe use enum for this...  I didn't since it was coming in as a char(2), so this seemed the natural fit.
        /// </summary>
        public char Direction
        {
            set
            {
                switch (value)
                {
                    case DirectionConst.NORTH:
                    case 'n':
                        _direction = DirectionConst.NORTH;
                        break;
                    case DirectionConst.SOUTH:
                    case 's':
                        _direction = DirectionConst.SOUTH;
                        break;
                    case DirectionConst.EAST:
                    case 'e':
                        _direction = DirectionConst.EAST;
                        break;
                    case DirectionConst.WEST:
                    case 'w':
                        _direction = DirectionConst.WEST;
                        break;
                    default:
                        _direction = DirectionConst.INVALID;
                        break;
                }
            }
            get { return _direction; }
        }
        private char _direction = DirectionConst.INVALID;

        /// <summary>
        /// True when Coordinate is a N/S coordinate, else false
        /// </summary>
        public bool IsLatitude
        {
            get
            {
                return (Direction == DirectionConst.NORTH || Direction == DirectionConst.SOUTH);
            }
        }

        /// <summary>
        /// True when Coordinate is an E/W coordinate, else false
        /// </summary>
        public bool IsLongitude
        {
            get
            {
                return (Direction == DirectionConst.EAST || Direction == DirectionConst.WEST);
            }
        }

        /// <summary>
        /// True when Degree, Minute, Second and Direction are properly set, else false
        /// </summary>
        public bool IsValid
        {
            get
            {
                return !(Degree == null || Minute == null || Second == null || Direction == DirectionConst.INVALID);
            }
        }

        /// <summary>
        /// Will return true if the coordinates should be prefixed with a minus
        /// </summary>
        private bool MinusDirection
        {
            get
            {
                return (Direction == DirectionConst.SOUTH || Direction == DirectionConst.WEST);
            }
        }

        /// <summary>
        /// Format to make compatible with Google Maps, which is where we are passing these coordinates
        /// </summary>
        /// <returns>String format of the coordinate</returns>
        public override string ToString()
        {
            return this.ToString(DEFAULT_COORD_MASK);
                
        }

        /// <summary>
        /// Format to make compatible with Google Maps, which is where we are passing these coordinates
        /// </summary>
        /// <param name="format_mask">Custom format mask for the string, taking 4 inputs: 0 = +/-, 1 = Degrees, 2 = Minutes, 3 = Seconds</param>
        /// <returns>String format of the coordinate</returns>
        public string ToString(string format_mask)
        {
            if (!IsValid)
            {
                return "";
            }

            if (String.IsNullOrEmpty(format_mask))
            {
                format_mask = DEFAULT_COORD_MASK;
            }

            // Format that Google Maps wants
            return String.Format(format_mask,
                MinusDirection ? "-" : "+",
                Degree,
                Minute,
                Second
                );
        }

        /// <summary>
        /// Return value as a single geocode value
        /// </summary>
        /// <returns></returns>
        public double ToGeoCode()
        {
            if (!IsValid) { return 0.0f; }

            // http://outreach.cast.uark.edu/east/east/gis/help/tutorials/av_geocode_coord.html
            // TODO: This is untested...
            return
                (MinusDirection ? -1d : 1d)
                *
                ((double)Degree + ((double)Minute / 60.0d) + ((double)Second / (60.0d * 60.0d)));
        }

        /// <summary>
        /// Convert image properties to a coordinate
        /// </summary>
        /// <param name="propDir">The property that contains N, S, E, or W to indicate direction</param>
        /// <param name="propCoord">The property that contains the Degrees, Minutes and Seconds.</param>
        /// <returns>Coord Object.</returns>
        public static Coord ExifGpsToCoord(PropertyItem propDir, PropertyItem propCoord)
        {
            // http://msdn.microsoft.com/en-us/library/ms534416(v=vs.85).aspx

            Coord ret = new Coord();

            try
            {
                ret.Degree = ((double)BitConverter.ToUInt32(propCoord.Value, 0) / (double)BitConverter.ToUInt32(propCoord.Value, 4));
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                ret.Degree = null;
            }

            try
            {
                ret.Minute = ((double)BitConverter.ToUInt32(propCoord.Value, 8) / (double)BitConverter.ToUInt32(propCoord.Value, 12));
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                ret.Minute = null;
            }

            try
            {
                ret.Second = ((double)BitConverter.ToUInt32(propCoord.Value, 16) / (double)BitConverter.ToUInt32(propCoord.Value, 20));
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                ret.Second = null;
            }

            try
            {
                // This property should be char(2), where 0 = NSE or W, and 1 = \n, hence get the char at 0
                ret.Direction = Encoding.ASCII.GetString(propDir.Value)[0]; //N, S, E, or W
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                ret.Direction = DirectionConst.INVALID;
            }

            return ret;
        }

        /// <summary>
        /// Get the Latitiude in which an image was taken from
        /// </summary>
        /// <param name="img">Image with proper Exif GPS properties</param>
        /// <returns>Latitude Coordinate</returns>
        public static Coord ExifGpsToLatitude(Image img)
        {
            // Get the longitue and latitutude, the numbers are from http://msdn.microsoft.com/en-us/library/ms534416(v=vs.85).aspx
            return Coord.ExifGpsToCoord(
                img.GetPropertyItem(1),
                img.GetPropertyItem(2)
                );
        }

        /// <summary>
        /// Get the Longitude in which an image was taken from
        /// </summary>
        /// <param name="img">Image with proper Exif GPS properties</param>
        /// <returns>Longitude Coordinate</returns>
        public static Coord ExifGpsToLongitude(Image img)
        {
            // Get the longitue and latitutude, the numbers are from http://msdn.microsoft.com/en-us/library/ms534416(v=vs.85).aspx
            return Coord.ExifGpsToCoord(
                img.GetPropertyItem(3),
                img.GetPropertyItem(4)
                );
        }

    }
}
