@echo off
setlocal
set nosign=false
set version=%1
if "%1" == "" echo Version number needed.
if "%1" == "" goto :eof
path %WindowsSdkDir%bin;%PATH%
set projectdir=..\DoubanFM\bin\Release
set imagedir=Images
set tempdir=Temp
set outputdir=Output
set compile=C:\Program Files (x86)\NSIS\Unicode\makensis.exe
set setup=DoubanFMSetup_%version%.exe

:copy
xcopy "%imagedir%" "%tempdir%\" /Q/E/Y
if %errorlevel% NEQ 0 goto :clear
xcopy "%projectdir%" "%tempdir%\" /Q/E/Y
if %errorlevel% NEQ 0 goto :clear
xcopy "DoubanFM.nsi" "%tempdir%\" /Q/Y
if %errorlevel% NEQ 0 goto :clear
if "%nosign%" == "true" goto :compile
:sign
call :SignFile "%tempdir%\DoubanFM.exe"
if %errorlevel% NEQ 0 goto :clear
call :SignFile "%tempdir%\DoubanFM.Core.dll"
if %errorlevel% NEQ 0 goto :clear
call :SignFile "%tempdir%\DoubanFM.Bass.dll"
if %errorlevel% NEQ 0 goto :clear
call :SignFile "%tempdir%\DwmHelper.dll"
if %errorlevel% NEQ 0 goto :clear
call :SignFile "%tempdir%\Hardcodet.Wpf.TaskbarNotification.dll"
if %errorlevel% NEQ 0 goto :clear
:compile
"%compile%" "/XOutFile \"%setup%\"" "/X!define PRODUCT_VERSION \"%version%\"" "%tempdir%\DoubanFM.nsi"
if %errorlevel% NEQ 0 goto :clear
xcopy "%tempdir%\%setup%" "%outputdir%\" /Q/Y
if %errorlevel% NEQ 0 goto :clear
if "%nosign%" == "true" goto :end
:signsetup
call :SignFile "%outputdir%\%setup%"
if %errorlevel% NEQ 0 goto :clear

:end
if "%nosign%" == "true" echo Warning: No digital signiture.
goto :clear

:clear
set error=%errorlevel%
if exist "%tempdir%" rmdir "%tempdir%" /s /q
goto :eof

:SignFile
signtool sign /n K.F.Storm /i K.F.Storm /sha1 445D84D888E121C10503F13E1F6A16757F1F78B2 /t "http://timestamp.globalsign.com/scripts/timstamp.dll" "%1"
@exit /B %errorlevel%

:eof
endlocal