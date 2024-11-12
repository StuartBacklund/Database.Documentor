set pathMSBuild="C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\"
set pathProjectFile="C:\Projects\Database.Documentor\Database.Documentor.sln"
@echo off
cls
cd %pathMSBuild%
msbuild.exe %pathProjectFile% /p:configuration=release 

pause
@REM pause