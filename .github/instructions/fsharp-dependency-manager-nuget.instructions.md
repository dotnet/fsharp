---
applyTo: 'src/FSharp.DependencyManager.Nuget/**'
---

NuGet dependency manager for FSI: FSharpDependencyManager parses #r/#i directives, builds restore inputs, and emits package-resolution metadata via ProjectFile helpers.

- `parsePackageReferenceOption` must keep reserved-package checks (`mscorlib`, `FSharp.Core`, `System.ValueTuple`, `NETStandard.Library`, `Microsoft.NETFramework.ReferenceAssemblies`) before accepting `Include`.
- `computeHashForResolutionInputs` must treat wildcard package versions as non-cacheable input.
- `validateAndFormatRestoreSources` must reject missing local directories for file URIs and only append valid restore roots.
