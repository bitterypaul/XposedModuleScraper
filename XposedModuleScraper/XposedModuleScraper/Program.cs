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
            public string apkUrl;
            public string description;
            public int bytes;

            public ModuleDetails()
            {

            }
            public ModuleDetails(string apkUrl, string description)
            {
                this.apkUrl = apkUrl;
                this.description = description;
            }
        };


        #endregion

        #region Methods
        static ModuleDetails GetModuleDetails(string moduleUrl)
        {
            ModuleDetails moduleDetails = new ModuleDetails();
            string html = webClient.DownloadString(moduleUrl);
            string modulebytestemp;
            moduleDetails.apkUrl = html.Substring(html.IndexOf("<span class=\"file\"><a href=\"") + "<span class=\"file\"><a href=\"".Length);
            moduleDetails.apkUrl = moduleDetails.apkUrl.Remove(moduleDetails.apkUrl.IndexOf("\" type=\"application/octet-stream; length"));
            modulebytestemp = html.Substring(html.IndexOf("\" type=\"application/octet-stream; length=") + "\" type=\"application/octet-stream; length=".Length);
            moduleDetails.bytes = Convert.ToInt32(modulebytestemp.Remove(modulebytestemp.IndexOf("\">")));
            return moduleDetails;
        }

        static void DisplayModuleDetails(List<ModuleDetails> modulesDetails)
        {
            for(int i = 0; i < 10; i++)
            {

                Console.WriteLine("\n");
                Console.WriteLine(modulesDetails.ElementAt(i).apkUrl);
                Console.WriteLine(modulesDetails.ElementAt(i).bytes);
                Console.WriteLine("\n");
            }
        }
        static List<ModuleDetails> GetModuleDetailsFromListingPage(string listingPageUrl)
        {
            List<ModuleDetails> modulesDetails = new List<ModuleDetails>();
            string html = webClient.DownloadString(listingPageUrl);
            html.Replace("/module/de.robv.android.xposed.installer", " ");
            string temp1 = " ", temp2= " ";
            temp1 = html;
            while ((temp1.Contains("<a href=\"/module/")))
            {
                temp2 = temp1.Substring(temp1.IndexOf("<a href=\"/module/") + 17);
                temp1 = temp2.Substring(temp2.IndexOf("</a>") + 4);
                temp2 = temp2.Remove(temp2.IndexOf("</a>"));
                allModules.Add(new ModuleDetails(temp2.Substring(temp2.IndexOf(">") + 1), temp2.Remove(temp2.IndexOf("\""))));
            }
            return modulesDetails;
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

            allModules.AddRange(GetModuleDetailsFromListingPage(UrlTemplate + 0));
            DisplayModuleDetails(allModules);

            Console.ReadLine();

#endif

            // retrieve all pages each of which displays 10 modules including the last page which can display 1,2,or even 10 modules
            //i < listingpages is used because the first page is is 0 total number of pages to be fetched is one less than total number of pages
            //          
            for (int i = 0; i < listingPages;i++)
            {
                allModules.AddRange(GetModuleDetailsFromListingPage(UrlTemplate + i));
            }
            #endregion


        }

    }
}
