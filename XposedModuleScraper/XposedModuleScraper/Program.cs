using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Diagnostics;
using System.Threading;

namespace XposedModuleScraper
{
    class Program
    {

        static void Main(string[] args)
        {
            #region Declarations
            //strings
            string FirstPageSource;                         //HTML Source Code for first page. We can get number of modules from this string.
            string temp1, temp2;                            //temporary strings to perform string operations.
            string UrlTemplate = "http://repo.xposed.info/module-overview?combine=&status=All&field_restrict_edits_value=All&sort_by=field_last_update_value&page=";
            //List<string>
            StringBuilder sb = new StringBuilder();
            List<string> apphtml = new List<string>();   // one for 
            Dictionary<string, string> appIdNames = new Dictionary<string, string>();
            Dictionary<string, string> appIdhtml = new Dictionary<string, string>();
            //structs


            //integers
            int pages;                                      //number of pages.
            int modules;                                    //number of modules.
            int i;                                          //iteration variable.
            int delayBetweenRequests = 1;                       //Time delay between successive requests to prevent being locked out from the system
            //Objects
            WebClient webClient = new WebClient();          //Declare WebClient Object
            #endregion
            #region Get First Page Source determine number of modules and pages
            FirstPageSource = webClient.DownloadString(UrlTemplate + 0);
            sb.Append(FirstPageSource);
            modules = Convert.ToInt32(FirstPageSource.Substring(FirstPageSource.IndexOf("Displaying 1 - 10 of") + 20, 18).Remove(FirstPageSource.Substring(FirstPageSource.IndexOf("Displaying 1 - 10 of") + 20, 18).IndexOf(" mod")).Trim());
#if DEBUG
            Console.WriteLine(modules);
            modules = 115;
#endif

            if ((modules % 10) == 0)
                pages = modules / 10;
            else
                pages = ((modules - (modules % 10)) / 10) + 1;
            #endregion
            #region retrieve all pages
            // retrieve all pages each of which displays 10 modules including the last page which can display 1,2,or even 10 modules
            //i < pages is used because the first page is already downloaded and the total number of pages to be fetched is one less than total number of pages
            for (i=1; i < pages;i++)
            {
                sb.Append(webClient.DownloadString(UrlTemplate + i));
                Thread.Sleep(delayBetweenRequests);
            }
            #endregion
            sb.Replace(@"<a href=""/module/de.robv.android.xposed.installer", " ");
            string sss = sb.ToString();

#if DEBUG 

            File.AppendAllText("htmlt.txt", sss);
            Console.ReadLine();

#endif
            string temp3 = " ", temp4= " ";
            temp1 = sss;

            i = 0;
            while((temp1.Contains(@"<a href=""/module/")))
            {
                i++;
                temp2 = temp1.Substring(temp1.IndexOf(@"<a href=""/module/") + 17);
                temp1 = temp2.Substring(temp2.IndexOf("</a>") + 4);
                temp2 = temp2.Remove(temp2.IndexOf("</a>"));
                temp3 = temp2.Substring(temp2.IndexOf(">") + 1);
                temp4 = temp2.Remove(temp2.IndexOf("\""));
                appIdNames.Add(temp4, temp3);
            }

#if DEBUG 
            foreach(KeyValuePair<string,string> appidname in appIdNames)
            {
                Console.WriteLine(appidname.Key);

                Console.WriteLine(appidname.Value);
            }
            
            Console.ReadLine();

#endif
            for (i = 0; i < appIdNames.Count; i++)
            {
                KeyValuePair<string, string> appidname = appIdNames.ElementAt(i);
                appIdhtml.Add(appidname.Value,webClient.DownloadString("http://repo.xposed.info/module/" + appidname.Key));
                //Thread.Sleep(delayBetweenRequests);
            }
            i = 0;
            int lengthk = 0;
            string leen = " ", urll =  " ";
            foreach(KeyValuePair<string,string> appidhtml in appIdhtml)
            {
                temp1 = appidhtml.Value;
                temp1 = temp1.Substring(temp1.IndexOf(@"<span class=""file""><a href=""") + 28);
                temp2 = temp1.Remove(temp1.IndexOf("</a>"));
                leen = temp2.Substring(temp2.IndexOf("length=") +7 );
                leen = leen.Remove(leen.IndexOf("\">"));
                urll = temp2.Remove(temp2.IndexOf(@""" type=""application/octet-stream; length="));
                Console.WriteLine(leen);
                Console.WriteLine(urll);
                Console.ReadLine();
            }













            Console.ReadLine();


        }
        
    }
}
