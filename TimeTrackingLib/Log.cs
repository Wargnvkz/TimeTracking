using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TimeTrackingLib
{

    public class Log : IDisposable
    {
        private DateTime CurrentDate = DateTime.MinValue;
        public DateTime LastLog = DateTime.MinValue;
        private string ModuleName;
        private string Prefix;
        public static string ErrorMessage = String.Empty;
        public static bool WasError = false;
        private static ConcurrentDictionary<Exception, DateTime> LogExceptions = new ConcurrentDictionary<Exception, DateTime>();
        private static int ExceptionClearTimeInSeconds = 5;
        private static Log _ApplicationInstance;
        private static bool? _CanWriteIntoLocalPath;
        private static bool CanWriteIntoALocalPath
        {
            get
            {
                if (!_CanWriteIntoLocalPath.HasValue)
                {
                    var rights = CanWriteToDirectory(LocalPath);
                    var tmpfilename = Path.GetFileName(Path.GetTempFileName());
                    var tmpfullpath = Path.Combine(LocalPath, tmpfilename);
                    var fs = File.Create(tmpfullpath);
                    fs.Close();
                    if (File.Exists(tmpfullpath))
                    {
                        _CanWriteIntoLocalPath = true && rights;
                        File.Delete(tmpfullpath);
                    }
                    else
                    {
                        _CanWriteIntoLocalPath = false;
                    }
                }
                return _CanWriteIntoLocalPath.Value;
            }
        }
        private static string _LocalPath;
        private static string LocalPath
        {
            get
            {
                if (string.IsNullOrEmpty(_LocalPath))
                {
                    _LocalPath = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
                }
                return _LocalPath;
            }
        }

        public static Log ApplicationInstance
        {
            get
            {
                if (_ApplicationInstance == null) _ApplicationInstance = new Log(LogModules.Application);
                return _ApplicationInstance;
            }
        }
        Mutex FileMutex = new Mutex();
        public Log(string moduleName, string LogPrefix)
        {
            Prefix = LogPrefix;
            ModuleName = moduleName;
            ChangeDate();
        }
        public Log(LogModules module)
        {
            Prefix = GetPrefix(module);
            ModuleName = GetModuleName(module);
            ChangeDate();
        }

        private static string GetPrefix(LogModules module)
        {
            return module.ToString();
        }
        private static string GetModuleName(LogModules module)
        {
            return module.ToString();
        }

        public static string GetLogFileName(string ModuleName, string LogPrefix, DateTime ShiftDate)
        {
            var path = GetPath(ModuleName);

            var filename = Path.Combine(path, $"{LogPrefix}_{ShiftDate.ToString("yyyyMMdd")}.log");
            return filename;
        }
        public static string GetLogFileName(LogModules module, DateTime ShiftDate)
        {
            return GetLogFileName(GetModuleName(module), GetPrefix(module), ShiftDate);
        }
        public static string GetPath(string ModuleName)
        {
            var directory = LocalPath;
            if (!CanWriteIntoALocalPath)
            {

                directory = Path.Combine(System.IO.Path.GetTempPath(), "TimeTracking");
            }

            var path = Path.Combine(directory, "Logs", ModuleName);

            if (!path.EndsWith("\\")) path = path + "\\";
            Directory.CreateDirectory(path);
            return path;
        }
        private static bool CanWriteToDirectory(string directoryPath)
        {
            try
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(directoryPath);
                var directorySecurity = directoryInfo.GetAccessControl();

                var rules = directorySecurity.GetAccessRules(true, true, typeof(System.Security.Principal.NTAccount));

                foreach (FileSystemAccessRule rule in rules)
                {
                    if ((rule.FileSystemRights & FileSystemRights.WriteData) == FileSystemRights.WriteData &&
                        rule.AccessControlType == AccessControlType.Allow)
                    {
                        return true;
                    }
                }

                return false;
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
            catch (Exception ex)
            {
                //Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }
        public static string GetPath(LogModules module)
        {
            return GetPath(GetModuleName(module));
        }

        public static DateTime GetCurrentShiftDate()
        {
            var date = DateTime.Now.Date;
            if (DateTime.Now.TimeOfDay < new TimeSpan(8, 0, 0))
            {
                date = date.AddDays(-1);
            }
            return date;
        }
        private void ChangeDate()
        {
            try
            {
                var date = GetCurrentShiftDate();
                if (date != CurrentDate)
                {
                    CurrentDate = date;
                    /*var filename = GetLogFileName(ModuleName, Prefix, CurrentDate);
                    var File = new StreamWriter(GetLogFileName(ModuleName, Prefix, CurrentDate), true);
                    File.AutoFlush = true;*/
                }
                WasError = false;
            }
            catch (Exception ex) { WasError = true; ErrorMessage = ex.Message; }
        }

        public void Add(string Message, bool ShowDateTime = true)
        {
            try
            {
                ChangeDate();
                StringBuilder sb = new StringBuilder();
                LastLog = DateTime.Now;
                if (ShowDateTime)
                {
                    sb.Append(LastLog.ToString("dd-MM-yyyy HH\\:mm\\:ss    "));
                }
                sb.Append(Message);
                var msg = sb.ToString();
                WriteTextAsync(msg);
                WasError = false;
            }
            catch (Exception ex) { WasError = true; ErrorMessage = ex.Message; }

        }

        public void Add(Exception ex, int loglevel = 0)
        {
            try
            {
                ChangeDate();
                StringBuilder sb = new StringBuilder();
                LastLog = DateTime.Now;
                sb.Append(LastLog.ToString("dd-MM-yyyy HH\\:mm\\:ss    "));
                sb.Append(ex.Message);
                sb.Append(ex.StackTrace);
                var msg = sb.ToString();
                WriteTextAsync(msg);
                WasError = false;
            }
            catch (Exception ex1) { WasError = true; ErrorMessage = ex1.Message; }

        }
        public void Add(string Message, Exception ex)
        {
            try
            {
                ChangeDate();
                StringBuilder sb = new StringBuilder();
                LastLog = DateTime.Now;
                sb.Append(LastLog.ToString("dd-MM-yyyy HH\\:mm\\:ss    "));
                sb.Append(Message);
                sb.Append(": ");
                sb.Append(ex.Message);
                sb.Append(ex.StackTrace);
                var msg = sb.ToString();
                WriteTextAsync(msg);
                WasError = false;
            }
            catch (Exception ex1) { WasError = true; ErrorMessage = ex1.Message; }

        }
        public void Add(string Message, byte[] packet, bool ShowDateTime = true)
        {
            try
            {
                ChangeDate();
                StringBuilder sb = new StringBuilder();
                LastLog = DateTime.Now;
                if (ShowDateTime)
                {
                    sb.Append(LastLog.ToString("dd-MM-yyyy HH\\:mm\\:ss    "));
                }
                sb.Append(Message);
                sb.Append("<");
                sb.Append(String.Join(", ", packet.Select(b => "0x" + b.ToString("X2"))));
                sb.Append(">");
                var msg = sb.ToString();
                WriteTextAsync(msg);
                WasError = false;
            }
            catch (Exception ex) { WasError = true; ErrorMessage = ex.Message; }

        }

        private void WriteTextAsync(string text)
        {
            var task = new Task(new Action(() => WriteTextSync(text)));
            task.Start();
        }
        private void WriteTextSync(string text)
        {
            FileMutex.WaitOne();
            try
            {
                using (var File = new StreamWriter(GetLogFileName(ModuleName, Prefix, CurrentDate), true))
                {
                    File.WriteLine(text);
                    File.Flush();
                }
            }
            catch { }
            FileMutex.ReleaseMutex();

        }
        public static string ByteArrayToHexString(byte[] data)
        {
            return String.Join(", ", data.Select(d => "0x" + d.ToString("X2")));
        }

        public void StopLog()
        {
            StopLog(null, null);
        }
        public void StopLog(object sender, EventArgs e)
        {
        }

        public void Dispose()
        {
            StopLog();
        }

        public enum LogModules
        {
            Server,
            Application
        }

        public static void ClearStoredExceptions()
        {
            lock (LogExceptions)
            {
                var ToRemove = new List<Exception>();
                foreach (var ex in LogExceptions)
                {
                    if (ex.Value.AddSeconds(ExceptionClearTimeInSeconds) < DateTime.Now)
                    {
                        ToRemove.Add(ex.Key);
                    }
                }
                ToRemove.ForEach(r => LogExceptions.TryRemove(r, out DateTime _));
            }
        }

        public LogText GetTextWriter()
        {
            return new LogText(this);
        }

        public class LogText : TextWriter
        {
            private Encoding encoding = Encoding.UTF8;
            Log log;
            public LogText(Log Log)
            {
                log = Log;
            }
            public override Encoding Encoding => encoding;

            public override void WriteLine(string value)
            {
                log.Add(value);
            }
            public override void WriteLine(object value)
            {
                if (value is string)
                {
                    log.Add(value as string);
                }
                else
                {
                    if (value is Exception)
                    {
                        log.Add(value as Exception);
                    }
                    else
                    {
                        log.Add(value.ToString());
                    }
                }
            }
            public override void Write(string value)
            {
                WriteLine(value);
            }
            public override void Write(object value)
            {
                WriteLine(value);
            }
        }

    }

}
