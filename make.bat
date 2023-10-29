@echo off
setlocal
if "%~1"=="" goto usage
call :%*
goto :EOF

:usage
echo Usage:
echo %~n0 cover             run coverage
echo %~n0 pack (version)    create nuget package and register package locally
echo %~n0 depack (version)  unregister package from local repo

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

::put packages in existing .nuget folder
set NUGETROOT="%userprofile%\.nuget"
for %%n in ("%~dp0src\bin\Release\*.nupkg") do (
	echo "Installing %%n"
	call nuget add "%%n" -source "%NUGETROOT:"=%\nuget"
)
::put symbols under sibling 'symbols' folder
for %%n in ("%~dp0src\bin\Release\*.snupkg") do (
	echo "Installing %%n"
	call nuget add "%%n" -source "%NUGETROOT:"=%\symbols"
)
goto :EOF

:depack
if "%~1"=="" echo "missing package version" && goto :EOF

set NUGETROOT="%userprofile%\.nuget"
call nuget delete "Rasberry.Cli" "%~1" -NonInteractive -Source "%NUGETROOT:"=%\nuget"
call nuget delete "Rasberry.Cli" "%~1" -NonInteractive -Source "%NUGETROOT:"=%\symbols"
goto :EOF
