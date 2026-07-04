@echo off
setlocal
powershell.exe -NoProfile -File "%~dp0Enable-Portable-Startup.ps1"
if errorlevel 1 (
  echo Failed to repair portable startup.
  pause
  exit /b 1
)
echo ExplorerTabUtility portable startup has been repaired.
endlocal
