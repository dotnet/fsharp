namespace FSharp.Compiler.Compilation

open Internal.Utilities.Collections
open FSharp.Compiler.AbstractIL.Internal.Library
open FSharp.Compiler.CompileOps
open FSharp.Compiler.TcGlobals

/// Global service state
type FrameworkImportsCacheKey = (*resolvedpath*)string list * string * (*TargetFrameworkDirectories*)string list* (*fsharpBinaries*)string

/// Represents a cache of 'framework' references that can be shared betweeen multiple incremental builds
type FrameworkImportsCache(keepStrongly) = 

    // Mutable collection protected via CompilationThreadToken 
    let frameworkTcImportsCache = AgedLookup<CompilationThreadToken, FrameworkImportsCacheKey, (TcGlobals * TcImports)>(keepStrongly, areSimilar=(fun (x, y) -> x = y)) 

    /// Reduce the size of the cache in low-memory scenarios
    member __.Downsize ctok = frameworkTcImportsCache.Resize(ctok, keepStrongly=0)

    /// Clear the cache
    member __.Clear ctok = frameworkTcImportsCache.Clear ctok

    /// This function strips the "System" assemblies from the tcConfig and returns a age-cached TcImports for them.
    member __.Get(ctok, tcConfig: TcConfig) =
      cancellable {
        // Split into installed and not installed.
        let frameworkDLLs, nonFrameworkResolutions, unresolved = TcAssemblyResolutions.SplitNonFoundationalResolutions(ctok, tcConfig)
        let frameworkDLLsKey = 
            frameworkDLLs 
            |> List.map (fun ar->ar.resolvedPath) // The cache key. Just the minimal data.
            |> List.sort  // Sort to promote cache hits.

        let! tcGlobals, frameworkTcImports = 
          cancellable {
            // Prepare the frameworkTcImportsCache
            //
            // The data elements in this key are very important. There should be nothing else in the TcConfig that logically affects
            // the import of a set of framework DLLs into F# CCUs. That is, the F# CCUs that result from a set of DLLs (including
            // FSharp.Core.dll and mscorlib.dll) must be logically invariant of all the other compiler configuration parameters.
            let key = (frameworkDLLsKey, 
                        tcConfig.primaryAssembly.Name, 
                        tcConfig.GetTargetFrameworkDirectories(), 
                        tcConfig.fsharpBinariesDir)

            match frameworkTcImportsCache.TryGet (ctok, key) with 
            | Some res -> return res
            | None -> 
                let tcConfigP = TcConfigProvider.Constant tcConfig
                let! ((tcGlobals, tcImports) as res) = TcImports.BuildFrameworkTcImports (ctok, tcConfigP, frameworkDLLs, nonFrameworkResolutions)
                frameworkTcImportsCache.Put(ctok, key, res)
                return tcGlobals, tcImports
          }
        return tcGlobals, frameworkTcImports, nonFrameworkResolutions, unresolved
      }

