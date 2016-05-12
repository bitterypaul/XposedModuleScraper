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


namespace XposedModuleScraper
{
    class Program
    {

        #region Declarations

        static int modules;
        static int listingPages;
        static int delayBetweenRequests = 1; //Time delay between successive requests to prevent being locked out from the system
        static int iterationVariable = 0;
        static WebClient webClient = new WebClient();
        static string UrlTemplate = "http://repo.xposed.info/module-overview?combine=&status=All&field_restrict_edits_value=All&sort_by=field_last_update_value&page=";
        static List<ModuleDetails> allModules = new List<ModuleDetails>();
        class ModuleDetails
        {
            public int moduleNumber;
            public string uid;
            public string title;
            public string moduleUrl;
            public string apkUrl;
            public string description;
            public int bytes;
            public bool HaveApk;
        };


        #endregion

        #region Methods
        static ModuleDetails GetModuleDetails(ModuleDetails moduleDetails)
        {
            string html = webClient.DownloadString("http://repo.xposed.info/module/" + moduleDetails.moduleUrl);
            if(html.Contains("External"))
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
                Console.WriteLine(moduleDetails.apkUrl);
                html = " ";
                return moduleDetails;
            }

        }

        static void DisplayModuleDetails()
        {
            for(int i = 0; i < allModules.Count ; i++)
            {
                Console.WriteLine("\n");
                Console.WriteLine(allModules.ElementAt(i).moduleUrl);
                Console.WriteLine(allModules.ElementAt(i).apkUrl);
                Console.WriteLine(allModules.ElementAt(i).bytes);
                Console.WriteLine("\n");
            }
        }
        static void LogApkUrls()
        {
            StringBuilder sb = new StringBuilder();
            for(int i = 0; i < allModules.Count; i++)
            {
                sb.AppendLine(allModules.ElementAt(i).apkUrl);
            }
            string sbb = sb.ToString();
            //File.AppendAllText("C:\\Scraped\\uri.txt", sb.ToString());
           // File.AppendAllText("C:\\Scraped\\a.txt", sbb);
        }

        static void GetModuleDetailsFromListingPage(string listingPageUrl)
        {
            string html = webClient.DownloadString(listingPageUrl);
            html.Replace("module/de.robv.android.xposed.installer", " ");
            string temp1 = " ", temp2= " ";
            temp1 = html;
            while ((temp1.Contains("<a href=\"/module/")))
            {
                temp2 = temp1.Substring(temp1.IndexOf("<a href=\"/module/") + 17);
                temp1 = temp2.Substring(temp2.IndexOf("</a>") + 4);
                temp2 = temp2.Remove(temp2.IndexOf("</a>"));
                string moduleUrl = temp2.Remove(temp2.IndexOf("\""));
                if(moduleUrl != "de.robv.android.xposed.installer")
                {
                    ModuleDetails moduleDetails = new ModuleDetails();
                    moduleDetails.moduleUrl = moduleUrl;
                    moduleDetails.title = temp2.Substring(temp2.IndexOf(">") + 1);
                    allModules.Add(GetModuleDetails(moduleDetails));
                    //File.AppendAllText("C:\\scc\\txt.txt", moduleUrl);
                }
            }
            Console.WriteLine("Number of Download Links fetched: " + allModules.Count);

        }
        static void SetDetails()
        {
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

        #endregion
        static void Main(string[] args)
        {
            #region initialSetup
            SetDetails();


#if DEBUG

            Console.WriteLine("DEBUG Session");
            

#endif

            // retrieve all pages each of which displays 10 modules including the last page which can display 1,2,or even 10 modules
            //i < listingpages is used because the first page is is 0 total number of pages to be fetched is one less than total number of pages
            //          
            for (int i = 0; i < listingPages;i++)
            {
                GetModuleDetailsFromListingPage(UrlTemplate + i);
                
                Thread.Sleep(1000);
            }
            #endregion

#if DEBUG
            
#endif


        }

    }
}
