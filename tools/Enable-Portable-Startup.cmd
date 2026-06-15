@echo off
setlocal
set "EXPLORER_TAB_UTILITY_EXE=%~dp0ExplorerTabUtility.exe"
powershell.exe -NoProfile -ExecutionPolicy Bypass -Command "$ErrorActionPreference='Stop'; $exe=[IO.Path]::GetFullPath($env:EXPLORER_TAB_UTILITY_EXE); $startup=[Environment]::GetFolderPath('Startup'); $linkPath=Join-Path $startup 'ExplorerTabUtility.lnk'; $shell=New-Object -ComObject WScript.Shell; $link=$shell.CreateShortcut($linkPath); $link.TargetPath=$exe; $link.WorkingDirectory=Split-Path $exe; $link.Description='ExplorerTabUtility portable startup'; $link.Save()"
if errorlevel 1 (
  echo Failed to enable startup.
  pause
  exit /b 1
)
start "" "%EXPLORER_TAB_UTILITY_EXE%"
endlocal
