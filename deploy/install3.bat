@echo off
setlocal EnableDelayedExpansion
set "CURRENT_DIR=%cd%"
set "FRAMEWORK_DIR=C:\Windows\Microsoft.NET\Framework"
for /f "delims=*" %%i in ('dir /s/b "C:\Windows\Microsoft.NET\Framework\v4.0.30319\RegAsm.exe"') do (
 if exist "%%i" echo RegAsm主程序的路径为： "%%i"
)
pause