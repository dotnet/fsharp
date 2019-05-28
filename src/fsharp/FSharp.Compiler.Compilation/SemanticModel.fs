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

[<AutoOpen>]
module SemanticModelHelpers =

    let tryGetBestCapturedEnv p (resolutions: TcResolutions) =
        let mutable bestSoFar = None
        
        resolutions.CapturedEnvs 
        |> ResizeArray.iter (fun (possm,env,ad) -> 
            if rangeContainsPos possm p then
                match bestSoFar with 
                | Some (bestm,_,_) -> 
                    if rangeContainsRange bestm possm then 
                      bestSoFar <- Some (possm,env,ad)
                | None -> 
                    bestSoFar <- Some (possm,env,ad))

        bestSoFar

[<Sealed>]
type SemanticModel (filePath, asyncLazyChecker: AsyncLazy<IncrementalChecker>) =

    let asyncLazyGetAllSymbols =
        AsyncLazy(async {
            let! checker = asyncLazyChecker.GetValueAsync ()
            let! _tcAcc, sink, symbolEnv = checker.CheckAsync filePath
            return (checker, sink.GetResolutions (), symbolEnv)
        })

    member __.TryFindSymbolAsync (line: int, column: int) : Async<FSharpSymbol option> =
        async {
            let! _, resolutions, symbolEnv = asyncLazyGetAllSymbols.GetValueAsync ()
            let mutable result = None

            let p = mkPos line column
            for i = 0 to resolutions.CapturedNameResolutions.Count - 1 do
                let cnr = resolutions.CapturedNameResolutions.[i]
                if Range.rangeContainsPos cnr.Range p then
                    result <- Some (FSharpSymbol.Create (symbolEnv, cnr.Item))

            return result
        }

    member __.FindSymbolUsesAsync (symbol: FSharpSymbol) =
        async {
            let result = ImmutableArray.CreateBuilder ()

            let! _, resolutions, symbolEnv = asyncLazyGetAllSymbols.GetValueAsync ()
            for i = 0 to resolutions.CapturedNameResolutions.Count - 1 do
                let cnr = resolutions.CapturedNameResolutions.[i]
                if ItemsAreEffectivelyEqual symbolEnv.g symbol.Item cnr.Item then
                    result.Add (FSharpSymbolUse (symbolEnv.g, cnr.DisplayEnv, symbol, cnr.ItemOccurence, cnr.Range))

            return result.ToImmutable ()
        }
