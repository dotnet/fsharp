namespace FSharp.Compiler.Compilation

open System.Threading
open System.Collections.Generic
open FSharp.Compiler
open FSharp.Compiler.AbstractIL.Internal.Library
open FSharp.Compiler.Compilation.Utilities

[<Sealed>]
type CompilationService (_compilationCacheSize: int, frameworkTcImportsCacheStrongSize, workspace: Microsoft.CodeAnalysis.Workspace) =
    // Caches
    let frameworkTcImportsCache = FrameworkImportsCache frameworkTcImportsCacheStrongSize
    let temporaryStorageService = workspace.Services.TemporaryStorage
    let incrementalCheckerMruCache = 
        // Is a Mru algorithm really the right cache for these? A Lru might be better.
        MruCache<struct (CompilationId * VersionStamp), IncrementalChecker> (
            cacheSize = 3, 
            maxWeakReferenceSize = 1000, 
            equalityComparer = EqualityComparer<struct (CompilationId * VersionStamp)>.Default
        ) 

    member __.CreateSourceSnapshot (filePath, sourceText) =
        let storage = temporaryStorageService.CreateTemporaryTextStorage ()
        storage.WriteText sourceText

        match
            temporaryStorageService.CreateSourceSnapshot (filePath, sourceText)
            |> Cancellable.run CancellationToken.None with
        | ValueOrCancelled.Value result -> result
        | ValueOrCancelled.Cancelled ex -> raise ex

    member __.CreateCompilation (options: CompilationOptions) = 
        Compilation.create options 
            { 
                frameworkTcImportsCache = frameworkTcImportsCache
                incrementalCheckerCache = incrementalCheckerMruCache
            }