# GoMoPho
Google motion photos video extractor.
Please note, that android backups to Google Drive does not upload the video. Read  https://www.bitquabit.com/post/moving-and-backing-up-google-moving-images/ why.

#### Compilation
```
dotnet build src\GoogleMotionImage.sln
```
Windows: Run the Build-and-test.bat file to compile and test the program.
Other: Copy the content of the batch file and run them in a console.

#### Testing in windows
```
Build-and-test.bat
```

#### Running
**Windows** 
```
.\bin\Windows\GoMoPhoConsole.exe
```
Either pass in a directory with your MVIMG files as an argument, or run it with no argument, then type or paste in the location of the directory with your MVIMG files.

```
GoMoPhoConsole.exe "C:\Users\Clive\OneDrive\Documents\Pictures\Camera Roll"
```
**Linux/MacOS**
Using dotnet runtime https://www.microsoft.com/net/download/linux-package-manager/rhel/runtime-current execute bin\netcoreapp2.0\GoMoPhoCoreConsole.dll

```
dotnet bin\netcoreapp2.0\GoMoPhoCoreConsole.dll test-image\
```
or
```
dotnet bin\netcoreapp2.0\GoMoPhoCoreConsole.dll
```

## Result
After completing the process will create video.mp4 files for any motion photo videos it finds.
This will not create the wiz-bang google AI versions.. https://ai.googleblog.com/2018/03/behind-motion-photos-technology-in.html
Just the originals.


```
No directory given to process

First argument is the folder i.e. "c:\somewhere with spaces\"
Second optional agument is the search pattern, i.e. default of *MVIMG_*.jpg
Third optional agument is the search pattern for video file, where file ends with i.e. default of .mp4


You can 1) type (or paste) in a folder now - without quotes and press <ENTER>
        2) press <ENTER> to exit


    Example typed text, afterwards pressing <ENTER>
    >c:\somewhere with spaces

> C:\temp\New folder (2)
Found the following number of google motion photos: 5
Searching C:\temp\New folder (2)\MVIMG_20180906_092408.jpg   Found the video
Searching C:\temp\New folder (2)\MVIMG_20180906_102344.jpg   Found the video
Searching C:\temp\New folder (2)\MVIMG_20180906_102350.jpg   Found the video
Searching C:\temp\New folder (2)\MVIMG_20180906_102352.jpg   Found the video
Searching C:\temp\New folder (2)\MVIMG_20180906_104633.jpg   Found the video
Finished. Processed 5 videos for 5 images files.
Press any key to exit
```