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

    let asyncLazyGetAllSymbols =
        AsyncLazy(async {
            let! checker = asyncLazyChecker.GetValueAsync ()
            let! _tcAcc, result = checker.CheckAsync filePath
            match result with
            | None -> return None
            | Some (sink, symbolEnv) ->
                let lines = sink.Lines

                let builder = ImmutableArray.CreateBuilder (lines.Count)
                builder.Count <- lines.Count
                for i = 0 to lines.Count - 1 do
                    if lines.[i] <> null then
                        let symbolUseDataList = lines.[i]
                        let symbols = ImmutableArray.CreateBuilder symbolUseDataList.Count

                        symbols.Count <- symbolUseDataList.Count

                        for j = 0 to lines.[i].Count - 1 do
                            let symbolUseData = lines.[i].[j]
                            symbols.[j] <- struct (FSharpSymbol.Create (symbolEnv, symbolUseData.Item), symbolUseData.Range)

                        builder.[i] <- symbols.ToImmutable ()
                    else
                        builder.[i] <- ImmutableArray.Empty

                return Some (builder.ToImmutable ())
        })

    member __.TryFindSymbolAsync (line: int, column: int) : Async<FSharpSymbol option> =
        async {
            match! asyncLazyGetAllSymbols.GetValueAsync () with
            | None -> return None
            | Some linesSymbol ->
                let lineIndex = line - 1

                if lineIndex >= linesSymbol.Length then
                    return None
                else
                    let symbols = linesSymbol.[lineIndex]
                    let mutable result = None
                    let mutable i = 0
                    while i < symbols.Length && result.IsNone do
                        let struct (symbol, m) = symbols.[i]
                        if Range.rangeContainsPos m (mkPos line column) then
                            result <- Some symbol
                        i <- i + 1
                    match result with
                    | None -> return None
                    | Some symbol ->
                        return Some symbol          
        }
