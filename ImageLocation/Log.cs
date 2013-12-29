using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Reflection;

namespace ShowPictureLocationLib
{
    /// <summary>
    /// Helper class to aid in logging messages
    /// </summary>
    public static class Log
    {
        private const string SOURCE_APP = "ShowPictureLocationLib.Log";

        /// <summary>
        /// Write an error to the application log, or Debug console depending on build type
        /// </summary>
        public static void Write(Exception ex)
        {
            if (ex != null)
            {
                Write(ex.ToString());
            }
        }

        /// <summary>
        /// Write an error to the application log, or Debug console depending on build type
        /// </summary>
        public static void Write(string txt, params object[] args)
        {
            try
            {
                string msg = String.Format(txt, args);
#if DEBUG
                Debug.WriteLine(msg);
#else

                string sSource = SOURCE_APP;
                try
                {
                    Assembly asb = Assembly.GetEntryAssembly();
                    if (asb == null)
                    {
                        asb = Assembly.GetCallingAssembly();
                    }
                    if (asb == null)
                    {
                        asb = Assembly.GetExecutingAssembly();
                    }
                    if (asb == null)
                    {
                        asb = Assembly.GetAssembly(typeof(Log));
                    }
                    if (asb != null)
                    {
                        sSource = String.Format("{0} - {1}", asb.GetName().Name, SOURCE_APP);
                    }
                }
                catch { }
            
                string sLog = "Application";

            if (!EventLog.SourceExists(sSource))
            {
                EventLog.CreateEventSource(sSource, sLog);
            }

            EventLog.WriteEntry(
                sSource, 
                msg,
                EventLogEntryType.Error,
                0
                );
#endif
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.ToString());
#if DEBUG
                // Smother the error in release builds
                throw;
#endif
            }
        }

        /// <summary>
        /// Itterate over inner exceptions getting messages from inner exceptions.
        /// </summary>
        /// <param name="ex">Exception to parse</param>
        /// <param name="maxDepth">Max depth (number of inner exceptions) to parse</param>
        /// <returns>String array of exceptions messages</returns>
        public static string[] GetExceptionMessages(Exception ex, int maxDepth)
        {
            List<string> msg = new List<string>();

            while (maxDepth >= 0 && ex != null)
            {
                if (!String.IsNullOrEmpty(ex.Message))
                {
                    msg.Add(ex.Message);
                }
                maxDepth--;
                ex = ex.InnerException;
            }

            return msg.ToArray();
        }
    }
}
