@echo off

set root=%~dp0%
cd %root%
set PathToExe="%root%\Build\ToF AR Basic.exe"

del tofar_log_settings
echo LogLevel=Debug >  tofar_log_settings
%PathToExe% -logFile - > %root%\Stdout.log