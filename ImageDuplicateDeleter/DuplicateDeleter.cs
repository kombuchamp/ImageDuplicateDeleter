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
        private readonly List<string> imagePaths = new List<string>();
        private readonly ImageConverter imageConverter = new ImageConverter();
        private readonly MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();

        public string Path { get; set; } = @".\";

        public DuplicateDeleter()
        {
            // Adding all kinds of images to the list
            this.imagePaths.AddRange(Directory.GetFiles(Path, @"*.jpg").ToList());
            this.imagePaths.AddRange(Directory.GetFiles(Path, @"*.jpeg").ToList());
            this.imagePaths.AddRange(Directory.GetFiles(Path, @"*.png").ToList());
            this.imagePaths.AddRange(Directory.GetFiles(Path, @"*.bmp").ToList());
            this.imagePaths.AddRange(Directory.GetFiles(Path, @"*.gif").ToList());
        }

        public IEnumerable<string> FindDuplicates()
        {
            Image pic1;
            Image pic2;

            var imagePaths = this.imagePaths; // Local scope copy of this field.
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
                                filesToDelete.Add(imagePaths[i]);
                                imagePaths.Remove(imagePaths[i]); i--; // Step backwards because we removed an element
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

            // Compare hashes of images
            return hash1.SequenceEqual(hash2);
        }
    }
}

