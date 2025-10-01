# xUnit3 API Migration Guide - Detailed Steps

## Overview

This document provides detailed instructions for completing the xUnit2 to xUnit3 API migration in the F# compiler test suite. Phase 1 (infrastructure setup) is complete. This guide covers Phase 2 (API migration).

## Current Status

**Phase 1: ✅ COMPLETED**
- Package versions updated to xUnit3
- NuGet sources configured
- Project files updated
- Packages restored successfully

**Phase 2: 🚧 IN PROGRESS**
- API migration identified but not yet implemented
- Build errors documented below

## Files Requiring Migration

### 1. XunitHelpers.fs (279 lines) - CRITICAL

**Location**: `tests/FSharp.Test.Utilities/XunitHelpers.fs`

**Purpose**: Custom xUnit extensibility for:
- Console output capture per test
- Internal test parallelization
- Batch trait injection for CI
- Custom test discovery

**Required Changes**:

#### Namespace Updates
```fsharp
// OLD (xUnit2)
open Xunit.Sdk
open Xunit.Abstractions

// NEW (xUnit3)
open Xunit.Sdk  // Still valid, but types moved
// Xunit.Abstractions removed - types moved to Xunit.Sdk
```

#### DataAttribute Migration
```fsharp
// OLD (xUnit2)
type StressAttribute([<ParamArray>] data: obj array) =
    inherit DataAttribute()
    override this.GetData _ = Seq.init this.Count (fun i -> [| yield! data; yield box i |])

// NEW (xUnit3) - DataAttribute removed, use Theory data supplier pattern
// Option 1: Use MemberData/ClassData attributes instead
// Option 2: Implement IDataDiscoverer interface
// See: https://xunit.net/docs/getting-started/v3/migration#custom-data-attributes
```

#### Test Runner Migration
```fsharp
// OLD (xUnit2)
type ConsoleCapturingTestRunner(...) =
    inherit XunitTestRunner(...)
    member _.BaseInvokeTestMethodAsync aggregator = base.InvokeTestMethodAsync aggregator
    override this.InvokeTestAsync (aggregator: ExceptionAggregator) = ...

// NEW (xUnit3)
// XunitTestRunner API changed significantly
// Need to use new test execution model
// See: https://xunit.net/docs/getting-started/v3/migration#custom-test-runners
```

#### Test Case Customization
```fsharp
// OLD (xUnit2)
type CustomTestCase(...) =
    inherit XunitTestCase(...)
    override testCase.RunAsync(...) = ...

type CustomTheoryTestCase(...) =
    inherit XunitTheoryTestCase(...)
    override testCase.RunAsync(...) = ...

// NEW (xUnit3)
// Test case model redesigned
// XunitTestCase and XunitTheoryTestCase removed
// Use new IXunitTestCase interface
```

#### Test Collection/Method/Class APIs
```fsharp
// OLD (xUnit2)
testCase.TestMethod.TestClass.TestCollection.CollectionDefinition
testCase.TestMethod.TestClass.Class
testCase.TestMethod.Method

// NEW (xUnit3)  
// Property names changed:
// - TestCollection API restructured
// - TestClass.Class removed
// - TestMethod.Method removed
// Need to use new metadata APIs
```

#### Framework Registration
```fsharp
// OLD (xUnit2)
type FSharpXunitFramework(sink: IMessageSink) =
    inherit XunitTestFramework(sink)
    override this.CreateExecutor(assemblyName) = ...
    override this.CreateDiscoverer(assemblyInfo) = ...

// NEW (xUnit3)
// Framework model changed
// XunitTestFramework still exists but API changed
// CreateExecutor and CreateDiscoverer signatures updated
```

### 2. DirectoryAttribute.fs (33 lines)

**Location**: `tests/FSharp.Test.Utilities/DirectoryAttribute.fs`

**Purpose**: Custom Theory data attribute that discovers test files in a directory

**Required Changes**:

```fsharp
// OLD (xUnit2)
open Xunit.Sdk

type DirectoryAttribute(dir: string) =
    inherit DataAttribute()
    override _.GetData _ = createCompilationUnitForFiles baselineSuffix directoryPath includes

// NEW (xUnit3)
// DataAttribute removed in xUnit3
// Options:
// 1. Use ClassData or MemberData with a data class
// 2. Implement custom data discoverer with IDataDiscoverer
// 3. Convert to TheoryData<T> pattern

// Recommended approach:
type DirectoryDataClass(dir: string) =
    interface IEnumerable<obj[]> with
        member _.GetEnumerator() = 
            (createCompilationUnitForFiles baselineSuffix directoryPath includes).GetEnumerator()
        member _.GetEnumerator() : IEnumerator = 
            (createCompilationUnitForFiles baselineSuffix directoryPath includes :> IEnumerable).GetEnumerator()

[<ClassData(typeof<DirectoryDataClass>, dir)>]
// Or use MemberData pointing to a static method
```

### 3. FileInlineDataAttribute.fs (179 lines)

**Location**: `tests/FSharp.Test.Utilities/FileInlineDataAttribute.fs`

**Purpose**: Inline data attribute that reads test data from files

**Required Changes**:

Similar to DirectoryAttribute - needs to migrate from `DataAttribute` base class to xUnit3 data patterns.

