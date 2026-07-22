# Vendored serviced VS-2017 Roslyn (Microsoft.CodeAnalysis.* 2.10.0-beta2-72429-17)

This folder is a committed **local NuGet feed** carrying the serviced Visual Studio 2017 (15.9)
Roslyn packages that the F# VS editor assemblies (`vsintegration/src/FSharp.Editor`,
`FSharp.LanguageService`) compile against.

## Why this is vendored

The exact serviced version `2.10.0-beta2-72429-17` exists **only** on the internal devdiv `VS` feed
(`devdiv.pkgs.visualstudio.com/_packaging/VS`); it is not on any public/approved feed, and it is the
one and only reason this branch's CI would otherwise need a cross-org devdiv service connection. That
connection is a moving target (the classic PAT connection was removed by the org PAT-disable migration)
and read access to the VS feed is not something this pipeline should have to negotiate for a
build-time, compile-only reference.

`main` does not have this problem because it compiles the editor against modern Roslyn pulled from the
public feeds. This is a **servicing** branch pinned (in the guarded `vsintegration/src/**/*.fsproj`
+ `eng/Versions.props`) to the serviced 2.10 Roslyn, so those `PackageReference`s must resolve from a
source the build can always reach. Committing the packages here makes the build **self-contained**:
no devdiv feed, no service connection, no authorization.

These Roslyn assets are **compile-time references only** (`ExcludeAssets=runtime;... PrivateAssets=all`
in the consuming projects) — they are never shipped in the VSIX; the runtime Roslyn is supplied by
Visual Studio itself.

## How it is wired

`NuGet.config` adds this folder as a package source (`roslyn-serviced-vs2017`). The existing (unchanged)
`PackageReference`s in the vsintegration projects then restore the serviced Roslyn from here.

## Provenance / integrity

The 12 `.nupkg` files are the complete restore closure at `2.10.0-beta2-72429-17`, taken verbatim from a
prior authenticated devdiv restore of this repo. Each package's assemblies are authored by
`Microsoft Corporation` (e.g. `Microsoft.CodeAnalysis.EditorFeatures.dll` file version `2.10.0.0`). The
packages are internal serviced builds and are not NuGet-repository-signed, which is expected for a local
folder feed (signature validation does not apply to local sources).

Do not edit or repack these files.
