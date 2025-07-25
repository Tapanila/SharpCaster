param(
    [string]$Runtime = "",
    [string]$Configuration = "Release",
    [switch]$Help
)

if ($Help) {
    Write-Host "SharpCaster Console AOT Build Script" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Usage: ./build-aot.ps1 [options]"
    Write-Host ""
    Write-Host "Options:"
    Write-Host "  -Runtime <rid>        Target runtime (auto-detected if not specified)"
    Write-Host "  -Configuration <cfg>  Build configuration (default: Release)"
    Write-Host "  -Help                 Show this help"
    Write-Host ""
    Write-Host "Current Platform Runtimes:"
    if ($IsWindows -or $env:OS -eq "Windows_NT") {
        Write-Host "  win-x64      Windows x64"
        Write-Host "  win-arm64    Windows ARM64"
    }
    if ($IsLinux) {
        Write-Host "  linux-x64    Linux x64"
        Write-Host "  linux-arm64  Linux ARM64"
    }
    if ($IsMacOS) {
        Write-Host "  osx-x64      macOS Intel"
        Write-Host "  osx-arm64    macOS Apple Silicon"
    }
    Write-Host ""
    Write-Host "Examples:"
    Write-Host "  ./build-aot.ps1                    # Auto-detect runtime"
    Write-Host "  ./build-aot.ps1 -Runtime win-x64   # Specific runtime"
    Write-Host ""
    Write-Host "Note: AOT compilation requires platform-specific build tools"
    exit 0
}

# Auto-detect runtime if not specified
if ([string]::IsNullOrEmpty($Runtime)) {
    if ($IsWindows -or $env:OS -eq "Windows_NT") {
        if ($env:PROCESSOR_ARCHITECTURE -eq "ARM64") {
            $Runtime = "win-arm64"
        } else {
            $Runtime = "win-x64"
        }
    } elseif ($IsLinux) {
        if ((uname -m) -eq "aarch64") {
            $Runtime = "linux-arm64"
        } else {
            $Runtime = "linux-x64"
        }
    } elseif ($IsMacOS) {
        if ((uname -m) -eq "arm64") {
            $Runtime = "osx-arm64"
        } else {
            $Runtime = "osx-x64"
        }
    } else {
        Write-Host "Unable to auto-detect runtime. Please specify -Runtime parameter." -ForegroundColor Red
        exit 1
    }
}

Write-Host "SharpCaster Console AOT Build" -ForegroundColor Cyan
Write-Host "Runtime: $Runtime" -ForegroundColor Yellow
Write-Host "Configuration: $Configuration" -ForegroundColor Yellow
Write-Host ""

$startTime = Get-Date
Write-Host "Building native executable..." -ForegroundColor Green

dotnet publish -r $Runtime -c $Configuration

$endTime = Get-Date
$duration = $endTime - $startTime

if ($LASTEXITCODE -eq 0) {
    $outputPath = "bin\$Configuration\net9.0\$Runtime\publish"
    $exeName = if ($Runtime.StartsWith("win")) { "SharpCaster.Console.exe" } else { "SharpCaster.Console" }
    $exePath = Join-Path $outputPath $exeName
    
    Write-Host ""
    Write-Host "✓ Build successful!" -ForegroundColor Green
    Write-Host "Duration: $($duration.TotalSeconds.ToString('F1'))s" -ForegroundColor Cyan
    
    if (Test-Path $exePath) {
        $sizeMB = [math]::Round((Get-Item $exePath).Length / 1MB, 2)
        Write-Host "Output: $outputPath\$exeName" -ForegroundColor Cyan
        Write-Host "Size: $sizeMB MB" -ForegroundColor Yellow
        
        # Test the executable
        Write-Host ""
        Write-Host "Testing executable..." -ForegroundColor Yellow
        & $exePath help | Select-Object -First 3
        if ($LASTEXITCODE -eq 0) {
            Write-Host "✓ Executable test passed!" -ForegroundColor Green
        } else {
            Write-Host "⚠ Executable test failed" -ForegroundColor Yellow
        }
    } else {
        Write-Host "⚠ Executable not found at expected location" -ForegroundColor Yellow
    }
} else {
    Write-Host ""
    Write-Host "✗ Build failed!" -ForegroundColor Red
    Write-Host "Duration: $($duration.TotalSeconds.ToString('F1'))s" -ForegroundColor Cyan
    
    # Platform-specific help
    if ($Runtime.StartsWith("win")) {
        Write-Host ""
        Write-Host "Windows AOT Prerequisites:" -ForegroundColor Yellow
        Write-Host "  Install Visual Studio with 'Desktop development with C++' workload" -ForegroundColor Yellow
        Write-Host "  For ARM64: Also install 'C++ ARM64 build tools'" -ForegroundColor Yellow
        Write-Host "  Download: https://visualstudio.microsoft.com/downloads/" -ForegroundColor Yellow
    } elseif ($Runtime.StartsWith("linux")) {
        Write-Host ""
        Write-Host "Linux AOT Prerequisites:" -ForegroundColor Yellow
        Write-Host "  Ubuntu/Debian: sudo apt-get install clang zlib1g-dev" -ForegroundColor Yellow
        Write-Host "  RHEL/CentOS:   sudo yum install clang zlib-devel" -ForegroundColor Yellow
    } elseif ($Runtime.StartsWith("osx")) {
        Write-Host ""
        Write-Host "macOS AOT Prerequisites:" -ForegroundColor Yellow
        Write-Host "  Install Xcode command line tools: xcode-select --install" -ForegroundColor Yellow
    }
    
    exit 1
}

Write-Host ""
Write-Host "Build completed successfully!" -ForegroundColor Green