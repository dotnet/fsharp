---
applyTo: 'src/LegacyMSBuildResolver/**'
---

Legacy .NET Framework reference-assembly resolver: LegacyMSBuildReferenceResolver locates framework roots, chooses the highest installed target, and decodes MSBuild ResolvedFrom values.

- `GetPathToDotNetFrameworkImplementationAssemblies` is only a last-resort path provider, so it must return at most one framework path per version.
- `DeriveTargetFrameworkDirectories` must normalize the input to a `v`-prefixed version before calling MSBuild helpers.
- `SupportedDesktopFrameworkVersions` and `HighestInstalledRefAssembliesOrDotNETFramework` must stay in sync, with the newest framework first.
