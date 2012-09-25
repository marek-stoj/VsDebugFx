@echo off
cls & nant -buildfile:VsDebugFx.build push-to-nuget
pause
