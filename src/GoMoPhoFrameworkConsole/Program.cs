using GoMoPho;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrameworkConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            string searchPattern = System.Configuration.ConfigurationManager.AppSettings["FilePattern"];
            string video = ".mp4";
            var hexSearch = System.Configuration.ConfigurationManager.AppSettings["Mp4Header"];
            var byteSearch = MovingPhotoExtraction.ToBytes(hexSearch.Split(' '));
            string folder = null;
            if (args.Length == 0)
            {
                Console.WriteLine("No directory given to process" + System.Environment.NewLine);
                Console.WriteLine("First argument is the folder i.e. \"c:\\somewhere with spaces\\\"");
                Console.WriteLine("Second optional agument is the search pattern, i.e. default of " + searchPattern);
                Console.WriteLine("Third optional agument is the search pattern for video file, where file ends with i.e. default of " + video);
                Console.WriteLine(System.Environment.NewLine);
                Console.WriteLine("You can 1) type (or paste) in a folder now - without quotes and press <ENTER>");
                Console.WriteLine("        2) press <ENTER> to exit");
                Console.WriteLine(System.Environment.NewLine);
                Console.WriteLine("    Example typed text, afterwards pressing <ENTER>");
                Console.WriteLine("    >c:\\somewhere with spaces");
                Console.WriteLine();
                Console.Write("> ");
                var possibleFolder = Console.ReadLine();
                if (System.IO.Directory.Exists(possibleFolder))
                {
                    folder = possibleFolder;
                }
                else
                {
                    Console.WriteLine($"Directory {possibleFolder} does not exist. Press any key to exit.");
                    Console.ReadKey();
                    return;
                }
            }
            else
            {
                folder = args[0];
                if (args.Length > 1)
                {
                    searchPattern = args[1];
                }
                if (args.Length > 2)
                {
                    video = args[2];
                }
            }
            var imageFiles = System.IO.Directory.GetFiles(folder, searchPattern);
            Console.WriteLine("Found the following number of google motion photos: " + imageFiles.Length);
            int count = 0;
            foreach (var item in imageFiles)
            {
                try
                {
                    Read(item, byteSearch);
                    count++;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Failed " + e.Message);
                    Console.WriteLine(e.StackTrace);
                    Console.WriteLine(System.Environment.NewLine);
                }
            }
            Console.WriteLine($"Finished. Processed {count} videos for {imageFiles.Length} images files.");
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        public static void Read(string file, byte[] byteSearch)
        {
            //var r1 = new ExifLib.ExifReader(file);
            //var m1 = MetadataExtractor.ImageMetadataReader.ReadMetadata(file);
            Console.Write("Searching " + file);
            var fileBytes = System.IO.File.ReadAllBytes(file);
            var indexOfMp4 = new BoyerMoore(byteSearch).Search(fileBytes);
            if (indexOfMp4 >= 0)
            {
                Console.WriteLine("   Found the video");
                var mp4File = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(file), System.IO.Path.GetFileNameWithoutExtension(file) + ".mp4");
                using (var mp4Stream = new System.IO.FileStream(mp4File, System.IO.FileMode.Create, System.IO.FileAccess.Write, System.IO.FileShare.None))
                {
                    mp4Stream.Seek(0, System.IO.SeekOrigin.Begin);
                    mp4Stream.Write(fileBytes, indexOfMp4, fileBytes.Length - indexOfMp4);
                }
            }
            else
            {
                Console.WriteLine("   Did not find video");
            }
        }
    }
}
