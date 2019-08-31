using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xabe.FFmpeg;

namespace GoMoPhoCoreConsole
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
            string outputFileName = null;
            while (filesToConvert.TryDequeue(out FileInfo fileToConvert))
            {
                try
                {
                    outputFileName = await OutputGif(fileToConvert);
                }
                catch (Exception e)
                {
                    if (FFmpeg.ExecutablesPath == null)
                    {
                        FFmpeg.ExecutablesPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "FFmpeg");
                        //Get latest version of FFmpeg. It's great idea if you don't know if you had installed FFmpeg.
                        await Console.Out.WriteLineAsync($"Getting FFMpeg, storing {FFmpeg.ExecutablesPath}");
                        await FFmpeg.GetLatestVersion();
                        filesToConvert.Enqueue(fileToConvert);
                    }
                    else
                    {
                        await Console.Out.WriteLineAsync($"Failed file {fileToConvert.Name} to {outputFileName}");
                        while (e != null)
                        {
                            await Console.Out.WriteLineAsync(e.Message);
                            await Console.Out.WriteLineAsync(e.StackTrace);
                            e = e.InnerException;
                        }
                    }
                }
            }
        }

        private static async Task<string> OutputGif(FileInfo fileToConvert)
        {
            //Save file to the same location with changed extension
            string outputFileName = Path.ChangeExtension(fileToConvert.FullName, ".gif");
            File.Delete(outputFileName);
            await Console.Out.WriteLineAsync($"Writing to {outputFileName}");
            await Conversion.ToGif(fileToConvert.FullName, outputFileName, 0).Start();
            await Console.Out.WriteLineAsync($"Finished converion file [{fileToConvert.Name}] to .gif");
            return outputFileName;
        }
    }
}
