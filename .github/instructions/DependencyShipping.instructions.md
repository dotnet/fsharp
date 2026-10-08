---
applyTo:
  - "Directory.Packages.props"
  - "eng/Packages.props"
  - "eng/Versions.props"
  - "eng/Version.Details.xml"
  - "eng/Version.Details.props"
  - "eng/Signing.props"
  - "src/**/*.{fsproj,props,targets}"
---

# Shipping binary dependencies

A package change is also a product-layout and signing change when any assembly from the package reaches a shipped archive, NuGet package, tool, or SDK layout. This includes new transitive assemblies introduced by a version update.

Before merging such a change:

- Enumerate the complete new DLL closure in every shipped layout, not only the directly referenced package. Inspect the publish/package output or the MSBuild items that compose it.
- Identify every repository that performs a later packaging or signing pass. A component's `eng/Signing.props` applies to that component's build; it does not automatically configure a downstream owner that rebundles the files. For example, FSI assemblies copied into the .NET SDK are signed again under the SDK's signing policy.
- Classify each new DLL at every signing boundary: Microsoft-owned, third-party re-signed with the repository's external certificate, or intentionally left unchanged. Do not assume an upstream signature or `FileSignInfo` survives a downstream container-signing pass.
- Keep product behavior and dependency declarations in dotnet/fsharp. If the integrated VMR build needs an additional final-layout policy entry owned by another component, make the smallest explicit integration change and request review from that component's owners; do not distort F# product files to satisfy a downstream packager.
- Validate both the F# build and the relevant VMR legs. A green dotnet/fsharp build does not exercise the SDK's Windows dry-run container signing. Treat `SIGN004` in a downstream layout as evidence that the final signing owner is missing or misclassifying a shipped file.
- When the same source commit flows to multiple aligned VMR branches, compare the generated product trees. Resolve F# product code, tests, baselines, and release notes from dotnet/fsharp; preserve genuinely VMR-owned infrastructure such as synchronized `eng/common` or final-layout policy.

For codeflow conflicts, run the exact `darc vmr resolve-conflict --subscription ...` command from the bot before resolving files. Review the staged result rather than reconstructing the flow by hand: crossing flows can duplicate release-note entries, and moved definitions can make an apparently necessary conflict side a duplicate.
