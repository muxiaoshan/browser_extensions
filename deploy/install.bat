@echo off
set "CURRENT_DIR=%cd%"
c:
cd c:\Windows\Microsoft.NET\Framework
for /R %%s in (.) do (
  rem echo file name is %%s
  set "FILE_NAME=%%s"
  echo %FILE_NAME%
  rem echo file name is "%FILE_NAME%"
  rem if %%s|findstr "^v4.0" ( echo v4.0 installed.)  
  if not x%FILE_NAME:v4.0=%==x%FILE_NAME% echo It contains v4.0
)
pause