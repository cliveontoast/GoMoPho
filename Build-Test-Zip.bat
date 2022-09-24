dotnet clean src\GoogleMotionImage.sln
del /Q /S /F .\bin
del /Q /S /F .\test-image\*.mp4
del /Q /S /F .\test-image\*.gif
del /Q /S /F .\split
dotnet build src\GoMoPhoFrameworkConsole\GoMoPhoFrameworkConsole.csproj
del /S packages.lock.json
dotnet publish src\GoMoPhoConsole\GoMoPhoCoreConsole.csproj --output .\bin\CrossPlatform
del /S packages.lock.json
dotnet publish src\GoMoPhoConsole\GoMoPhoCoreConsole.csproj --runtime win-x64 --output .\bin\WindowsCore
del /S packages.lock.json
dotnet publish src\GoMoPhoConsole\GoMoPhoCoreConsole.csproj --runtime linux-x64 --output .\bin\GNULinux
del /S packages.lock.json
dotnet publish src\GoMoPhoConsole\GoMoPhoCoreConsole.csproj --runtime ubuntu-x64 --output .\bin\Ubuntu
del /S packages.lock.json
dotnet publish src\GoMoPhoConsole\GoMoPhoCoreConsole.csproj --runtime osx-x64 --output .\bin\macOS

dotnet bin\CrossPlatform\GoMoPhoCoreConsole.dll d test-image\ g h pattern "MVIMG_20180910_12441*.jpg"
rem windows only
bin\Windows\GoMoPhoConsole.exe d test-image\ g s split h
bin\netcoreapp3.1\win-x64\GoMoPhoCoreConsole.exe d test-image\ g s split h
bin\netcoreapp3.1\win-x64\GoMoPhoCoreConsole.exe d test-image\ g s split2
test-image\MVIMG_20180910_124410.mp4
test-image\MVIMG_20180910_124410.gif
split\MVIMG_20180910_124410.mp4
split\MVIMG_20180910_124410.gif
split\MVIMG_20180910_124410.jpg
split2\MVIMG_20180910_124410.mp4
split2\MVIMG_20180910_124410.gif
split2\MVIMG_20180910_124410.jpg

git restore src\GoMoPho\packages.lock.json
git restore src\GoMoPhoFrameworkConsole\packages.lock.json

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