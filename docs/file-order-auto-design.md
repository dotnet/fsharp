# `--file-order-auto+` — Design & implementation

This document is the engineering companion to
[`file-order-auto-migration.md`](./file-order-auto-migration.md) (user-facing
guide) and [`file-order-auto-release-notes.md`](./file-order-auto-release-notes.md)
(what's in the fork, status). It explains *how* the flag works inside the
compiler, and the chain of analyser refinements that took it from
"works on a toy fixture" to "matches baseline on real-world F# projects".

The flag is off by default. With it off, this fork is byte-identical to
upstream — same diagnostics, same type inference, same FSharp.Core
compilation. With it on, the compiler computes a dependency order for the
project's `.fs`/`.fsi` files and reorders the inputs before type checking.

## The shape of the change

The pipeline insertion point sits between parse and check:

```
parsedInputs ──▶ [ enter phase: stub TcEnv ] ──▶
              ──▶ [ symbol collection: extract decls + refs ] ──▶
              ──▶ [ dep graph + Tarjan SCC ] ──▶
              ──▶ [ apply file order, synthesise cycle groups ] ──▶
              ──▶ check
```

Three sub-systems do the work:

1. **Enter phase** (`SymbolCollection.runEnterPhase`) — pre-populates `TcEnv`
   with stub `Entity` shells for every file's top-level modules and types,
   so namespace references resolve regardless of file order.
2. **Symbol collection** (`SymbolCollection.collectFileDeclarations`) — walks
   each parsed AST and produces a `FileDeclarations` record: top-level
   modules, opens, and identifier references.
3. **File ordering** (`SymbolCollection.computeCompilationUnits` →
   `applyAutoFileOrder` in `fsc.fs` / `computeReorderedFileNames` in
   `IncrementalBuild.fs`) — runs Tarjan's SCC over the dependency graph;
   single-file SCCs get topologically sorted, multi-file SCCs become cycle
   groups.

The build path additionally **synthesises** cycle groups
(`CycleGroupProcessing.fs`) into a single recursive namespace, so files
that mutually reference each other still type-check. FCS keeps cycle
groups in original order and lets the existing type checker report the
cycle as a normal error (Phase 2 limitation, see release notes).

## The enter phase — why it exists

Type checking in F# is sequential: file *N* sees only the entities checked
in files *0..N-1*. To make file order irrelevant for *type-resolution*
purposes (without rewriting type checking), the enter phase pre-populates
`TcEnv` with shells for every file before sequential checking begins.
This is conceptually similar to Dotty's "Enter" phase.

### What we stub, and what we don't

**Type stubs** — yes. F# tolerates forward references to types as long as
the entity is present in the environment with a name and arity. The stub
includes type parameters synthesised from `TypeParamCount` so generic
references like `MyType<'A>` resolve from another file:

```fsharp
let typars : Typars =
    [ for i in 0 .. stub.TypeParamCount - 1 ->
        let nm = sprintf "T%d" i
        Construct.NewRigidTypar nm stub.Name.idRange ]
```

**Module stubs** — no. F# rejects re-declaration of an existing module
entity (`FS0245 'X' is not a concrete module or namespace`). We tried
stubbing modules; FsCheck failed with FS0245 wherever a real `module Foo`
followed a stub. We removed module stubs entirely; type stubs alone are
enough for cross-file type resolution.

**FSharp.Core** — skipped. The pre-population stubs would shadow primitive
types. The flag is silently ignored when compiling FSharp.Core itself.

## Symbol collection — the analyser

The analyser produces, per file:
- the file's top-level modules / namespaces with all nested types and values
- every `open` declaration
- every qualified identifier reference

These three things drive the dependency graph. Files **declare** modules
and types; files **reference** identifiers. A reference whose qualified
prefix matches another file's declaration is a dependency edge.

This sounds simple. It wasn't — getting it right on real OSS code took the
chain below.

### Custom AST walker (replaces FCM)

`FileContentMapping.PrefixedIdentifier` (the existing `--graphBased`
infrastructure) drops the last segment of every qualified identifier — it
maps `FsCheck.FSharp.Prop.forAll` to `FsCheck.FSharp` for the purposes of
graph slicing. That truncation is wrong for our use case: we need the full
path so `Prop.forAll` can resolve to the file that defines `module Prop`.

`SymbolCollection.collectFullPathRefs` is a hand-rolled walker that
preserves full identifier paths through every `SynExpr`, `SynPat`,
`SynType`, `SynMemberDefn`, and `SynModuleDecl` shape.

### Top-level value and type-member registration

A `let x = ...` at module scope is registered as `qualName.x` (Value).
A type's static members, instance members, abstract slots, and
auto-properties are registered as `qualName.TypeName.MemberName`
(Member), populated via `tryGetMemberName` →
`collectMemberNamesFromDefns` / `collectMemberNamesFromSigs`.

This is what lets `Result.map` from one file resolve to a `module Result`
defining `let map = ...` in another file, or `Foo.bar` to resolve to a
`type Foo` with `static member bar = ...`.

### Kind-aware matching

`ExportKind = Module | Type | Value | Member` distinguishes how a name was
registered. The prefix-iteration matcher accepts `Module | Value | Member`
hits, but **rejects bare-`Type` matches** unless a `Member` is also
registered for that path.

Why: without this, FsCheck's project type `Random.Type` would be reached
by every reference to `Random.X` regardless of what `X` is. The `Type`
without a `Member` means "the type exists but you didn't reference any
of its members", which is not a real dependency.

### Surgical single-ident capture

A bare `SynExpr.Ident foo` could be a function reference or a local
parameter. Capturing every `SynExpr.Ident` broke FsToolkit (locals like
`let result = ResultBuilder()` matched the `Result` module). Capturing
none broke Suave (`transferStream conn` from inside `open Suave.Sockets`
didn't resolve to `Sockets.transferStream`).

The compromise: capture single idents **only at function-application
heads**:

```fsharp
| SynExpr.App(funcExpr = e1; argExpr = e2) ->
    (match e1 with
     | SynExpr.Ident ident -> addIds [ ident ]
     | _ -> ())
    walkExpr e1; walkExpr e2
```

A bare `foo` outside an application doesn't get captured (avoids the
local-binding false positive); `foo arg` does (recovers the AutoOpen
function-reference case).

## Resolution — exportMap + aliasMap

The export map is `qualified-prefix → set of file indices that declare it`.
A file's identifier refs are matched against this map (longest-prefix
first) and each match is a dependency on the declaring file.

