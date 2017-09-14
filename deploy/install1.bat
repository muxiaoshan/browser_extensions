@echo off
set "CURRENT_DIR=%cd%"
for /f "delims=" %%i in ('dir c:\Windows\Microsoft.NET\Framework /ad /b') do ( 
	 set k=%%i
     rem set k=!k:~3!
     echo %k%>%CURRENT_DIR%\framework\%k%.txt
)
pause