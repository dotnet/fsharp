namespace FSharp.Compiler.Compilation

open System.Threading
open FSharp.Compiler
open FSharp.Compiler.AbstractIL.Internal.Library
open FSharp.Compiler.Compilation.Utilities

[<Sealed>]
type CompilationService (_compilationCacheSize: int, frameworkTcImportsCacheStrongSize, workspace: Microsoft.CodeAnalysis.Workspace) =
    // Caches
    let frameworkTcImportsCache = FrameworkImportsCache frameworkTcImportsCacheStrongSize
    let temporaryStorageService = workspace.Services.TemporaryStorage

    member __.CreateSourceSnapshot (filePath, sourceText) =
        let storage = temporaryStorageService.CreateTemporaryTextStorage ()
        storage.WriteText sourceText

        match
            temporaryStorageService.CreateSourceSnapshot (filePath, sourceText)
            |> Cancellable.run CancellationToken.None with
        | ValueOrCancelled.Value result -> result
        | ValueOrCancelled.Cancelled ex -> raise ex

    member __.CreateCompilation options = Compilation.create options frameworkTcImportsCache