using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xabe.FFmpeg;
using Xabe.FFmpeg.Downloader;

namespace GoMoPho
{
    public class FFmpegGif
    {
        public static void CreateGifs(ConcurrentQueue<FileInfo> filesToConvert, string suppliedFfmpegLocation)
        {
            Run(filesToConvert, suppliedFfmpegLocation).Wait();
        }

        public static async Task Run(ConcurrentQueue<FileInfo> filesToConvert, string suppliedFfmpegLocation)
        {
            await Console.Out.WriteLineAsync($"Found {filesToConvert.Count} files to convert to gif");
            //Run conversion
            await RunConversion(filesToConvert, suppliedFfmpegLocation);
        }

        private static async Task RunConversion(ConcurrentQueue<FileInfo> filesToConvert, string suppliedFfmpegLocation)
        {
            AttemptToUseSuppliedFfmpegLocation(suppliedFfmpegLocation);
            if (FFmpeg.ExecutablesPath == null && !await GetFFmpeg())
            {
                await Console.Out.WriteLineAsync("Could not get FFmpeg, sorry");
                return;
            }
            string outputFileName = null;
            while (filesToConvert.TryDequeue(out FileInfo fileToConvert))
            {
                try
                {
                    outputFileName = await OutputGif(fileToConvert);
                }
                catch (Exception e)
                {
                    await Console.Out.WriteLineAsync($"Failed to create GIF of {fileToConvert.Name}");
                    await PrintException(e);
                }
            }
        }

        private static void AttemptToUseSuppliedFfmpegLocation(string suppliedFfmpegLocation)
        {
            if (FFmpeg.ExecutablesPath != null) return;
            if (suppliedFfmpegLocation == null) return;
            var file = new FileInfo(suppliedFfmpegLocation);
            if (file.Exists)
            {
                FFmpeg.SetExecutablesPath(file.DirectoryName, file.Name);
                Console.WriteLine($"Set ffmpeg directory path to {FFmpeg.ExecutablesPath}");
                return;
            }
            var dir = new DirectoryInfo(suppliedFfmpegLocation);
            if (dir.Exists)
            {
                var expectedFile = Path.Combine(dir.FullName, "ffmpeg");
                if (File.Exists(expectedFile))
                {
                    Console.WriteLine($"Set ffmpeg directory path to {FFmpeg.ExecutablesPath} found ffmpeg in it");
                    FFmpeg.SetExecutablesPath(dir.FullName);
                    return;
                }
                expectedFile = expectedFile + ".exe";
                if (File.Exists(expectedFile))
                {
                    Console.WriteLine($"Set ffmpeg directory path to {FFmpeg.ExecutablesPath} found ffmpeg.exe in it");
                    FFmpeg.SetExecutablesPath(dir.FullName);
                    return;
                }
            }
            Console.WriteLine($"Could not find ffmpeg from supplied path: {suppliedFfmpegLocation}");
        }

        private static async Task<bool> GetFFmpeg()
        {
            try
            {
                //Get latest version of FFmpeg. It's great idea if you don't know if you had installed FFmpeg.
                await Console.Out.WriteLineAsync($"Getting FFMpeg, saving to {TempLocation}");
                await FFmpegDownloader.GetLatestVersion(FFmpegVersion.Official, TempLocation);
                FFmpeg.SetExecutablesPath(TempLocation);
            }
            catch (Exception e)
            {
                await Console.Out.WriteLineAsync($"Could not get FFMpeg");
                await PrintException(e);
                return false;
            }
            return true;
        }

        public static string TempLocation => Path.Combine(Path.GetTempPath(), "FFmpeg");

        private static async Task<Exception> PrintException(Exception e)
        {
            while (e != null)
            {
                await Console.Out.WriteLineAsync(e.Message);
                await Console.Out.WriteLineAsync(e.StackTrace);
                e = e.InnerException;
            }

            return e;
        }

        private static async Task<string> OutputGif(FileInfo fileToConvert)
        {
            //Save file to the same location with changed extension
            string outputFileName = Path.ChangeExtension(fileToConvert.FullName, ".gif");
            await Console.Out.WriteLineAsync($"Writing to {outputFileName}");
            var input = await FFmpeg.GetMediaInfo(fileToConvert.FullName);
            // We only want to copy 1 of the video streams to the gif
            // It seems Google includes multiple streams in some gifs with key frames
            // (the photos it thinks you might want to pick)
            var videoStream = input.VideoStreams.First();
            var conversion = FFmpeg.Conversions.New()
                .AddStream(videoStream)
                .SetOutput(outputFileName)
                .SetOverwriteOutput(true);
            await conversion.Start();
            await Console.Out.WriteLineAsync($"Finished conversion for file [{fileToConvert.Name}] to .gif");
            try
            { 
                File.SetCreationTime(outputFileName, fileToConvert.CreationTime);
                File.SetLastAccessTime(outputFileName, fileToConvert.LastAccessTime);
                File.SetLastWriteTime(outputFileName, fileToConvert.LastWriteTime);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Did not correct file date of {outputFileName} {e.Message}");
            }
            return outputFileName;
        }
    }
}
