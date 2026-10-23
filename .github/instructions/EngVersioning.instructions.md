---
applyTo:
  - "eng/Packages.props"
  - "eng/Versions.props"
  - "eng/Version.Details.xml"
  - "eng/Version.Details.props"
---

These files feed the VMR source-build. The `System.*` dotnet/runtime packages marked *"Necessary for source-build"* in `eng/Version.Details.xml` are overridden at build time by the VMR's generated `PackageVersions.fsharp.props`, which replaces the darc-named `$(System*Version)` properties (e.g. `$(SystemCollectionsImmutableVersion)`) with the live source-built runtime version.

Rules:

- Consume those packages in `eng/Packages.props` through `$(System*Version)` **directly**. Never route them through a renamed or computed alias (e.g. a `$(System*CentralVersion)`) or a version floor — source-build overrides the darc-named property, and a renamed/eagerly-computed intermediate freezes the value so the override never reaches CPM, causing prebuilt packages and `NU1109` downgrades in the source-only build.
- When a transitive consumer (MSBuild, Roslyn) requires a higher minimum than the flowed baseline, raise the declared version in `eng/Version.Details.{xml,props}` instead of adding a floor. Source-build overrides it back down to the live runtime version; product/PR restore uses the declared value.
- fsharp PR/CI does **not** run the VMR source-only prebuilt gate (`SB_CentOSStream10_Online_MsftSdk_x64`), so a green PR restore does not prove source-build correctness. Reason about these edits against source-build.

See `docs/postmortems/regression-sourcebuild-cpm-runtime-version-floor.md` for why.
