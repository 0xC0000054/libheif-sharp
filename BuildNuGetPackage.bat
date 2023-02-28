@echo off
:: ContinuousIntegrationBuild is required for deterministic builds
dotnet pack src\LibHeifSharp.csproj -c Release -p:ContinuousIntegrationBuild=true
pause