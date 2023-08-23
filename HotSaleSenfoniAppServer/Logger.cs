using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;

namespace HotSaleSenfoniAppServer
{
    public class Logger
    {
        #region Write
        public static void Write(Exception exception, [CallerMemberName] string callerName = "", [CallerLineNumber] int lineNumber = 0)
        {
            System.Diagnostics.Trace.WriteLine(string.Concat("Trace :: ", DateTime.Now.ToString(), ", Caller: ", callerName, ", lineNumber : ", lineNumber, ", Exception: ", exception.Message, ", StackTrace:", exception.StackTrace));
        }

        public static void Write(string str, [CallerMemberName] string callerName = "", [CallerLineNumber] int lineNumber = 0)
        {
            System.Diagnostics.Trace.WriteLine(string.Concat("Trace :: ", DateTime.Now.ToString(), ", Caller: ", callerName, ", lineNumber : ", lineNumber, ", Mesaj: ", str));
        }

        public static void Write(object obj, [CallerMemberName] string callerName = "", [CallerLineNumber] int lineNumber = 0)
        {
            System.Diagnostics.Trace.WriteLine(string.Concat("Trace :: ", DateTime.Now.ToString(), ", Caller: ", callerName, ", lineNumber : ", lineNumber, ", Object: ", obj != null ? obj.ToString() : "null"));
        }
        #endregion

        #region Verbose
        public static void V(Exception exception, [CallerMemberName] string callerName = "", [CallerLineNumber] int lineNumber = 0)
        {
            System.Diagnostics.TraceLevel level = System.Diagnostics.TraceLevel.Off;
            Enum.TryParse<System.Diagnostics.TraceLevel>(System.Configuration.ConfigurationManager.AppSettings["tracelavel"].ToString(), out level);
            if (level.GetHashCode() >= 4)
            {
                System.Diagnostics.Trace.WriteLine(string.Concat("Verbose :: ", DateTime.Now.ToString(), ", Caller: ", callerName, ", lineNumber : ", lineNumber, ", Exception: ", exception.Message, ", StackTrace:", exception.StackTrace));
            }
        }

        public static void V(string str, [CallerMemberName] string callerName = "", [CallerLineNumber] int lineNumber = 0)
        {
            System.Diagnostics.TraceLevel level = System.Diagnostics.TraceLevel.Off;
            Enum.TryParse<System.Diagnostics.TraceLevel>(System.Configuration.ConfigurationManager.AppSettings["tracelavel"].ToString(), out level);
            if (level.GetHashCode() >= 4)
            {
                System.Diagnostics.Trace.WriteLine(string.Concat("Verbose :: ", DateTime.Now.ToString(), ", Caller: ", callerName, ", lineNumber : ", lineNumber, ", Mesaj: ", str));
            }
        }

        public static void V(object obj, [CallerMemberName] string callerName = "", [CallerLineNumber] int lineNumber = 0)
        {
            System.Diagnostics.TraceLevel level = System.Diagnostics.TraceLevel.Off;
            Enum.TryParse<System.Diagnostics.TraceLevel>(System.Configuration.ConfigurationManager.AppSettings["tracelavel"].ToString(), out level);
            if (level.GetHashCode() >= 4)
            {
                System.Diagnostics.Trace.WriteLine(string.Concat("Verbose :: ", DateTime.Now.ToString(), ", Caller: ", callerName, ", lineNumber : ", lineNumber, ", Object: ", obj != null ? obj.ToString() : "null"));
            }
        }
        #endregion

        #region Info
        public static void I(Exception exception, [CallerMemberName] string callerName = "", [CallerLineNumber] int lineNumber = 0)
        {
            System.Diagnostics.TraceLevel level = System.Diagnostics.TraceLevel.Off;
            Enum.TryParse<System.Diagnostics.TraceLevel>(System.Configuration.ConfigurationManager.AppSettings["tracelavel"].ToString(), out level);
            if (level.GetHashCode() >= 3)
            {
                System.Diagnostics.Trace.WriteLine(string.Concat("Info :: ", DateTime.Now.ToString(), ", Caller: ", callerName, ", lineNumber : ", lineNumber, ", Exception: ", exception.Message, ", StackTrace:", exception.StackTrace));
            }
        }

        public static void I(string str, [CallerMemberName] string callerName = "", [CallerLineNumber] int lineNumber = 0)
        {
            System.Diagnostics.TraceLevel level = System.Diagnostics.TraceLevel.Off;
            Enum.TryParse<System.Diagnostics.TraceLevel>(System.Configuration.ConfigurationManager.AppSettings["tracelavel"].ToString(), out level);
            if (level.GetHashCode() >= 3)
            {
                System.Diagnostics.Trace.WriteLine(string.Concat("Info :: ", DateTime.Now.ToString(), ", Caller: ", callerName, ", lineNumber : ", lineNumber, ", Mesaj: ", str));
            }
        }

