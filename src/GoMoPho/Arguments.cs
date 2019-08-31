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
        
        public Arguments(string[] args)
        {
            ProcessArgs(args);
            while (!System.IO.Directory.Exists(Directory))
            {
                Console.Write(@"Please type in a directory to process motion photos and press ENTER i.e. C:\\Users\\Clive\\OneDrive\\Documents\\Pictures\\Camera Roll
> ");
                Directory = Console.ReadLine();
            }
            while (!string.IsNullOrWhiteSpace(SplitDirectory) && !System.IO.Directory.Exists(SplitDirectory))
            {
                Console.WriteLine(@"Split directory not found. Shall it be created (y/n)? ");
                var key = Console.ReadKey();
                IsValid = IsValid && key.Key == ConsoleKey.Y;
                if (IsValid)
                {
                    System.IO.Directory.CreateDirectory(OutputDirectory);
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
                Console.WriteLine("Press H to show the help, otherwise press any other key to provide a folder");
                var key = Console.ReadKey();
                if (key.Key == ConsoleKey.H)
                {
                    Help("Next time you run the application, you can follow the instructions below");
                }
            }
            int i = 0;
            while (i < args.Length)
            {
                switch (args[i++].ToLower())
                {
                    case "d":
                    case "directory":
                        Directory = args[i++];
                        break;

                    case "p":
                    case "pattern":
                        SearchPattern = args[i++];
                        break;

                    case "g":
                    case "gif":
                        GifExport = true;
                        break;

                    case "s":
                    case "split":
                        SplitDirectory = args[i++];
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
" + preamble + @"

Please provide command line arguments as follows:

    d <Directory>
        A directory to search for google motion photos

    p <search pattern>
        A search pattern other than *MVIMG_*.jpg in which to find google motion photos

    g 
        Extract a encoded gif file along with the mp4 video file

    s <output directory>
        A separate directory that will create two files per motion photo
            1) jpg file without the embedded motion photo
            2) mp4 file of the motion photo

For example if you type the following arguments when running me:
    d ""C:\Users\Clive\OneDrive\Documents\Pictures\Camera Roll""

Then I will go through all the motion photos like *MVIMG_*.jpg and create a new .mp4 file for each one found.

");
        }
    }
}
