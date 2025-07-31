$ErrorActionPreference = 'Stop'

$packageName = 'sharpcaster'
$toolsDir = "$(Split-Path -parent $MyInvocation.MyCommand.Definition)"

# Remove the shim
Uninstall-BinFile -Name 'sharpcaster'

# Remove the executable
$filePath = Join-Path $toolsDir 'sharpcaster.exe'
if (Test-Path $filePath) {
    Remove-Item $filePath -Force
    Write-Host "SharpCaster executable removed." -ForegroundColor Green
}

Write-Host "SharpCaster has been uninstalled successfully!" -ForegroundColor Green