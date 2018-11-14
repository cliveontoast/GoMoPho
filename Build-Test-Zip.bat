dotnet clean src\GoogleMotionImage.sln
dotnet build src\GoogleMotionImage.sln
dotnet publish src\GoMoPhoConsole\GoMoPhoCoreConsole.csproj
dotnet publish src\GoMoPhoConsole\GoMoPhoCoreConsole.csproj -r win-x64
dotnet publish src\GoMoPhoConsole\GoMoPhoCoreConsole.csproj -r linux-x64
dotnet publish src\GoMoPhoConsole\GoMoPhoCoreConsole.csproj -r ubuntu-x64
dotnet publish src\GoMoPhoConsole\GoMoPhoCoreConsole.csproj -r osx-x64

dotnet bin\netcoreapp2.0\publish\GoMoPhoCoreConsole.dll test-image\
rem windows only
bin\Windows\GoMoPhoConsole.exe test-image\
test-image\MVIMG_20180910_124410.mp4
cd bin
set /P GoVersion=
set zipper=c:\Program Files\7-Zip\7z.exe
if NOT EXIST "%zipper%" SET zipper=c:\Program Files (x86)\7-Zip\7z.exe
"%zipper%" a GoMoPho.runtime.%GoVersion%.zip netcoreapp2.0\publish
"%zipper%" a GoMoPho.WindowsFramework.%GoVersion%.zip Windows
"%zipper%" a GoMoPho.WindowsNative.%GoVersion%.zip netcoreapp2.0\win-x64\publish
"%zipper%" a GoMoPho.macOS.%GoVersion%.zip netcoreapp2.0\osx-x64\publish
"%zipper%" a GoMoPho.GnuLinux.%GoVersion%.zip netcoreapp2.0\linux-x64\publish
"%zipper%" a GoMoPho.Ubuntu.%GoVersion%.zip netcoreapp2.0\ubuntu-x64\publish
cd ..