$ErrorActionPreference = 'Stop'

$packageName = 'sharpcaster'
$toolsDir = "$(Split-Path -parent $MyInvocation.MyCommand.Definition)"

# Detect architecture and set appropriate download URL
$architecture = Get-OSArchitectureWidth
$processorArch = $env:PROCESSOR_ARCHITECTURE

if ($processorArch -eq "ARM64" -or $env:PROCESSOR_ARCHITEW6432 -eq "ARM64") {
    $url = 'https://github.com/Tapanila/SharpCaster/releases/download/3.0.0-beta1/sharpcaster-win-arm.exe'
    $checksum = ''
    $archSuffix = 'arm64'
    Write-Host "Detected ARM64 architecture, downloading ARM64 version..." -ForegroundColor Yellow
} else {
    $url = 'https://github.com/Tapanila/SharpCaster/releases/download/3.0.0-beta1/sharpcaster-win-x64.exe'
    $checksum = ''
    $archSuffix = 'x64'
    Write-Host "Detected x64 architecture, downloading x64 version..." -ForegroundColor Yellow
}

$packageArgs = @{
  packageName   = $packageName
  unzipLocation = $toolsDir
  fileType      = 'exe'
  url64bit      = $url
  softwareName  = 'sharpcaster*'
  checksum64    = $checksum
  checksumType64= 'sha256'
  silentArgs    = '/VERYSILENT /SUPPRESSMSGBOXES /NORESTART /SP-'
  validExitCodes= @(0)
}

# Download and install the executable
$filePath = Join-Path $toolsDir 'sharpcaster.exe'
Get-ChocolateyWebFile -PackageName $packageName -FileFullPath $filePath -Url64bit $url -Checksum64 $checksum -ChecksumType64 $packageArgs.checksumType64

# Create a shim for the executable
Install-BinFile -Name 'sharpcaster' -Path $filePath

Write-Host "SharpCaster ($archSuffix) has been installed successfully!" -ForegroundColor Green
Write-Host "You can now use 'sharpcaster' command from anywhere in your terminal." -ForegroundColor Yellow
Write-Host "Run 'sharpcaster --help' to get started." -ForegroundColor Yellow