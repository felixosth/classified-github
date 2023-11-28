@ECHO OFF
echo Installing the service..
set file="%~dp0InSupport.O3C.API.exe"
REM echo %file%
SC CREATE "O3CAPI" binPath=%file% DisplayName="O3C API" start=auto
pause