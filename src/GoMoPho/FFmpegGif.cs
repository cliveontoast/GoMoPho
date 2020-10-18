using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xabe.FFmpeg;

namespace GoMoPho
{
    public class FFmpegGif
    {
        public static void CreateGifs(ConcurrentQueue<FileInfo> filesToConvert)
        {
            Run(filesToConvert).Wait();
        }

        public static async Task Run(ConcurrentQueue<FileInfo> filesToConvert)
        {
            await Console.Out.WriteLineAsync($"Find {filesToConvert.Count} files to convert to gif");
            //Run conversion
            await RunConversion(filesToConvert);
        }

        private static async Task RunConversion(ConcurrentQueue<FileInfo> filesToConvert)
        {
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
                    await Console.Out.WriteLineAsync($"Failed file {fileToConvert.Name} to {outputFileName}");
                    await PrintException(e);
                }
            }
        }

        private static async Task<bool> GetFFmpeg()
        {
            try
            {
                FFmpeg.ExecutablesPath = TempLocation;
                //Get latest version of FFmpeg. It's great idea if you don't know if you had installed FFmpeg.
                await Console.Out.WriteLineAsync($"Getting FFMpeg, storing {FFmpeg.ExecutablesPath}");
                await FFmpeg.GetLatestVersion();
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
            File.Delete(outputFileName);
            await Console.Out.WriteLineAsync($"Writing to {outputFileName}");
            await Conversion.ToGif(fileToConvert.FullName, outputFileName, 0).Start();
            await Console.Out.WriteLineAsync($"Finished converion file [{fileToConvert.Name}] to .gif");
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
