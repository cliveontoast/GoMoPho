using System;

namespace GoMoPho
{
    public class Arguments
    {
        public string Directory { get; set; }

        public string SearchPattern { get; set; }

        public bool GifExport { get; set; }

        public string SplitDirectory { get; set; }

        public bool IsValid { get; set; } = true;

        public bool ExtractJpg { get; set; }

        public string OutputDirectory => string.IsNullOrWhiteSpace(SplitDirectory) ? null : SplitDirectory;

        public bool Headless { get; private set; }

        public bool AutoCreateOutputDir { get; private set; }

        public Arguments(string[] args, Func<(string directory, bool success)> directoryPicker)
        {
            ProcessArgs(args);
            while (!System.IO.Directory.Exists(Directory))
            {
                var result = directoryPicker();
                IsValid = result.success;
                if (IsValid)
                    Directory = result.directory;
                else
                    return;
            }
            while (!string.IsNullOrWhiteSpace(SplitDirectory) && !System.IO.Directory.Exists(SplitDirectory))
            {
                if (AutoCreateOutputDir || Headless)
                {
                    Console.WriteLine(@"Split directory not found. Creating split directory automatically.");
                    System.IO.Directory.CreateDirectory(OutputDirectory);
                }
                else
                {
                    Console.WriteLine(@"Split directory not found. Shall it be created (y/n)? ");
                    var key = Console.ReadKey();
                    Console.WriteLine();
                    if (IsValid && key.Key == ConsoleKey.Y)
                    {
                        System.IO.Directory.CreateDirectory(OutputDirectory);
                    }
                }
            }
            if (System.IO.Directory.Exists(SplitDirectory))
            {
                ExtractJpg = true;
            }
        }

        private void ProcessArgs(string[] args)
        {
            Console.WriteLine(@"Welcome to google motion photos extractor.
If you provide a folder that contains google motion photos,
then I will extract any videos found");
            if (args.Length == 0)
            {
                Console.WriteLine("Press A to show more advanced options, otherwise press any other key to provide a folder");
                var key = Console.ReadKey();
                if (key.Key == ConsoleKey.A)
                {
                    Help("You may pass command line arguments to the application in the future.");
                }
            }
            int i = 0;
            while (i < args.Length)
            {
                switch (args[i++].ToLower())
                {
                    case "d":
                    case "directory":
                        if (args.Length > i)
                            Directory = args[i++];
                        break;

                    case "p":
                    case "pattern":
                        if (args.Length > i)
                            SearchPattern = args[i++];
                        break;

                    case "g":
                    case "gif":
                        GifExport = true;
                        break;

                    case "s":
                    case "split":
                        if (args.Length > i)
                            SplitDirectory = args[i++];
                        break;

                    case "h":
                    case "headless":
                        Headless = true;
                        break;

                    case "a":
                    case "AutoCreateOutputDir":
                        AutoCreateOutputDir = true;
                        break;

                    default:
                        Help(@"You have not supplied valid arguments in the command line

");
                        return;
                }
            }
        }

        private static void Help(string preamble)
        {
            Console.WriteLine(@"
" + preamble + $@"

Please provide command line arguments as follows:

    d <Directory>
        A directory to search for google motion photos

    p <search pattern>
        A search pattern other than *MVIMG_*.jpg in which to find google motion photos

    g 
        Extract a encoded gif file along with the mp4 video file.
        This may download approximately 130MB and store it here: {FFmpegGif.TempLocation}

    s <output directory>
        A separate directory that will create two files per motion photo
            1) jpg file without the embedded motion photo
            2) mp4 file of the motion photo
   
    h
        Headless / no-prompt mode.

    a
        Create ouput directory automatically if it does not exists


For example if you type the following arguments when running me:
    d ""C:\Users\Clive\OneDrive\Documents\Pictures\Camera Roll"" s ""Output\"" g p ""*.jpg""

Then I will write to the ""Output\"" folder mp4 videos, jpeg files without the embedded video, and a gif file too.

");
        }
    }
}
