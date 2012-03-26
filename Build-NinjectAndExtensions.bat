SET NoPause=true
mkdir ..\dist-all
del /S /Q ..\dist-all\*
call build-release.cmd
IF ERRORLEVEL 1 GOTO FAILED
xcopy /S dist\* ..\dist-all
cd ..

IF NOT EXIST .\ninject.extensions.factory GOTO ENDFACTORY
	cd ninject.extensions.factory
	del lib\Ninject\*.zip
	copy ..\Ninject\dist\*.zip lib\Ninject
	call UnzipDependencies.cmd
	call build-release.cmd
	IF ERRORLEVEL 1 GOTO FAILED
	xcopy /S dist\* ..\dist-all
	cd ..
:ENDFACTORY

IF NOT EXIST .\ninject.extensions.contextpreservation GOTO ENDCTXPRESERVATION
	cd ninject.extensions.contextpreservation
	del lib\Ninject\*.zip
	del lib\Ninject.Extensions.Factory\*.zip
	copy ..\Ninject\dist\*.zip lib\Ninject
	copy ..\Ninject.Extensions.Factory\dist\*.zip lib\Ninject.Extensions.Factory
	call UnzipDependencies.cmd
	call build-release.cmd
	IF ERRORLEVEL 1 GOTO FAILED
	xcopy /S dist\* ..\dist-all
	cd ..
:ENDCTXPRESERVATION

IF NOT EXIST .\ninject.extensions.namedscope GOTO ENDNSC
	cd ninject.extensions.namedscope
	del lib\Ninject\*.zip
	del lib\Ninject.Extensions.ContextPreservation\*.zip
	copy ..\Ninject\dist\*.zip lib\Ninject
	copy ..\ninject.extensions.contextpreservation\dist\*.zip lib\ninject.extensions.contextpreservation
	call UnzipDependencies.cmd
	call build-release.cmd
	IF ERRORLEVEL 1 GOTO FAILED
	xcopy /S dist\* ..\dist-all
	cd ..
:ENDNSC

IF NOT EXIST .\ninject.extensions.childkernel GOTO ENDCK
	cd ninject.extensions.childkernel
	del lib\Ninject\*.zip
	copy ..\Ninject\dist\*.zip lib\Ninject
	call UnzipDependencies.cmd
	call build-release.cmd
	IF ERRORLEVEL 1 GOTO FAILED
	xcopy /S dist\* ..\dist-all
	cd ..
:ENDCK

IF NOT EXIST .\ninject.extensions.bbveventbroker GOTO ENDBBVEB
	cd ninject.extensions.bbveventbroker
	del lib\Ninject\*.zip
	del lib\Ninject.Extensions.ContextPreservation\*.zip
	del lib\Ninject.Extensions.NamedScope\*.zip
	copy ..\Ninject\dist\*.zip lib\Ninject
	copy ..\ninject.extensions.contextpreservation\dist\*.zip lib\ninject.extensions.contextpreservation
	copy ..\ninject.extensions.namedscope\dist\*.zip lib\ninject.extensions.namedscope
	call UnzipDependencies.cmd
	call build-release.cmd
	IF ERRORLEVEL 1 GOTO FAILED
	xcopy /S dist\* ..\dist-all
	cd ..
:ENDBBVEB

IF NOT EXIST .\ninject.extensions.dependencycreation GOTO ENDDC
	cd ninject.extensions.dependencycreation
	del lib\Ninject\*.zip
	del lib\Ninject.Extensions.ContextPreservation\*.zip
	del lib\Ninject.Extensions.NamedScope\*.zip
	copy ..\Ninject\dist\*.zip lib\Ninject
	copy ..\ninject.extensions.contextpreservation\dist\*.zip lib\ninject.extensions.contextpreservation
	copy ..\ninject.extensions.namedscope\dist\*.zip lib\ninject.extensions.namedscope
	call UnzipDependencies.cmd
	call build-release.cmd
	IF ERRORLEVEL 1 GOTO FAILED
	xcopy /S dist\* ..\dist-all
	cd ..
:ENDDC

IF NOT EXIST .\ninject.extensions.conventions GOTO ENDCONV
	cd ninject.extensions.conventions
	del lib\Ninject\*.zip
	del lib\Ninject.Extensions.Factory\*.zip
	copy ..\Ninject\dist\*.zip lib\Ninject
	copy ..\Ninject.Extensions.Factory\dist\*.zip lib\Ninject.Extensions.Factory
	call UnzipDependencies.cmd
	call build-release.cmd
	IF ERRORLEVEL 1 GOTO FAILED
	xcopy /S dist\* ..\dist-all
	cd ..
:ENDCONV

IF NOT EXIST ninject.extensions.interception GOTO ENDIC
	cd ninject.extensions.interception
	del lib\Ninject\*.zip
	copy ..\Ninject\dist\*.zip lib\Ninject
	call UnzipDependencies.cmd
	call build-release.cmd
	IF ERRORLEVEL 1 GOTO FAILED
	xcopy /S dist\* ..\dist-all
	cd ..
:ENDIC

IF NOT EXIST .\ninject.extensions.logging GOTO ENDLOG
	cd ninject.extensions.logging
	del lib\Ninject\*.zip
	copy ..\Ninject\dist\*.zip lib\Ninject
	call UnzipDependencies.cmd
	call build-release.cmd
	IF ERRORLEVEL 1 GOTO FAILED
	xcopy /S dist\* ..\dist-all
	cd ..
