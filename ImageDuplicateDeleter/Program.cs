using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageDuplicateDeleter
{
    class Program
    {
        static void Main(string[] args)
        {
            var deleter = new DuplicateDeleter();

            foreach (var item in deleter.imagePaths)
            {
                Console.WriteLine(item);
            }

            Console.WriteLine(new string('-', 50));

            try
            {
                deleter.Delete();
            }
            catch (Exception e)
            {
                var defaultColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Couldnt delete the file: \n({e.Message})\n");
                Console.ForegroundColor = defaultColor;
            }

            Console.ReadKey(); // Delay on exit
        }
    }
}
