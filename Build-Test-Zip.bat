dotnet clean src\GoogleMotionImage.sln
dotnet build src\GoogleMotionImage.sln
dotnet publish src\GoMoPhoConsole\GoMoPhoCoreConsole.csproj
dotnet bin\netcoreapp2.0\publish\GoMoPhoCoreConsole.dll test-image\
rem windows only
bin\Windows\GoMoPhoConsole.exe test-image\
test-image\MVIMG_20180910_124410.mp4
cd bin
set /P GoVersion=
"c:\Program Files\7-Zip"\7z a GoMoPho.%GoVersion%.zip netcoreapp2.0\publish Windows
cd ..