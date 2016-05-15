using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Diagnostics;
using System.Threading;
using System.Runtime.CompilerServices;
using System.ComponentModel;

namespace XposedModuleScraper
{
    class Program
    {

        #region Declarations

        static int modules;
        static int listingPages;
        static WebClient webClient = new WebClient();
        static string UrlTemplate = "http://repo.xposed.info/module-overview?combine=&status=All&field_restrict_edits_value=All&sort_by=field_last_update_value&page=";
        static List<ModuleDetails> allModules = new List<ModuleDetails>();

        class ModuleDetails
        {
            public int moduleNumber;
            public string uid;
            public string title;
            public string moduleUrl;
            public string html;
            public string apkUrl;
            public string fileName;
            public string description;
            public int bytes;
            public bool HaveApk;
        };



        #endregion

        #region Methods
        static void SetModuleDetails()
        {
            for (int i = 0; i < allModules.Count; i++)
            {
                if (allModules[i].html.Contains("External") || allModules[i].html == null)
                {
                }
                else
                {
                    string modulebytestemp;
                    allModules[i].apkUrl = allModules[i].html.Substring(allModules[i].html.IndexOf("<span class=\"file\"><a href=\"") + "<span class=\"file\"><a href=\"".Length);
                    allModules[i].apkUrl = allModules[i].apkUrl.Remove(allModules[i].apkUrl.IndexOf("\" type=\"application/octet-stream; length"));
                    modulebytestemp = allModules[i].html.Substring(allModules[i].html.IndexOf("\" type=\"application/octet-stream; length=") + "\" type=\"application/octet-stream; length=".Length);
                    allModules[i].bytes = Convert.ToInt32(modulebytestemp.Remove(modulebytestemp.IndexOf("\">")));
                    allModules[i].fileName = MakeValidFileName(allModules[i].title);
                }
            }

        }

        static void SetDetails()
        {

            ServicePointManager.DefaultConnectionLimit = 1000;
            string FirstPageSource = webClient.DownloadString(UrlTemplate + 0);
            modules = Convert.ToInt32(FirstPageSource.Substring(FirstPageSource.IndexOf("Displaying 1 - 10 of") + 20, 18).Remove(FirstPageSource.Substring(FirstPageSource.IndexOf("Displaying 1 - 10 of") + 20, 18).IndexOf(" mod")).Trim());

            if ((modules % 10) == 0)
                listingPages = modules / 10;
            else
                listingPages = ((modules - (modules % 10)) / 10) + 1;

        }

        static char[] _invalids;
        public static string MakeValidFileName(string text, char? replacement = '_', bool fancy = true)
        {
            StringBuilder sb = new StringBuilder(text.Length);
            var invalids = _invalids ?? (_invalids = Path.GetInvalidFileNameChars());
            bool changed = false;
            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                if (invalids.Contains(c))
                {
                    changed = true;
                    var repl = replacement ?? '\0';
                    if (fancy)
                    {
                        if (c == '"') repl = '”'; // U+201D right double quotation mark
                        else if (c == '\'') repl = '’'; // U+2019 right single quotation mark
                        else if (c == '/') repl = '⁄'; // U+2044 fraction slash
                    }
                    if (repl != '\0')
                        sb.Append(repl);
                }
                else
                    sb.Append(c);
            }
            if (sb.Length == 0)
                return "_";
            return changed ? sb.ToString() : text;
        }


        public static void DownloadUrlsInParallel1(int it)
        {
            List<string> html = new List<string>();
            if (it == 0)
            {
                List<Uri> urls = new List<Uri>();
                for (int i = 0; i < allModules.Count; i++)
                {
                    urls.Add(new Uri("http://repo.xposed.info/module/" + allModules[i].moduleUrl));
                }
                List<string> htmlListingPages = DownloadUrlsInParallel(urls.ToArray());
                for (int i = 0; i < allModules.Count; i++)
                {
                    allModules[i].html = htmlListingPages[i];
                }
            }
        }
        public static List<string> DownloadUrlsInParallel(Uri[] urls)
        {
            var tasks = urls
                .Select(url => Task.Factory.StartNew(
                    state =>
                    {
                        using (var client = new System.Net.WebClient())
                        {
                            var u = (Uri)state;
                            string result = client.DownloadString(u);
                            return result;
                        }
                    }, url)
                )
                .ToArray();

            Task.WaitAll(tasks);
            List<string> ret = new List<string>();
            foreach (var t in tasks)
            {
                ret.Add(t.Result);
            }
            return ret;
        }




        public static void DownloadFilesInParallel()
        {
            List<int> apknumber = new List<int>();
            for (int i = 0; i < allModules.Count; i++)
            {
                if (allModules[i].apkUrl != null)
                {
                    apknumber[i] = i;
                }
            }
            var tasks = apknumber
                .Select(urlfilename => Task.Factory.StartNew(
                    state =>
                    {
                        using (var client = new System.Net.WebClient())
                        {
                            var u = (int)state;
                            if (allModules[u].apkUrl != null)
                            {
                                client.DownloadFile(allModules[u].apkUrl, MakeValidFileName(allModules[u].fileName));
                            }
                        }
                    }, urlfilename)
                )
                .ToArray();

            Task.WaitAll(tasks);
            foreach (var t in tasks)
            {
                Console.WriteLine(t.IsCompleted ? "y" : "n");
            }
        }
        #endregion
        static void Main(string[] args)
        {
            SetDetails();
            Uri[] uri = new Uri[listingPages];
            for (int i = 0; i < listingPages; i++)
            {
                uri[i] = new Uri(UrlTemplate + i);
            }
            List<string> htmlListingPages = DownloadUrlsInParallel(uri);
            for (int i = 0; i < htmlListingPages.Count; i++)
            {
                string html = htmlListingPages[i];
                string temp1 = " ", temp2 = " ";
                temp1 = html;
                while ((temp1.Contains("<a href=\"/module/")))
                {
                    temp2 = temp1.Substring(temp1.IndexOf("<a href=\"/module/") + 17);
                    temp1 = temp2.Substring(temp2.IndexOf("</a>") + 4);
                    temp2 = temp2.Remove(temp2.IndexOf("</a>"));
                    string moduleUrl = temp2.Remove(temp2.IndexOf("\""));

                    if (moduleUrl != "de.robv.android.xposed.installer")
                    {
                        ModuleDetails moduleDetails = new ModuleDetails();
                        moduleDetails.moduleUrl = moduleUrl;
                        moduleDetails.title = temp2.Substring(temp2.IndexOf(">") + 1);
                        allModules.Add(moduleDetails);
                    }
                }

            }

            DownloadUrlsInParallel1(0);
            for (int i = 0; i < allModules.Count; i++)
            {
                SetModuleDetails();
            }

            DownloadFilesInParallel();

        }
    }
}
