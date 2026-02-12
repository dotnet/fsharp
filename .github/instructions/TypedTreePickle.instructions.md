---
applyTo:
  - "src/Compiler/TypedTree/TypedTreePickle.{fs,fsi}"
  - "src/Compiler/Driver/CompilerImports.{fs,fsi}"
---

# Pickling: Binary Compatibility Rules

F# embeds metadata into compiled DLLs as binary resources. When project B references project A's DLL, the compiler deserializes that metadata to reconstruct type information. **The compiler that wrote the metadata and the compiler that reads it may be different versions.** This is the central constraint on every change you make here.

## The Compatibility Contract

A library compiled with F# SDK version X will be consumed by projects using SDK version Y. Both directions must work:

- **Forward compatibility**: A newer compiler must read metadata written by an older compiler.
- **Backward compatibility**: An older compiler must read metadata written by a newer compiler.

This means:

1. **Never remove, reorder, or reinterpret** existing serialized data. DLLs compiled with older formats exist in the wild permanently.
2. **Additions must be invisible to old readers.** New data goes in stream B, where readers that don't know about it get `0` (the default sentinel) past end-of-stream. New readers detect presence via a tag byte they write unconditionally.
3. **Tag values are forever.** Once a byte value means something in a reader's `match`, that meaning cannot change. Old DLLs encode that value with the old semantics.

## Reading and Writing Must Be Perfectly Aligned

The format uses two parallel byte streams. Every `p_*` (write) function has a corresponding `u_*` (read) function. They must produce and consume the **exact same byte sequence** under **every possible code path** — including paths gated by feature flags, language versions, or target frameworks that your current build may not exercise.

The dangerous scenario: a write is conditional on some flag, but the corresponding read is unconditional (or vice versa). One skipped byte shifts every subsequent read in that stream, producing `FS0229` errors that manifest far from the actual bug — often only when a specific combination of compiler versions and project configurations is used.

Branching on the **shape of the data being serialized** (e.g., which `TType` case, how many type parameters) is normal and correct — the reader sees the same data and branches the same way. The byte count varies with the data, and that's fine.

What is **not safe** is varying the byte count based on **compiler configuration** — feature flags, `LangVersion`, target framework, or any other setting that lives in the compiler process but is not encoded in the stream. The reader has no access to the writer's settings; it only sees bytes. If a flag causes the writer to skip a byte, the reader will consume whatever byte happens to be next, and every subsequent read shifts.

When reviewing a change, ask: "Does the number of bytes written depend on anything the reader cannot reconstruct from the stream itself?" If yes, the change will break cross-version compatibility.

## Reasoning About Evolution

Before making a change, think through these scenarios:

1. **You write new data. An old compiler reads the DLL.** The old reader doesn't know about your new bytes. Will it silently get `0` defaults and behave correctly, or will it crash or misinterpret data?

2. **An old compiler wrote the DLL. Your new reader processes it.** Your new reader expects data that isn't there. Does it handle the missing-data case (tag value `0`, end-of-stream) gracefully?

3. **Feature flags and language versions.** A single compiler binary may write different data depending on `LangVersion` or feature toggles. The reader processing that DLL has no access to the writer's flags — it only sees bytes. Every flag-dependent write path must still produce a byte-aligned stream that any reader can consume.

4. **Multi-TFM projects.** A solution may compile `netstandard2.0` (with `LangVersion=8.0`) and `net9.0` (with `LangVersion=preview`). The `net9.0` build references the `netstandard2.0` output. Both DLLs were produced by the same compiler binary but with different settings. This is where conditional writes break.

## Testing With the CompilerCompat Suite

If your change alters what gets serialized, **add coverage for it** in the CompilerCompat projects. Add the new type, constraint, or API shape to `CompilerCompatLib/Library.fs` and a corresponding consumer in `CompilerCompatApp/Program.fs`. This ensures your specific change is exercised across compiler versions, not just the pre-existing test surface.

Then run the cross-version compatibility tests:

```bash
dotnet fsi tests/FSharp.Compiler.ComponentTests/CompilerCompatibilityTests.fsx
```

This suite (`tests/projects/CompilerCompat/`) compiles a library with one compiler version, packs it as a NuGet package, then builds a consuming application with a different compiler version. The application references the library as a package — not a project reference — so the consuming compiler must deserialize the library's pickled metadata from the DLL, exercising the real import path.

It tests both directions:

| Scenario | Question answered |
|---|---|
| Library: released SDK → App: your local build | Can your new reader handle the old format? |
| Library: your local build → App: released SDK | Can old readers handle your new format? |
| Same, across .NET major versions (e.g., 9 ↔ current) | Does it hold across SDK generations? |

Also run the import regression tests:
```bash
dotnet test tests/FSharp.Compiler.ComponentTests -c Release --filter "FullyQualifiedName~Import" /p:BUILDING_USING_DOTNET=true
```

For a detailed example of what goes wrong when these rules are violated, see `docs/postmortems/regression-fs0229-bstream-misalignment.md`.
