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
            string UrlTemplate = "http://repo.xposed.info/module-overview?combine=&status=All&field_restrict_edits_value=All&sort_by=field_last_update_value&page=";
            List<string> htmlPages = new List<string>();
            FirstPageSource = new System.Net.WebClient().DownloadString(UrlTemplate + 0);
            htmlPages.Add(FirstPageSource);
            int modules = Convert.ToInt32(FirstPageSource.Substring(FirstPageSource.IndexOf("Displaying 1 - 10 of") + 20, 18).Remove(FirstPageSource.Substring(FirstPageSource.IndexOf("Displaying 1 - 10 of") + 20, 18).IndexOf(" mod")).Trim());
            int pages;
            modules = Convert.ToInt32(Console.ReadLine());
            if((modules % 10) == 0)
            {
                pages = modules / 10;
            }
            else
            {
                pages = ((modules - (modules % 10)) / 10) + 1;
            }

            


            for (int i=0; i < 100;i++)
            {
                htmlPages.Add(new System.Net.WebClient().DownloadString(UrlTemplate + i));
            }
            

            string temp1, temp2, temp3, temp4;
            List<string> appIDNames = new List<string>();
            for(int i=0;i< 100;i++)
            {

                temp1 = htmlPages.ElementAt(i).Substring(htmlPages.ElementAt(i).IndexOf(@"<tbody>") + 7, htmlPages.ElementAt(i).IndexOf("</tbody>"));
                temp2 = temp1.Substring(temp1.IndexOf(@"<a href=""/module/"""), temp1.IndexOf("</a>"));
                temp1 = temp1.Substring(temp1.IndexOf("</a>"));
                appIDNames.Add(temp1);






                if (i > (modules / 10) - 5)
                {
                    string fa = htmlPages.ElementAt(i);
                    if (!(fa.Contains(@"<div class=""view-content"">""")))
                    {
                        htmlPages.RemoveAt(i);
                        break;
                    }
                }
            }





        }
        
    }
}
