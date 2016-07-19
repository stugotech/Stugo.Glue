@echo off

echo Pushing to NuGet feed
.\tools\NuGet\NuGet.exe push Stugo.Glue\bin\Release\*.nupkg -Source https://www.myget.org/F/stugo-private/api/v2/package