### Separate `aliasMap` for AutoOpen

`[<AutoOpen>]` makes a module's content reachable via the *parent*
without an explicit `open`. So a reference to `Foo.bar` inside namespace
`X.Y` could resolve to `X.Y.AutoOpened.Foo.bar` (where `AutoOpened` is
attribute-tagged AutoOpen).

Naïvely registering AutoOpened content under the parent's prefix in the
main `exportMap` regressed Suave (30 → 200 errors) and Expecto (0 → 6).
The reason: AutoOpen aliases share prefixes, so the cycle detector saw
phantom mutual dependencies between unrelated files.

The fix: a **separate `aliasMap`** consulted only as a *fallback* in
`addDepFromExportMap`. AutoOpened content is registered with its
"reachable-via-parent" path in `aliasMap`, never mixed into the main
`exportMap`. Cycle detection runs over `exportMap` alone, so aliases
can't introduce false sharedPrefix/cycle matches.

```fsharp
let topAlias =
    if topMod.IsAutoOpen
       && topMod.Kind = SynModuleOrNamespaceKind.NamedModule
       && segments.Length > 1 then
        Some (segments |> List.take (segments.Length - 1) |> String.concat ".")
    else None
```

### Opens-as-prefixes with local-name shadowing

`open Foo.Bar` makes everything declared in `Foo.Bar` reachable as a
1-segment ident. We model this by treating each `open` as an additional
resolution prefix when matching the file's identifier refs.

But: a local declaration **shadows** an open. If a file has
`open FsCheck.FSharp` AND a local `module Prop`, then `Prop.X` refers to
the local `Prop`, not `FsCheck.FSharp.Prop`. The matcher checks the
file's own declarations before walking opens.

### Opens skip shared prefixes

`open FsCheck` inside a file already in `namespace FsCheck` is
redundant — every file in `namespace FsCheck` has implicit access to its
siblings. If we treated this as an open, we'd broadcast a phantom
dependency from the file to *every* contributor to `namespace FsCheck`.
The matcher detects this and skips.

### NamedModule vs DeclaredNamespace prefix scoping

`module X.Y = ...` (NamedModule) implicitly sees siblings of parent `X`.
`namespace X.Y` (DeclaredNamespace) does not. The matcher honours this
distinction when resolving enclosing-prefix references.

## Cycle groups (Level B, build-only)

Files in an SCC of size > 1 form a cycle group. The build path synthesises
the group into one `ParsedImplFileInput` whose `SynModuleOrNamespace`
entries are marked `isRecursive = true` — effectively a `namespace rec`
that wraps the original modules.

### Cross-namespace synthesis guard

