# GoMoPho
Google motion photos video extractor.

History: Android backups to Google Drive did not upload the video. Read  https://www.bitquabit.com/post/moving-and-backing-up-google-moving-images/ why.

Google Drive should now support backing up the video portion. https://www.guidingtech.com/share-motion-photos-google/

#### Running the zip release
Choose the zip file for your computer and unzip it to a new location. *Native* is compiled for the operating system, while *CrossPlatform* runs on all platforms via the dotnet runtime https://www.microsoft.com/net/download
- Windows (native) - GoMoPho.WindowsFramework
- GNU/Linux (native) - GoMoPho.GNULinux
- macOS (native) - GoMoPho.macOS
- Ubuntu (native) - GoMoPho.Ubuntu
- any operating system - choose GoMoPho.CrossPlatform if you have/download .NET Core 

My windows build process does not support unix permissions. The native compiled GNU/Linux versions require unzipping and then applying execute permissions. The CrossPlatform version does not.
The following bash commandline will unzip and apply the execute permission to the GoMoPhoCoreConsole file
```
unzip -d ./GoMoPho GoMoPho.GNULinux.x.x.zip && cd ./GoMoPho && source grant_perms.sh
```

#### Running
When running any of the options below, you can either pass in a directory with your MVIMG files as an argument, or run it with no argument, then type or paste in the location of the directory with your MVIMG files.

**Windows .NET framework** 
```
.\bin\Windows\GoMoPhoConsole.exe
```
or
```
GoMoPhoConsole.exe d "C:\Users\Clive\OneDrive\Documents\Pictures\Camera Roll" g s split
```
or to get some help on the options available
```
GoMoPhoConsole.exe help 
```
**.NET Core runtime on GNU/Linux MacOS or Windows** 
Use the dotnet runtime https://www.microsoft.com/net/download to execute ./bin/CrossPlatform/GoMoPhoCoreConsole.dll
```
dotnet bin/CrossPlatform/GoMoPhoCoreConsole.dll d test-image
```
This has been tested on Windows and macOS.

**.NET core natively compiled Windows** 
```
bin\WindowsCore\GoMoPhoCoreConsole.exe d test-image\
```

**.NET core natively compiled GNU/Linux**
```
bin/GNULinux/GoMoPhoCoreConsole d test-image
```
This has not been tried, feedback on this guide would be great

**.NET core natively compiled GNU/Linux Ubuntu-x64**
```
bin/Ubuntu/GoMoPhoCoreConsole d test-image
```
This has not been tried, feedback on this guide would be great

**.NET core natively compiled MacOS**
```
bin/macOS/GoMoPhoCoreConsole d test-image
```
This has been tested, when executed the OS warns that it is not signed.
There are no plans for this to be signed, so an exception is required.

## Extra reading and options
This will not create the wiz-bang google AI versions.. https://ai.googleblog.com/2018/03/behind-motion-photos-technology-in.html
Just the originals.

On running the application without any arguments you are greeted with the following kind message

```
Welcome to google motion photos extractor.
If you provide a folder that contains google motion photos,
then I will extract any videos found
Press A to show more advanced options, otherwise press any other key to provide a folder
```

If you press any other key besides A, running the windows version you select a folder in a GUI. Otherwise you will be asked to provide a folder/directory to process 

```
Please type in a directory to process motion photos and press ENTER i.e. C:\\Users\\Clive\\OneDrive\\Documents\\Pictures\\Camera Roll
> 
```

On providing a folder, it will proceed to extract any videos available

```
> C:\temp\New folder
Found the following number of google motion photos: 4
Searching C:\temp\New folder\MVIMG_20180910_124410.jpg   Found the video
Searching C:\temp\New folder\MVIMG_20181011_143701.jpg   Found the video
Searching C:\temp\New folder\MVIMG_20181021_082233.jpg   Found the video
Searching C:\temp\New folder\MVIMG_20181021_143747.jpg   Found the video
Finished. Processed 4 files, found 4 videos for 4 images files.
Press any key to exit
```


If you press A you will see the instructions on how to provide arguments 

```
You may pass command line arguments to the application in the future.

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
    d "C:\Users\Clive\OneDrive\Documents\Pictures\Camera Roll" s "Output\" g p "*.jpg"

Then I will write to the "Output\" folder mp4 videos, jpeg files without the embedded video, and a gif file too.


Please type in a directory to process motion photos and press ENTER i.e. C:\\Users\\Clive\\OneDrive\\Documents\\Pictures\\Camera Roll
>
```

Providing a full set of arguments to GoMoPho will extract the videos, create a gif, and a new jpeg file without the video embedded in it anymore

```
.\GoMoPhoConsole.exe d "c:\temp\New Folder" g s "c:\temp\Processed" p *.jpg
Welcome to google motion photos extractor.
If you provide a folder that contains google motion photos,
then I will extract any videos found
Split directory not found. Shall it be created (y/n)?
y
Found the following number of google motion photos: 4
Searching c:\temp\New Folder\MVIMG_20180910_124410.jpg   Found the video
Searching c:\temp\New Folder\MVIMG_20181011_143701.jpg   Found the video
Searching c:\temp\New Folder\MVIMG_20181021_082233.jpg   Found the video
Searching c:\temp\New Folder\MVIMG_20181021_143747.jpg   Found the video
Finished. Processed 4 files, found 4 videos for 4 images files.
Find 4 files to convert to gif
Writing to c:\temp\Processed\MVIMG_20180910_124410.gif
Getting FFMpeg, storing C:\Users\Clive\AppData\Local\FFmpeg
Writing to c:\temp\Processed\MVIMG_20181011_143701.gif
Finished converion file [MVIMG_20181011_143701.mp4] to .gif
Writing to c:\temp\Processed\MVIMG_20181021_082233.gif
Finished converion file [MVIMG_20181021_082233.mp4] to .gif
Writing to c:\temp\Processed\MVIMG_20181021_143747.gif
Finished converion file [MVIMG_20181021_143747.mp4] to .gif
Writing to c:\temp\Processed\MVIMG_20180910_124410.gif
Finished converion file [MVIMG_20180910_124410.mp4] to .gif
Press any key to exit

```

#### Compilation
```
dotnet build src\GoogleMotionImage.sln
```
Windows: Run  Build-Test-Zip.bat file to compile and test the program.

#### Testing in windows
```
Build-Test-Zip.bat
```
