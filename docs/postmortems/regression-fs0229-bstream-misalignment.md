# Regression: FS0229 B-Stream Misalignment in TypedTreePickle

## Summary

A metadata unpickling regression causes `FS0229` errors when the F# compiler (post-nullness-checking merge) reads metadata from assemblies compiled with `LangVersion < 9.0`. The root cause is a stream alignment bug in `TypedTreePickle.fs` where the secondary metadata stream ("B-stream") gets out of sync between writer and reader.

## Error Manifestation

```
error FS0229: Error reading/writing metadata for assembly '<AssemblyName>': 
  The data read from the stream is inconsistent, reading past end of resource, 
  u_ty - 4/B  OR  u_ty - 1/B, byte = <N>
```

This error occurs when consuming metadata from an assembly where:
1. The assembly was compiled by the current compiler (which writes B-stream data)
2. The compilation used `LangVersion 8.0` or earlier (which disables `langFeatureNullness`)
3. The assembly references BCL types whose type parameters carry `NotSupportsNull` or `AllowsRefStruct` constraints

**Affected real-world library**: [FsToolkit.ErrorHandling](https://github.com/demystifyfp/FsToolkit.ErrorHandling), which uses `<LangVersion>8.0</LangVersion>` for `netstandard2.0`/`netstandard2.1` TFMs.

## Root Cause

### Dual-Stream Metadata Format

F# compiler metadata uses two serialization streams:
- **Stream A** (main): Type tags, type constructor references, type parameter references, etc.
- **Stream B** (secondary): Nullness information per type + newer constraint data (e.g., `NotSupportsNull`, `AllowsRefStruct`)

These streams are written in parallel during pickling and read in parallel during unpickling. The invariant is: **every byte written to stream B by the writer must have a corresponding read in the reader**.

### The Bug

In `p_ty2` (the type pickle function), nullness information is written to stream B **conditionally**:

```fsharp
// BEFORE FIX (buggy)
| TType_app(tc, tinst, nullness) ->
    if st.oglobals.langFeatureNullness then
        match nullness.Evaluate() with
        | NullnessInfo.WithNull -> p_byteB 12 st
        | NullnessInfo.WithoutNull -> p_byteB 13 st
        | NullnessInfo.AmbivalentToNull -> p_byteB 14 st
    // No else branch - B-stream byte skipped when langFeatureNullness = false!
    p_byte 2 st
    p_tcref "typ" tc st
    p_tys tinst st
```

But in `u_ty` (the type unpickle function), the B-stream byte is read **unconditionally**:

```fsharp
| 2 ->
    let tagB = u_byteB st   // Always reads, regardless of langFeatureNullness at compile time
    let tcref = u_tcref st
    let tinst = u_tys st
    match tagB with
    | 0 -> TType_app(tcref, tinst, KnownAmbivalentToNull)
    | 12 -> TType_app(tcref, tinst, KnownWithNull)
    ...
```

This affects type tags 1 (TType_app no args), 2 (TType_app), 3 (TType_fun), and 4 (TType_var).

Meanwhile, `p_tyar_constraints` **unconditionally** writes constraint data to B-stream:

```fsharp
let p_tyar_constraints cxs st =
    let cxs1, cxs2 = cxs |> List.partition (function
        | TyparConstraint.NotSupportsNull _ | TyparConstraint.AllowsRefStruct _ -> false
        | _ -> true)
    p_list p_tyar_constraint cxs1 st
    p_listB p_tyar_constraintB cxs2 st  // Always writes to B, regardless of langFeatureNullness
```

### Misalignment Cascade

When `langFeatureNullness = false`:

1. Writer processes types → skips B-bytes for each type tag 1-4
2. Writer processes type parameter constraints → writes `NotSupportsNull` data to B-stream (value `0x01`)
3. Reader processes types → reads B-stream expecting nullness tags → gets constraint data instead
4. Constraint byte `0x01` is not a valid nullness tag (valid values: 0, 9-20) → `ufailwith "u_ty - 4/B"` or similar

The misalignment cascades: once one byte is read from the wrong position, all subsequent B-stream reads are shifted.

## Fix

Added `else p_byteB 0 st` to all four type cases in `p_ty2`, ensuring a B-byte is always written regardless of `langFeatureNullness`:

```fsharp
// AFTER FIX
| TType_app(tc, tinst, nullness) ->
    if st.oglobals.langFeatureNullness then
        match nullness.Evaluate() with
        | NullnessInfo.WithNull -> p_byteB 12 st
        | NullnessInfo.WithoutNull -> p_byteB 13 st
        | NullnessInfo.AmbivalentToNull -> p_byteB 14 st
    else
        p_byteB 0 st  // Keep B-stream aligned
    p_byte 2 st
    p_tcref "typ" tc st
    p_tys tinst st
```

Value `0` means "no nullness info / AmbivalentToNull" and is already handled by all reader match cases.

## Timeline

| Date | PR | Change |
|------|-----|--------|
| Jul 2024 | [#15181](https://github.com/dotnet/fsharp/pull/15181) | Nullness checking: introduced B-stream for nullness bytes, conditional write in `p_ty2` |
| Aug 2024 | [#15310](https://github.com/dotnet/fsharp/pull/15310) | Nullness checking applied to codebase |
| Sep 2024 | [#17706](https://github.com/dotnet/fsharp/pull/17706) | `AllowsRefStruct`: added constraint data to B-stream unconditionally via `p_listB` |

The bug was latent from #15181 but only manifested when #17706 added unconditional B-stream writes for constraints. Before #17706, the B-stream was empty when `langFeatureNullness = false`, so the reader's unconditional reads would hit the end-of-stream sentinel (returning 0) harmlessly. After #17706, constraint data appeared in the B-stream even without nullness, causing the misalignment.

## Regression Tests

Two tests added in `tests/FSharp.Compiler.ComponentTests/Import/ImportTests.fs`:

1. **Basic test**: Compiles a library with `LangVersion=8.0` containing generic types with BCL constraints (e.g., `IEquatable<'T>`), then references it from another compilation and verifies no FS0229 error.

2. **Stress test**: Multiple type parameters with various constraint patterns, function types, and nested generics — all compiled at `LangVersion=8.0` and successfully consumed.

## Reproduction

To reproduce the original bug (before fix):

1. Clone [FsToolkit.ErrorHandling](https://github.com/demystifyfp/FsToolkit.ErrorHandling)
2. Inject the pre-fix compiler via `UseLocalCompiler.Directory.Build.props`
3. Build `netstandard2.0` TFM (uses `LangVersion=8.0`)
4. Build `net9.0` TFM that references the `netstandard2.0` output
5. The `net9.0` build fails with `FS0229: u_ty - 4/B`

## Files Changed

- `src/Compiler/TypedTree/TypedTreePickle.fs` — Added `else p_byteB 0 st` to four locations in `p_ty2`
- `tests/FSharp.Compiler.ComponentTests/Import/ImportTests.fs` — Two regression tests
- `tests/FSharp.Compiler.ComponentTests/FSharp.Compiler.ComponentTests.fsproj` — Added `ImportTests.fs` include (was missing since migration)
