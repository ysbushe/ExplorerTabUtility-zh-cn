$ErrorActionPreference = 'Stop'

$appDirectory = Split-Path -Parent $MyInvocation.MyCommand.Path
$exe = Join-Path $appDirectory 'ExplorerTabUtility.exe'
if (-not (Test-Path -LiteralPath $exe -PathType Leaf)) {
    throw "ExplorerTabUtility.exe not found: $exe"
}

$exe = [IO.Path]::GetFullPath($exe)
$taskName = 'ExplorerTabUtility'

function Test-AppReference {
    param([string]$Text)

    if ([string]::IsNullOrWhiteSpace($Text)) {
        return $false
    }

    return $Text.IndexOf($exe, [StringComparison]::OrdinalIgnoreCase) -ge 0
}

function Remove-AppStartupFile {
    param([string]$Path)

    if (-not (Test-Path -LiteralPath $Path)) {
        return
    }

    $extension = [IO.Path]::GetExtension($Path)
    $belongsToApp = $false

    if ($extension -ieq '.vbs') {
        $belongsToApp = Test-AppReference -Text (Get-Content -LiteralPath $Path -Raw)
    }
    elseif ($extension -ieq '.lnk') {
        $shell = New-Object -ComObject WScript.Shell
        try {
            $shortcut = $shell.CreateShortcut($Path)
            $targetPath = [IO.Path]::GetFullPath($shortcut.TargetPath)
            $belongsToApp = [string]::Equals($targetPath, $exe, [StringComparison]::OrdinalIgnoreCase)
        }
        catch {
            Write-Warning "Startup shortcut could not be inspected and was left unchanged: $Path"
        }
        finally {
            if ($shortcut -ne $null) {
                [Runtime.InteropServices.Marshal]::FinalReleaseComObject($shortcut) | Out-Null
            }
            if ($shell -ne $null) {
                [Runtime.InteropServices.Marshal]::FinalReleaseComObject($shell) | Out-Null
            }
        }
    }

    if ($belongsToApp) {
        Remove-Item -LiteralPath $Path -Force
    }
}

function Remove-AppScheduledTask {
    param([string]$Name)

    $xml = schtasks.exe /Query /TN $Name /XML 2>$null | Out-String
    if ($LASTEXITCODE -eq 0 -and (Test-AppReference -Text $xml)) {
        schtasks.exe /Delete /TN $Name /F | Out-Null
    }
}

# Manual optional repair tool for the portable edition.
# It uses the transparent current-user Run key and removes older startup
# leftovers created by previous portable packages.
$startup = [Environment]::GetFolderPath('Startup')
foreach ($name in @('ExplorerTabUtility.vbs', 'ExplorerTabUtility.lnk')) {
    Remove-AppStartupFile -Path (Join-Path $startup $name)
}

Remove-AppScheduledTask -Name $taskName

$runKey = 'HKCU:\Software\Microsoft\Windows\CurrentVersion\Run'
New-Item -Path $runKey -Force | Out-Null
Set-ItemProperty -Path $runKey -Name $taskName -Value "`"$exe`""

$approvedKey = 'HKCU:\Software\Microsoft\Windows\CurrentVersion\Explorer\StartupApproved\Run'
New-Item -Path $approvedKey -Force | Out-Null
New-ItemProperty -Path $approvedKey -Name $taskName -PropertyType Binary -Value ([byte[]](0x02,0,0,0,0,0,0,0,0,0,0,0)) -Force | Out-Null

try {
    Start-Process -FilePath $exe
}
catch {
    Write-Warning "Startup entry was repaired, but ExplorerTabUtility could not be started automatically: $($_.Exception.Message)"
}
