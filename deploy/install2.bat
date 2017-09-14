@echo off
mode con lines=30 cols=60
rem use administrator mode
%1 mshta vbscript:CreateObject("Shell.Application").ShellExecute("cmd.exe","/c %~s0 ::","","runas",1)(window.close)&&exit
cd /d "%~dp0"
setlocal EnableDelayedExpansion
set "CURRENT_DIR=%cd%"
set FRAMEWORK_DIR=C:\Windows\Microsoft.NET\Framework
if not exist %FRAMEWORK_DIR%\v4.0.* goto notinstalled
:installed
echo Microsoft.NetFramework 4.0 is installed. Do RegAsm.exe
cd /d %FRAMEWORK_DIR%\v4.0.*
rem pwd
echo RegAsm dll
set FRAMEWORK_V4_DIR=%cd%
echo %FRAMEWORK_V4_DIR%
%FRAMEWORK_V4_DIR%\RegAsm.exe /unregister "%CURRENT_DIR%\DiagnoseAssistant1.dll"
%FRAMEWORK_V4_DIR%\RegAsm.exe /codebase "%CURRENT_DIR%\DiagnoseAssistant1.dll"
echo copy fzzl
if not exist c:\fzzl md c:\fzzl
xcopy /y "%CURRENT_DIR%\fzzl" "c:\fzzl"
echo copy dll to IE
if exist "C:\Program Files (x86)\Internet Explorer" copy /y "%CURRENT_DIR%\mysql.data.dll" "C:\Program Files (x86)\Internet Explorer\mysql.data.dll"
if exist "C:\Program Files\Internet Explorer" copy /y "%CURRENT_DIR%\mysql.data.dll" "C:\Program Files\Internet Explorer\mysql.data.dll"
goto end
:notinstalled
echo Microsoft.NetFramework 4.0 is not installed. Please install it first.
:end
pause