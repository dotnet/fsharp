namespace FSharp.Compiler.Compilation

open System.Threading
open System.Collections.Generic
open FSharp.Compiler
open FSharp.Compiler.AbstractIL.Internal.Library
open FSharp.Compiler.AbstractIL.ILBinaryReader
open FSharp.Compiler.Compilation.Utilities
open Internal.Utilities

type CompilationServiceOptions =
    {
        CompilationGlobalOptions: CompilationGlobalOptions
        Workspace: Microsoft.CodeAnalysis.Workspace // TODO: Temporary, eventually remove when we want to make an interface into its dependencies, i.e. Microsoft.CodeAnalysis.Host.ITemporaryStorageService
    }

    static member Create workspace =
        {
            CompilationGlobalOptions = CompilationGlobalOptions.Create ()
            Workspace = workspace
        }

[<Sealed>]
type CompilationService (options: CompilationServiceOptions) =
    // Caches
    let frameworkTcImportsCache = FrameworkImportsCache 8 // TODO: Is 8 ok?
    let temporaryStorageService = options.Workspace.Services.TemporaryStorage
    let incrementalCheckerMruCache = 
        // Is a Mru algorithm really the right cache for these? A Lru might be better.
        MruWeakCache<struct (CompilationId * VersionStamp), IncrementalChecker> (
            cacheSize = 3, 
            weakReferenceCacheSize = 1000, 
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

    member __.CreateCompilation (compilationOptions: CompilationOptions) = 
        Compilation.create compilationOptions options.CompilationGlobalOptions
            { 
                frameworkTcImportsCache = frameworkTcImportsCache
                incrementalCheckerCache = incrementalCheckerMruCache
            }