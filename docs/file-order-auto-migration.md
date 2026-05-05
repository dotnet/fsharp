# Migrating to `--file-order-auto+`

This fork of the F# compiler relaxes the file-ordering rule. With
`--file-order-auto+`, the compiler computes a dependency order for your
project's source files instead of requiring you to list them in topological
order in the `.fsproj`.

## Enabling

### MSBuild (`dotnet build`, `dotnet run`, IDEs)

```xml
<PropertyGroup>
  <FSharpAutoFileOrder>true</FSharpAutoFileOrder>
</PropertyGroup>
```

### Direct `fsc`

Pass `--file-order-auto+` on the command line.

### F# Compiler Service (Ionide, custom tooling)

Add `"--file-order-auto+"` to `FSharpProjectOptions.OtherOptions`. The
`IncrementalBuilder` will pre-parse the source list and reorder it before
type checking. There is no separate API.

The flag is off by default. The standard manual ordering remains the default
behaviour and is unchanged.

## What changes

- The compiler computes a dependency order for `.fs` and `.fsi` files based
  on which top-level modules each file declares and which qualified
  identifiers it references. You can list files in any order in the `.fsproj`
  and the compiler will sort them.
- `.fsi` and its paired `.fs` are kept adjacent (sig immediately before impl).
- Auto-generated files (`AssemblyInfo`, `obj/`, etc.) are placed first, as
  they have always been.
- The `[<EntryPoint>]` constraint that the entry-point file must be last is
  preserved — the auto-order pass updates the `IsLastCompiland` flag on the
  reordered last file.

## What does NOT change

- Type inference. Files still see only what was checked before them.
- Module visibility, accessibility, signatures.
- The semantics of `module rec` or `namespace rec` within a single file.
- The behaviour of FSharp.Core compilation. The flag is silently ignored when
  compiling FSharp.Core itself, because the synthesised stubs would shadow
  primitive types.

## Cycles

A *cycle group* is a set of files that mutually reference each other (file A
declares something that B references, B declares something that A references).
With `--file-order-auto+`, cycle groups are detected via Tarjan's SCC.

- **Build path (`fsc`)**: cycle groups are *synthesised* into a single
  recursive namespace, mimicking `namespace rec`. The original files'
  diagnostics are preserved; no namespace-flattening hack is performed.
- **FCS / IDE**: cycle groups are kept in their original `.fsproj` order
  and the compiler will report the cycle as a normal type error. This is the
  documented Phase 2 limitation — IDE support for cycle groups is build-only
  for now. See `conductor/tracks/05_tooling_integration/design.md`.
- **Cycle groups containing `.fsi` files**: fall back to original order.
  Sig/impl pairing inside a synthesised cycle group is a known gap.

If you are migrating an existing project that relies on the `and` keyword to
make types mutually recursive, see [Migrating off `and`](#migrating-off-and).

## Migrating off `and`

When `--file-order-auto+` is set, every `and`-joined type declaration emits
warning **FS3887**:

> The 'and' keyword for mutually recursive types is unnecessary when using
> `--file-order-auto`. Consider placing types in separate declarations. This
> keyword may be removed in a future version.

The warning is emitted on the *tail* declarations of an `and`-chain, not the
head. So `type X = ... and Y = ... and Z = ...` produces two warnings (for `Y`
and `Z`).

To migrate, replace each `and`-joined chain with separate `type` declarations.
The auto-order pass will recognise the cross-references and place them in a
cycle group automatically.

The warning is suppressable like any other:

```xml
<NoWarn>3887</NoWarn>
```

or

```bash
fsc --file-order-auto+ --nowarn:3887 ...
```

## How it works (briefly)

1. Each parsed file is walked to extract its top-level modules and the
   qualified identifiers it references (the "enter phase", inspired by Dotty).
2. An export map is built: module name → contributing file index. `.fsi`/`.fs`
   pairs are collapsed to one logical contributor.
3. Per-file dependencies are computed by matching each file's identifier
   references against the export map.
4. Tarjan's SCC produces compilation units: a unit is either a single file
   (DAG node) or a cycle group (mutually recursive files).
5. Single files get a topological order with deterministic tie-breaking by
   original `.fsproj` index.
6. Cycle groups (build path only) are synthesised as a recursive namespace
   wrapping the original modules.
7. `IsLastCompiland` is fixed up on the reordered last file so
   `[<EntryPoint>]` validation still works.

## Known limitations

- **FCS does not synthesise cycle groups.** IDE diagnostics for cycle-heavy
  projects (Fantomas, the F# compiler itself) will differ from the build:
  the IDE shows the cycle as a type error, the build resolves it. Track this
  as expected behaviour, not a bug.
- **No incremental graph invalidation in FCS.** Each `IncrementalBuilder`
  construction re-runs the auto-order pass. For large projects, expect a
  one-time pre-parse cost on project load. Subsequent edits use the existing
  IncrementalBuilder caches.
- **`dotnet fsi` is not wired.** The flag is parsed by `fsc` and FCS but not
  by F# Interactive. Multi-file `fsi` invocations will not auto-order.
- **Cycle groups with `.fsi` files** fall back to original order rather than
  being synthesised.
- **FSharp.Core itself** cannot be compiled with the flag (intentional — the
  pre-population stubs would shadow primitive types).

## FAQ

**Do I have to enable this?**
No. The default is unchanged. Manual ordering still works exactly as it does
upstream.

**Will my project break if I turn it on?**
If your project compiles cleanly upstream, it should compile with the flag
on. If you hit a regression, that is a bug — please file it.

**What if I have cycles?**
Build will succeed (cycle group synthesis). IDE will show a type error until
you split the cycle or until cycle group synthesis is added to FCS.

**Can I gradually adopt this?**
Yes. The flag is per-project. You can enable it on one library and leave the
rest of the solution on manual ordering.

**Does this affect compile time?**
A small one-time cost per project load (the dependency-order pass parses every
file once). Subsequent rebuilds use the same caching as the manual path.
