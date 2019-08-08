set pathMSBuild="C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\"
set pathProjectFile="C:\Users\Stuart\source\repos\Database.Documentor\Database.Documentor.sln"
@echo off
cls
cd %pathMSBuild%
msbuild.exe %pathProjectFile% /p:configuration=release 

pause
@REM pause