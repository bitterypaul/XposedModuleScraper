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


        //temporary variables
        string ahtml = " ";

        class ModuleDetails
        {
            public int moduleNumber;
            public string uid;
            public string title;
            public string apkUrl;
            public string description;
            public int bytes;
        };

        #endregion

        #region Methods
        static ModuleDetails GetModuleDetails(string moduleUrl)
        {
            ModuleDetails moduleDetails = new ModuleDetails();
            string html = webClient.DownloadString(moduleUrl);
            string modulebytestemp;
            //Console.WriteLine();
            moduleDetails.apkUrl = html.Substring(html.IndexOf("<span class=\"file\"><a href=\"") + "<span class=\"file\"><a href=\"".Length);
            moduleDetails.apkUrl = moduleDetails.apkUrl.Remove(moduleDetails.apkUrl.IndexOf("\" type=\"application/octet-stream; length"));
            modulebytestemp = html.Substring(html.IndexOf("\" type=\"application/octet-stream; length=") + "\" type=\"application/octet-stream; length=".Length);
            moduleDetails.bytes = Convert.ToInt32(modulebytestemp.Remove(modulebytestemp.IndexOf("\">")));
            
            return moduleDetails;
        }

        static List<ModuleDetails> GetModuleDetailsFromListingPage(string listingPage)
        {
            List<ModuleDetails> modulesDetails = new List<ModuleDetails>();


            return modulesDetails;
        }
        static void SetDetails()
        {
            string FirstPageSource = webClient.DownloadString(UrlTemplate + 0);
            modules = Convert.ToInt32(FirstPageSource.Substring(FirstPageSource.IndexOf("Displaying 1 - 10 of") + 20, 18).Remove(FirstPageSource.Substring(FirstPageSource.IndexOf("Displaying 1 - 10 of") + 20, 18).IndexOf(" mod")).Trim());
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
            ModuleDetails mm = GetModuleDetails("http://repo.xposed.info/module/xyz.paphonb.cmvisualizer");
            Console.WriteLine(mm.apkUrl);
            Console.WriteLine(mm.bytes);
            Console.ReadLine();




            Console.WriteLine(modules);
            modules = 115;
#endif
            #endregion

            //#region retrieve all pages
            // retrieve all pages each of which displays 10 modules including the last page which can display 1,2,or even 10 modules
            //i < pages is used because the first page is already downloaded and the total number of pages to be fetched is one less than total number of pages
//            for (i=1; i < pages;i++)
//            {
//                sb.Append(webClient.DownloadString(UrlTemplate + i));
//                Thread.Sleep(delayBetweenRequests);
//            }
//            #endregion
//            sb.Replace(@"<a href=""/module/de.robv.android.xposed.installer", " ");
//            string sss = sb.ToString();

//#if DEBUG 

//            File.AppendAllText("htmlt.txt", sss);
//            Console.ReadLine();

//#endif
//            string temp3 = " ", temp4= " ";
//            temp1 = sss;

//            i = 0;
//            while((temp1.Contains(@"<a href=""/module/")))
//            {
//                i++;
//                temp2 = temp1.Substring(temp1.IndexOf(@"<a href=""/module/") + 17);
//                temp1 = temp2.Substring(temp2.IndexOf("</a>") + 4);
//                temp2 = temp2.Remove(temp2.IndexOf("</a>"));
//                temp3 = temp2.Substring(temp2.IndexOf(">") + 1);
//                temp4 = temp2.Remove(temp2.IndexOf("\""));
//                appIdNames.Add(temp4, temp3);
//            }
            
//            foreach(KeyValuePair<string,string> appidname in appIdNames)
//            {
//                Console.WriteLine(appidname.Key);

//                Console.WriteLine(appidname.Value);
//            }
         
//            for (i = 0; i < appIdNames.Count; i++)
//            {
//                KeyValuePair<string, string> appidname = appIdNames.ElementAt(i);
//                appIdhtml.Add(appidname.Value,webClient.DownloadString("http://repo.xposed.info/module/" + appidname.Key));
//                //Thread.Sleep(delayBetweenRequests);
//            }
//            i = 0;
//            int lengthk = 0;
//            string leen = " ", urll =  " ";
//            foreach(KeyValuePair<string,string> appidhtml in appIdhtml)
//            {
//                temp1 = appidhtml.Value;
//                temp1 = temp1.Substring(temp1.IndexOf(@"<span class=""file""><a href=""") + 28);
//                temp2 = temp1.Remove(temp1.IndexOf("</a>"));
//                leen = temp2.Substring(temp2.IndexOf("length=") +7 );
//                leen = leen.Remove(leen.IndexOf("\">"));
//                urll = temp2.Remove(temp2.IndexOf(@""" type=""application/octet-stream; length="));
//            }
//            [MethodImpl(MethodImplOptions.AggressiveInlining)]


    }
        
    }
}
