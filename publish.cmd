@echo off

del /Q Stugo.Glue\bin\Release\*.nupkg
call .\build.cmd
.\tools\NuGet\NuGet.exe push Stugo.Glue\bin\Release\*.nupkg -Source https://www.myget.org/F/stugo-private/api/v2/package