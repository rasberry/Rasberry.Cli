@echo off
cd /d "%~dp0"
if exist "test\TestResults\report" rmdir /s/q "test\TestResults\report"
call dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura /p:Exclude="[xunit*]\*" /p:CoverletOutput="TestResults/"
call reportgenerator "-reports:coverage.cobertura.xml" "-targetdir:test/TestResults/report" "-reporttypes:Html"