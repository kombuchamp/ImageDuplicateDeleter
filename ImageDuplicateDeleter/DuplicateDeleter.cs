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
        public List<string> imagePaths = new List<string>(); // public for tests!

        private readonly ImageConverter imageConverter = new ImageConverter();
        private readonly MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();

        public DuplicateDeleter()
        {
            // Adding all kinds of images to the list (consider adding user options?)
            this.imagePaths.AddRange(Directory.GetFiles(@".\", @"*.jpg").ToList());
            this.imagePaths.AddRange(Directory.GetFiles(@".\", @"*.jpeg").ToList());
            this.imagePaths.AddRange(Directory.GetFiles(@".\", @"*.png").ToList());
            this.imagePaths.AddRange(Directory.GetFiles(@".\", @"*.bmp").ToList());
            this.imagePaths.AddRange(Directory.GetFiles(@".\", @"*.gif").ToList());
        }

        public IEnumerable<string> FindDuplicates()
        {
            Image pic1;
            Image pic2;

            var imagePaths = this.imagePaths; // Local scope copy of this field. (Consider choosing more proper name?)
            var filesToDelete = new List<string>();

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
                                imagePaths.Remove(imagePaths[i]); i--; // Step backwards because we removed element
                            }
                        }

                    }

                    imagePaths.RemoveAt(0); // file at this index is already checked
                }
            };

            //DeleteFiles(filesToDelete);
            return filesToDelete;
        }

        public void DeleteFiles(IEnumerable<string> filesToDelete)
        {
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
            // Remove deleted files from file list
            this.imagePaths.Except(filesToDelete);
        }

        // Compares two images and returns true if they are equal
        public bool IsDuplicateImage(Image pic1, Image pic2)  // public for tests!
        {
            if (pic1.Size != pic2.Size)
                return false;

            // Convert images to byte arrays
            byte[] rawPic1 = imageConverter.ConvertTo(pic1, typeof(byte[])) as byte[];
            byte[] rawPic2 = imageConverter.ConvertTo(pic2, typeof(byte[])) as byte[];

            // Generate hash for each image
            var hash1 = md5.ComputeHash(rawPic1);
            var hash2 = md5.ComputeHash(rawPic2);


            return hash1.SequenceEqual(hash2);
        }
    }
}

