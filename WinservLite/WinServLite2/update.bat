@echo off
echo Updating WinServLite2 from DC-01...
taskkill /im "WinServLite2.exe" /f
set UPDATEDIR=%CD%
set ARGS=/S /Y /K /H /R
pushd "\\dc-01\InSupport_InstallationsService\WinServ Lite 2.0 Beta"
xcopy ".\*.dll" "%UPDATEDIR%" %ARGS%
xcopy ".\*.exe" "%UPDATEDIR%" %ARGS%
xcopy ".\*.config" "%UPDATEDIR%" %ARGS%
xcopy ".\*.xml" "%UPDATEDIR%" %ARGS%
xcopy ".\*.pdb" "%UPDATEDIR%" %ARGS%
popd
echo Done!
pause