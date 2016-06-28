@echo off

if "%1"=="debug" (set RELEASE=DebugInstall) else (set RELEASE=Release)
"C:\Program Files (x86)\MSBuild\14.0\Bin\MSBuild.exe" /p:Configuration=%RELEASE% /p:Platform="Any CPU" Stugo.Glue.sln