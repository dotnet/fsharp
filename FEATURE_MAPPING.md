# FSharpQA to ComponentTests Feature Mapping

Living document mapping fsharpqa patterns to test framework equivalents.  Update as new patterns are solved.

## Compiler Invocation

| FsharpQA | ComponentTests | Notes |
|----------|----------------|-------|
| `SOURCE=file.fs` | `[<FileInlineData("file.fs")>]` + `getCompilation` | File loaded from resources/ |
| `SOURCE=file.fs` (inline) | `FSharp """source"""` | For small tests |
| `SOURCE="a. fsi a.fs"` | `withAdditionalSourceFile (SourceFromPath "a.fsi")` | Order matters |
| `SCFLAGS="--opt"` | `withOptions ["--opt"]` | |
| `SCFLAGS="-a"` | `asLibrary` | Shorthand available |
| `SCFLAGS="--test: ErrorRanges"` | `withErrorRanges` | Shorthand available |
| `SCFLAGS="--warnaserror+"` | `withOptions ["--warnaserror+"]` | |
| `SCFLAGS="--nowarn: XX"` | `withNoWarn XX` | |
| `SCFLAGS="--warnon:XX"` | `withWarnOn XX` | |
| `COMPILE_ONLY=1` | Use `typecheck` or `compile` (not `run`) | `typecheck` is fastest |
| `FSIMODE=EXEC` | `asFsx` | Script compilation |
| `FSIMODE=PIPE` | Needs framework addition | FSI process with stdin |

## Compilation Method Selection

| Scenario | Method | Reason |
|----------|--------|--------|
| Check errors/warnings only | `typecheck` | Fastest, no codegen |
| Need output assembly | `compile` | Required for IL checks |
| Need to run executable | `compile` then `run` | Or use `compileExeAndRun` |
| FSI script evaluation | `asFsx` + `typecheck` | |

## Expected Results

| FsharpQA | ComponentTests | Notes |
|----------|----------------|-------|
| `<Expects status="success">` | `shouldSucceed` | |
| `<Expects status="error">` | `shouldFail` | |
| `<Expects status="warning">` (no errors) | `shouldSucceed` + check diagnostics | Warnings don't fail by default with `ignoreWarnings` |
| `<Expects status="skip">reason` | `[<Fact(Skip="reason")>]` | On test attribute |
| `id="FS0001"` | `Error 1` or `Warning 1` | Drop "FS" prefix and leading zeros |
| `span="(5,1-10,15)"` | `Line 5, Col 1, Line 10, Col 15` | In withDiagnostics tuple |
| Message text | Regex matched in `withDiagnostics` | Partial match OK |
| `<Expects status="notin">` | Manual assertion (see ADDITIONS) | Not built-in yet |

## Diagnostic Assertions

```fsharp
// Single error with full location and message
|> withDiagnostics [
    (Error 1, Line 5, Col 1, Line 5, Col 10, "expected type")
]

// Just error code
|> withErrorCode 1

// Message pattern only
|> withDiagnosticMessageMatches "expected.*type"

// Multiple diagnostics
|> withDiagnostics [
    (Error 1, Line 5, Col 1, Line 5, Col 10, "first error")
    (Warning 20, Line 10, Col 1, Line 10, Col 5, "warning message")
]
```

## Multi-File Compilation

```fsharp
// Signature + implementation
FsFromPath "impl.fs"
|> withAdditionalSourceFile (SourceFromPath "sig.fsi")

// Multiple F# files
FsFromPath "file1.fs"
|> withAdditionalSourceFiles [
    SourceFromPath "file2.fs"
    SourceFromPath "file3.fs"
]
```

## C# Interop

```fsharp
// Define C# library
let csLib = 
    CSharp """
    public class Helper { 
        public static int Value = 42; 
    }
    """
    |> withName "CsLib"

// Reference from F#
FSharp """
let x = Helper.Value
"""
|> withReferences [csLib]
|> compile
|> shouldSucceed
```

## Platform Classification

| Source Content | Classification | Attribute |
|----------------|----------------|-----------|
| Pure F# code, FSharp.Core only | CrossPlatform | (none - default) |
| `System.Windows.Forms` | WindowsOnly | `[<Trait("Category", "WindowsOnly")>]` |
| `System.Runtime. Remoting` | DesktopOnly | `[<Trait("Category", "DesktopOnly")>]` |
| P/Invoke Windows DLLs | WindowsOnly | + skip check in test |
| COM interop | WindowsOnly | + skip check in test |

```fsharp
// Desktop-only test pattern
[<Fact>]
[<Trait("Category", "DesktopOnly")>]
let ``test requiring net472`` () =
    if not TestHelpers.isNetFramework then
        Assert.Skip("Requires .NET Framework")
    // ... test code
```

## Common Compiler Options

| Option | Helper |
|--------|--------|
| `--langversion:X` | `withLangVersion "X"` or `withLangVersion90`, `withLangVersionPreview` |
| `--optimize+` | `withOptimize` |
| `--optimize-` | `withNoOptimize` |
| `--debug+` | `withDebug` |
| `--debug-` | `withNoDebug` |
| `--debug: portable` | `withPortablePdb` |
| `--debug:embedded` | `withEmbeddedPdb` |
| `--define:X` | `withDefines ["X"]` |
| `--preferreduilang:X` | `withCulture "X"` |

## env.lst Prefix Tags

| Tag | Handling |
|-----|----------|
| `ReqENU` | Skip on non-English OR use regex for locale-independent matching |
| `NoMT` | Ignore (obsolete multi-targeting tag) |
| `NoHostedCompiler` | Usually CrossPlatform, test standalone compiler behavior |

## Line Number Adjustment

Source files are copied unchanged (preserving `<Expects>` comments). Line numbers in `<Expects span="...">` remain valid.  No adjustment needed.

If you DO remove `<Expects>` lines from source files, subtract that count from all line numbers in assertions. 