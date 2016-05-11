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
            string FirstPageSource;
            // Get and process HTML Code of first page
            #region first page HTML Code
            //FirstPageSource = GetHTMLSource("http://repo.xposed.info/module-overview");
            FirstPageSource = new System.Net.WebClient().DownloadString("http://repo.xposed.info/module-overview");
            //Process the HTML Code

            // create a easy to type variable name for convenience
            string a = FirstPageSource;
            int a1 = a.IndexOf("Displaying 1 - 10 of") + 20;
            string astr = a.Substring(a1, 18);
            string astr1 = astr.Remove(astr.IndexOf(" mod"));
            string astr2 = astr1.Trim();
            int modules = Convert.ToInt32(astr2);
            
            #endregion
            string UrlTemplate = "http://repo.xposed.info/module-overview?combine=&status=All&field_restrict_edits_value=All&sort_by=field_last_update_value&page=";

            int pageDeterminer = (modules / 10) + 1;


            string[] pagesHTMLsource = new string[pageDeterminer];
            for(int i = 1; i <= 10; i++)
            {
                pagesHTMLsource[i] = new System.Net.WebClient().DownloadString(UrlTemplate + i);
            }



        }
        
    }
}
