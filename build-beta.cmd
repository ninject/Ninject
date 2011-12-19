@echo off
set nantfile=Ninject.build
set nantexe=tools\nant\nant.exe
set buildlog=Ninject-Nant-Build.log
set unittestlog=Ninject-Nant-unit-tests.log

%nantexe% -buildfile:%nantfile% clean %1 %2 %3 %4 %5 %6 %7 %8
IF ERRORLEVEL 1 GOTO Failed
%nantexe% -buildfile:%nantfile% "-D:product.additionalVersionTag=-beta" package-source %1 %2 %3 %4 %5 %6 %7 %8
IF ERRORLEVEL 1 GOTO Failed
%nantexe% -buildfile:%nantfile% "-D:product.additionalVersionTag=-beta" "-D:build.config=release" allPlatforms %1 %2 %3 %4 %5 %6 %7 %8
IF ERRORLEVEL 1 GOTO Failed
%nantexe% -buildfile:%nantfile% -q -nologo revert

IF ERRORLEVEL 1 GOTO Failed

echo "Release build completed."
GOTO End

:Failed
%nantexe% -buildfile:%nantfile% -q -nologo revert
echo "============================================================"
echo "BUILD FAILED"
echo "============================================================"

IF NOT "%NoPause%"=="true" goto END 
exit /B 1

:End
if "%NoPause%"=="true" goto ENDBATCHFILE 
pause
:ENDBATCHFILE