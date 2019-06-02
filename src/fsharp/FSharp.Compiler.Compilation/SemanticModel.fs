namespace FSharp.Compiler.Compilation

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
open FSharp.Compiler.SourceCodeServices

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

    let tryFindSymbol line column (resolutions: TcResolutions) (symbolEnv: SymbolEnv) =
        let mutable result = None

        let p = mkPos line column
        for i = 0 to resolutions.CapturedNameResolutions.Count - 1 do
            let cnr = resolutions.CapturedNameResolutions.[i]
            if Range.rangeContainsPos cnr.Range p then
                result <- Some (FSharpSymbol.Create (symbolEnv, cnr.Item))

        result

    let findSymbols (symbol: FSharpSymbol) (resolutions: TcResolutions) (symbolEnv: SymbolEnv) =
        let result = ImmutableArray.CreateBuilder ()

        for i = 0 to resolutions.CapturedNameResolutions.Count - 1 do
            let cnr = resolutions.CapturedNameResolutions.[i]
            if ItemsAreEffectivelyEqual symbolEnv.g symbol.Item cnr.Item then
                result.Add (FSharpSymbolUse (symbolEnv.g, cnr.DisplayEnv, symbol, cnr.ItemOccurence, cnr.Range))

        result.ToImmutable ()

    let getToolTipText line column (resolutions: TcResolutions) (symbolEnv: SymbolEnv) =
        let mutable result = None

        let p = mkPos line column
        for i = 0 to resolutions.CapturedNameResolutions.Count - 1 do
            let cnr = resolutions.CapturedNameResolutions.[i]
            if Range.rangeContainsPos cnr.Range p then
                let items = [ FSharp.Compiler.SourceCodeServices.SymbolHelpers.FormatStructuredDescriptionOfItem false symbolEnv.infoReader cnr.Range cnr.DisplayEnv cnr.ItemWithInst ]
                result <- Some (SourceCodeServices.FSharpToolTipText items)

        result

    let hasDots (longDotId: LongIdentWithDots) =
        match longDotId with
        | LongIdentWithDots (_, []) -> false
        | _ -> true

    let getIdents (p: pos) (longDotId: LongIdentWithDots) =
        match longDotId with
        | LongIdentWithDots (id, dotms) ->
            match dotms |> List.rev |> List.tryFindIndex (fun dotm -> p.Line = dotm.EndLine && p.Column >= dotm.EndColumn) with
            | Some index ->
                id.[..index]
                |> List.map (fun x -> x.idText)
            | _ ->
                id.[..id.Length - 2]
                |> List.map (fun x -> x.idText)

    let getIdentsForCompletion line column (lineStr: string) (parsedInput: ParsedInput) =
        let p = mkPos line column
        let resultOpt = AstTraversal.Traverse(p, parsedInput, { new AstTraversal.AstVisitorBase<_>() with 
            member __.VisitExpr(_path, traverseSynExpr, defaultTraverse, expr) =
                defaultTraverse(expr)

            member __.VisitModuleDecl(defaultTraverse, decl) =
                match decl with
                | SynModuleDecl.Open (longDotId, _) ->
                    Some (getIdents p longDotId)
                | _ ->
                    defaultTraverse decl                      
        })
        match resultOpt with
        | None -> []
        | Some result -> result

    let getCompletionItems (symbolEnv: SymbolEnv) nenv ad m idents =
        let g = symbolEnv.g
        let amap = symbolEnv.amap
        let ncenv = NameResolver (g, amap, symbolEnv.infoReader, NameResolution.FakeInstantiationGenerator)
        ResolvePartialLongIdent ncenv nenv (ConstraintSolver.IsApplicableMethApprox g amap m) m ad idents false

    let getCompletionSymbols line column lineStr (parsedInput: ParsedInput) (resolutions: TcResolutions) (symbolEnv: SymbolEnv) =
        match tryGetBestCapturedEnv (mkPos line column) resolutions with
        | None -> []
        | Some (m, nenv, ad) ->
            // Look for a "special" completion context
            let completionContextOpt = UntypedParseImpl.TryGetCompletionContext(mkPos line column, parsedInput, lineStr)
            
            match completionContextOpt with
            | None -> []
            | Some completionContext ->
                match completionContext with
                | CompletionContext.Invalid -> []

                | CompletionContext.OpenDeclaration ->
                    let idents = getIdentsForCompletion line column lineStr parsedInput
                    getCompletionItems symbolEnv nenv ad m idents
                    |> List.choose (fun item ->
                        match item with
                        | Item.ModuleOrNamespaces _ -> Some (FSharpSymbol.Create (symbolEnv, item))
                        | _ -> None
                    )
                | _ -> []

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
            return tryFindSymbol line column resolutions symbolEnv
        }

    member __.FindSymbolUsesAsync symbol =
        async {
            let! _, resolutions, symbolEnv = asyncLazyGetAllSymbols.GetValueAsync ()
            return findSymbols symbol resolutions symbolEnv
        }

    member __.GetToolTipTextAsync (line, column) =
        async {
            let! _, resolutions, symbolEnv = asyncLazyGetAllSymbols.GetValueAsync ()
            return getToolTipText line column resolutions symbolEnv
        }

    member __.GetCompletionSymbolsAsync (line, column) =
        async {
            let! checker, resolutions, symbolEnv = asyncLazyGetAllSymbols.GetValueAsync ()

            let syntaxTree = checker.GetSyntaxTree filePath
            let! (parsedInputOpt, _) = syntaxTree.GetParseResultAsync ()

            match parsedInputOpt with
            | None -> 
                return []
            | Some parsedInput ->
                let! sourceText = syntaxTree.GetSourceTextAsync ()
                let lineStr = sourceText.Lines.[line - 1].ToString().Replace("\n", "").Replace("\r", "").TrimEnd(' ')
                return getCompletionSymbols line column lineStr parsedInput resolutions symbolEnv
        }
