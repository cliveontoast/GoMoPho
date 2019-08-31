using GoMoPho;
using GoMoPhoCoreConsole;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GoMoPhoConsole
{
    class Program
    {
        static ConcurrentQueue<FileInfo> filesToConvert;

        static void Main(string[] args)
        {
            filesToConvert = new ConcurrentQueue<FileInfo>();

            string searchPattern = System.Configuration.ConfigurationManager.AppSettings["FilePattern"];
            var hexSearches = System.Configuration.ConfigurationManager.AppSettings.AllKeys.Where(a => a.StartsWith("Mp4Header")).ToList();
            var byteSearches = hexSearches.Select(a => MovingPhotoExtraction.ToBytes(System.Configuration.ConfigurationManager.AppSettings[a].Split(' '))).ToList();

            var options = new Arguments(args);
            if (!options.IsValid)
            {
                Console.WriteLine("Cannot continue with current options. Exiting");
                return;
            }
            var folder = options.Directory;
            var imageFiles = Directory.GetFiles(folder, searchPattern);
            Console.WriteLine("Found the following number of google motion photos: " + imageFiles.Length);

            int count = 0;
            int successCount = 0;
            foreach (var item in imageFiles)
            {
                try
                {
                    if (Read(item, byteSearches, options.OutputDirectory, options.ExtractJpg))
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
            if (options.GifExport)
            {
                FFmpegGif.CreateGifs(filesToConvert);
            }
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        public static bool Read(string file, List<byte[]> byteSearch, string outputDirectory, bool extractJpg)
        {
            //var r1 = new ExifLib.ExifReader(file);
            //var m1 = MetadataExtractor.ImageMetadataReader.ReadMetadata(file);
            Console.Write("Searching " + file);
            var fileBytes = File.ReadAllBytes(file);
            int indexOfMp4 = -1;
            foreach (var search in byteSearch)
            {
                indexOfMp4 = new BoyerMoore(search).Search(fileBytes);
                if (indexOfMp4 >= 0) break;
            }
            if (indexOfMp4 >= 0)
            {
                Console.WriteLine("   Found the video");
                WriteVideo(file, fileBytes, indexOfMp4, extractJpg, null, outputDirectory);
                return true;
            }
            else
            {
                Console.Write("   Did not find known video, trying generic approach");
                // http://dev.exiv2.org/projects/exiv2/wiki/The_Metadata_in_JPEG_files says that a JPEG start of image with 0xFFD8 and ends with 0xFFD9.
                var indexOfMp4_Part1 = MovingPhotoExtraction.ToBytes("00 00 00".Split(' '));
                var indexOfMp4_Part2 = MovingPhotoExtraction.ToBytes("66 74 79 70".Split(' '));
                var indexOfMp4_Part3 = MovingPhotoExtraction.ToBytes("6D 70 34 32".Split(' '));

                BoyerMoore bm;
                int endOfJpeg = GetEndOfJpeg(fileBytes, out bm);

                bm.SetPattern(indexOfMp4_Part2);
                var part2s = bm.SearchAll(fileBytes);
                bm.SetPattern(indexOfMp4_Part3);
                foreach (var part2 in part2s)
                {
                    if (part2 < endOfJpeg)
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
                            WriteVideo(file, fileBytes, part2 - 4, extractJpg, endOfJpeg, outputDirectory);
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

        private static int GetEndOfJpeg(byte[] fileBytes, out BoyerMoore bm)
        {
            var endOfJpegBytes = MovingPhotoExtraction.ToBytes("FF D9".Split(' '));
            bm = new BoyerMoore(endOfJpegBytes);
            int endOfJpeg = bm.Search(fileBytes);
            return endOfJpeg;
        }

        private static void WriteVideo(string file, byte[] fileBytes, int indexOfMp4, bool extractJpeg, int? indexOfJpegEnd = null, string outputDirectory = null)
        {
            outputDirectory = outputDirectory ?? Path.GetDirectoryName(file);
            var mp4File = Path.Combine(outputDirectory, Path.GetFileNameWithoutExtension(file) + ".mp4");
            using (var mp4Stream = new FileStream(mp4File, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                mp4Stream.Seek(0, SeekOrigin.Begin);
                mp4Stream.Write(fileBytes, indexOfMp4, fileBytes.Length - indexOfMp4);
            }
            filesToConvert.Enqueue(new FileInfo(mp4File));

            if (extractJpeg)
            {
                int jpegEndIdx = indexOfJpegEnd ?? GetEndOfJpeg(fileBytes, out BoyerMoore bm);
                var jpgFile = Path.Combine(outputDirectory, Path.GetFileName(file));
                using (var jpgStream = new FileStream(jpgFile, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    jpgStream.Seek(0, SeekOrigin.Begin);
                    jpgStream.Write(fileBytes, 0, jpegEndIdx+2);
                }
            }
        }
    }
}
