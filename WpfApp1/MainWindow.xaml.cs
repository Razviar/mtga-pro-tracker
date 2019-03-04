using DesktopNotifications;
using Microsoft.Toolkit.Uwp.Notifications;
using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
//using Windows.Data.Xml.Dom;

namespace MTGApro
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        private static extern IntPtr GetForegroundWindow();


        public Window4 win4 = new Window4();
        public static Parser[] indicators;
        public static bool juststarted = true;
        public static bool overlayactive = false;
        public static int[] upltimers = {5000,7000,9000,10000};
        public static long loglen = 0;
        public static bool isrestarting = false;
        public static string tokeninput = "";
        public static int version = 63;
        public static bool hasnewmessage = false;
        public static int gamerunningtimer =0;
        public static int runtime = 0;
        public static int chunk = 50;
        public static int upltimerOverride = 0;
        public static bool needsupdate = false;
        public static bool manualresync = false;
        public static bool gamestarted = false;
        public static bool gamefocused = false;
        public static bool blkmsg = false;
        public static bool trawarn = false;
        public static System.Windows.Forms.NotifyIcon ni=new System.Windows.Forms.NotifyIcon();
        public static string Usertoken = "";
        public static string mtgauid = "";
        public static string mtganick = "";
        public static bool updatenotified = false;
        public static string ouruid = "";
        public static string language = "";
        public static string tmplog = "";
        public static bool playerswith = false;
        public static bool needtoken = false;
        public static Dictionary<string, string> Usermtgaid = new Dictionary<string,string>();
        public static Dictionary<string, string> Credentials = new Dictionary<string, string>();
        public static Dictionary<string, string> Utokens = new Dictionary<string, string>();
        public static Dictionary<string, string> ParsedCreds = new Dictionary<string, string>();
        public static Dictionary<double, string[]> parsed = new Dictionary<double, string[]>();
        public static string[] hashes;
        public static double parsedtill = 0;
        public static Window1.AppSettingsStorage appsettings = new Window1.AppSettingsStorage();
        public static Window1.OverlaySettingsStorage ovlsettings = new Window1.OverlaySettingsStorage();
        public static BackgroundWorker worker = new BackgroundWorker();
        public static BackgroundWorker workerloader = new BackgroundWorker();
        public static readonly Encoding encoding = Encoding.UTF8;
        public static string errreport = @"";

        //-----
        public static Curmatch TheMatch = new Curmatch();

        //-----

        static string GetMd5Hash(MD5 md5Hash, string input)
        {
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }

        public static string Cut(string str, string start, string end, bool last)
        {
            try
            {
                if (end == @">END")
                {
                    if (str.IndexOf(start) > -1)
                    {
                        int startIndex = ((!last) ? (str.IndexOf(start)) : (str.LastIndexOf(start)));
                        int endIndex = str.Length;
                        int length = endIndex - startIndex - start.Length;
                        return str.Substring((startIndex + start.Length), length);
                    }
                    else
                    {
                        return @"NULL";
                    }
                }
                else
                {
                    if (str.IndexOf(start) > -1 && str.IndexOf(end) > -1)
                    {
                        int startIndex = ((!last) ? (str.IndexOf(start)) : (str.LastIndexOf(start)));
                        int endIndex = str.IndexOf(end, (startIndex + start.Length));
                        int length = endIndex - startIndex - start.Length;
                        return str.Substring((startIndex + start.Length), length);
                    }
                    else
                    {
                        return @"NULL";
                    }
                }
            }
            catch (Exception)
            {
                return @"NULL";
            }
        }

        public class Curmatch
        {
            public int Teamid { get; set; }
            public string Matchid { get; set; }
            public Dictionary<int,int> Udeck { get; set; }
            public Dictionary<int, int> Udeckinst { get; set; }
            public Dictionary<int, int> Edeck { get; set; }
            public Dictionary<int, int> Edeckinst { get; set; }
            public Dictionary<int, int> Draftdeck { get; set; }
            public bool Hasnewdata { get; set; }
            public bool IsDrafting { get; set; }
            public bool IsFighting { get; set; }
            public int DraftPack { get; set; }
            public int DraftPick { get; set; }
            public int TurnNumber { get; set; }
            public int DecisionPlayer { get; set; }
            public Dictionary<int, int> Timers { get; set; }


            public Curmatch(int teamid = 0, string matchid = @"")
            {
                Teamid = teamid;
                Matchid = matchid;
                Udeck = new Dictionary<int, int>();
                Udeckinst = new Dictionary<int, int>();
                Edeck = new Dictionary<int, int>();
                Edeckinst = new Dictionary<int, int>();
                Draftdeck = new Dictionary<int, int>();
                Timers = new Dictionary<int, int>() { {1,0},{2,0} };
                Hasnewdata = false;
                IsDrafting = false;
                IsFighting = false;
                DraftPack = 0;
                DraftPick = 0;
                TurnNumber = 0;
                DecisionPlayer = 0;
            }
        }

        public class Parser
        {
            public string Indicators { get; set; }
            public string Loginput { get; set; }
            public bool Send { get; set; }
            public bool Needrunning { get; set; }
            public bool Addup { get; set; }
            public string Stopper { get; set; }
            public string Needtohave { get; set; }
            public string Ignore { get; set; }

            public Parser(string indocators, bool send=true, bool needrunning=false, bool addup=false, string stopper= @"(Filename:", string needtohave=@"", string loginput = @"", string ignore=@"")
            {
                Send = send;
                Indicators = indocators;
                if (loginput != null)
                {
                    Loginput = loginput;
                }
                else
                {
                    loginput = @"";
                }
                Needrunning = needrunning;
                Addup = addup;
                Stopper = stopper;
                Needtohave = needtohave;
                Ignore = ignore;
            }
        }

        public class Response
        {
            public string Status { get; set; }
            public string Data { get; set; }
            public int Chunk { get; set; }
            public int Timer { get; set; }
            public string Package { get; set; }

            public Response(string status, string data, int chunk=50, int timer=0, string package=@"")
            {
                Status = status;
                Data = data;
                Chunk = chunk;
                Timer = timer;
                Package = package;
            }
        }

        public static DateTime tmstmptodate(double stamp)
        {
            DateTime dateTime = new DateTime(1970, 1, 1);
            dateTime = dateTime.AddSeconds(stamp);
            return dateTime;
        }

        public static string tmstmp()
        {
            Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            return unixTimestamp.ToString();
        }

        public static double ConvertToUnixTimestamp(DateTime date)
        {
            TimeZone localZone = TimeZone.CurrentTimeZone;
            DateTime currentDate = DateTime.Now;
            double currentOffset = localZone.GetUtcOffset(currentDate).TotalSeconds;

            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            TimeSpan diff = date - origin;
            return Math.Floor(diff.TotalSeconds - currentOffset);
        }

        public static void CopyTo(Stream src, Stream dest)
        {
            byte[] bytes = new byte[4096];

            int cnt;

            while ((cnt = src.Read(bytes, 0, bytes.Length)) != 0)
            {
                dest.Write(bytes, 0, cnt);
            }
        }

        public static byte[] Zip(string str)
        {
            var bytes = Encoding.UTF8.GetBytes(str);

            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(mso, CompressionMode.Compress))
                {
                    //msi.CopyTo(gs);
                    CopyTo(msi, gs);
                }

                return mso.ToArray();
            }
        }

        //Load application settings

        public void GetAppData()
        {
            try
            {
                RegistryKey RkApp = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\\MTGAProtracker");
                if (RkApp != null)
                {

                    try
                    {
                        var credload = RkApp.GetValue("credentials");
                        var umtgaidload = RkApp.GetValue("nicks");
                        var utokensload = RkApp.GetValue("utokens");
                        var lasttokenload = RkApp.GetValue("lasttoken");
                        var ovact = RkApp.GetValue("overlayactive");

                        if (credload != null) Credentials = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(credload.ToString());
                        if (umtgaidload != null) Usermtgaid = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(umtgaidload.ToString());
                        if (utokensload != null) Utokens = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(utokensload.ToString());
                        if (lasttokenload != null && !playerswith) Usertoken = lasttokenload.ToString();
                        if (ovact!=null) overlayactive = Convert.ToBoolean(ovact.ToString());

                        if(overlayactive) ovactbut.Text = "Disable In-game overlay";

                        try
                        {
                            object pt = RkApp.GetValue("parsedtill");
                            parsedtill = Double.Parse(RkApp.GetValue("parsedtill").ToString());
                        }
                        catch(Exception)
                        {
                            parsedtill = 0;
                        }

                        foreach (var crd in Utokens)
                        {
                            if (crd.Value == Usertoken) ouruid = crd.Key;
                        }
                    }
                    catch (Exception ee)
                    {
                        ErrReport(ee);
                    }

                    RkApp.Close();
                }
            }
            catch (Exception ee)
            {
                ErrReport(ee);
            }
        }

        //Sets application settings

        public void SetAppData()
        {
            try
            {
                string ids = Newtonsoft.Json.JsonConvert.SerializeObject(Credentials);
                string nicks = Newtonsoft.Json.JsonConvert.SerializeObject(Usermtgaid);
                string tokens = Newtonsoft.Json.JsonConvert.SerializeObject(Utokens);
                //File.WriteAllText(@"dbg", ids);

                RegistryKey RkApp = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\\MTGAProtracker",true);
                if (RkApp == null)
                {
                    RkApp = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\\MTGAProtracker",true);
                }

                RkApp.SetValue("parsedtill", parsedtill.ToString());
                RkApp.SetValue("lasttoken", Usertoken);
                RkApp.SetValue("credentials", ids);
                RkApp.SetValue("utokens", tokens);
                RkApp.SetValue("attempts", 0);
                RkApp.SetValue("overlayactive", overlayactive);

                try
                {
                    RkApp.SetValue("nicks", nicks);
                }
                catch (Exception ee)
                {
                    ErrReport(ee);
                }
                RkApp.Close();
            }
            catch (Exception ee)
            {
                ErrReport(ee);
            }
        }

        //Better error reporter

        public static void ErrReport(Exception e)
        {
            StackTrace st = new StackTrace(e, true);
            StackFrame frame = st.GetFrame(st.FrameCount - 1);
            int line = frame.GetFileLineNumber();
            int col = frame.GetFileColumnNumber();
            string func = frame.GetMethod().Name;
            string file = frame.GetFileName();
            Dictionary<string, object> report = new Dictionary<string, object> { { @"cmd", @"cm_errreport" }, { @"token", Usertoken }, { @"function", func }, { @"line", line.ToString() }, { @"col", col.ToString() }, { @"file", file }, { @"errmsg", e.Message }, { @"version", version.ToString() }, { @"cm_errreport", e.Message+"///"+e.InnerException + "///" + e.Source + "///" + e.StackTrace + "///" + Environment.OSVersion.Version.Major + "///" + Environment.OSVersion.Version.Minor } };
            var responseString = MakeRequest(new Uri(@"https://mtgarena.pro/mtg/donew.php"), report);
            if (responseString == "ERRCONN")
            {
                try
                {
                    File.WriteAllText(@"upload_err_log.txt", Newtonsoft.Json.JsonConvert.SerializeObject(report));
                    MessageBox.Show("Hi! App just crashed and we were unable to send automated report. Please send it manually to admin@mtgarena.pro. You can find upload_err_log.txt file in the app instalattion folder. Thanks for your help!");
                }
                catch (Exception)
                {
                    errreport = Newtonsoft.Json.JsonConvert.SerializeObject(report);
                    MessageBox.Show("Hi! App just crashed and we were unable to send automated report. Moreover, you PC didn't let us to save report into the file. So please send it manually to admin@mtgarena.pro. You will see raw erorr report in the new window. Thanks for your help!");
                    WindowErr windowErr = new WindowErr();
                    windowErr.Show();
                }
            }
            else
            {
                try
                {
                    var info = Newtonsoft.Json.JsonConvert.DeserializeObject<Response>(responseString);
                    if (info.Status != @"ok")
                    {
                        try
                        {
                            File.WriteAllText(@"upload_err_log.txt", Newtonsoft.Json.JsonConvert.SerializeObject(report));
                            MessageBox.Show("Hi! App just crashed and we were unable to send automated report. Please send it manually to admin@mtgarena.pro. You can find upload_err_log.txt file in the app instalattion folder. Thanks for your help!");
                        }
                        catch (Exception)
                        {
                            errreport = Newtonsoft.Json.JsonConvert.SerializeObject(report);
                            MessageBox.Show("Hi! App just crashed and we were unable to send automated report. Moreover, you PC didn't let us to save report into the file. So please send it manually to admin@mtgarena.pro. You will see raw erorr report in the new window. Thanks for your help!");
                            WindowErr windowErr = new WindowErr();
                            windowErr.Show();
                        }
                    }
                }
                catch (Exception)
                {
                    try
                    {
                        File.WriteAllText(@"upload_err_log.txt", Newtonsoft.Json.JsonConvert.SerializeObject(report));
                        MessageBox.Show("Hi! App just crashed and we were unable to send automated report. Please send it manually to admin@mtgarena.pro. You can find upload_err_log.txt file in the app instalattion folder. Thanks for your help!");
                    }
                    catch (Exception)
                    {
                        errreport = Newtonsoft.Json.JsonConvert.SerializeObject(report);
                        MessageBox.Show("Hi! App just crashed and we were unable to send automated report. Moreover, you PC didn't let us to save report into the file. So please send it manually to admin@mtgarena.pro. You will see raw erorr report in the new window. Thanks for your help!");
                        WindowErr windowErr = new WindowErr();
                        windowErr.Show();
                    }
                }
            }
        }

        //remote server request conductor

        public static string MakeRequest(Uri uri, Dictionary<string, object> data)
        {
            try
            {
                string formDataBoundary = String.Format("----------{0:N}", Guid.NewGuid());
                string contentType = "multipart/form-data; boundary=" + formDataBoundary;

                byte[] formData = WriteMultipartForm(data, formDataBoundary);

                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(uri);
                httpWebRequest.ContentType = "multipart/form-data; boundary=" + formDataBoundary;
                httpWebRequest.Method = "POST";
                httpWebRequest.ContentLength = formData.Length;

                using (Stream requestStream = httpWebRequest.GetRequestStream())
                {
                    requestStream.Write(formData, 0, formData.Length);
                    requestStream.Close();
                }

                var response = (HttpWebResponse)httpWebRequest.GetResponse();
                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                response.Close();
                return responseString;
            }
            catch (Exception ee)
            {
                ErrReport(ee);
                return "ERRCONN";
            }
        }


        public static byte[] WriteMultipartForm(Dictionary<string, object> postParameters, string boundary)
        {
            try
            {
                Stream formDataStream = new MemoryStream();
                bool needsCLRF = false;

                foreach (var param in postParameters)
                {
                    // Thanks to feedback from commenters, add a CRLF to allow multiple parameters to be added.
                    // Skip it on the first parameter, add it to subsequent parameters.
                    if (needsCLRF)
                        formDataStream.Write(encoding.GetBytes("\r\n"), 0, encoding.GetByteCount("\r\n"));

                    needsCLRF = true;

                    if (param.Value is String)
                    {
                        string postData = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"\r\n\r\n{2}", boundary, param.Key, param.Value);
                        formDataStream.Write(encoding.GetBytes(postData), 0, encoding.GetByteCount(postData));
                    }
                    else if (param.Value is byte[])
                    {
                        byte[] writezip = (byte[])param.Value;
                        string header = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"; filename=\"{2}\";\r\nContent-Type: {3}\r\n\r\n",
                        boundary,
                        param.Key,
                        param.Key,
                        "application/octet-stream");

                        formDataStream.Write(encoding.GetBytes(header), 0, encoding.GetByteCount(header));

                        // Write the file data directly to the Stream, rather than serializing it to a string.
                        formDataStream.Write(writezip, 0, writezip.Length);
                    }
                }

                // Add the end of the request.  Start with a newline
                string footer = "\r\n--" + boundary + "--\r\n";
                formDataStream.Write(encoding.GetBytes(footer), 0, encoding.GetByteCount(footer));

                // Dump the Stream into a byte[]
                formDataStream.Position = 0;
                byte[] formData = new byte[formDataStream.Length];
                formDataStream.Read(formData, 0, formData.Length);
                formDataStream.Close();

                return formData;
            }
            catch (Exception ee)
            {
                ErrReport(ee);
                return null;
            }
        }



        public static string GenPath(bool dir)
        {
            string path;
            if (tmplog != @"")
            {
                path = tmplog;
            }
            else if (appsettings.Path == @"")
            {
                path = Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)).FullName;
                if (Environment.OSVersion.Version.Major >= 6)
                {
                    path = Directory.GetParent(path).ToString();
                }

                path += @"\AppData\LocalLow\Wizards Of The Coast\MTGA\" + (dir ? @"" : @"output_log.txt");
            }
            else 
            {
                path = appsettings.Path;
                if (dir) path=path.Replace(@"output_log.txt","");
            }

            return path;
        }


        //Getting log parsed and data of current match updated
        public void ParseLog()
        {
            try
            {
                using (FileStream logFileStream = new FileStream(GenPath(false), FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 32768))
                {
                    //File.WriteAllText(@"checks_log.txt", Convert.ToString(loglen) + @" " + Convert.ToString(logFileStream.Length));
                    if (juststarted || logFileStream.Length < loglen)
                    {
                        logFileStream.Position = 0;
                        loglen = logFileStream.Length;
                    }
                    else if (logFileStream.Length >= loglen)
                    {
                        logFileStream.Position = loglen;
                        loglen = logFileStream.Length;
                    }

                    //loglen = 0;

                    using (StreamReader logFileReader = new StreamReader(logFileStream))
                    {
                        string line;
                        bool[] write = new bool[indicators.Length];
                        int nowriting = -1;
                        int brackets = 0;
                        int bracketssq = 0;
                        int increment = 0;
                        double laststamp = 0;
                        double timestamp = 0;
                        string playerId = "";
                        string screenName = "";
                        string strdate = "";
                        Dictionary<double, StringBuilder[]> builders = new Dictionary<double, StringBuilder[]>();

                        while ((line = logFileReader.ReadLine()) != null)
                        {
                            if (line.IndexOf(@"[UnityCrossThreadLogger]") > -1 || line.IndexOf(@"[Client GRE]") > -1)
                            {
                                 strdate = line;
                            }

                            if (line.IndexOf("\"playerId\":") > -1 && line.IndexOf("null") == -1 && playerId=="")
                            {
                                playerId = Cut(line, "\"playerId\": \"", "\"", false);
                            }

                            if (line.IndexOf("\"screenName\":") > -1)
                            {
                                screenName = Cut(line, "\"screenName\": \"", "\"", false);
                                if (ParsedCreds.ContainsKey(@"screenName"))
                                {
                                    if (screenName != ParsedCreds[@"screenName"])
                                    {
                                        builders.Clear();
                                    }
                                }
                            }

                            if (line.IndexOf("\"language\": \"") > -1)
                            {
                                language = Cut(line, "\"language\": \"", "\"", false);
                            }

                            if (line.IndexOf("\"matchId\": \"") > -1)
                            {
                                TheMatch.Matchid = Cut(line, "\"matchId\": \"", "\"", false);
                            }

                            if (line.IndexOf("MatchState_MatchComplete") > -1)
                            {
                                TheMatch = new Curmatch();
                                TheMatch.Hasnewdata = true;
                            }

                        if (nowriting==-1)
                            {
                                for (int j = 0; j < indicators.Length; j++)
                                {
                                    if (line.IndexOf(indicators[j].Indicators) > -1)
                                    {
                                        try
                                        {
                                            if (strdate.IndexOf(@"[UnityCrossThreadLogger]") > -1)
                                            {
                                                strdate = Cut(strdate, @"[UnityCrossThreadLogger]", @">END", false);
                                            }
                                            else if (strdate.IndexOf(@"[Client GRE]") > -1)
                                            {
                                                strdate = Cut(strdate, @"]", @": ", false);
                                            }

                                            DateTime d = new DateTime();
                                            if (DateTime.TryParseExact(strdate, "M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out d))
                                            {

                                            }
                                            else if (DateTime.TryParseExact(strdate, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out d))
                                            {

                                            }
                                            else if (DateTime.TryParseExact(strdate, "dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out d))
                                            {

                                            }
                                            else if (DateTime.TryParse(strdate, CultureInfo.CurrentCulture, DateTimeStyles.None, out d))
                                            {

                                            }
                                            else
                                            { 
                                                Showmsg(Colors.Red, @"Can't parse date from log! Please contact admin@mtgarena.pro", @"CLR", false, @"attention");
                                                ToastShowCheck(@"Can't parse date from log! Please contact admin@mtgarena.pro");
                                            }
                                            double tst = ConvertToUnixTimestamp(d);
                                            if (laststamp != tst)
                                            {
                                                laststamp = tst;
                                                increment = 0;
                                            }
                                            timestamp = (1000 * tst) + increment;
                                        }
                                        catch (Exception ee)
                                        {
                                            ErrReport(ee);
                                        }

                                        if (!indicators[j].Addup)
                                        {
                                            foreach (var b in builders)
                                            {
                                                if (b.Value[j].Length>0)
                                                {
                                                    builders[b.Key][j].Length = 0;
                                                }
                                            }
                                        }

                                        if (!builders.ContainsKey(timestamp))
                                        {
                                            builders.Add(timestamp, new StringBuilder[indicators.Length]);
                                            for(int index = 0; index < indicators.Length; ++index)
                                            {
                                                builders[timestamp][index] = new StringBuilder();
                                            }
                                        }

                                        int lll = builders[timestamp][j].Length;
                                        while (lll > 0)
                                        {
                                            timestamp++;
                                            increment++;
                                            if (!builders.ContainsKey(timestamp))
                                            {
                                                builders.Add(timestamp, new StringBuilder[indicators.Length]);
                                                for (int index = 0; index < indicators.Length; ++index)
                                                {
                                                    builders[timestamp][index] = new StringBuilder();
                                                }
                                            }
                                            lll = builders[timestamp][j].Length;
                                        }
                                        //builders[timestamp][j].Length=0;
                                        
                                        nowriting = j;

                                        if (line.IndexOf(@"{") > -1)
                                        {
                                            brackets++;
                                        }

                                        if (line.IndexOf(@"}") > -1)
                                        {
                                            brackets--;
                                        }

                                        if (line.IndexOf(@"[") > -1)
                                        {
                                            bracketssq++;
                                        }

                                        if (line.IndexOf(@"]") > -1)
                                        {
                                            bracketssq--;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (line.IndexOf(@"{") > -1)
                                {
                                    brackets++;
                                }

                                if (line.IndexOf(@"}") > -1)
                                {
                                    brackets--;
                                }

                                if (line.IndexOf(@"[") > -1)
                                {
                                    bracketssq++;
                                }

                                if (line.IndexOf(@"]") > -1)
                                {
                                    bracketssq--;
                                }

                                if (brackets==0 && bracketssq==0)
                                {
                                    if (line.IndexOf(indicators[nowriting].Stopper) == -1)
                                    {
                                        builders[timestamp][nowriting].Append(line);
                                    }
                                    write[nowriting] = false;
                                    nowriting = -1;
                                }
                                else if (line != @"" && line != Environment.NewLine && (line.IndexOf(indicators[nowriting].Ignore) == -1 || indicators[nowriting].Ignore == @""))
                                {
                                    builders[timestamp][nowriting].Append(line);
                                }
                            }
                        }

                        if (playerId != @"" && screenName != @"")
                        {
                            if (ParsedCreds.ContainsKey(@"playerId"))
                            {
                                ParsedCreds[@"playerId"] = playerId;
                            }
                            else
                            {
                                ParsedCreds.Add(@"playerId", playerId);
                            }

                            if (ParsedCreds.ContainsKey(@"screenName"))
                            {
                                ParsedCreds[@"screenName"] = screenName;
                            }
                            else
                            {
                                ParsedCreds.Add(@"screenName", screenName);
                            }
                        }

                        foreach (var output in builders) {
                            if (!parsed.ContainsKey(output.Key))
                            {
                                parsed.Add(output.Key, new string[indicators.Length]);
                                for (int index = 0; index < indicators.Length; ++index)
                                {
                                    parsed[output.Key][index] = @"";
                                }
                            }

                            for (int index = 0; index < indicators.Length; ++index)
                            {
                                parsed[output.Key][index] = output.Value[index].ToString();
                                // if (output.Value[index].ToString() != @"") File.WriteAllText(@"parsed"+ output.Key + @"_" + index, output.Value[index].ToString());
                            }

                            int[] orderofactions = new int[] { 2, 4, 13, 16, 5, 7, 9, 18};
                            
                            //Partial local data parsing if overlay is active and needs updates

                            if (!juststarted && overlayactive)
                            {
                                foreach (var index in orderofactions) {
                                    if (index == 4 && parsed[output.Key][index] != @"")
                                    {
                                        try
                                        {
                                            dynamic stuff = JObject.Parse(parsed[output.Key][index]);
                                            if (stuff.matchGameRoomStateChangedEvent.gameRoomInfo.stateType == @"MatchGameRoomStateType_Playing")
                                            {
                                                var resplay = stuff.matchGameRoomStateChangedEvent.gameRoomInfo.gameRoomConfig.reservedPlayers;
                                                for (int irp = 0; irp < resplay.Count; irp++)
                                                {
                                                    if (resplay[irp].userId == playerId)
                                                    {
                                                        TheMatch.Teamid = resplay[irp].systemSeatId;
                                                    }
                                                }
                                                TheMatch.Udeck.Clear();
                                                TheMatch.Udeckinst.Clear();
                                                TheMatch.Edeck.Clear();
                                                TheMatch.Edeckinst.Clear();
                                                TheMatch.Hasnewdata = true;
                                                TheMatch.IsDrafting = false;
                                                TheMatch.IsFighting = true;
                                                TheMatch.DraftPick = 0;
                                                TheMatch.DraftPack = 0;
                                                TheMatch.Timers = new Dictionary<int, int>() { { 1, 0 }, { 2, 0 } };
                                            }
                                            else if(stuff.matchGameRoomStateChangedEvent.gameRoomInfo.stateType == @"MatchGameRoomStateType_MatchCompleted")
                                            {
                                                TheMatch.IsFighting = false;
                                            }
                                        }
                                        catch (Exception ee)
                                        {
                                            ErrReport(ee);

                                        }
                                    }
                                    else if (index == 2 && parsed[output.Key][index] != @"")
                                    {
                                        TheMatch.Hasnewdata = true;
                                    }
                                    else if (index == 16 && parsed[output.Key][index] != @"")
                                    {
                                        try
                                        {
                                            string toparse = "{\"mulligan\":{" + parsed[output.Key][index].TrimEnd(',', '}', ' ') + @"}}";
                                            dynamic stuff = JObject.Parse(toparse);
                                            if (stuff.Decision == 1)
                                            {
                                                TheMatch.Udeck.Clear();
                                            }
                                        }
                                        catch (Exception ee)
                                        {
                                            ErrReport(ee);

                                        }
                                    }
                                    else if (index == 13 && parsed[output.Key][index] != @"")
                                    {
                                        try
                                        {
                                            string toparse = "{\"gameInfo\":{" + parsed[output.Key][index].TrimEnd(',', '}', ' ') + @"}}";
                                            dynamic stuff = JObject.Parse(toparse);
                                            if (stuff.gameInfo.matchState == @"MatchState_GameComplete" || stuff.gameInfo.matchState == @"MatchState_MatchComplete" || stuff.gameInfo.stage == @"GameStage_Start")
                                            {
                                                TheMatch.Udeck.Clear();
                                                TheMatch.Udeckinst.Clear();
                                                TheMatch.Edeck.Clear();
                                                TheMatch.Edeckinst.Clear();
                                                TheMatch.Hasnewdata = true;
                                                TheMatch.IsDrafting = false;
                                                TheMatch.DraftPick = 0;
                                                TheMatch.DraftPack = 0;
                                                TheMatch.Timers = new Dictionary<int, int>() { { 1, 0 }, { 2, 0 } };
                                                if (stuff.gameInfo.stage == @"GameStage_Start")
                                                {
                                                    TheMatch.IsFighting = true;
                                                }
                                                else
                                                {
                                                    TheMatch.IsFighting = false;
                                                }
                                            }
                                        }
                                        catch (Exception ee)
                                        {
                                            ErrReport(ee);

                                        }
                                    }
                                    else if (index == 5 && parsed[output.Key][index] != @"")
                                    {
                                        try
                                        {
                                            string toparse = "{\"gameObjects\":[" + parsed[output.Key][index].TrimEnd(',', ']', ' ') + @"]}";
                                            // char[] charsToTrim = { ']', ',', ' ' };
                                            dynamic stuff = JObject.Parse(toparse);
                                            for (int irp = 0; irp < stuff.gameObjects.Count; irp++)
                                            {
                                                if (stuff.gameObjects[irp].type == @"GameObjectType_Card")
                                                {

                                                    if (stuff.gameObjects[irp].ownerSeatId == TheMatch.Teamid)
                                                    {
                                                        if (!TheMatch.Udeckinst.ContainsKey((int)stuff.gameObjects[irp].instanceId) && stuff.gameObjects[irp].visibility == @"Visibility_Private")
                                                        {
                                                            if (!TheMatch.Udeck.ContainsKey(Window4.cdb_mtga_id[(int)stuff.gameObjects[irp].grpId]))
                                                            {
                                                                TheMatch.Udeck.Add(Window4.cdb_mtga_id[(int)stuff.gameObjects[irp].grpId], 1);
                                                            }
                                                            else
                                                            {
                                                                TheMatch.Udeck[Window4.cdb_mtga_id[(int)stuff.gameObjects[irp].grpId]]++;
                                                            }
                                                            TheMatch.Udeckinst.Add((int)stuff.gameObjects[irp].instanceId, (int)stuff.gameObjects[irp].grpId);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (!TheMatch.Edeckinst.ContainsKey((int)stuff.gameObjects[irp].instanceId) && stuff.gameObjects[irp].visibility == @"Visibility_Public")
                                                        {
                                                            if (!TheMatch.Edeck.ContainsKey(Window4.cdb_mtga_id[(int)stuff.gameObjects[irp].grpId]))
                                                            {
                                                                TheMatch.Edeck.Add(Window4.cdb_mtga_id[(int)stuff.gameObjects[irp].grpId], 1);
                                                            }
                                                            else
                                                            {
                                                                TheMatch.Edeck[Window4.cdb_mtga_id[(int)stuff.gameObjects[irp].grpId]]++;
                                                            }
                                                            TheMatch.Edeckinst.Add((int)stuff.gameObjects[irp].instanceId, (int)stuff.gameObjects[irp].grpId);
                                                        }
                                                    }
                                                    TheMatch.Hasnewdata = true;
                                                }
                                            }
                                        }
                                        catch (Exception ee)
                                        {
                                            ErrReport(ee);
                                        }
                                    }
                                    else if ((index == 7 || index == 9) && parsed[output.Key][index] != @"")
                                    {
                                        try
                                        {
                                            dynamic stuff = JObject.Parse(parsed[output.Key][index]);
                                            TheMatch.DraftPack = stuff.packNumber;
                                            TheMatch.DraftPick = stuff.pickNumber;
                                            TheMatch.Draftdeck.Clear();
                                            try
                                            {
                                                for (int irp = 0; irp < stuff.draftPack.Count; irp++)
                                                {
                                                    if (!TheMatch.Draftdeck.ContainsKey(Window4.cdb_mtga_id[(int)stuff.draftPack[irp]]))
                                                    {
                                                        TheMatch.Draftdeck.Add(Window4.cdb_mtga_id[(int)stuff.draftPack[irp]], 1);
                                                    }
                                                    else
                                                    {
                                                        TheMatch.Draftdeck[Window4.cdb_mtga_id[(int)stuff.draftPack[irp]]]++;
                                                    }
                                                }
                                            }
                                            catch (Exception ee)
                                            {
                                                ErrReport(ee);
                                            }
                                            TheMatch.Hasnewdata = true;
                                            if (TheMatch.DraftPack == 2 && TheMatch.DraftPick == 14)
                                            {
                                                TheMatch.IsDrafting = false;
                                            }
                                            else
                                            {
                                                TheMatch.IsDrafting = true;
                                            }
                                        }
                                        catch (Exception ee)
                                        {
                                            ErrReport(ee);
                                            TheMatch.Hasnewdata = true;
                                            TheMatch.IsDrafting = false;
                                        }
                                    }
                                    else if (index == 18 && parsed[output.Key][index] != @"")
                                    {
                                        try
                                        {
                                            string toparse = "{\"turnInfo\":{" + parsed[output.Key][index].TrimEnd(',', '}', ' ') + @"}}";
                                            dynamic stuff = JObject.Parse(toparse);
                                            TheMatch.DecisionPlayer = stuff.turnInfo.decisionPlayer;
                                            TheMatch.TurnNumber = stuff.turnInfo.turnNumber;
                                        }
                                        catch (Exception ee)
                                        {
                                            //ErrReport(ee);
                                            TheMatch.Hasnewdata = true;
                                            TheMatch.IsDrafting = false;
                                        }
                                    }
                                }
                            }
                        }

                        /*for (int j = 0; j < indicators.Length; j++) {
                            if(indicators[j].Loginput!=@"") File.WriteAllText(@"parsed_"+j, indicators[j].Loginput);
                        }*/

                        logFileReader.Close();
                        logFileReader.Dispose();
                    }
                    logFileStream.Close();
                    logFileStream.Dispose();
                }
            }
            catch (SecurityException)
            {
                Showmsg(Colors.Red, @"Can't parse data from log! Disable antivirus", @"CLR", false, @"attention");
                ToastShowCheck(@"Can't parse data from log! Disable antivirus");

            }
            catch (IOException)
            {
                Showmsg(Colors.Red, @"Can't parse data from log! IO Error", @"CLR", false, @"attention");
                ToastShowCheck(@"Can't parse data from log! IO Error");
            }
            catch (UnauthorizedAccessException)
            {
                Showmsg(Colors.Red, @"Can't parse data from log! Access denied", @"CLR", false, @"attention");
                ToastShowCheck(@"Can't parse data from log! Access denied");
            }
            catch (Exception ee)
            {
                ErrReport(ee);
            }
        }

        public void Showmsg(Color color, string msg, string msgupl, bool block, string ico)
        {
            if (!blkmsg)
            {
                Dispatcher.BeginInvoke(new ThreadStart(delegate {
                    Status_light.Fill = new SolidColorBrush(color);
                    Status_light.Visibility = Visibility.Visible;
                    Status_msg.Text = msg;
                    Stream iconStream = Application.GetResourceStream(new Uri(@"pack://application:,,,/Resources/" + ico + ".ico")).Stream;
                    Icon = new BitmapImage(new Uri(@"pack://application:,,,/Resources/" + ico + ".ico"));
                    ni.Icon = new System.Drawing.Icon(iconStream);
                }));

                if (msgupl != @"" && msgupl != @"CLR")
                {
                    Dispatcher.BeginInvoke(new ThreadStart(delegate { Status_lastupl.Text = msgupl; }));
                }else if (msgupl==@"CLR")
                {
                    Dispatcher.BeginInvoke(new ThreadStart(delegate { Status_lastupl.Text = @""; }));
                }
            }

            if (block)
            {
                blkmsg = block;
            }
        }

        public static void NumericalSort(string[] ar)
        {
            Regex rgx = new Regex("([^0-9]*)([0-9]+)");
            Array.Sort(ar, (a, b) =>
            {
                var ma = rgx.Matches(a);
                var mb = rgx.Matches(b);
                for (int i = 0; i < ma.Count; ++i)
                {
                    int ret = ma[i].Groups[1].Value.CompareTo(mb[i].Groups[1].Value);
                    if (ret != 0)
                        return ret;

                    ret = int.Parse(ma[i].Groups[2].Value) - int.Parse(mb[i].Groups[2].Value);
                    if (ret != 0)
                        return ret;
                }

                return 0;
            });
        }

        //Handling log sending and local storage (if needed)
        public void Sendlog(Dictionary<string, object> requestdict, bool juststash=false)
        {
            try
            {
                string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"MTGAproTracker");
                if (Directory.Exists(path))
                {
                    string[] files = Directory.GetFiles(path, "storage*.mtg");

                    if (files.Length > 0)
                    {
                        NumericalSort(files);
                        var n = 0;

                        foreach (string f in files)
                        {
                            var lrd = Loadfromstorage(f);
                            Dispatcher.BeginInvoke(new ThreadStart(delegate
                            {
                                Uplprogress.Maximum = files.Length;
                                Uplprogress.Visibility = Visibility.Visible;
                                Resyncbut.IsEnabled = false;
                                Scanbut.IsEnabled = false;
                            }));
                            n++;
                            if (Dispatchlog(lrd, true) || lrd == null)
                            {
                                File.Delete(f);
                                Dispatcher.BeginInvoke(new ThreadStart(delegate
                                {
                                    Uplprogress.Value = n;
                                }));
                            }
                            else
                            {
                                break;
                            }
                            Thread.Sleep(upltimerOverride);
                        }

                        Dispatcher.BeginInvoke(new ThreadStart(delegate
                        {
                            Uplprogress.Visibility = Visibility.Hidden;
                            Resyncbut.IsEnabled = true;
                            Scanbut.IsEnabled = true;
                        }));
                        Showmsg(Colors.Green, @"Upload complete!", @"Last upload: " + DateTime.Now.ToString(), false, @"icon" + appsettings.Icon.ToString());
                    }
                }

                //requestdict.Add(@"generror", "500");
                if (!juststash)
                {
                    Dispatchlog(requestdict, false);
                }

            }
            catch (Exception ee)
            {
                ErrReport(ee);
            }
        }

        //Committing log sparses to server

        public bool Dispatchlog(Dictionary<string, object> requestdict, bool fromstash)
        {
            if (Int32.Parse(requestdict[@"version"].ToString()) != version)
            {
                return true;
            }

            if (requestdict.Count > (chunk+8))
            {
                int sparsecounter = 0;
                int elemcounter = 0;
                Dictionary<string, object> requestdictsparse=new Dictionary<string, object>();

                foreach (var pair in requestdict)
                {
                    if (elemcounter == 0)
                    {
                        if (!requestdictsparse.ContainsKey(@"cmd"))
                        {
                            requestdictsparse.Add(@"cmd", requestdict[@"cmd"]);
                            requestdictsparse.Add(@"uid", requestdict[@"uid"]);
                            requestdictsparse.Add(@"token", requestdict[@"token"]);
                            requestdictsparse.Add(@"version", requestdict[@"version"]);
                            requestdictsparse.Add(@"mtganick", requestdict[@"mtganick"]);
                            requestdictsparse.Add(@"mtgauid", requestdict[@"mtgauid"]);
                            requestdictsparse.Add(@"language", requestdict[@"language"]);
                            requestdictsparse.Add(@"setdate", requestdict[@"setdate"]);
                        }
                    }

                    if (pair.Key.IndexOf(@"cm_uploadpack[") > -1)
                    {
                        requestdictsparse.Add(pair.Key,pair.Value);
                        ++elemcounter;
                    }

                    if (elemcounter == chunk)
                    {
                        Savetostorage(requestdictsparse, sparsecounter);
                        requestdictsparse.Clear();
                        elemcounter = 0;
                        ++sparsecounter;
                    }
                }

                if (requestdictsparse.Count > 0)
                {
                    Savetostorage(requestdictsparse, sparsecounter);
                    requestdictsparse.Clear();
                    elemcounter = 0;
                    ++sparsecounter;
                }

                Showmsg(Colors.Orange, @"Big log. Sparses: " + sparsecounter, @"Saved locally: " + DateTime.Now.ToString(), false, @"icon" + appsettings.Icon.ToString());
                Sendlog(null, true);
                return true;
            }

               var upload = MakeRequest(new Uri(@"https://mtgarena.pro/mtg/donew.php"), requestdict);
               if (upload == "ERRCONN")
                {
                    if (!fromstash)
                    {
                        Savetostorage(requestdict);
                        Showmsg(Colors.Orange, @"Game info found", @"Saved locally: " + DateTime.Now.ToString(), false, @"icon" + appsettings.Icon.ToString());
                    }
                    return false;
                }
                else
                {
                    try
                    {
                        var info = Newtonsoft.Json.JsonConvert.DeserializeObject<Response>(upload);
                        if (info.Status == @"ok")
                        {
                            if (fromstash)
                            {
                                Showmsg(Colors.Green, @"Uploading data...", @"Last upload: " + DateTime.Now.ToString(), false, @"icon" + appsettings.Icon.ToString());
                            }
                            else
                            {
                                Showmsg(Colors.Green, @"Game info found.", @"Last upload: " + DateTime.Now.ToString(), false, @"icon" + appsettings.Icon.ToString());
                            }
                            
                            chunk = info.Chunk;
                            upltimerOverride = info.Timer;

                        if (info.Data == "restart")
                        {
                            ni.Visible = false;
                            ni.Dispose();
                            Process.Start(Application.ResourceAssembly.Location, "restarting");
                            Environment.Exit(0);
                        }
                        else if(info.Data == "has_update" && !fromstash)
                        {
                            if (!updatenotified)
                            {
                                updatenotified = true;
                                Dispatcher.BeginInvoke(new ThreadStart(delegate
                                {
                                    Updater.Visibility = Visibility.Visible;
                                    ToastShowCheck("Update of MTGA Pro Tracker is ready to be installed!");
                                }));
                            }
                        }
                        else if (info.Data == "has_message" && !fromstash)
                        {
                            Dispatcher.BeginInvoke(new ThreadStart(delegate {
                                Messenger.Visibility = Visibility.Visible;
                                ToastShowCheck(@"You've got new notification!");
                            }));
                        }

                            return true;
                        }
                        else if (info.Status == @"NEED_UPDATE" && !fromstash)
                        {
                            Showmsg(Colors.Red, @"Upload failed: update tracker to resume", @"CLR", false, @"attention");
                            ToastShowCheck(@"Upload failed: update tracker to resume");
                            needsupdate = true;
                            worker.CancelAsync();
                            return false;
                        }
                    }
                    catch (Exception ee)
                    {
                        ErrReport(ee);
                        if (!fromstash)
                        {
                            Savetostorage(requestdict);
                            Showmsg(Colors.Orange, @"Game info found", @"Saved locally: " + DateTime.Now.ToString(), false, @"icon" + appsettings.Icon.ToString());
                        }
                        return false;
                    }
                }


            return true;
        }

        //Saving logs to local storage for futher commit

        public void Savetostorage(Dictionary<string, object> dictionary,int sparse=0)
        {
            try
            {
                string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"MTGAproTracker");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                using (FileStream fs = File.OpenWrite(path + @"\storage_" + dictionary[@"setdate"] + @"_" + sparse + @".mtg"))
                using (BinaryWriter writer = new BinaryWriter(fs))
                {
                    // Put count.
                    writer.Write(dictionary.Count);
                    // Write pairs.
                    foreach (var pair in dictionary)
                    {
                        writer.Write(pair.Key);
                        if (pair.Value is String)
                        {
                            writer.Write(pair.Value.ToString());
                        }
                        else if (pair.Value is byte[] bts)
                        {
                            writer.Write(Convert.ToBase64String(bts));
                        }

                    }
                }
            }
            catch (Exception ee)
            {
                ErrReport(ee);
            }
        }

        public Dictionary<string, object> Loadfromstorage(string f)
        {
            try
            {
                var result = new Dictionary<string, object>();

                using (FileStream fs = File.OpenRead(f))
                using (BinaryReader reader = new BinaryReader(fs))
                {
                    // Get count.
                    int count = reader.ReadInt32();
                    // Read in all pairs.
                    for (int i = 0; i < count; i++)
                    {
                        string key = reader.ReadString();
                        object value;
                        if (key.IndexOf(@"cm_uploadpack[") == -1)
                        {
                            value = reader.ReadString();
                        }
                        else
                        {
                            value = Convert.FromBase64String(reader.ReadString());
                        }
                        result[key] = value;
                    }
                }
                return result;
            }
            catch (Exception ee)
            {
                ErrReport(ee);
                return null;
            }
        }


        public void Getindicators()
        {
            try
            {
                var indic= MakeRequest(new Uri(@"https://mtgarena.pro/mtg/donew.php"), new Dictionary<string, object> { { @"cmd", @"cm_getindicators" }, { @"cm_init", version.ToString() } });
                if (indic != @"ERRCONN")
                {
                    indicators = Newtonsoft.Json.JsonConvert.DeserializeObject<Parser[]>(indic);
                    hashes = new string[indicators.Length];
                }
                else
                {
                    Showmsg(Colors.Red, @"Unable to establish initial connection", @"CLR", false, @"attention");
                }
            }
            catch (Exception ee)
            {
                ErrReport(ee);
            }
        }

        //Checking if there's updates for the app

        public void Checkupdates()
        {
            try
            {
                var checkver = MakeRequest(new Uri(@"https://mtgarena.pro/mtg/donew.php"), new Dictionary<string, object> { { @"cmd", @"cm_init" }, { @"cm_init", version.ToString() } });

                if (checkver != @"")
                {
                    var info = Newtonsoft.Json.JsonConvert.DeserializeObject<Response>(checkver);
                    if (info != null)
                    {
                        Dispatcher.BeginInvoke(new ThreadStart(delegate
                        {
                            var decver = Math.Round(Convert.ToDouble(version / 10));
                            var onever = version % 10;
                            Version.Text = @"v.1." + Convert.ToString(decver) + @"." + Convert.ToString(onever);
                        }));

                        chunk = info.Chunk;
                        upltimerOverride = info.Timer;

                        if (info.Status == @"update")
                        {
                            Dispatcher.BeginInvoke(new ThreadStart(delegate
                            {
                                Updater.Visibility= Visibility.Visible;
                                ToastShowCheck("Update of MTGA Pro Tracker is ready to be installed!");
                                Version.Text = @"";
                            }));
                        }
                    }
                }
            }
            catch (Exception ee)
            {
                ErrReport(ee);
            }
        }

        //Key processor of log

        public void Checklog()
        {
            try
            {
                GetAppData();

                if (File.Exists(GenPath(false)) && Usertoken != null)
                {
                    Showmsg(Colors.YellowGreen, @"Reading log...", @"", false, @"icon" + appsettings.Icon.ToString());
                    Dispatcher.BeginInvoke(new ThreadStart(delegate {
                        Resyncbut.IsEnabled = false;
                        Scanbut.IsEnabled = false;
                    }));
                    ParseLog();
                    Dispatcher.BeginInvoke(new ThreadStart(delegate {
                        Resyncbut.IsEnabled = true;
                        Scanbut.IsEnabled = true;
                    }));

                    bool somethingnew = false;
                    bool credok = true;
                    double lastime = 0;
                    List<double> todelete = new List<double>();

                    if (runtime > 0)
                    {
                        gamerunningtimer = (Convert.ToInt32(tmstmp()) - runtime);
                        runtime = Convert.ToInt32(tmstmp());
                    }

                    Dictionary<string, object> requestdict = new Dictionary<string, object> { { @"cmd", @"cm_uploadpackfile" }, { @"uid", ouruid }, { @"token", Usertoken }, { @"version", version.ToString() }, { @"runtime", gamerunningtimer.ToString() },{@"currentmatch", TheMatch.Matchid } };

                    if (ParsedCreds.ContainsKey(@"screenName"))
                    {
                        if (mtganick != ParsedCreds[@"screenName"] || juststarted)
                        {
                            juststarted = false;
                            mtganick = ParsedCreds[@"screenName"];
                            mtgauid = ParsedCreds[@"playerId"];

                            requestdict.Add(@"mtganick", mtganick);
                            requestdict.Add(@"mtgauid", mtgauid);
                            if (language != "") requestdict.Add(@"language", language);

                            Dispatcher.BeginInvoke(new ThreadStart(delegate
                            {
                                Ingame_user.Text = "MTGA nick:";
                                Ingame_nick.Content = mtganick;
                            }));

                            if (!Credentials.ContainsKey(ouruid) && !Credentials.ContainsValue(mtgauid))
                            {
                                Usermtgaid.Add(ouruid, mtganick);
                                Credentials.Add(ouruid, mtgauid);
                                Dispatcher.BeginInvoke(new ThreadStart(delegate { SetAppData(); }));
                            }

                            playerswith = false;

                            if (Credentials[ouruid] != mtgauid)
                            {
                                Usertoken = null;
                                ouruid = "";
                                credok = false;
                                playerswith = true;
                                needtoken = true;

                                foreach (var tkn in Credentials)
                                {
                                    if (tkn.Value == mtgauid && ouruid != tkn.Key)
                                    {
                                        ouruid = tkn.Key;
                                        Usertoken = Utokens[ouruid];

                                        Dispatcher.BeginInvoke(new ThreadStart(delegate
                                        {
                                        /*Token.Content = selnick;*/
                                            Token_msg.Text = @"Detected user: ";
                                            TokenInput.Text = Usertoken;
                                        }));

                                        credok = true;
                                        needtoken = false;
                                        GetAppData();
                                    }
                                }

                                if (credok == false)
                                {
                                    Dispatcher.BeginInvoke(new ThreadStart(delegate
                                    {
                                        Token_msg.Text = @"";
                                        Token.Content = "";
                                        TokenInput.Text = "";
                                    }));
                                }
                            }
                        }
                    }

                    if (credok && !needsupdate && !playerswith)
                    {
                        foreach (var writing in parsed)
                        {
                            for (var j = 0; j < indicators.Length; j++)
                            {
                                if(writing.Value[j]!=@"" && writing.Value[j] !=null)
                                {
                                    if (indicators[j].Send && ((manualresync && indicators[j].Addup) || (writing.Key > parsedtill && (indicators[j].Addup || (!indicators[j].Addup && writing.Value[j] != hashes[j])))) && (indicators[j].Needtohave == @"" || writing.Value[j].IndexOf(indicators[j].Needtohave) > -1))
                                    {
                                        requestdict.Add(@"cm_uploadpack["+writing.Key+"][" + j.ToString() + "]", Zip(writing.Value[j]));
                                        if (!requestdict.ContainsKey(@"setdate")) requestdict.Add(@"setdate", tmstmp());
                                        somethingnew = true;
                                        hashes[j] = writing.Value[j];
                                    }
                                }
                            }
                            todelete.Add(writing.Key);
                            lastime = writing.Key;
                        }

                        parsedtill = lastime;

                        foreach (var del in todelete)
                        {
                            parsed.Remove(del);
                        }

                        Showmsg(Colors.YellowGreen, @"Awaiting for updates...", @"", false, @"icon" + appsettings.Icon.ToString());

                        if (somethingnew && !playerswith)
                        {
                            Sendlog(requestdict);
                            Dispatcher.BeginInvoke(new ThreadStart(delegate { SetAppData(); }));
                            if (manualresync)
                            {
                                manualresync = false;
                            }

                            if (tmplog != @"")
                            {
                                tmplog = @"";
                                juststarted = true;
                                hashes = new string[indicators.Length];
                                loglen = 0;
                                parsedtill = 0;
                                manualresync = true;
                            }
                        }
                    }
                    else if (needtoken)
                    {
                        Showmsg(Colors.Red, @"Another MTGA User! Input different Token", @"CLR", false, @"attention");
                        ToastShowCheck(@"Another MTGA User! Input different Token");
                    }
                    else if (playerswith)
                    {
                        Showmsg(Colors.Yellow, @"Another MTGA User! Switching tokens...", @"CLR", false, @"attention");
                        //ToastShowCheck(@"Another MTGA User! Awaiting different Token");
                    }
                    
                    else if (Usertoken == null)
                    {
                        Showmsg(Colors.Red, @"No valid user token installed!", @"CLR", false, @"attention");
                        ToastShowCheck(@"No valid user token installed!");
                    }
                    else
                    {
                        Showmsg(Colors.Red, @"Game info NOT found!", @"CLR", false, @"attention");
                        ToastShowCheck(@"Game info NOT found!");
                    }
                }
                
            }
            catch (Exception ee)
            {
                ErrReport(ee);
            }
        }

        

        public delegate void ToastShow(string msg);

        public void ToastShowCheck(string msg)
        {
            /*if (Environment.OSVersion.Version.Major == 10)
            {
                ToastShow toastShow = delegate (string message)
                {
                    try
                    {
                        string hashmsg = GetMd5Hash(MD5.Create(), message);
                        int num = 0;
                        try
                        {
                            var hist = DesktopNotificationManagerCompat.History.GetHistory(hashmsg);
                            num = hist.Count;
                        }
                        catch (Exception ee)
                        {
                            ErrReport(ee);
                        }

                        if (num == 0)
                        {
                            // Construct the visuals of the toast (using Notifications library)
                            ToastContent toastContent = new ToastContent()
                            {
                                Visual = new ToastVisual()
                                {
                                    BindingGeneric = new ToastBindingGeneric()
                                    {
                                        Children =
                                        {
                                            new AdaptiveText()
                                            {
                                                Text = message
                                            }
                                        }
                                    }
                                }
                            };

                            // Create the XML document (BE SURE TO REFERENCE WINDOWS.DATA.XML.DOM)
                            var doc = new Windows.Data.Xml.Dom.XmlDocument();
                            doc.LoadXml(toastContent.GetContent());

                            // And create the toast notification
                            var toast = new Windows.UI.Notifications.ToastNotification(doc);
                            try
                            {
                                toast.Tag = hashmsg;
                            }
                            catch (Exception ee)
                            {
                                ErrReport(ee);
                            }

                            try
                            {
                                DesktopNotificationManagerCompat.CreateToastNotifier().Show(toast);
                            }
                            catch (Exception ee)
                            {
                                ErrReport(ee);
                            }
                        }
                    }
                    catch (Exception ee)
                    {
                        ErrReport(ee);
                    }
                };

                toastShow(msg);
            }
            else
            {*/
                ni.BalloonTipTitle = "MTGA Pro Tracker";
                ni.BalloonTipText = msg;
                ni.ShowBalloonTip(20000);
           /* }*/
        }

        public MainWindow()
        {
            InitializeComponent();

            //throw new Exception();

            Checkupdates();
            Getindicators();

            /*if (Environment.OSVersion.Version.Major == 10 || (Environment.OSVersion.Version.Major == 6 && Environment.OSVersion.Version.Minor > 1))
            {
                try
                {
                    DesktopNotificationManagerCompat.RegisterAumidAndComServer<MyNotificationActivator>("MTGApro");
                    DesktopNotificationManagerCompat.RegisterActivator<MyNotificationActivator>();
                }
                catch (Exception ee)
                {
                    ErrReport(ee);
                }
            }*/

            RegistryKey RkTokens = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\\MTGAProtracker");

            if (RkTokens != null)
            {
                try
                {
                    appsettings = Newtonsoft.Json.JsonConvert.DeserializeObject<Window1.AppSettingsStorage>(RkTokens.GetValue("appsettings").ToString());
                }
                catch (Exception ee)
                {
                    ErrReport(ee);
                }

                try
                {
                    ovlsettings = Newtonsoft.Json.JsonConvert.DeserializeObject<Window1.OverlaySettingsStorage>(RkTokens.GetValue("ovlsettings").ToString());
                }
                catch (Exception ee)
                {
                    ErrReport(ee);
                }
            }

            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += new UnhandledExceptionEventHandler(CrashHandler);

            Stream iconStream = Application.GetResourceStream(new Uri(@"pack://application:,,,/Resources/icon" + appsettings.Icon + ".ico")).Stream;
            Icon = new BitmapImage(new Uri(@"pack://application:,,,/Resources/icon" + appsettings.Icon + ".ico"));
            ni.Icon = new System.Drawing.Icon(iconStream);
            ni.Visible = true;
            ni.DoubleClick +=
                delegate (object sender, EventArgs args)
                {
                    Show();
                    WindowState = WindowState.Normal;
                };

            var cm = new System.Windows.Forms.ContextMenu();
            System.Windows.Forms.MenuItem[] menuitems = { new System.Windows.Forms.MenuItem("Open App"), new System.Windows.Forms.MenuItem(@"-"), new System.Windows.Forms.MenuItem(@"My Decks"), new System.Windows.Forms.MenuItem(@"My Collection"), new System.Windows.Forms.MenuItem(@"My Progress"), new System.Windows.Forms.MenuItem(@"My Matches"), new System.Windows.Forms.MenuItem(@"Deckbuilder"), new System.Windows.Forms.MenuItem(@"Deck Converter"), new System.Windows.Forms.MenuItem(@"Deck List Browser"), new System.Windows.Forms.MenuItem(@"-"), new System.Windows.Forms.MenuItem(@"Exit") };

            menuitems[0].Click += delegate (object sender, EventArgs args)
            {
                Show();
                WindowState = WindowState.Normal;
            };

            menuitems[2].Click += delegate (object sender, EventArgs args)
            {
                Uri link = new Uri(@"https://mtgarena.pro/decks/#my");
                Process.Start(new ProcessStartInfo(link.AbsoluteUri));
            };

            menuitems[3].Click += delegate (object sender, EventArgs args)
            {
                Uri link = new Uri(@"https://mtgarena.pro/collection/");
                Process.Start(new ProcessStartInfo(link.AbsoluteUri));
            };

            menuitems[4].Click += delegate (object sender, EventArgs args)
            {
                Uri link = new Uri(@"https://mtgarena.pro/progress/");
                Process.Start(new ProcessStartInfo(link.AbsoluteUri));
            };

            menuitems[5].Click += delegate (object sender, EventArgs args)
            {
                Uri link = new Uri(@"https://mtgarena.pro/matches/");
                Process.Start(new ProcessStartInfo(link.AbsoluteUri));
            };

            menuitems[6].Click += delegate (object sender, EventArgs args)
            {
                Uri link = new Uri(@"https://mtgarena.pro/deckbuilder/");
                Process.Start(new ProcessStartInfo(link.AbsoluteUri));
            };


            menuitems[7].Click += delegate (object sender, EventArgs args)
            {
                Uri link = new Uri(@"https://mtgarena.pro/converter/");
                Process.Start(new ProcessStartInfo(link.AbsoluteUri));
            };


            menuitems[8].Click += delegate (object sender, EventArgs args)
            {
                Uri link = new Uri(@"https://mtgarena.pro/decks/");
                Process.Start(new ProcessStartInfo(link.AbsoluteUri));
            };


            menuitems[10].Click += delegate (object sender, EventArgs args)
            {
                ni.Visible = false;
                ni.Dispose();
                Environment.Exit(0);
            };

            cm.MenuItems.AddRange(menuitems);

            ni.ContextMenu = cm;

            GetAppData();

            worker.DoWork += Worker_DoWork;
            worker.WorkerSupportsCancellation = true;
            workerloader.DoWork += Workerloader_DoWork;
            workerloader.RunWorkerCompleted += Workerloader_RunWorkerCompleted;


            if (Usertoken != null && Usertoken != @"")
            {
                TokenInput.Text = Usertoken;
            }
            else
            {
                Showmsg(Colors.Red, @"No valid user token installed!", @"CLR", false, "attention");
                ToastShowCheck(@"No valid user token installed!");
            }

            if (appsettings.Minimized)
            {
                Hide();
                ShowInTaskbar = true;
                WindowState = WindowState.Minimized;
            }
        }

        //Operations conducted every 2 seconds

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            if (Directory.Exists(GenPath(true)))
            {
                while (!worker.CancellationPending)
                {
                    try
                    {
                        if (!trawarn)
                        {
                            string[] enemies = { "LotusTracker", "MTGATracker", "MTG-Arena-Tool", "MTG Arena Tool" };
                            bool warn = false;
                            for (var i = 0; i < enemies.Length; i++)
                            {
                                Process[] enemylocator = Process.GetProcessesByName(enemies[i]);
                                if (enemylocator.Length > 1)
                                {
                                    warn = true;
                                    break;
                                }
                            }

                            if (warn)
                            {
                                MessageBox.Show("Another MTGA tracker detected! Simultaneous use of several trackers may cause all of them work inconsistently and/or break normal game log cleanup process leading to log file bloating. We recommend you to use only one tracker to make records more accurate and tracking process more safe. You can continue to use several trackers, but you have been warned. Any bugreports related to tracking software coexistence will be ignored.","MTGA Pro Notice");
                                trawarn = true;
                            }
                        }

                        Process[] locator = Process.GetProcessesByName("MTGA");

                        Dispatcher.BeginInvoke(new ThreadStart(delegate
                        {
                            var activatedHandle = GetForegroundWindow();
                            gamefocused = false;

                            WindowInteropHelper wih = new WindowInteropHelper(win4);
                            IntPtr Handle = wih.Handle;

                            foreach (Process p in locator)
                            {
                                if (activatedHandle == p.MainWindowHandle || activatedHandle == Handle)
                                {
                                    gamefocused = true;
                                }
                            }
                        }));

                        if (locator.Length > 0)
                        {
                            if (gamestarted == false)
                            {
                                juststarted = true;
                                runtime = Convert.ToInt32(tmstmp());
                            }
                            gamestarted = true;
                            Showmsg(Colors.YellowGreen, @"Awaiting for updates...", @"", false, @"icon" + appsettings.Icon.ToString());

                            if ((gamefocused || ovlsettings.Streamer) && overlayactive && (ovlsettings.Decklist || TheMatch.IsDrafting || TheMatch.IsFighting))
                            {
                                Dispatcher.BeginInvoke(new ThreadStart(delegate
                                {
                                    try
                                    {
                                        if (overlayactive && !juststarted && !Window4.windowhidden && (ovlsettings.Decklist || TheMatch.IsDrafting || TheMatch.IsFighting))
                                        {
                                            if (win4.IsVisible == false)
                                            {
                                                win4.Show();
                                            }
                                            if (TheMatch.Hasnewdata || !Window4.wasshown) win4.updatelive();
                                            TheMatch.Hasnewdata = false;
                                        }
                                    }
                                    catch (Exception ee)
                                    {
                                        ErrReport(ee);
                                    }
                                }));
                            }
                            else
                            {
                                Dispatcher.BeginInvoke(new ThreadStart(delegate
                                {
                                    try
                                    {
                                        if (win4.IsVisible == true)
                                        {
                                            win4.Hide();
                                        }
                                    }
                                    catch (Exception ee)
                                    {
                                        ErrReport(ee);
                                    }
                                }));
                            }
                        }
                        else
                        {
                            if (runtime > 0)
                            {
                                gamerunningtimer = (Convert.ToInt32(tmstmp()) - runtime);
                            }
                            runtime = 0;
                            gamestarted = false;

                            try
                            {
                                FileInfo nfo = new FileInfo(GenPath(false));
                                long curloglen = nfo.Length;
                                if (curloglen > 314572800 && parsedtill > 0)
                                {
                                    File.Delete(GenPath(false));
                                    juststarted = false;
                                }
                            }
                            catch (Exception ee)
                            {
                                //ErrReport(ee);
                            }

                            try
                            {
                                Dispatcher.BeginInvoke(new ThreadStart(delegate
                                {
                                    if (win4.IsVisible == true)
                                    {
                                        win4.Hide();
                                    }
                                }));
                            }
                            catch (Exception ee)
                            {
                                ErrReport(ee);
                            }

                            Showmsg(Colors.Yellow, @"MTGA is not running!", @"CLR", false, @"icon" + appsettings.Icon.ToString());
                        }
                        

                        if (indicators.Length > 0)
                        {
                            if (!playerswith)
                            {
                                if (juststarted || gamestarted)
                                {
                                    Checklog();
                                }
                            }
                        }
                        else
                        {
                            Getindicators();
                        }
                    }
                    catch (Exception ee)
                    {
                        ErrReport(ee);
                    }
                    //Thread.Sleep(upltimers[appsettings.Upl]);
                    Thread.Sleep(1000);
                }
            }
            else
            {
                Showmsg(Colors.Red, "MTGA is not installed!", @"CLR", true, "attention");
            }
        }

        //Checking token
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            tokeninput = textBox.Text;

            if (!workerloader.IsBusy)
            {
                workerloader.RunWorkerAsync();
            }
        }

        private void Workerloader_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!worker.IsBusy && Usertoken!=@"" && Usertoken != null)
            {
                Showmsg(Colors.Green, @"Monitoring is OK.", @"", false, @"icon" + appsettings.Icon.ToString());
                worker.RunWorkerAsync();
            }
        }

        //Loading initial data on startup

        private void Workerloader_DoWork(object sender, DoWorkEventArgs e)
        {
            TimeZone localZone = TimeZone.CurrentTimeZone;
            DateTime currentDate = DateTime.Now;
            string currentOffset = localZone.GetUtcOffset(currentDate).TotalSeconds.ToString();

            var responseString = MakeRequest(new Uri(@"https://mtgarena.pro/mtg/donew.php"), new Dictionary<string, object> { { @"cmd", @"cm_userbytokenid" }, { @"cm_userbytokenid", tokeninput }, { @"version", version.ToString() }, { @"usertime", currentOffset } });
            if (responseString != @"ERRCONN")
            {
                var info = Newtonsoft.Json.JsonConvert.DeserializeObject<Response>(responseString);
            
                if (info != null)
                {
                    if (info.Status == "NO_USER")
                    {
                        Dispatcher.BeginInvoke(new ThreadStart(delegate {Token_msg.Text = @"User not found. Check your token";
                            Token_msg.Width = 200;
                            Token.Content = "";
                            Stream iconStream = Application.GetResourceStream(new Uri(@"pack://application:,,,/Resources/attention.ico")).Stream;
                            Icon = new BitmapImage(new Uri(@"pack://application:,,,/Resources/attention.ico"));
                            ni.Icon = new System.Drawing.Icon(iconStream);
                        }));
                        Usertoken="";
                        ouruid = "";
                    }
                    else if (info.Status == "DO_RESET")
                    {
                        try
                        {
                            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"MTGAproTracker");
                            try
                            {
                                DirectoryInfo dir = new DirectoryInfo(path);

                                foreach (FileInfo file in dir.EnumerateFiles("*.mtg"))
                                {
                                    file.Delete();
                                }
                            }
                            catch (Exception ee)
                            {
                                ErrReport(ee);
                            }
                            Registry.CurrentUser.DeleteSubKey(@"SOFTWARE\\MTGAProtracker");
                            ni.Visible = false;
                            ni.Dispose();
                            Process.Start(Application.ResourceAssembly.Location, "restarting");
                            Environment.Exit(0);
                        }
                        catch (Exception ee)
                        {
                            ErrReport(ee);
                            ni.Visible = false;
                            ni.Dispose();
                            Process.Start(Application.ResourceAssembly.Location, "restarting");
                            Environment.Exit(0);
                        }
                    }
                    else if(info.Status == "BAD_TOKEN")
                    {
                        Dispatcher.BeginInvoke(new ThreadStart(delegate {Token_msg.Text = @"Check your token";
                            Token_msg.Width = 200;
                            Token.Content = "";
                            Stream iconStream = Application.GetResourceStream(new Uri(@"pack://application:,,,/Resources/attention.ico")).Stream;
                            Icon = new BitmapImage(new Uri(@"pack://application:,,,/Resources/attention.ico"));
                            ni.Icon = new System.Drawing.Icon(iconStream);
                        }));
                        Usertoken = "";
                    }
                    else
                    {
                        Usertoken = tokeninput;
                        ouruid = info.Data;
                        if(!Utokens.ContainsKey(ouruid))
                        {
                            Utokens.Add(ouruid, Usertoken);
                            if (mtgauid!="")
                            {
                                Usermtgaid.Add(ouruid, mtganick);
                                Credentials.Add(ouruid, mtgauid);
                            }
                        }
                        else
                        {
                            Utokens[ouruid] = Usertoken;
                        }
                        
                        SetAppData();
                        playerswith = false;

                        Dispatcher.BeginInvoke(new ThreadStart(delegate {
                            Token.Content = info.Status;
                            Token_msg.Width = 103;
                            Token.Width = 163;
                            Token_msg.Text = @"Detected user: ";
                            Stream iconStream = Application.GetResourceStream(new Uri(@"pack://application:,,,/Resources/icon" + appsettings.Icon + ".ico")).Stream;
                            Icon = new BitmapImage(new Uri(@"pack://application:,,,/Resources/icon" + appsettings.Icon + ".ico"));
                            ni.Icon = new System.Drawing.Icon(iconStream);
                        }));                      
                    }
                }
            }
            else
            {
                Dispatcher.BeginInvoke(new ThreadStart(delegate {
                    Token_msg.Text = @"Error validating token";
                    Token_msg.Width = 200;
                    Token.Content = "";
                    Stream iconStream = Application.GetResourceStream(new Uri(@"pack://application:,,,/Resources/attention.ico")).Stream;
                    Icon = new BitmapImage(new Uri(@"pack://application:,,,/Resources/attention.ico"));
                    ni.Icon = new System.Drawing.Icon(iconStream);
                }));
                Usertoken = "";
                ouruid = "";
            }
        }

        //Open link
        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }


        protected override void OnStateChanged(EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
            {
                Hide();
                ShowInTaskbar = false;
            }
            else
            {
                ShowInTaskbar = true;
            }

            base.OnStateChanged(e);
        }

       

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            ShowInTaskbar = true;
            WindowState = WindowState.Minimized;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //throw new ArgumentException("Parameter cannot be null", "original");
            try
            {
                Window1 win1 = new Window1();
                win1.Show();
            }
            catch (Exception ee)
            {
                ErrReport(ee);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"MTGAproTracker");
                try
                {
                    //throw new Exception();
                    DirectoryInfo dir = new DirectoryInfo(path);

                    foreach (FileInfo file in dir.EnumerateFiles("*.mtg"))
                    {
                        file.Delete();
                    }
                }
                catch (Exception ee)
                {
                    ErrReport(ee);
                }
                Registry.CurrentUser.DeleteSubKey(@"SOFTWARE\\MTGAProtracker");
                ni.Visible = false;
                ni.Dispose();
                Process.Start(Application.ResourceAssembly.Location, "restarting");
                Environment.Exit(0);                
            }
            catch(Exception)
            {
                MessageBox.Show("There is no app data stored, so no need to do reset!");
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            try
            {
                ni.Visible = false;
                ni.Dispose();
                Environment.Exit(0);
            }
            catch (Exception ee)
            {
                ErrReport(ee);
            }
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            try
            {
                Window2 win2 = new Window2();
                win2.Show();
            }
            catch (Exception ee)
            {
                ErrReport(ee);
            }
        }


        private void Button_Click_7(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!overlayactive)
                {
                    overlayactive = true;
                    ovactbut.Text = "Disable In-game overlay";
                }
                else
                {
                    overlayactive = false;
                    ovactbut.Text = "Activate In-game overlay";
                    if (win4.IsVisible == true)
                    {
                        win4.Hide();
                    }
                }
                SetAppData();
            }
            catch (Exception ee)
            {
                ErrReport(ee);
            }
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            try
            {
                Button btn = sender as Button;

                string curpath = Directory.GetCurrentDirectory();
                string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"MTGAproTracker");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                using (WebClient myWebClient = new WebClient())
                {
                    myWebClient.DownloadFileAsync(new Uri(@"https://teslegends.pro/mtg/tracker/latest.zip"), path + @"\update.zip");
                    myWebClient.DownloadFileCompleted += MyWebClient_DownloadFileCompleted;
                    btn.Content = "Loading...";
                }
            }
            catch (Exception ee)
            {
                ErrReport(ee);

            }
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            juststarted = true;
            hashes = new string[indicators.Length];
            loglen = 0;
            parsedtill = 0;
            manualresync = true;
            Showmsg(Colors.Green, @"Launching re-sync...", @"", false, @"icon" + appsettings.Icon.ToString());
        }

        static string ProgramFilesx86()
        {
            if (8 == IntPtr.Size
                || (!String.IsNullOrEmpty(Environment.GetEnvironmentVariable("PROCESSOR_ARCHITEW6432"))))
            {
                return Environment.GetEnvironmentVariable("ProgramFiles(x86)");
            }

            return Environment.GetEnvironmentVariable("ProgramFiles");
        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog
            {
                DefaultExt = ".htm",
                FileName = "Log*.htm",
                Filter = "MTGA Logs|output_log.txt"
            };

            string path = ProgramFilesx86();

            path += @"\Wizards of the Coast\MTGA\MTGA_Data\Logs\Logs";
            if (Directory.Exists(path))
            {
                dlg.InitialDirectory = path;
            }

            var result = dlg.ShowDialog();


            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                tmplog = dlg.FileName;
                juststarted = true;
                hashes = new string[indicators.Length];
                loglen = 0;
                parsedtill = 0;
                manualresync = true;
                Showmsg(Colors.Green, @"Opening the old log...", @"", false, @"icon" + appsettings.Icon.ToString());
            }
        }

        private void Messenger_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Window3 win3 = new Window3();
                Messenger.Visibility = Visibility.Hidden;
                win3.Show();
            }
            catch (Exception ee)
            {
                ErrReport(ee);
            }
        }

        private void MyWebClient_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            string curpath = Directory.GetCurrentDirectory();
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"MTGAproTracker");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            if (Directory.Exists(path + @"\update"))
            {
                Directory.Delete(path + @"\update", true);
            }

            ZipFile.ExtractToDirectory(path + @"\update.zip", path + @"\update\");
            Process.Start(path + @"\update\MTGApro_Installer.msi");
            Environment.Exit(0);
        }


        public void CrashHandler(object sender, UnhandledExceptionEventArgs args)
        {
            Exception ee = (Exception)args.ExceptionObject;

            ErrReport(ee);

            RegistryKey RkApp = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\\MTGAProtracker", true);
            if (RkApp == null)
            {
                RkApp = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\\MTGAProtracker", true);
            }

            var attempts = RkApp.GetValue("attempts").ToString();
            int at = 0;
            if (attempts != null)
            {
                at = Int32.Parse(attempts);
            } 

            if (at <= 3)
            {
                at++;
                RkApp.SetValue("attempts", at.ToString());
                Process.Start(Application.ResourceAssembly.Location, "restarting");
                Environment.Exit(0);
            }
        }

        private void Border_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        #region dragmove
        private bool inDrag = false;
        private Point anchorPoint;
        private bool iscaptured = false;

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            anchorPoint = PointToScreen(e.GetPosition(this));
            inDrag = true;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (inDrag)
            {
                if (!iscaptured)
                {
                    CaptureMouse();
                    iscaptured = true;
                }
                Point currentPoint = PointToScreen(e.GetPosition(this));
                this.Left = this.Left + currentPoint.X - anchorPoint.X;
                this.Top = this.Top + currentPoint.Y - anchorPoint.Y;
                anchorPoint = currentPoint;
            }
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (inDrag)
            {
                inDrag = false;
                iscaptured = false;
                ReleaseMouseCapture();
            }
        }

        #endregion
    }
}
