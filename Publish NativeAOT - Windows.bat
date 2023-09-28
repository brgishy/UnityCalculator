
@ECHO OFF

CD UnityCalculator

:: Making sure library files exist, and download if they do not
IF NOT EXIST ".\Native\win-x64" MKDIR ".\Native\win-x64"

IF NOT EXIST .\Native\win-x64\libHarfBuzzSharp.lib ECHO Downloading libHarfBuzzSharp.lib (Approx 105 MB)...
IF NOT EXIST .\Native\win-x64\libHarfBuzzSharp.lib powershell -Command "Invoke-WebRequest https://github.com/brgishy/StaticFiles/raw/main/AvaloniaLibs/win-x64/libHarfBuzzSharp.lib?download= -Outfile .\Native\win-x64\libHarfBuzzSharp.lib"

IF NOT EXIST .\Native\win-x64\libSkiaSharp.lib ECHO Downloading libSkiaSharp.lib (Approx 392 MB)...
IF NOT EXIST .\Native\win-x64\libSkiaSharp.lib powershell -Command "Invoke-WebRequest https://github.com/brgishy/StaticFiles/raw/main/AvaloniaLibs/win-x64/libSkiaSharp.lib?download= -Outfile .\Native\win-x64\libSkiaSharp.lib"

:: Performing the Publish
SET CONFIG=Release
SET RUNTIME=win-x64

dotnet publish ^
  -r %RUNTIME% ^
  -c %CONFIG% ^
  -p:PublishAOT=true ^
  -p:TrimmerDefaultAction=link ^
  -p:InvariantGlobalization=true ^
  -p:IlcGenerateStackTraceData=false ^
  -p:IlcOptimizationPreference=Size ^
  -p:DebugType=none ^
  -p:IlcFoldIdenticalMethodBodies=true ^
  -p:IlcTrimMetadata=true  

:: Removing unecessary files and copying the output to the root folder
DEL  .\bin\%CONFIG%\net7.0\%RUNTIME%\publish\*.dll
MOVE .\bin\%CONFIG%\net7.0\%RUNTIME%\publish\*.exe ..

PAUSE
