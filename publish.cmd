@echo off

del /Q Stugo.Glue\bin\Release\*.nupkg
call .\build.cmd
.\tools\NuGet\NuGet.exe push Stugo.Glue\bin\Release\*.nupkg -Source https://www.nuget.org/api/v2/package