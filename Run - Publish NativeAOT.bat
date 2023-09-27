
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

DEL .\bin\%CONFIG%\net7.0\%RUNTIME%\publish\libSkiaSharp.dll
DEL .\bin\%CONFIG%\net7.0\%RUNTIME%\publish\libHarfBuzzSharp.dll

COPY .\bin\%CONFIG%\net7.0\%RUNTIME%\publish\UnityCalculator.exe .

pause
