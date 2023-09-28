# Unity Calculator
Simple Avalonia app for estimating Unity vs Unreal costs.

Trying to figure out if Unity is right for you.  You can use this tool to see how much
it will cost you over the lifetime of your project and how it compares to Unreal.

# Running
If you have Visual Studio and .Net 7 installed, you only have to open UnityCalculator.sln and hit F5
![image](Images/Screenshot.png)

# Building as single file exe
If you want to build it as a single file exe, you only need to run "Publish NativeAOT - Windows.bat".

This bat file will download all NativeLib dependencies (hosted in my StaticFiles git repository) 
and compilethe application as a NativeAOT one and place the SpeedReader.exe in the root folder.
You'll now be able to run the app anywhere!
