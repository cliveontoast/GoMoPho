dotnet clean src\GoogleMotionImage.sln
del /Q /S /F .\bin
dotnet build src\GoogleMotionImage.sln
dotnet publish src\GoMoPhoConsole\GoMoPhoCoreConsole.csproj -o ..\..\bin\CrossPlatform
dotnet publish src\GoMoPhoConsole\GoMoPhoCoreConsole.csproj -r win-x64 -o ..\..\bin\WindowsCore
dotnet publish src\GoMoPhoConsole\GoMoPhoCoreConsole.csproj -r linux-x64 -o ..\..\bin\GNULinux
dotnet publish src\GoMoPhoConsole\GoMoPhoCoreConsole.csproj -r ubuntu-x64 -o ..\..\bin\Ubuntu
dotnet publish src\GoMoPhoConsole\GoMoPhoCoreConsole.csproj -r osx-x64 -o ..\..\bin\macOS

dotnet bin\CrossPlatform\GoMoPhoCoreConsole.dll d test-image\ g s split
rem windows only
bin\Windows\GoMoPhoConsole.exe d test-image\ g s split
test-image\MVIMG_20180910_124410.mp4
test-image\MVIMG_20180910_124410.gif

set /P GoVersion=
set zipper=c:\Program Files\7-Zip\7z.exe
if NOT EXIST "%zipper%" SET zipper=c:\Program Files (x86)\7-Zip\7z.exe

cd bin\CrossPlatform
copy ..\..\README.md .
echo Execute via the following > readme.txt
echo dotnet GoMoPhoCoreConsole.dll [directory] >> readme.txt
echo visit https://github.com/cliveontoast/GoMoPho >> readme.txt
"%zipper%" a ..\GoMoPho.CrossPlatform.%GoVersion%.zip .
cd ..\..\bin\Windows
copy ..\..\README.md .
echo Execute via the following > readme.txt
echo GoMoPhoConsole.exe [directory] >> readme.txt
echo visit https://github.com/cliveontoast/GoMoPho >> readme.txt
"%zipper%" a ..\GoMoPho.Windows.%GoVersion%.zip .

cd ..\..\bin\WindowsCore
copy ..\..\README.md .
echo Execute via the following > readme.txt
echo GoMoPhoCoreConsole.exe [directory] >> readme.txt
echo visit https://github.com/cliveontoast/GoMoPho >> readme.txt
"%zipper%" a ..\GoMoPho.WindowsCore.%GoVersion%.zip .
cd ..\..\bin\macOS
copy ..\..\README.md .
echo Execute via the following > readme.txt
echo ./GoMoPhoCoreConsole [directory] >> readme.txt
echo visit https://github.com/cliveontoast/GoMoPho >> readme.txt
"%zipper%" a ..\GoMoPho.macOS.%GoVersion%.zip .

cd ..\..\bin\GNULinux
copy ..\..\README.md .
echo Execute permissions must be added, when unzipping, use "unzip -d ./GoMoPho GoMoPho.GNULinux.x.x.zip && cd ./GoMoPho && source grant_perms.sh" or run the grant_perms.sh afterwards > readme.txt
echo ./grant_perms.sh # once only >> readme.txt
echo ./GoMoPhoCoreConsole [directory] >> readme.txt
echo visit https://github.com/cliveontoast/GoMoPho >> readme.txt
copy ..\..\src\grant_perms.sh .
"%zipper%" a ..\GoMoPho.GNULinux.%GoVersion%.zip .

cd ..\..\bin\Ubuntu
copy ..\..\README.md .
echo Execute permissions must be added, when unzipping, use "unzip -d ./GoMoPho GoMoPho.GNULinux.x.x.zip && cd ./GoMoPho && source grant_perms.sh" or run the grant_perms.sh afterwards > readme.txt
echo ./grant_perms.sh # once only >> readme.txt
echo ./GoMoPhoCoreConsole [directory] >> readme.txt
echo visit https://github.com/cliveontoast/GoMoPho >> readme.txt
copy ..\..\src\grant_perms.sh .
"%zipper%" a ..\GoMoPho.Ubuntu.%GoVersion%.zip .
cd ..\..