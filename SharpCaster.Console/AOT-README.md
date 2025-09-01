# AOT (Ahead-of-Time) Compilation Guide

The SharpCaster Console application is configured for AOT compilation by default, creating fast-starting, self-contained native executables.

## 🚀 Quick Start

### Platform-Specific Build Script
```powershell
# Auto-detect current platform and build
./build-aot.ps1

# Build for specific runtime
./build-aot.ps1 -Runtime win-x64

# Show help
./build-aot.ps1 -Help
```

### Manual dotnet commands
```bash
# AOT compilation (default - requires prerequisites)
dotnet publish -r win-x64 -c Release

# Disable AOT for self-contained build
dotnet publish -r win-x64 -c Release -p:PublishAot=false --self-contained
```

## 📋 Prerequisites for AOT Compilation

### Windows
- **Visual Studio 2022** with "Desktop development with C++" workload
- Or **Visual Studio Build Tools 2022** with C++ build tools
- For ARM64: Also install "C++ ARM64 build tools"

### Linux (Ubuntu/Debian)
```bash
sudo apt-get update
sudo apt-get install clang zlib1g-dev
```

### Linux (RHEL/CentOS/Fedora)
```bash
sudo yum install clang zlib-devel
# or
sudo dnf install clang zlib-devel
```

### macOS
```bash
xcode-select --install
```

## 🎯 Benefits of AOT Compilation

### Performance
- **Faster startup time** - No JIT compilation needed
- **Lower memory usage** - No IL code or JIT compiler overhead
- **Better code optimization** - Full static analysis and optimization

### Deployment
- **Single executable** - Everything bundled into one file
- **No .NET runtime required** - Completely self-contained
- **Smaller deployment size** - Only includes used code (trimming)

### Size Comparison
| Build Type | Approximate Size | Files | .NET Runtime Required |
|------------|------------------|-------|-----------------------|
| Regular | ~200 KB | 1 | ✅ Yes |
| Self-contained | ~70 MB | ~100 | ❌ No |
| AOT | ~15-25 MB | 1 | ❌ No |

## 🏗️ Project Configuration

The project is already configured for AOT compatibility:

```xml
<PropertyGroup>
  <IsAotCompatible>true</IsAotCompatible>
  <InvariantGlobalization>true</InvariantGlobalization>
  <TrimMode>full</TrimMode>
  <JsonSerializerIsReflectionEnabledByDefault>false</JsonSerializerIsReflectionEnabledByDefault>
</PropertyGroup>

<!-- AOT is enabled conditionally -->
<PropertyGroup Condition="'$(PublishAot)' == 'true'">
  <PublishAot>true</PublishAot>
  <PublishTrimmed>true</PublishTrimmed>
  <PublishSingleFile>false</PublishSingleFile>
</PropertyGroup>
```

## 🔧 Supported Runtimes

| Platform | Runtime ID | Notes |
|----------|------------|---------|
| Windows x64 | `win-x64` | Default |
| Windows ARM64 | `win-arm64` | Requires ARM64 build tools |
| Linux x64 | `linux-x64` | Most common |
| Linux ARM64 | `linux-arm64` | Raspberry Pi, etc. |
| macOS x64 | `osx-x64` | Intel Macs |
| macOS ARM64 | `osx-arm64` | Apple Silicon Macs |

## 🐛 Troubleshooting

### "Platform linker not found" Error
This means AOT prerequisites are not installed:
- **Windows**: Install Visual Studio with C++ workload
- **Linux**: Install `clang` and `zlib1g-dev`
- **macOS**: Run `xcode-select --install`

### JSON Serialization Issues
The app uses source-generated JSON serialization for AOT compatibility. If you add new types, ensure they're included in the `ConsoleJsonContext` or the main `SharpcasteSerializationContext`.

### Trimming Warnings
The app is configured for full trimming. If you see trimming warnings, you may need to add `[DynamicallyAccessedMembers]` attributes or configure the trimmer.

### Large Executable Size
AOT executables are larger than regular builds but much smaller than self-contained builds. The size includes:
- Your application code
- .NET runtime (optimized)
- Only the libraries you actually use

## 📊 Performance Comparison

| Metric | Regular | Self-contained | AOT |
|--------|---------|----------------|----- |
| Cold start | ~500ms | ~300ms | ~50ms |
| Memory usage | ~25MB | ~30MB | ~15MB |
| File size | ~200KB | ~70MB | ~20MB |
| .NET required | ✅ Yes | ❌ No | ❌ No |

## 💡 Tips

1. **Use the build script** - It auto-detects your platform and handles prerequisites checking
2. **Test thoroughly** - AOT can expose runtime issues that don't appear in regular builds
3. **Profile startup time** - AOT provides the biggest benefit for command-line tools with frequent starts
4. **Platform-specific compilation** - AOT requires building on the target platform with appropriate toolchain

## 🔗 Learn More

- [.NET Native AOT Documentation](https://docs.microsoft.com/en-us/dotnet/core/deploying/native-aot/)
- [AOT Prerequisites](https://docs.microsoft.com/en-us/dotnet/core/deploying/native-aot/prerequisites)
- [Trimming and AOT Compatibility](https://docs.microsoft.com/en-us/dotnet/core/deploying/trimming/)