Synthesis only works when the cycle group is *within a single namespace
prefix*. If the group spans `namespace X.A` and `namespace X.B`,
wrapping them in a single `namespace rec X` would put `module B` inside
`namespace rec X` — which conflicts with the original `namespace X.B`
declaration (`FS0247 namespace and module 'X.B' both defined`).

When the cycle group is cross-namespace, we **fall back to original file
order** for that group. The user-visible behaviour matches what they'd
get without the flag.

### Recursive open-hoisting

Inside a synthesised `namespace rec X`, `open` declarations must come
*first* in each module/namespace block — otherwise `FS3200 'open'
declarations may only be the first declaration in a module`. The
synthesiser walks each block and reorders all `open` decls to the top,
recursively into nested modules.

### Sig+impl pairing

`.fsi` files are paired with their matching `.fs`. The pair is treated
as one logical contributor in `buildExportMap` so both halves participate
in the dependency graph.

For ordering: a consumer might depend on the *signature* file (e.g. it
references `Foo.bar` and `Foo.fsi` declares it). If the impl gets a
later topological position, the consumer ends up before the pair.
Fixed via **sig→impl redirect**: dependency edges that point at a sig
are rewritten to point at its impl before topological sort, so the
pair is positioned correctly.

Cycle groups containing `.fsi` files fall back to original order
(synthesis would need to merge sig/impl pairs into the recursive block,
which we haven't implemented).

## Edge cases hardened

- **Empty-longId guards**: every `match ids with | [id] -> id | _ -> List.last ids`
  pattern now handles `[]` to avoid `FS0193 internal error: input list was empty`.
  Triggered by Expecto.
- **`IsLastCompiland` fixup**: the `[<EntryPoint>]` constraint requires
  the last file to be the entry point. After reordering, we update the
  flag on whichever file is now last.
- **FSharp.Core skip**: detected via project file path; flag silently no-ops.

## Pipeline integration points

| Layer | File | Hook |
|---|---|---|
| `fsc` driver | `src/Compiler/Driver/fsc.fs` | `applyAutoFileOrder` called after parse, before check |
| FCS | `src/Compiler/Service/IncrementalBuild.fs` | `computeReorderedFileNames` runs the same logic for `IncrementalBuilder` |
| MSBuild | `src/FSharp.Build/Microsoft.FSharp.NetSdk.props` + `Targets` + `Fsc.fs` | `FSharpAutoFileOrder=true` → `--file-order-auto+` |
| Compiler options | `src/Compiler/Driver/CompilerOptions.fs` | flag parsing, default-on/off, help text |
| Localized strings | `src/Compiler/FSComp.txt` + 13× `xlf` | `optsFileOrderAuto`, FS3887 message |

## The chain of refinements (TL;DR)

The history of getting from skeleton-implementation to OSS-buildable, in
the order the unblocks happened:

1. Custom AST walker (full-path identifier refs, replaces FCM truncation).
2. Type-member registration (`qualName.TypeName.MemberName`).
3. Kind-aware matching (`ExportKind`; reject bare-Type matches).
4. Opens skip shared prefixes (no phantom self-broadcasts).
5. Opens-as-prefixes for ident resolution + local-name shadowing.
6. NamedModule vs DeclaredNamespace prefix scoping.
7. Type stubs include type parameters (`Typars` from `TypeParamCount`).
8. No module stubs, only type stubs (avoids FS0245).
9. Cross-namespace cycle synthesis guard (avoids FS0247).
10. Recursive open-hoisting in synthesised cycle groups (FS3200 fix).
11. Empty-longId guards (FS0193 internal error).
12. Separate `aliasMap` for AutoOpen (the unblock that landed Suave).
13. Sig→impl redirect (preserves consumer ordering across `.fsi` pairs).
14. Surgical single-ident capture (Suave AutoOpen aliases without
    breaking FsToolkit's local bindings).

Each refinement was driven by a specific OSS project's failure mode. See
`tests/file-order-auto-test/oss-sweep/RESULTS.md` for the per-project
notes and the commit hashes that introduced each refinement.

## What this isn't

- **Not a parallel/graph-based compilation feature**. The graph drives
  *ordering*, not parallelism. Type checking remains sequential.
- **Not a rewrite of type inference**. Files still see only what was
  checked before them; the enter phase only adds *names with arities*
  to the environment, not types.
- **Not a replacement for `module rec` / `namespace rec`** at file
  granularity. Within a single file, `rec` semantics are unchanged.
- **Not a graph-based file partitioning** (the `--graphBased` flag in
  upstream). Different feature with different goals.
