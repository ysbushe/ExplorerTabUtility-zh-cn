$ErrorActionPreference = 'Stop'

$appDirectory = Split-Path -Parent $MyInvocation.MyCommand.Path
$exe = Join-Path $appDirectory 'ExplorerTabUtility.exe'
if (-not (Test-Path -LiteralPath $exe -PathType Leaf)) {
    throw "ExplorerTabUtility.exe not found: $exe"
}

$exe = [IO.Path]::GetFullPath($exe)
$runKey = 'HKCU:\Software\Microsoft\Windows\CurrentVersion\Run'
$valueName = 'ExplorerTabUtility'
$valueData = "`"$exe`""

Write-Host 'ExplorerTabUtility portable startup helper'
Write-Host 'This optional tool only writes the current-user HKCU Run entry.'
Write-Host 'It does not create VBS files, scheduled tasks, or Startup folder shortcuts.'
Write-Host "Startup target: $exe"

New-Item -Path $runKey -Force | Out-Null
Set-ItemProperty -Path $runKey -Name $valueName -Value $valueData

Write-Host 'Startup entry updated.'
