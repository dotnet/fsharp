# xUnit3 API Migration Guide

## Status: ✅ COMPLETE

This migration is complete. All API changes have been implemented and verified.

**Last Verified**: November 28, 2025  
**Test Results**: 5,939 tests passing  
**Verification Command**: `./build.sh -c Release --testcoreclr`

## Summary of Changes Made

### 1. Custom Data Attributes - IDataAttribute Interface Pattern

The key solution was implementing `Xunit.v3.IDataAttribute` interface instead of inheriting from `DataAttribute`:

```fsharp
// Solution: Implement IDataAttribute interface
type DirectoryAttribute(dir: string) =
    inherit Attribute()
    interface Xunit.v3.IDataAttribute with
        member _.GetData(testMethod: MethodInfo, disposalTracker: DisposalTracker) =
            let data = // ... generate test data
            let rows = data 
                |> Seq.map (fun row -> Xunit.TheoryDataRow(row) :> Xunit.ITheoryDataRow)
                |> Seq.toArray 
                :> Collections.Generic.IReadOnlyCollection<_>
            ValueTask<_>(rows)  // Use constructor for net472 compatibility
        
        // Required interface members with default values
        member _.Explicit = Nullable()
        member _.Label = null
        member _.Skip = null
        // ... etc
```

### 2. Files Migrated

- **DirectoryAttribute.fs** - Implemented IDataAttribute interface
- **FileInlineDataAttribute.fs** - Implemented IDataAttribute interface  
- **XunitHelpers.fs** - StressAttribute implemented IDataAttribute, removed console capturing
- **XunitSetup.fs** - Configuration for xUnit3
- **Compiler.fs** - Removed Xunit.Abstractions import

### 3. ValueTask Compatibility

Changed from `ValueTask.FromResult()` (not available in net472) to `ValueTask<T>(value)` constructor.

### 4. Console Output Capture

Removed custom console capturing code. xUnit3's built-in capture is used with TestConsole auto-install for FSI tests.

## Package Versions

```xml
<XunitVersion>3.1.0</XunitVersion>
<XunitRunnerConsoleVersion>3.0.0-pre.25</XunitRunnerConsoleVersion>
<XunitRunnerVisualStudioVersion>3.1.4</XunitRunnerVisualStudioVersion>
<FsCheckVersion>2.16.6</FsCheckVersion>
<MicrosoftTestPlatformVersion>17.14.1</MicrosoftTestPlatformVersion>
```

## References

- xUnit3 Migration Guide: https://xunit.net/docs/getting-started/v3/migration
- xUnit3 Extensibility: https://xunit.net/docs/getting-started/v3/migration-extensibility
- IDataAttribute interface: https://github.com/xunit/xunit/blob/main/src/xunit.v3.core/Abstractions/Attributes/IDataAttribute.cs
