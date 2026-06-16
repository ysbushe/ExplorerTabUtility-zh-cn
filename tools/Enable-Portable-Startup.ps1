$ErrorActionPreference = 'Stop'

$appDirectory = Split-Path -Parent $MyInvocation.MyCommand.Path
$exe = Join-Path $appDirectory 'ExplorerTabUtility.exe'
if (-not (Test-Path -LiteralPath $exe -PathType Leaf)) {
    throw "ExplorerTabUtility.exe not found: $exe"
}

$exe = [IO.Path]::GetFullPath($exe)
$workDirectory = Split-Path -Parent $exe
$taskName = 'ExplorerTabUtility'
$user = "$env:USERDOMAIN\$env:USERNAME"

$action = New-ScheduledTaskAction -Execute $exe -WorkingDirectory $workDirectory
$trigger = New-ScheduledTaskTrigger -AtLogOn -User $user
$settings = New-ScheduledTaskSettingsSet `
    -AllowStartIfOnBatteries `
    -DontStopIfGoingOnBatteries `
    -ExecutionTimeLimit (New-TimeSpan -Hours 0)
$principal = New-ScheduledTaskPrincipal `
    -UserId $user `
    -LogonType Interactive `
    -RunLevel Limited

Register-ScheduledTask `
    -TaskName $taskName `
    -Action $action `
    -Trigger $trigger `
    -Settings $settings `
    -Principal $principal `
    -Description 'Start ExplorerTabUtility zh-CN portable edition at user logon.' `
    -Force | Out-Null

$startup = [Environment]::GetFolderPath('Startup')
$linkPath = Join-Path $startup 'ExplorerTabUtility.lnk'
$shell = New-Object -ComObject WScript.Shell
$shortcut = $shell.CreateShortcut($linkPath)
try {
    $shortcut.TargetPath = $exe
    $shortcut.WorkingDirectory = $workDirectory
    $shortcut.Description = 'ExplorerTabUtility portable startup fallback'
    $shortcut.Save()
}
finally {
    if ($shortcut -ne $null) {
        [Runtime.InteropServices.Marshal]::FinalReleaseComObject($shortcut) | Out-Null
    }
    if ($shell -ne $null) {
        [Runtime.InteropServices.Marshal]::FinalReleaseComObject($shell) | Out-Null
    }
}

Start-ScheduledTask -TaskName $taskName
