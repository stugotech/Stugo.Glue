@echo off

if [%1]==[] goto usage

git tag -a %1 -m "v%1"
del /Q Stugo.Core\bin\Release\*.nupkg
call .\build.cmd

goto :eof
:usage
echo Specify version number