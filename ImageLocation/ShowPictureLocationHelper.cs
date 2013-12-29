using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace ShowPictureLocationLib
{
    public static class ShowPictureLocationHelper
    {
        /// <summary>
        /// Delegate to write alerts and errors to the user.
        /// </summary>
        /// <param name="messages">One or more messages to display.</param>
        public delegate void HelpWriter(params object[] messages);

        /// <summary>
        /// Show picture location, on success, blank string will be returned,
        /// else error messages will be returend, seperated by the new line
        /// </summary>
        /// <param name="path_to_file">Full path to valid image file</param>
        /// <returns>Blank string or error messages</returns>
        public static string ShowLocation(string path_to_file)
        {
            StringBuilder sb = new StringBuilder();
            
            ShowLocation(path_to_file, new HelpWriter(m =>
            {
                if (m == null || m.Length == 0)
                {
                    return;
                }

                sb.AppendLine(String.Join("\n", m.Select(o => String.Format("{0}", o)).Where(s => !String.IsNullOrEmpty(s))));
            }));

            return sb.ToString();
        }

       
        /// <summary>
        /// Show picture location wiht a custom alert handler
        /// </summary>
        /// <param name="path_to_file">Fully qualified path to image</param>
        /// <param name="Help">Delegate method to post alerts to users.</param>
        public static void ShowLocation(string path_to_file, HelpWriter Help)
        {
            try
            {
                
                // Ensure we have a valid file path passed in.
                if (String.IsNullOrEmpty(path_to_file))
                {
                    Help("No file passed in.");
                    return;
                }

                // Ensure we have a valid file path passed in.
                if (!File.Exists(path_to_file))
                {
                    Help(String.Format("File \"{0}\" not found.", path_to_file));
                    return;
                }

                GeoPoint gp = null;

                try
                {
                    gp = GeoPoint.GetFromImageFile(path_to_file); // 0 = .exe. 1 == parameter
                }
                catch (Exception ex)
                {
                    Help(HelpMerge("Error getting coordinates from image file.", Log.GetExceptionMessages(ex, 10)));
                    return;
                }

                // If valid coordinates, pass URL to default browser.
                if (gp != null && gp.IsValid)
                {
                    string url = AppSetting("MAP_URL_MASK", @"https://maps.google.com/maps?q=##LAT_COORD##,##LONG_COORD##");
                    string coord_mask = AppSetting("COORD_FORMAT_MASK", Coord.DEFAULT_COORD_MASK);
                    bool encode_coord = false;
                    
                    try
                    {
                        encode_coord = Boolean.Parse(
                            AppSetting("ENCODE_COORD", encode_coord.ToString())
                            );
                    }
                    catch (Exception ex)
                    {
                        Help("Error getting url encodong details.  Ensure app config is present and has proper settings.", ex.Message, "Using default setting.");
                    }

                    // If they have requested to encode the escaped values, like might be necessary for some
                    // browsers, enable that here, defaulting to a non-encoded (s=>s) delegate.
                    Func<string, string> CoordEncoder = new Func<string, string>(s => s);
                    if (encode_coord)
                    {
                        CoordEncoder = new Func<string, string>(s => System.Web.HttpUtility.UrlEncode(s));
                    }

                    url = url
                        .Replace("##LAT_COORD##", CoordEncoder(gp.Latitude.ToString(coord_mask)))
                        .Replace("##LONG_COORD##", CoordEncoder(gp.Longitude.ToString(coord_mask)))
                        .Replace("##LAT_GEO##", CoordEncoder(gp.Latitude.ToGeoCode().ToString()))
                        .Replace("##LONG_GEO##", CoordEncoder(gp.Longitude.ToGeoCode().ToString()));

                    Process.Start(url);
                }
                else
                {
                    Help("Unable to resolve coordinates.", gp == null ? "" : gp.ToString());
                    return;
                }
            }
            catch (Exception ex2)
            {
                Help(HelpMerge("Unknown failure, see additional details.", Log.GetExceptionMessages(ex2, 10)));
                return;
            }
        }

        /// <summary>
        /// I originally coded help one way, then changed, this it to support backwards compatability...
        /// </summary>
        /// <param name="details"></param>
        private static object[] HelpMerge(string message, params string[] details)
        {
            List<object> obj_arr = new List<object>();
            obj_arr.Add(message);
            if (details != null) { obj_arr.AddRange(details); }
            return obj_arr.ToArray();
        }

        /// <summary>
        /// Helper to get app setting
        /// </summary>
        /// <param name="setting">Setting to get</param>
        /// <param name="error_default">Value if setting is blank or there is an error.</param>
        /// <returns>Setting value or error default</returns>
        private static string AppSetting(string setting, string error_default)
        {
            try
            {
                string value = System.Configuration.ConfigurationManager.AppSettings[setting];
                if (!String.IsNullOrEmpty(value))
                {
                    return value;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }

            return error_default;
        }
    }
}
