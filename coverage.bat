@echo off

::https://github.com/jke94/NetCoreCoverage
::https://github.com/danielpalme/ReportGenerator

cd /d "%~dp0"
if exist "test\TestResults" rmdir /s/q "test\TestResults"
call dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura /p:Exclude="[xunit*]\*" /p:CoverletOutput="TestResults/"
call reportgenerator "-reports:test/TestResults/coverage.cobertura.xml" "-targetdir:test/TestResults/report" "-reporttypes:HtmlInline"