```fsharp
// OLD (xUnit2)
open Xunit.Sdk
open Xunit.Abstractions

type FileInlineDataAttribute(...) =
    inherit DataAttribute()
    override _.GetData _ = ...

type FileInlineDataRow(...) =
    interface IXunitSerializable with
        member this.Serialize(info: IXunitSerializationInfo) = ...
        member this.Deserialize(info: IXunitSerializationInfo) = ...

// NEW (xUnit3)
// IXunitSerializable interface changed or removed
// Serialization model updated
// Need to check xUnit3 serialization APIs
```

### 4. Compiler.fs (2088 lines)

**Location**: `tests/FSharp.Test.Utilities/Compiler.fs`

**Required Changes**:

```fsharp
// OLD (xUnit2)
open Xunit
open Xunit.Abstractions  // ← This namespace removed

// NEW (xUnit3)
open Xunit
// Remove Xunit.Abstractions import - not used in this file
```

**Status**: ✅ COMPLETED (commit e9d7dc0)

### 5. XunitSetup.fs (14 lines)

**Location**: `tests/FSharp.Test.Utilities/XunitSetup.fs`

**Current Code**:
```fsharp
[<CollectionDefinition(nameof NotThreadSafeResourceCollection, DisableParallelization = true)>]
type NotThreadSafeResourceCollection = class end

[<assembly: TestFramework("FSharp.Test.FSharpXunitFramework", "FSharp.Test.Utilities")>]
```

**Required Changes**:
- Verify `CollectionDefinition` still supports `DisableParallelization` parameter
- Verify `TestFramework` attribute still works with custom framework
- Likely minimal changes needed

## Migration Strategy

### Recommended Approach

1. **Start with Simple Fixes**
   - ✅ Remove `Xunit.Abstractions` imports where not needed (Compiler.fs - done)
   - Fix namespace issues in remaining files

2. **Migrate Data Attributes** (DirectoryAttribute, FileInlineDataAttribute, StressAttribute)
   - Convert to `ClassData` or `MemberData` pattern
   - This avoids need for custom `DataAttribute` subclasses
   - Example migration pattern documented above

3. **Simplify or Remove Custom Extensibility** (if possible)
   - Evaluate if `ConsoleCapturingTestRunner` is still needed
     - xUnit3 has better built-in output capture
     - May be able to use `ITestOutputHelper` directly
   - Evaluate if custom parallelization is still needed
     - xUnit3 has improved parallelization options in config
   - Batch trait injection might be simplified

4. **Migrate Complex Extensibility** (if needed)
   - Custom test framework (FSharpXunitFramework)
   - Custom test case runners
   - Custom test discovery
   - Reference official xUnit3 extensibility docs

5. **Test and Validate**
   - Build FSharp.Test.Utilities
   - Build one test project (e.g., FSharp.Core.UnitTests)
   - Run tests: `dotnet test --logger:"console;verbosity=normal"`
   - Validate output capture works
   - Validate parallelization works
   - Validate batch filtering works

## Key xUnit3 API Changes

### Removed/Changed Types

| xUnit2 Type | xUnit3 Status |
|------------|---------------|
| `DataAttribute` | ❌ Removed - use ClassData/MemberData |
| `XunitTestCase` | ⚠️ Changed - new interface model |
| `XunitTheoryTestCase` | ⚠️ Changed - new interface model |
| `XunitTestRunner` | ⚠️ Changed - API updated |
| `ExceptionAggregator` | ⚠️ Changed - API updated |
| `IXunitSerializable` | ⚠️ Changed - serialization model updated |
| `Xunit.Abstractions` namespace | ❌ Removed - types moved to Xunit.Sdk |
| `TestMethod`, `TestClass`, `TestCollection` | ⚠️ Changed - API updated |

### Available Packages

- `xunit.v3.assert` - Assert methods
- `xunit.v3.extensibility.core` - Extensibility APIs (Xunit.Sdk)
- `xunit.v3.common` - Common types
- `xunit.v3.core` - Core test execution (for test projects)

## Resources

- xUnit3 Migration Guide: https://xunit.net/docs/getting-started/v3/migration
- xUnit3 Extensibility: https://xunit.net/docs/getting-started/v3/extensibility
- WPF Migration Example: https://github.com/dotnet/wpf/pull/10890/files
- WinForms Migration Example: https://github.com/dotnet/winforms/pull/13540/files

## Build Errors Summary

Current build errors (as of last build):
- 50+ errors related to missing/changed types
- Primary issues:
  1. `DataAttribute` not found (DirectoryAttribute, FileInlineDataAttribute, StressAttribute)
  2. `Xunit.Abstractions` namespace not found
  3. `XunitTestRunner` API changes
  4. Test collection/method/class API changes

## Estimated Effort

- Simple namespace fixes: 30 minutes
- Data attribute migration: 2-3 hours
- Custom test runner migration (if keeping): 4-6 hours
- Custom test runner removal (if simplifying): 1-2 hours
- Testing and validation: 2-3 hours

**Total**: 6-14 hours depending on approach

## Next Steps

1. Decide: Keep or simplify custom extensibility?
2. If keeping: Study xUnit3 extensibility docs and examples
3. If simplifying: Convert to standard xUnit3 patterns
4. Implement migrations file-by-file
5. Test incrementally
6. Update documentation

## Current Blockers

None - all information available to proceed. Requires dedicated development time to implement the API changes documented above.