:ENDLOG

IF NOT EXIST .\ninject.extensions.messagebroker GOTO ENDMB
	cd ninject.extensions.messagebroker
	del lib\Ninject\*.zip
	copy ..\Ninject\dist\*.zip lib\Ninject
	call UnzipDependencies.cmd
	call build-release.cmd
	IF ERRORLEVEL 1 GOTO FAILED
	xcopy /S dist\* ..\dist-all
	cd ..
:ENDMB

IF NOT EXIST .\ninject.extensions.wf GOTO ENDWF
	cd ninject.extensions.wf
	del lib\Ninject\*.zip
	copy ..\Ninject\dist\*.zip lib\Ninject
	call UnzipDependencies.cmd
	call build-release.cmd
	IF ERRORLEVEL 1 GOTO FAILED
	xcopy /S dist\* ..\dist-all
	cd ..
:ENDWF

IF NOT EXIST .\ninject.extensions.weakeventmessagebroker GOTO ENDWEAKEB
	cd ninject.extensions.weakeventmessagebroker
	del lib\Ninject\*.zip
	copy ..\Ninject\dist\*.zip lib\Ninject
	call UnzipDependencies.cmd
	call build-release.cmd
	IF ERRORLEVEL 1 GOTO FAILED
	xcopy /S dist\* ..\dist-all
	cd ..
:ENDWEAKEB

IF NOT EXIST .\ninject.extensions.xml GOTO ENDXML
	cd ninject.extensions.xml
	del lib\Ninject\*.zip
	copy ..\Ninject\dist\*.zip lib\Ninject
	call UnzipDependencies.cmd
	call build-release.cmd
	IF ERRORLEVEL 1 GOTO FAILED
	xcopy /S dist\* ..\dist-all
	cd ..
:ENDXML

IF NOT EXIST .\ninject.mockingkernel GOTO ENDMK
	cd ninject.mockingkernel
	del lib\Ninject\*.zip
	copy ..\Ninject\dist\*.zip lib\Ninject
	call UnzipDependencies.cmd
	call build-release.cmd
	IF ERRORLEVEL 1 GOTO FAILED
	xcopy /S dist\* ..\dist-all
	cd ..
:ENDMK

IF NOT EXIST .\ninject.web.common GOTO ENDWEBCOMMON
	cd ninject.web.common
	del lib\Ninject\*.zip
	copy ..\Ninject\dist\*.zip lib\Ninject
	call UnzipDependencies.cmd
	call build-release.cmd
	IF ERRORLEVEL 1 GOTO FAILED
	xcopy /S dist\* ..\dist-all
	cd ..
:ENDWEBCOMMON

IF NOT EXIST .\ninject.extensions.wcf GOTO ENDWCF
	cd ninject.extensions.wcf
	del lib\Ninject\*.zip
	del lib\Ninject.Web.Common\*.zip
	copy ..\Ninject\dist\*.zip lib\Ninject
	copy ..\ninject.web.common\dist\*.zip lib\ninject.web.common
	call UnzipDependencies.cmd
	call build-release.cmd
	IF ERRORLEVEL 1 GOTO FAILED
	xcopy /S dist\* ..\dist-all
	cd ..
:ENDWCF

IF NOT EXIST .\ninject.web GOTO ENDWEB
	cd ninject.web
	del lib\Ninject\*.zip
	del lib\Ninject.Web.Common\*.zip
	copy ..\Ninject\dist\*.zip lib\Ninject
	copy ..\ninject.web.common\dist\*.zip lib\ninject.web.common
	call UnzipDependencies.cmd
	call build-release.cmd
	IF ERRORLEVEL 1 GOTO FAILED
	xcopy /S dist\* ..\dist-all
	cd ..
:ENDWEB

:MVC
IF NOT EXIST .\ninject.web.mvc GOTO ENDMVC
	cd ninject.web.mvc
	del lib\Ninject\*.zip
	del lib\Ninject.Web.Common\*.zip
	copy ..\Ninject\dist\*.zip lib\Ninject
	copy ..\ninject.web.common\dist\*.zip lib\ninject.web.common
	cd mvc1
	call UnzipDependencies.cmd
	call build-release.cmd
	IF ERRORLEVEL 1 GOTO FAILED
	xcopy /S dist\* ..\..\dist-all
	cd ..
	cd mvc2
	call build-release.cmd
	IF ERRORLEVEL 1 GOTO FAILED
	xcopy /S dist\* ..\..\dist-all
	cd ..
	cd mvc3
	call build-release.cmd
	IF ERRORLEVEL 1 GOTO FAILED
	xcopy /S dist\* ..\..\dist-all
	cd ..
	cd ..
:ENDMVC

IF NOT EXIST .\ninject.web.mvc.fluentvalidation GOTO ENDMVCFV
	cd ninject.web.mvc.fluentvalidation
	del lib\Ninject\*.zip
	copy ..\Ninject\dist\*.zip lib\Ninject
	call UnzipDependencies.cmd
	call build-release.cmd
	IF ERRORLEVEL 1 GOTO FAILED
	xcopy /S dist\* ..\dist-all
	cd ..
:ENDMVCFV


pause
goto END

:FAILED
cd ..
pause

:END