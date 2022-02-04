@echo off
setlocal
if "%~1"=="" goto :EOF
call :%*
goto :EOF

:cover
:coverage
::https://github.com/jke94/NetCoreCoverage
::https://github.com/danielpalme/ReportGenerator

cd /d "%~dp0"
if exist "test\TestResults" rmdir /s/q "test\TestResults"
call dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura /p:Exclude="[xunit*]\*" /p:CoverletOutput="TestResults/"
call reportgenerator "-reports:test/TestResults/coverage.cobertura.xml" "-targetdir:test/TestResults/report" "-reporttypes:HtmlInline"

goto :EOF

:pack
call dotnet pack -c Release

::use existing nuget cache folder
set NUGETROOT="%userprofile%\.nuget"
for %%n in ("%~dp0src\bin\Release\*.nupkg") do (
	echo "Installing %%n"
	call nuget add "%%n" -source "%NUGETROOT:"=%\packages"
)
::put symbols under sibling 'symbols' folder
for %%n in ("%~dp0src\bin\Release\*.snupkg") do (
	echo "Installing %%n"
	call nuget add "%%n" -source "%NUGETROOT:"=%\symbols"
)
goto :EOF