        public static void I(object obj, [CallerMemberName] string callerName = "", [CallerLineNumber] int lineNumber = 0)
        {
            System.Diagnostics.TraceLevel level = System.Diagnostics.TraceLevel.Off;
            Enum.TryParse<System.Diagnostics.TraceLevel>(System.Configuration.ConfigurationManager.AppSettings["tracelavel"].ToString(), out level);
            if (level.GetHashCode() >= 3)
            {
                System.Diagnostics.Trace.WriteLine(string.Concat("Info :: ", DateTime.Now.ToString(), ", Caller: ", callerName, ", lineNumber : ", lineNumber, ", Object: ", obj != null ? obj.ToString() : "null"));
            }
        }
        #endregion

        #region Warning
        public static void W(Exception exception, [CallerMemberName] string callerName = "", [CallerLineNumber] int lineNumber = 0)
        {
            System.Diagnostics.TraceLevel level = System.Diagnostics.TraceLevel.Off;
            Enum.TryParse<System.Diagnostics.TraceLevel>(System.Configuration.ConfigurationManager.AppSettings["tracelavel"].ToString(), out level);
            if (level.GetHashCode() >= 2)
            {
                System.Diagnostics.Trace.WriteLine(string.Concat("Warning :: ", DateTime.Now.ToString(), ", Caller: ", callerName, ", lineNumber : ", lineNumber, ", Exception: ", exception.Message, ", StackTrace:", exception.StackTrace));
            }
        }

        public static void W(string str, [CallerMemberName] string callerName = "", [CallerLineNumber] int lineNumber = 0)
        {
            System.Diagnostics.TraceLevel level = System.Diagnostics.TraceLevel.Off;
            Enum.TryParse<System.Diagnostics.TraceLevel>(System.Configuration.ConfigurationManager.AppSettings["tracelavel"].ToString(), out level);
            if (level.GetHashCode() >= 2)
            {
                System.Diagnostics.Trace.WriteLine(string.Concat("Warning :: ", DateTime.Now.ToString(), ", Caller: ", callerName, ", lineNumber : ", lineNumber, ", Mesaj: ", str));
            }
        }

        public static void W(object obj, [CallerMemberName] string callerName = "", [CallerLineNumber] int lineNumber = 0)
        {
            System.Diagnostics.TraceLevel level = System.Diagnostics.TraceLevel.Off;
            Enum.TryParse<System.Diagnostics.TraceLevel>(System.Configuration.ConfigurationManager.AppSettings["tracelavel"].ToString(), out level);
            if (level.GetHashCode() >= 2)
            {
                System.Diagnostics.Trace.WriteLine(string.Concat("Warning :: ", DateTime.Now.ToString(), ", Caller: ", callerName, ", lineNumber : ", lineNumber, ", Object: ", obj != null ? obj.ToString() : "null"));
            }
        }
        #endregion

        #region Error
        public static void E(Exception exception, [CallerMemberName] string callerName = "", [CallerLineNumber] int lineNumber = 0)
        {
            System.Diagnostics.TraceLevel level = System.Diagnostics.TraceLevel.Off;
            Enum.TryParse<System.Diagnostics.TraceLevel>(System.Configuration.ConfigurationManager.AppSettings["tracelavel"].ToString(), out level);
            if (level.GetHashCode() >= 1)
            {
                System.Diagnostics.Trace.WriteLine(string.Concat("Error :: ", DateTime.Now.ToString(), ", Caller: ", callerName, ", lineNumber : ", lineNumber, ", Exception: ", exception.Message, ", StackTrace:", exception.StackTrace));
            }
        }

        public static void E(string str, [CallerMemberName] string callerName = "", [CallerLineNumber] int lineNumber = 0)
        {
            System.Diagnostics.TraceLevel level = System.Diagnostics.TraceLevel.Off;
            Enum.TryParse<System.Diagnostics.TraceLevel>(System.Configuration.ConfigurationManager.AppSettings["tracelavel"].ToString(), out level);
            if (level.GetHashCode() >= 1)
            {
                System.Diagnostics.Trace.WriteLine(string.Concat("Error :: ", DateTime.Now.ToString(), ", Caller: ", callerName, ", lineNumber : ", lineNumber, ", Mesaj: ", str));
            }
        }

