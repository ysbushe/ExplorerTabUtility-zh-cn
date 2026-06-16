@echo off
setlocal
powershell.exe -NoProfile -ExecutionPolicy Bypass -File "%~dp0Enable-Portable-Startup.ps1"
if errorlevel 1 (
  echo Failed to enable scheduled startup.
  pause
  exit /b 1
)
echo ExplorerTabUtility startup task has been enabled.
endlocal
