# Open-Source F# Project Sweep

Single-pass test of how `--file-order-auto+` behaves against real-world F# projects, run against this fork's fsc.

## How to reproduce

```bash
# 1. Clone targets
mkdir -p /tmp/fsharp-oss-sweep && cd /tmp/fsharp-oss-sweep
git clone --depth 1 https://github.com/fsprojects/Argu
git clone --depth 1 https://github.com/fsprojects/FSharpPlus
git clone --depth 1 https://github.com/fscheck/FsCheck

# 2. Build with the fork's fsc + --file-order-auto+
FSC=$(pwd)/../fsharp/artifacts/bin/fsc/Release/net10.0/fsc.dll  # adjust path
dotnet build <project>.fsproj -c Release \
    -p:DotnetFscCompilerPath="$FSC" \
    -p:OtherFlags="--file-order-auto+ --nowarn:3885"
```

`--nowarn:3885` is needed to bypass the `and`-keyword deprecation warning if
the project's `<TreatWarningsAsErrors>` is on; the warning itself is the
intended migration nudge, not a bug.

## Results

| Project | Baseline (no flag) | --file-order-auto+ | Notes |
|---|---|---|---|
| Argu | OK | **OK** | ~30 .fs files, 5s clean build. Real-world success on a non-trivial library. |
| FsCheck | OK | FAIL | 36 type errors. See "Known limitation: same-namespace single-ident type qualifiers" below. |
| FSharpPlus | OK | FAIL | 166 errors. Heavy SRTP / overload-resolution patterns; same root cause as FsCheck plus extension-method ordering. |
| Saturn | (not tested) | n/a | Uses Paket, which isn't installed on this dev box; environmental skip. |

## Known limitation: same-namespace single-ident type qualifiers

The dependency analyser reads `FileContentMapping.PrefixedIdentifier`
entries from the F# AST. That structure intentionally drops the **last**
segment of any long identifier:

- `MathHelpers.pi` → captured as `["MathHelpers"]`
- `Random.CreateWithSeedAndGamma` → captured as `["Random"]`
- `Result.isOk` → captured as `["Result"]`

Inside files that share a namespace with each other (every `namespace X`
file in FsCheck, every `namespace FSharpPlus.Internals` file, etc.), this
truncation makes two genuinely different references look identical to our
analyser:

- `Random.X` where `Random` is a project type with static methods → real
  cross-file dep on the file declaring `type Random`.
- `Result.isOk` where `Result` is `FSharp.Core.Result` (auto-opened) → no
  cross-file dep, but our analyser sees the same single-segment `Result`
  and matches it against any `type Result` defined in the same project
  namespace.

The collision creates *false dependency edges* that turn DAG-shaped
projects into one giant SCC, after which Level B cycle-group synthesis
wraps everything in a `namespace rec` and FS3200 fires
("In a recursive declaration group, 'open' declarations must come first").

The right fix is to capture full identifier paths from the AST (not just
the truncated qualifier), so `Random.CreateWithSeedAndGamma` and
`Result.isOk` are distinguishable. That's a structural change to either
`FileContentMapping` upstream or a parallel walker in this fork. Out of
scope for this iteration.

## Partial mitigations already applied

Two analyser fixes landed during the sweep that materially improved the
state on real code:

1. **Sig/impl pair collapse in export map** (commit `8fed06d62`,
   `SymbolCollection.fs:556`): a `.fsi`/`.fs` pair declaring the same
   module no longer inflates the contributor count, so consumers detect
   their dependency edge correctly.
2. **NamedModule vs DeclaredNamespace prefix scoping**
   (`SymbolCollection.fs:686`): for files declared as `namespace X.Y`
   (DeclaredNamespace), only the file's own namespace is considered when
   prefix-resolving relative refs. Parent namespaces are NOT auto-imported
   in F# semantics, so trying them produces false edges. NamedModule
   (`module X.Y`) keeps the parent prefix because its contents *are*
   implicitly visible to siblings of the parent.

## Recommendation

For a v1 PR, ship the current state with this document as the explicit
limitation. Argu builds cleanly; that demonstrates the feature works on
real-world code that doesn't trip the truncated-path collision. FsCheck
and FSharpPlus require the structural full-path capture work before they
can be reliably auto-ordered.
