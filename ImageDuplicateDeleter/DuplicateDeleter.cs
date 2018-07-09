using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using System.Security.Cryptography;

namespace ImageDuplicateDeleter
{
    public class DuplicateDeleter
    {
        public List<string> imagePaths; // public for tests!
        private readonly ImageConverter imageConverter = new ImageConverter();
        //private readonly SHA256Managed hasher = new SHA256Managed(); // Hashing algorithm has bugs atm

        public DuplicateDeleter()
        {
            this.imagePaths = Directory.GetFiles(@".\", @"*.jpg").ToList(); // TODO: add options here
        }

        public void Delete()
        {
            Image pic1;
            Image pic2;

            List<string> filesToDelete = new List<string>();

            while (imagePaths.Count != 0)
            {
                using (pic1 = Image.FromFile(imagePaths[0]))
                {

                    for (int i = 1; i < imagePaths.Count; i++)
                    {
                        using (pic2 = Image.FromFile(imagePaths[i]))
                        {
                            if (IsDuplicateImage(pic1, pic2))
                            {
                                Console.WriteLine($"Found duplicate: {imagePaths[i]}");

                                filesToDelete.Add(imagePaths[i]);
                                imagePaths.Remove(imagePaths[i]);
                            }
                        }

                    }

                    imagePaths.RemoveAt(0); // file at this index is already checked

                    pic1.Dispose();
                }
            };



            //Deletion routine
            foreach (var item in filesToDelete)
            {
                try
                {
                    File.Delete(item);
                }
                catch (IOException)
                {
                    throw;
                }
            }
        }

        // Compares two images and returns true if they are equal
        public bool IsDuplicateImage(Image pic1, Image pic2)  // public for tests!
        {
            if (pic1.Size != pic2.Size)
                return false;

            // Convert images to byte arrays
            byte[] rawPic1 = (byte[])imageConverter.ConvertTo(pic1, typeof(byte[]));
            byte[] rawPic2 = (byte[])imageConverter.ConvertTo(pic2, typeof(byte[]));

            // TODO: This method of comparation won't work in some cases. Consider if its needed at all

            // Generate hash for each image
            //var hash1 = hasher.ComputeHash(rawPic1);
            //var hash2 = hasher.ComputeHash(rawPic1);

            return rawPic1.SequenceEqual(rawPic2);
        }
    }
}

