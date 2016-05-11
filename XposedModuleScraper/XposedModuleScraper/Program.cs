using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Diagnostics;
using System.Threading;
using System.Net;

namespace XposedModuleScraper
{
    class Program
    {
        static void Main(string[] args)
        {
            #region Declarations
            //strings
            string FirstPageSource;                         //HTML Source Code for first page. We can get number of modules from this string.
            string temp1, temp2, temp3, temp4;              //temporary strings to perform string operations.
            string UrlTemplate = "http://repo.xposed.info/module-overview?combine=&status=All&field_restrict_edits_value=All&sort_by=field_last_update_value&page=";
            //List<string>
            List<string> htmlPages = new List<string>();    // one for pages which list 10 modules per page.
            List<string> appIDNamesUnFormatted = new List<string>();   // one for 
            Dictionary<string, string> appIdNames = new Dictionary<string, string>();
            //integers
            int pages;                                      //number of pages.
            int modules;                                    //number of modules.
            int i;                                          //iteration variable.
            #endregion
            #region Get First Page Source and number of modules
            FirstPageSource = new WebClient().DownloadString(UrlTemplate + 0);
            htmlPages.Add(FirstPageSource);
            modules = Convert.ToInt32(FirstPageSource.Substring(FirstPageSource.IndexOf("Displaying 1 - 10 of") + 20, 18).Remove(FirstPageSource.Substring(FirstPageSource.IndexOf("Displaying 1 - 10 of") + 20, 18).IndexOf(" mod")).Trim());

#if DEBUG
            modules = Convert.ToInt32(Console.ReadLine());

#endif
            #endregion
            #region Determine number of Pages
            if ((modules % 10) == 0)
                pages = modules / 10;
            else
                pages = ((modules - (modules % 10)) / 10) + 1;
            #endregion
            #region retrieve all pages
            // retrieve all pages each of which displays 10 modules including the last page which can display 1,2,or even 10 modules
            for (i=1; i <= pages;i++)
            {
                htmlPages.Add(new System.Net.WebClient().DownloadString(UrlTemplate + i));
            }
            #endregion

            for (i=0;i< 100;i++)
            {

                temp1 = htmlPages.ElementAt(i).Substring(htmlPages.ElementAt(i).IndexOf(@"<tbody>") + 7, htmlPages.ElementAt(i).IndexOf("</tbody>"));
                temp2 = temp1.Substring(temp1.IndexOf(@"<a href=""/module/"""), temp1.IndexOf("</a>"));
                temp1 = temp1.Substring(temp1.IndexOf("</a>"));
                appIDNamesUnFormatted.Add(temp1);

                
            }
            





        }
        
    }
}
