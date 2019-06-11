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
open Microsoft.CodeAnalysis.Text

[<AutoOpen>]
module FSharpSemanticModelHelpers =

    open FSharp.Compiler.SourceCodeServices

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

//[<NoEquality;NoComparison>]
//type BinderKind =
//    | NormalBind of resolutions: Dictionary<int, ResizeArray<CapturedNameResolution>>
//    | SpeculativeBind of m: range * nenv: NameResolutionEnv *ad: AccessibilityLogic.AccessorDomain

//[<Sealed>]
//type Binder (senv: SymbolEnv, kind: BinderKind) =

//    member __.TryBindModuleOrNamespace (node: FSharpSyntaxNode) =
//            match node.Kind, (node.Parent |> Option.map (fun x -> x.Kind)) with
//            | FSharpSyntaxNodeKind.Ident (index, id), Some (FSharpSyntaxNodeKind.ModuleOrNamespace (SynModuleOrNamespace (longId=longId))) ->
//                match kind with
//                | NormalBind resolutions ->
//                    match resolutions.TryGetValue node.Range.EndLine with
//                    | true, cnrs ->
//                        cnrs
//                        |> Seq.tryFind (fun cnr -> Range.equals cnr.Range node.Range)
//                        |> Option.map (fun cnr ->
//                            cnr.NameResolutionEnv.
//                            let symbol = FSharpSymbol.Create (senv, cnr.Item)
//                            FSharpSymbolUse (senv.g, cnr.DisplayEnv, symbol, cnr.ItemOccurence, cnr.Range)
//                        )
//                    | _ ->
//                        None
//                | SpeculativeBind (m, nenv, ad) ->
//                    match
//                        ResolveLongIndentAsModuleOrNamespace 
//                            TcResultsSink.NoSink 
//                            ResultCollectionSettings.AtMostOneResult 
//                            senv.amap
//                            m
//                            (index = 0)
//                            OpenQualified
//                            nenv
//                            ad
//                            id
//                            longId.[..(index - 1)]
//                            false with
//                    | Result results ->
//                        match results with
//                        | [(_, modref, _)] -> 
//                            let item = Item.ModuleOrNamespaces [modref]
//                            let symbol = FSharpSymbol.Create (senv, item)
//                            FSharpSymbolUse (senv.g, nenv.DisplayEnv, symbol, ItemOccurence.)
//                        | _ -> None
//                    | Exception _ -> None
//            | _ ->
//                None

[<Sealed>]
type FSharpSyntaxReference (syntaxTree: FSharpSyntaxTree, span: TextSpan) =

    member __.SyntaxTree = syntaxTree

    member __.Span = span


[<NoEquality;NoComparison;RequireQualifiedAccess>]
type FSharpDefinitionLocation =
    | Source of FSharpSyntaxReference
    | Metadata

type IFSharpDefinitionAndDeclarationFinder =

    abstract FindDefinitions: FSharpSyntaxNode -> ImmutableArray<FSharpDefinitionLocation>

    abstract FindDeclarations: FSharpSyntaxNode -> ImmutableArray<FSharpSyntaxReference>

[<NoEquality;NoComparison;RequireQualifiedAccess>]
type FSharpSymbolKind =
    | Namespace

[<Sealed>]
type FSharpSymbol (internalSymbolUse: InternalFSharpSymbolUse) =

    member __.InternalSymbolUse = internalSymbolUse

    static member Create (cnr: CapturedNameResolution, senv: SymbolEnv) =
        let internalSymbol = InternalFSharpSymbol.Create (senv, cnr.Item)
        let internalSymbolUse = InternalFSharpSymbolUse (senv.g, cnr.DisplayEnv, internalSymbol, cnr.ItemOccurence, cnr.Range)
        FSharpSymbol (internalSymbolUse)

[<NoEquality;NoComparison;RequireQualifiedAccess>]
type FSharpSymbolInfo =
    | ReferredByNode of FSharpSymbol
    | Candidates of ImmutableArray<FSharpSymbol>

    member this.TryGetSymbol () =
        match this with
        | ReferredByNode symbol -> Some symbol
        | _ -> None

    member this.CandidateSymbols =
        match this with
        | Candidates candidateSymbols -> candidateSymbols
        | _ -> ImmutableArray.Empty

    member this.GetAllSymbols () =
        match this.TryGetSymbol () with
        | Some symbol -> ImmutableArray.Create symbol
        | _ -> this.CandidateSymbols

[<Sealed>]
type FSharpSemanticModel (filePath, asyncLazyChecker: AsyncLazy<IncrementalChecker>, compilationObj: obj) =

    let lazySyntaxTree =
        lazy
            // This will go away when incremental checker doesn't build syntax trees.
            let checker = asyncLazyChecker.GetValueAsync () |> Async.RunSynchronously
            checker.GetSyntaxTree filePath

    let asyncLazyGetAllSymbols =
        AsyncLazy(async {
            let! checker = asyncLazyChecker.GetValueAsync ()
            let! tcAcc, sink, symbolEnv = checker.CheckAsync filePath
            return (checker, tcAcc, sink.GetResolutions (), symbolEnv)
        })

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

    member __.GetSymbolInfo (node: FSharpSyntaxNode, ct) =
        let _checker, _tcAcc, resolutions, symbolEnv = Async.RunSynchronously (asyncLazyGetAllSymbols.GetValueAsync (), cancellationToken = ct)

        let cnrs = resolutions.CapturedNameResolutions
        let candidateSymbols = ImmutableArray.CreateBuilder ()
        let mutable symbol = ValueNone
        let mutable i = 0
        while symbol.IsNone || i >= cnrs.Count do
            let cnr = cnrs.[i]
            if rangeContainsRange node.Range cnr.Range then
                let candidateSymbol = FSharpSymbol.Create (cnr, symbolEnv)
                candidateSymbols.Add candidateSymbol
                if Range.equals node.Range cnr.Range then
                    symbol <- ValueSome candidateSymbol // no longer a candidate

        match symbol with
        | ValueSome symbol ->
            FSharpSymbolInfo.ReferredByNode symbol
        | _ ->
            FSharpSymbolInfo.Candidates (candidateSymbols.ToImmutable ())

    member this.TryGetEnclosingSymbol (position: int, ct) =
        let rootNode = this.SyntaxTree.GetRootNode ct
        match rootNode.TryFindToken position with
        | Some token ->
            let symbolInfo = this.GetSymbolInfo (token.ParentNode, ct)
            symbolInfo.TryGetSymbol ()
        | _ ->
            None

    member __.SyntaxTree: FSharpSyntaxTree = lazySyntaxTree.Value

    member __.CompilationObj = compilationObj
