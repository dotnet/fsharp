namespace FSharp.Compiler.Compilation

open System
open System.IO
open System.Threading
open System.Threading.Tasks
open System.Collections.Immutable
open System.Collections.Generic
open System.Collections.Concurrent
open Internal.Utilities.Collections
open FSharp.Compiler
open FSharp.Compiler.Infos
open FSharp.Compiler.AbstractIL.Internal.Library
open FSharp.Compiler.Ast
open FSharp.Compiler.Tastops
open FSharp.Compiler.CompileOps
open FSharp.Compiler.Range
open FSharp.Compiler.Driver
open FSharp.Compiler.Tast
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.AbstractIL.ILBinaryReader
open FSharp.Compiler.ErrorLogger
open FSharp.Compiler.CompileOptions
open FSharp.Compiler.TypeChecker
open FSharp.Compiler.NameResolution
open Internal.Utilities
open FSharp.Compiler.Compilation.Utilities

[<Sealed>]
type SemanticModel (filePath, asyncLazyChecker: AsyncLazy<IncrementalChecker>) =

    let asyncLazyGetSymbols =
        AsyncLazy(async {
            let! checker = asyncLazyChecker.GetValueAsync ()
            let! _tcAcc, sink, symbolEnv = checker.CheckAsync filePath
            return (sink.GetResolutions (), symbolEnv)
        })

    member __.TryFindSymbolAsync (line: int, column: int) : Async<FSharpSymbol option> =
        async {
            let mutable result = None

            // TODO: This is inefficient but works for now. Switch over to using a lexer to grab the token range and ask name resolution what it is.
            let! resolutions, symbolEnv = asyncLazyGetSymbols.GetValueAsync ()
            for i = 0 to resolutions.CapturedNameResolutions.Count - 1 do
                let cnr = resolutions.CapturedNameResolutions.[i]
                if Range.rangeContainsPos cnr.Range (mkPos line column) then
                    result <- Some (FSharpSymbol.Create (symbolEnv, cnr.Item))

            return result               
        }
