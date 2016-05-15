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
        static int delayBetweenRequests = 1; //Time delay between successive requests to prevent being locked out from the system
        static List<string> htmlListingPages = new List<string>();
        static int threads = 10;
        static WebClient webClient = new WebClient();
        static string UrlTemplate = "http://repo.xposed.info/module-overview?combine=&status=All&field_restrict_edits_value=All&sort_by=field_last_update_value&page=";
        static List<ModuleDetails> allModules = new List<ModuleDetails>();
        static List<WebClient> webclientThreads = new List<WebClient>();

        class ModuleDetails
        {
            public int moduleNumber;
            public string uid;
            public string title;
            public string moduleUrl;
            public string apkUrl;
            public string fileName;
            public string description;
            public int bytes;
            public bool HaveApk;
        };


        #endregion

        #region Methods
        static ModuleDetails GetModuleDetails(ModuleDetails moduleDetails)
        {
            string html = webClient.DownloadString("http://repo.xposed.info/module/" + moduleDetails.moduleUrl);
            if (html.Contains("External"))
            {
                Console.WriteLine("cannot Download " + moduleDetails.moduleUrl);
                ModuleDetails moduleDe = new ModuleDetails();
                moduleDe.title = "rgvergv";
                moduleDe.apkUrl = "rgverv eve";
                return moduleDe;
            }
            else
            {
                string modulebytestemp;
                moduleDetails.apkUrl = html.Substring(html.IndexOf("<span class=\"file\"><a href=\"") + "<span class=\"file\"><a href=\"".Length);
                moduleDetails.apkUrl = moduleDetails.apkUrl.Remove(moduleDetails.apkUrl.IndexOf("\" type=\"application/octet-stream; length"));
                modulebytestemp = html.Substring(html.IndexOf("\" type=\"application/octet-stream; length=") + "\" type=\"application/octet-stream; length=".Length);
                moduleDetails.bytes = Convert.ToInt32(modulebytestemp.Remove(modulebytestemp.IndexOf("\">")));
                Console.WriteLine("Downloading file " + allModules.Count + " of " + modules);
                Console.WriteLine(moduleDetails.apkUrl);
                moduleDetails.fileName = MakeValidFileName(moduleDetails.title);

                html = " ";
                return moduleDetails;
            }

        }

        static void GetModuleDetailsFromListingPage(string listingPageUrl)
        {


        }
        static void SetDetails()
        {

            ServicePointManager.DefaultConnectionLimit = 1000;
            string FirstPageSource = webClient.DownloadString(UrlTemplate + 0);
            modules = Convert.ToInt32(FirstPageSource.Substring(FirstPageSource.IndexOf("Displaying 1 - 10 of") + 20, 18).Remove(FirstPageSource.Substring(FirstPageSource.IndexOf("Displaying 1 - 10 of") + 20, 18).IndexOf(" mod")).Trim());
#if DEBUG 
            Console.WriteLine("Please enter number of modules to be downloaded.");
            modules = Convert.ToInt32(Console.ReadLine());
#endif

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

        public static List<string> DownloadUrlsInParallel(Uri[] urls)
        {
            var tasks = urls
                .Select(url => Task.Factory.StartNew(
                    state =>
                    {
                        using (var client = new System.Net.WebClient())
                        {
                            var u = (Uri)state;
                            Console.WriteLine("starting to download {0}", u);
                            string result = client.DownloadString(u);
                            Console.WriteLine("finished downloading {0}", u);
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

                string temp1 = " ", temp2 = " ";
                temp1 = t.Result;
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
                        allModules.Add(GetModuleDetails(moduleDetails));
                    }
                }

            }
            return ret;
        }
        #endregion
        static void Main(string[] args)
        {
            ServicePointManager.DefaultConnectionLimit = 1000;

            var watch = Stopwatch.StartNew();

            Uri[] uri = new Uri[87];

            for (int i = 0; i < 87; i++)
            {
                uri[i] = new Uri(UrlTemplate + i);

            }

            DownloadUrlsInParallel(uri);

            Console.WriteLine("async: " + watch.Elapsed);

            //for (int i = 0; i < 10; i++)
            //{
            //    webclientThreads.Add(new WebClient());
            //    webclientThreads[i].DownloadStringCompleted += (sender, e) =>
            //    {
            //        string pageSourceCode = e.Result;
            //        htmlListingPages.Add(pageSourceCode);

            //    };

            //}
            //for (int i = 0; i < 80; i++)
            //{
            //    if (i > 10)
            //    {
            //        for(;;)
            //        {
            //            if (!(webclientThreads[i %10].IsBusy))
            //            {
            //                webclientThreads[i %10].DownloadStringAsync(new Uri(UrlTemplate + i));
            //                break;
            //            }
            //            else
            //                Thread.Sleep(300);
            //        }
            //    }


            //}

            Console.ReadLine();
            //for (int i = 0; i < threads; i++)
            //{
            //    webclientThreads.Add(new WebClient());
            //    webclientThreads[i].DownloadStringCompleted += (sender, e) =>
            //    {
            //        string pageSourceCode = e.Result;
            //        string temp1 = " ", temp2 = " ";
            //        temp1 = pageSourceCode;
            //        while ((temp1.Contains("<a href=\"/module/")))
            //        {
            //            temp2 = temp1.Substring(temp1.IndexOf("<a href=\"/module/") + 17);
            //            temp1 = temp2.Substring(temp2.IndexOf("</a>") + 4);
            //            temp2 = temp2.Remove(temp2.IndexOf("</a>"));
            //            string moduleUrl = temp2.Remove(temp2.IndexOf("\""));
            //            if (moduleUrl != "de.robv.android.xposed.installer")
            //            {
            //                ModuleDetails moduleDetails = new ModuleDetails();
            //                moduleDetails.moduleUrl = moduleUrl;
            //                moduleDetails.title = temp2.Substring(temp2.IndexOf(">") + 1);
            //                allModules.Add(GetModuleDetails(moduleDetails));
            //            }
            //        }

            //    };

            //}


            //webclientThreads[i].DownloadStringAsync(new Uri(UrlTemplate + i));



            //var count = 100;
            //var root = "http://repo.xposed.info/module-overview?combine=&status=All&field_restrict_edits_value=All&sort_by=field_last_update_value&page=";

            //ServicePointManager.DefaultConnectionLimit = 1000;
            //DateTime tt = DateTime.Now;
            //DateTime aa = DateTime.Now;
            //var watch = Stopwatch.StartNew();
            //var completed = new CountdownEvent(count);

            //TimeSpan ee = tt - aa;
            //List<WebClient> webclientThreads = new List<WebClient>();

            //for (int i = 0; i < count; i++)
            //{
            //    webclientThreads.Add(new WebClient());
            //}
            //for (int i = 0; i < count; i++)
            //{

            //    webclientThreads[i].DownloadStringCompleted += (sender, e) =>
            //    {
            //        string pageSourceCode = e.Result;
            //        //aa = DateTime.Now;
            //        //ee = tt - aa;

            //        Console.WriteLine("async: " + i);
            //    };

            //}

            //for (int i = 0; i < count; i++)
            //{
            //    webclientThreads[i].DownloadStringAsync(new Uri(UrlTemplate + i));
            //}

            //ee = tt - aa;
            //Console.WriteLine("async: " + ee.ToString());


            #region all
            Console.ReadLine();
            #region initialSetup
            SetDetails();


#if DEBUG

            Console.WriteLine("DEBUG Session");

#endif

            // retrieve all pages each of which displays 10 modules including the last page which can display 1,2,or even 10 modules
            //i < listingpages is used because the first page is is 0 total number of pages to be fetched is one less than total number of pages
            //          
            for (int i = 0; i < listingPages; i++)
            {
                GetModuleDetailsFromListingPage(UrlTemplate + i);

                Thread.Sleep(1000);
            }
            #endregion


            int allModulesCount = allModules.Count;
            int bytes = 0;
            Console.WriteLine("All details fetched.\n Starting download\n");
            for (int i = 0; i < allModulesCount; i++)
            {
                Console.WriteLine("Downloading File " + i + " of " + allModulesCount);
                webClient.DownloadFile(new Uri(allModules.ElementAt(i).apkUrl, UriKind.Absolute), allModules.ElementAt(i).fileName);
                Console.WriteLine("File " + allModules.ElementAt(i).fileName + " downloaded");
                bytes += allModules.ElementAt(i).bytes;
                Console.WriteLine("Size: " + (bytes / 1024));

            }
            Console.WriteLine((bytes / 1024 / 1024) + " MB");

#if DEBUG

            Console.ReadLine();
#endif

            #endregion
        }
    }
}
