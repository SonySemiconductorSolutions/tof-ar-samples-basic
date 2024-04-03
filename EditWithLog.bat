@echo off

set PathToUnityExe="C:\Program Files\Unity\Hub\Editor\2021.3.18f1\Editor\Unity.exe"
set root=%~dp0%
cd %root%
set ProjectPath=%root%

del %ProjectPath%\tofar_log_settings
echo LogLevel=Debug >  %ProjectPath%\tofar_log_settings
%PathToUnityExe% -projectPath %ProjectPath% -logFile - > %ProjectPath%\StdoutEditor.log