        public static void E(object obj, [CallerMemberName] string callerName = "", [CallerLineNumber] int lineNumber = 0)
        {
            System.Diagnostics.TraceLevel level = System.Diagnostics.TraceLevel.Off;
            Enum.TryParse<System.Diagnostics.TraceLevel>(System.Configuration.ConfigurationManager.AppSettings["tracelavel"].ToString(), out level);
            if (level.GetHashCode() >= 1)
            {
                System.Diagnostics.Trace.WriteLine(string.Concat("Error :: ", DateTime.Now.ToString(), ", Caller: ", callerName, ", lineNumber : ", lineNumber, ", Object: ", obj != null ? obj.ToString() : "null"));
            }
        }
        #endregion

        public static string ToXml(object serilizeObject)
        {
            if (object.ReferenceEquals(serilizeObject, null)) return string.Empty;

            return ToJson(serilizeObject);

            //var xmlstring = "";

            //try
            //{
            //    XmlSerializer xmlSerializer = new XmlSerializer(serilizeObject.GetType());
            //    using (var sww = new StringWriter())
            //    {
            //        using (XmlWriter writer = XmlWriter.Create(sww))
            //        {
            //            xmlSerializer.Serialize(writer, serilizeObject);
            //            xmlstring = sww.ToString();
            //        }
            //    }
            //}
            //catch (Exception exc)
            //{
            //    return exc.Message;
            //}

            //return xmlstring;
        }

        public static string ToJson(object serilizeObject)
        {
            if (object.ReferenceEquals(serilizeObject, null)) return string.Empty;

            try
            {
                return JsonConvert.SerializeObject(serilizeObject);
            }
            catch (Exception exc)
            {
                return exc.Message;
            }

        }

        public static void WriteFileLog(Exception exc, [CallerMemberName] string callerName = "", [CallerLineNumber] int lineNumber = 0)
        {
            WriteFileLog(string.Concat("Exception:", exc.Message, "\tTrace:", exc.StackTrace), callerName, lineNumber);
        }

        public static void WriteFileLog(string str, [CallerMemberName] string callerName = "", [CallerLineNumber] int lineNumber = 0)
        {
            string logdir = HttpContext.Current.Server.MapPath($"~/Log/{DateTime.Now.ToString("yyyy")}/{DateTime.Now.ToString("MM")}/{DateTime.Now.ToString("dd")}/");
            if (!Directory.Exists(logdir)) Directory.CreateDirectory(logdir);

            using (StreamWriter filewriter = new StreamWriter(HttpContext.Current.Server.MapPath($"~/Log/{DateTime.Now.ToString("yyyy")}/{DateTime.Now.ToString("MM")}/{DateTime.Now.ToString("dd")}/{Guid.NewGuid().ToString()}.log"), false, System.Text.Encoding.GetEncoding("windows-1254")))
            {
                filewriter.WriteLine($"{DateTime.Now}\t{callerName}\t{lineNumber}");
                filewriter.WriteLine(str);
            }

            //using (StreamWriter filewriter = new StreamWriter(HttpContext.Current.Server.MapPath($"~/Log/{Guid.NewGuid().ToString()}.log"), false, System.Text.Encoding.GetEncoding("windows-1254")))
            //{
            //    filewriter.WriteLine($"{DateTime.Now}\t{callerName}\t{lineNumber}");
            //    filewriter.WriteLine(str);
            //}
        }

        public static List<FileInfo> GetLogFiles(string logdir)
        {
            List<FileInfo> fileInf = null;
            if (string.IsNullOrWhiteSpace(logdir))
            {
                logdir = HttpContext.Current.Server.MapPath($"~/Log/");
            }
            string[] files = Directory.GetFiles(logdir);
            if (files != null && files.Length > 0)
            {
                fileInf = new List<FileInfo>();
                foreach (string f in files)
                {
                    FileInfo finf = new FileInfo(f);
                    if (finf.Exists && finf.Extension.Equals(".log"))
                    {
                        fileInf.Add(finf);
                    }
                }
            }
            return fileInf;
        }

        public static List<DirectoryInfo> GetLogDirs(string logdir)
        {
            List<DirectoryInfo> directoryInf = null;
            if (string.IsNullOrWhiteSpace(logdir))
            {
                logdir = HttpContext.Current.Server.MapPath($"~/Log/");
            }
            string[] directories = Directory.GetDirectories(logdir);
            if (directories != null && directories.Length > 0)
            {
                directoryInf = new List<DirectoryInfo>();
                foreach (string d in directories)
                {
                    DirectoryInfo dir = new DirectoryInfo(d);
                    if (dir.Exists && !dir.Name.Equals(".svn"))
                    {
                        directoryInf.Add(dir);
                    }
                }
            }
            return directoryInf;
        }
    }
}