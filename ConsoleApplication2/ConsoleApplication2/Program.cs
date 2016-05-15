using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyDownloader.Core;

namespace ConsoleApplication2
{
    class Program
    {
        static void Main(string[] args)
        {
            Downloader fo = new Downloader();
            
            Downloader download = DownloadManager.Instance.Add("http://jogos.download.uol.com.br/videos/pc/thewitcher12.wmv", @"c:\temp\thewitcher12.wmv", 3, true);
            




            Console.ReadLine();

        }
    }
}
