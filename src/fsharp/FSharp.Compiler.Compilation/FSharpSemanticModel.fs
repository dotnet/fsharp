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
module FSharpSemanticModelHelpers =

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
                match expr with
                | SynExpr.LongIdent(_, longDotId, _, _) ->
                    Some (getIdents p longDotId)
                | _ ->
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
            | None ->
                let idents = getIdentsForCompletion line column lineStr parsedInput
                getCompletionItems symbolEnv nenv ad m idents
                |> List.map (fun item -> FSharpSymbol.Create (symbolEnv, item))
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
type FSharpSemanticModel (filePath, asyncLazyChecker: AsyncLazy<IncrementalChecker>, compilationObj: obj) =

    let getSyntaxTreeAsync =
        async {
            // This will go away when incremental checker doesn't build syntax trees.
            let checker = asyncLazyChecker.GetValueAsync () |> Async.RunSynchronously
            return checker.GetSyntaxTree filePath
        }

    let asyncLazyGetAllSymbols =
        AsyncLazy(async {
            let! checker = asyncLazyChecker.GetValueAsync ()
            let! tcAcc, sink, symbolEnv = checker.CheckAsync filePath
            return (checker, tcAcc, sink.GetResolutions (), symbolEnv)
        })

    member __.TryFindSymbolAsync (line: int, column: int) : Async<FSharpSymbol option> =
        async {
            let! checker, _tcAcc, resolutions, symbolEnv = asyncLazyGetAllSymbols.GetValueAsync ()
            return tryFindSymbol line column resolutions symbolEnv
        }

    member __.FindSymbolUsesAsync symbol =
        async {
            let! _, _, resolutions, symbolEnv = asyncLazyGetAllSymbols.GetValueAsync ()
            return findSymbols symbol resolutions symbolEnv
        }

    member __.GetToolTipTextAsync (line, column) =
        async {
            let! _, _, resolutions, symbolEnv = asyncLazyGetAllSymbols.GetValueAsync ()
            return getToolTipText line column resolutions symbolEnv
        }

    member __.GetCompletionSymbolsAsync (line, column) =
        async {
            let! ct = Async.CancellationToken
            let! checker, _, resolutions, symbolEnv = asyncLazyGetAllSymbols.GetValueAsync ()

            let syntaxTree = checker.GetSyntaxTree filePath
            let (parsedInputOpt, _) = syntaxTree.GetParseResult ct

            match parsedInputOpt with
            | None -> 
                return []
            | Some parsedInput ->
                let sourceText = syntaxTree.GetText ct
                let lineStr = sourceText.Lines.[line - 1].ToString().Replace("\n", "").Replace("\r", "").TrimEnd(' ')
                return getCompletionSymbols line column lineStr parsedInput resolutions symbolEnv
        }

    member __.SyntaxTree =
        // This will go away when incremental checker doesn't build syntax trees.
        getSyntaxTreeAsync |> Async.RunSynchronously

    member __.CompilationObj = compilationObj
