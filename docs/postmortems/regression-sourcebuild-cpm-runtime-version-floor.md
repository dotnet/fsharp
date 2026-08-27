# Regression: renamed CPM runtime-package pins broke VMR source-build

## Summary

To satisfy CPM transitive pinning, the central versions of the dotnet/runtime packages that MSBuild and Roslyn pull (`System.Collections.Immutable`, `System.Reflection.Metadata`, `System.Composition`, `System.Diagnostics.DiagnosticSource`, `System.Security.Cryptography.Xml`) were routed through computed `$(System*CentralVersion)` properties with a version floor, instead of the darc-named `$(System*Version)` properties. Source-build injects live runtime versions by overriding the darc-named properties, so the renamed, eagerly-floored pins froze at the floor value and the override never reached CPM. In the .NET 11 RC1 VMR the floor demanded `10.0.10` while source-build supplied the live `10.0.9`, producing prebuilt packages and `NU1109` downgrade errors that blocked the fsharp forward-flow (dotnet/dotnet#8413, #8414, #8415). Caught at the VMR source-only gate; it never shipped.

## Error Manifestation

The `VMR Source-Only Build SB_CentOSStream10_Online_MsftSdk_x64` leg failed two ways.

Prebuilt detection:

```text
eng/finish-source-only.proj(141,5): error : Prebuilt packages are not allowed in source-only builds.
Detected 1 prebuilt package(s):
 - System.Diagnostics.DiagnosticSource.10.0.10
```

And, once the floor was partially relaxed, a downgrade in `fsc`/`fsi`:

```text
error NU1109: Detected package downgrade: System.Collections.Immutable from 10.0.10 to centrally defined 10.0.9.
  fsi -> Microsoft.Build.Framework 18.11.0-1.26417.9 -> System.Collections.Immutable (>= 10.0.10)
  fsi -> System.Collections.Immutable (>= 10.0.9)
```

fsharp's own PR/CI legs were green throughout — the failure appeared only downstream in dotnet/dotnet.

## Root Cause

fsharp declares these `System.*` packages in `eng/Version.Details.xml` with an empty `<Sha>` and the note *"Necessary for source-build. This allows the live version of the package to be used by source-build."* When the VMR builds fsharp it passes `/p:DotNetPackageVersionPropsPath=…/PackageVersions.fsharp.props`, a generated file that **unconditionally overrides the darc-named properties** — `$(SystemCollectionsImmutableVersion)`, etc. — with the live runtime version it just source-built. Whatever CPM reads must be that exact property for the override to take effect.

The CPM-transitive-pinning work introduced an indirection instead:

```xml
<!-- eng/Versions.props (removed) -->
<SystemCollectionsImmutableCentralVersion>$(SystemCollectionsImmutableVersion)</SystemCollectionsImmutableCentralVersion>
<SystemCollectionsImmutableCentralVersion
    Condition="...VersionLessThan($(SystemCollectionsImmutableVersion), $(SystemRuntimeCentralFloorVersion))">
  $(SystemRuntimeCentralFloorVersion)   <!-- floor, e.g. 10.0.10 -->
</SystemCollectionsImmutableCentralVersion>
```

`eng/Packages.props` then pinned `Version="$(SystemCollectionsImmutableCentralVersion)"`. Two mistakes combined:

1. **Rename.** Source-build overrides `$(SystemCollectionsImmutableVersion)`, not `$(...CentralVersion)`. The alias is a different property that source-build has no knowledge of.
2. **Eager floor.** `$(...CentralVersion)` is computed once, before arcade imports `PackageVersions.fsharp.props`, and is `max(flowed, floor)`. Even in source-build it evaluated to the floor.

So the central pin froze at the floor (`10.0.10`). Source-build only produces the live runtime (`10.0.9`), which cannot satisfy a `10.0.10` pin from within the source tree, so restore reached an external feed → prebuilt; and where the graph also carried a `10.0.9` constraint, `NU1109` downgrade.

The violated assumption: **a source-build "live version" package must be consumed through the exact darc-named `$(System*Version)` property; any renamed or computed intermediate silently defeats the override.** roslyn's `eng/Packages.props` does the correct thing — it references `$(SystemCollectionsImmutableVersion)` directly.

## Why It Escaped

fsharp's PR/CI restores are allowed to pull from nuget.org, where `10.0.10` exists. The floored pin resolved cleanly there, so every fsharp leg — including its own `Source-Build (Managed)` legs — was green. The only environment that rejects an out-of-tree package is the VMR source-**only** prebuilt gate (`SB_CentOSStream10_Online_MsftSdk_x64`), which fsharp CI does not run. The defect was therefore structurally invisible until the change forward-flowed into dotnet/dotnet, roughly a day later.

It also recurred: the same source-build/runtime coherency was fought three times — the MSBuild pin (#20073), the CPM floor (#20084, extended by #20100), and the MSBuild unpin (#20278) — before the direct-property pattern fixed it.

## Fix

[#20290](https://github.com/dotnet/fsharp/pull/20290): reference `$(System*Version)` directly in `eng/Packages.props` (matching roslyn), delete the `$(...CentralVersion)`/`SystemRuntimeCentralFloorVersion` scaffolding from `eng/Versions.props`, and declare the packages at `10.0.10` in `eng/Version.Details.{xml,props}` so product/PR restore meets the MSBuild 18.11 / Roslyn 5.11 transitive minimum. In source-build, `PackageVersions.fsharp.props` overrides those same properties down to the live runtime version, so the pin always tracks what the VMR actually builds. Verified by feeding a simulated `PackageVersions.fsharp.props` through `/p:DotNetPackageVersionPropsPath`: with the fix CPM resolves the injected `10.0.9`; with the old alias it stayed frozen at `10.0.10`.

## Timeline

| Date (2026) | PR | Event |
| --- | --- | --- |
| 08-04 | #20084 | CPM + transitive pinning introduces `$(System*CentralVersion)` + floor (latent). |
| 08-13 | #20073 | MSBuild pinned at `18.10.0-1.26370.18` (kept net10 assets for the then-net10 product). |
| 08-18 | #20100 | Floor extended to the Roslyn-pulled `System.*` packages. |
| 08-18 | #20278 | MSBuild unpinned to `18.11` line; floor bumped to `10.0.10`, expanded to 5 packages. |
| 08-19 | dotnet/dotnet#8413/#8414/#8415 | Forward-flow source-only build fails: prebuilt `DiagnosticSource 10.0.10` + `NU1109`. |
| 08-19 | #20290 | Direct `$(System*Version)`, floor removed, declared `10.0.10`. |

## Prevention

`.github/instructions/EngVersioning.instructions.md` scoped to `eng/Packages.props`, `eng/Versions.props`, and `eng/Version.Details.{xml,props}` encodes the rule: consume source-build "live version" packages through the darc-named `$(System*Version)` property directly; never route them through a renamed or computed central alias/floor. When a transitive consumer (MSBuild, Roslyn) needs a higher minimum than the flowed baseline, raise the declared version in `eng/Version.Details.*` rather than adding a floor — source-build overrides it back down to the live runtime version. fsharp CI cannot reproduce the VMR source-only prebuilt gate, so these files must be reasoned about against source-build, not just a green PR restore.
