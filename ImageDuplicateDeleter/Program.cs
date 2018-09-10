using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ImageDuplicateDeleter
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                FindAndDeleteDuplicates();
            }
            else if (args.Length == 1 && PathIsValid(args[0]))
            {
                FindAndDeleteDuplicates(args[0]);
            }
            else
                ShowHelp();
        }


        static void FindAndDeleteDuplicates(string path = "")
        {
            var deleter = new DuplicateDeleter();
            var duplicates = deleter.FindDuplicates();

            if (duplicates.ToList().Count == 0)
            {
                Console.WriteLine("No duplicates found.");
                return;
            }

            Console.WriteLine("Duplicates found:\n");
            foreach (var item in duplicates)
            {
                Console.WriteLine(item);
            }

            Console.Write("\nDelete duplicates? (y/n)");
            var keyPressed = Console.ReadKey();

            switch (keyPressed.Key)
            {
                case ConsoleKey.Y:
                    try
                    {
                        deleter.DeleteFiles(duplicates);
                    }
                    catch (Exception e)
                    {
                        var defaultColor = Console.ForegroundColor;
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Couldnt delete the file: \n({e.Message})\n");
                        Console.ForegroundColor = defaultColor;
                    }
                    Console.WriteLine("\nDuplicates have been deleted.");
                    break;
                case ConsoleKey.N:
                    break;
                default:
                    Console.WriteLine("\nInvalid command");
                    break;
            }

        }

        private static bool PathIsValid(string path)
        {
            path = Regex.Replace(path, "[\"]", "");
            return Directory.Exists(path);
        }

        private static void ShowHelp()
        {
            Console.WriteLine(res.Help.help);
        }
    }
}
