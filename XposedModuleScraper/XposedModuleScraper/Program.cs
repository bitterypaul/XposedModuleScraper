using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;

namespace XposedModuleScraper
{
    class Program
    {
        static void Main(string[] args)
        {


            WebRequest req = HttpWebRequest.Create("http://repo.xposed.info/module-overview");
            req.Method = "GET";

            string source;
            using (StreamReader reader = new StreamReader(req.GetResponse().GetResponseStream()))
            {
                source = reader.ReadToEnd();
            }

            Console.WriteLine(source);
            Console.Read();
        }
    }
}
