using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            int i = 1;
            














            List<string> ddd = File.ReadLines("C:\\Users\\Vignesh\\Downloads\\aria2\\tt2.txt").ToList();
            List<string> daa = new List<string>();
            while(ddd.Count !=0 )
            {
                if(ddd.ElementAt(i).Contains("apk"))
                {
                    daa.Add(ddd.ElementAt(i));

                }
            }
            Console.ReadLine();
            File.WriteAllLines("C:\\Users\\Vignesh\\Downloads\\aria2\\tt3.txt", daa.ToArray());
        }
    }
}
