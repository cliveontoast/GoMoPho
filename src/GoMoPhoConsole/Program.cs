using GoMoPho;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GoMoPhoConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            string searchPattern = System.Configuration.ConfigurationManager.AppSettings["FilePattern"];
            string video = ".mp4";
            var hexSearches = System.Configuration.ConfigurationManager.AppSettings.AllKeys.Where(a => a.StartsWith("Mp4Header")).ToList();
            var byteSearches = hexSearches.Select(a => MovingPhotoExtraction.ToBytes(System.Configuration.ConfigurationManager.AppSettings[a].Split(' '))).ToList();
            string possibleFolder = null;
            if (args.Length == 0)
            {
                Console.WriteLine("No directory given to process" + Environment.NewLine);
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
                possibleFolder = Console.ReadLine();
            }
            else
            {
                possibleFolder = args[0];
                if (args.Length > 1)
                {
                    searchPattern = args[1];
                }
                if (args.Length > 2)
                {
                    video = args[2];
                }
            }
            if (!System.IO.Directory.Exists(possibleFolder))
            {
                Console.WriteLine($"Directory {possibleFolder} does not exist. Press any key to exit.");
                Console.ReadKey();
                return;
            }
            var folder = possibleFolder;
            var imageFiles = System.IO.Directory.GetFiles(folder, searchPattern);
            Console.WriteLine("Found the following number of google motion photos: " + imageFiles.Length);
            int count = 0;
            int successCount = 0;
            foreach (var item in imageFiles)
            {
                try
                {
                    if (Read(item, byteSearches))
                    {
                        successCount++;
                    }
                    count++;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Failed " + e.Message);
                    Console.WriteLine(e.StackTrace);
                    Console.WriteLine(System.Environment.NewLine);
                }
            }
            Console.WriteLine($"Finished. Processed {count} files, found {successCount} videos for {imageFiles.Length} images files.");
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        public static bool Read(string file, List<byte[]> byteSearch)
        {
            //var r1 = new ExifLib.ExifReader(file);
            //var m1 = MetadataExtractor.ImageMetadataReader.ReadMetadata(file);
            Console.Write("Searching " + file);
            var fileBytes = System.IO.File.ReadAllBytes(file);
            int indexOfMp4 = -1;
            foreach (var search in byteSearch)
            {
                indexOfMp4 = new BoyerMoore(search).Search(fileBytes);
                if (indexOfMp4 >= 0) break;
            }
            if (indexOfMp4 >= 0)
            {
                Console.WriteLine("   Found the video");
                WriteVideo(file, fileBytes, indexOfMp4);
                return true;
            }
            else
            {
                Console.Write("   Did not find known video, trying generic approach");
                // http://dev.exiv2.org/projects/exiv2/wiki/The_Metadata_in_JPEG_files says that a JPEG start of image with 0xFFD8 and ends with 0xFFD9.
                var endOfJpeg = MovingPhotoExtraction.ToBytes("FF D9".Split(' '));
                var indexOfMp4_Part1 = MovingPhotoExtraction.ToBytes("00 00 00".Split(' '));
                var indexOfMp4_Part2 = MovingPhotoExtraction.ToBytes("66 74 79 70".Split(' '));
                var indexOfMp4_Part3 = MovingPhotoExtraction.ToBytes("6D 70 34 32".Split(' '));

                var bm = new BoyerMoore(endOfJpeg);

                var endOf = bm.Search(fileBytes);
                bm.SetPattern(indexOfMp4_Part2);
                var part2s = bm.SearchAll(fileBytes);
                bm.SetPattern(indexOfMp4_Part3);
                foreach (var part2 in part2s)
                {
                    if (part2 < endOf)
                    {
                        Console.Write("Not yet at end of jpeg");
                    }
                    var part3 = bm.SearchAll(fileBytes).FirstOrDefault(a => a > part2);
                    // part 3 is just a bit further than part2
                    if (part3 > part2 && part2 + 20 > part3)
                    {
                        var minus4 = fileBytes[part2 - 4];
                        var minus3 = fileBytes[part2 - 3];
                        var minus2 = fileBytes[part2 - 2];
                        var minus1 = fileBytes[part2 - 1];
                        if (minus4 == 0 && minus3 == 0 && minus2 == 0)
                        {
                            Console.WriteLine("... Found video via pattern search");
                            WriteVideo(file, fileBytes, part2 - 4);
                            return true;
                        }
                        Console.WriteLine($"... Found part2 and 3, not 1: {minus4} {minus3} {minus2} {minus1}");
                    }
                }
                Console.WriteLine($"Failed to find the video out of {part2s.Count} possibilites");
                Console.WriteLine("Please report this output to https://github.com/cliveontoast/GoMoPho/issues and attach your moving image jpeg file");
            }
            return false;
        }

        private static void WriteVideo(string file, byte[] fileBytes, int indexOfMp4)
        {
            var mp4File = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(file), System.IO.Path.GetFileNameWithoutExtension(file) + ".mp4");
            using (var mp4Stream = new System.IO.FileStream(mp4File, System.IO.FileMode.Create, System.IO.FileAccess.Write, System.IO.FileShare.None))
            {
                mp4Stream.Seek(0, System.IO.SeekOrigin.Begin);
                mp4Stream.Write(fileBytes, indexOfMp4, fileBytes.Length - indexOfMp4);
            }
        }
    }
}
