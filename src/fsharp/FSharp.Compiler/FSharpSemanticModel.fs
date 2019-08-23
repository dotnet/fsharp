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
open FSharp.Compiler.Compilation.IncrementalChecker
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

    let tryGetBestCapturedTypeCheckEnvEnv p (resolutions: TcResolutions) =
        let mutable bestSoFar = None
        
        resolutions.CapturedTypeCheckEnvs 
        |> ResizeArray.iter (fun (possm,env) -> 
            if rangeContainsPos possm p then
                match bestSoFar with 
                | Some (bestm,_) -> 
                    if rangeContainsRange bestm possm then 
                      bestSoFar <- Some (possm,env)
                | None -> 
                    bestSoFar <- Some (possm,env))

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
type FSharpSymbol private (senv: SymbolEnv, item: Item) =

    member __.Item = item

    member __.Name = item.DisplayName

    static member Create (senv: SymbolEnv, item) =
        FSharpSymbol (senv, item)

[<NoEquality;NoComparison;RequireQualifiedAccess>]
type FSharpSymbolInfo =
    | ReferredByNode of FSharpSymbol
    | Candidates of ImmutableArray<FSharpSymbol>

    member this.Symbol =
        match this with
        | ReferredByNode symbol -> Some symbol
        | _ -> None

    member this.CandidateSymbols =
        match this with
        | Candidates candidateSymbols -> candidateSymbols
        | _ -> ImmutableArray.Empty

    member this.GetAllSymbols () =
        match this.Symbol with
        | Some symbol -> ImmutableArray.Create symbol
        | _ -> this.CandidateSymbols

    static member Empty = Candidates ImmutableArray.Empty

[<Sealed>]
type FSharpSemanticModel (filePath, asyncLazyChecker: AsyncLazy<IncrementalChecker>, compilationObj: obj) =

    let getChecker ct =
        Async.RunSynchronously (asyncLazyChecker.GetValueAsync (), cancellationToken = ct)

    let check ct =
        let checker = getChecker ct
        Async.RunSynchronously (checker.CheckAsync filePath, cancellationToken = ct)

    let getErrors ct =
        let tcAcc, _, _ = check ct
        tcAcc.tcErrorsRev.Head

    let lazySyntaxTree =
        lazy
            // This will go away when incremental checker doesn't build syntax trees.
            let checker = getChecker CancellationToken.None
            checker.GetSyntaxTree filePath

    let asyncLazyGetAllSymbols =
        AsyncLazy(async {
            let! ct = Async.CancellationToken
            let checker = getChecker ct
            let tcAcc, sink, senv = check ct
            return (checker, tcAcc, sink.GetResolutions (), senv)
        })

    static let getSymbolInfo symbolEnv (cnrs: ResizeArray<CapturedNameResolution>) (node: FSharpSyntaxNode) =
        let nodeRange = node.SyntaxTree.ConvertSpanToRange node.Span
        let candidateSymbols = ImmutableArray.CreateBuilder ()
        let mutable symbol = ValueNone
        let mutable i = 0
        while symbol.IsNone && i < cnrs.Count do
            let cnr = cnrs.[i]
            if rangeContainsRange nodeRange cnr.Range then
                let candidateSymbol = FSharpSymbol.Create (symbolEnv, cnr.Item)
                candidateSymbols.Add candidateSymbol
                if Range.equals nodeRange cnr.Range then
                    symbol <- ValueSome candidateSymbol // no longer a candidate
            i <- i + 1

        match symbol with
        | ValueSome symbol ->
            FSharpSymbolInfo.ReferredByNode symbol
        | _ ->
            FSharpSymbolInfo.Candidates (candidateSymbols.ToImmutable ())

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

    member this.FindToken (position, ct) =
        let rootNode = this.SyntaxTree.GetRootNode ct
        rootNode.FindToken position

    member __.GetSymbolInfo (node: FSharpSyntaxNode, ct) =
        let _checker, _tcAcc, resolutions, symbolEnv = Async.RunSynchronously (asyncLazyGetAllSymbols.GetValueAsync (), cancellationToken = ct)
        let cnrs = resolutions.CapturedNameResolutions
        getSymbolInfo symbolEnv cnrs node

    member this.TryGetEnclosingSymbol (position: int, ct) =
        let token = this.FindToken (position, ct)
        if token.IsNone then
            None
        else
            let symbolInfo = this.GetSymbolInfo (token.GetParentNode ct, ct)
            symbolInfo.Symbol

    /// VERY EXPERIMENTAL.
    /// But, if we figure out the right heuristics, this can be fantastic.
    member this.GetSpeculativeSymbolInfo (position: int, node: FSharpSyntaxNode, ct) =
        let token = this.FindToken (position, ct)
        if token.IsNone then
            FSharpSymbolInfo.Empty
        else
            match node.GetAncestorsAndSelf () |> Seq.tryPick (fun x -> match x.Kind with FSharpSyntaxNodeKind.Expr synExpr -> Some synExpr | _ -> None) with
            | Some synExpr ->
                let checker, tcAcc, resolutions, symbolEnv = Async.RunSynchronously (asyncLazyGetAllSymbols.GetValueAsync (), cancellationToken = ct)

                let range = this.SyntaxTree.ConvertSpanToRange token.Span
                match tryGetBestCapturedTypeCheckEnvEnv range.Start resolutions with
                | Some (m, env) ->
                    match env with
                    | :? TcEnv as tcEnv ->
                        let tcState = tcAcc.tcState.NextStateAfterIncrementalFragment tcEnv
                        let resultOpt = Async.RunSynchronously (checker.SpeculativeCheckAsync (filePath, tcState, synExpr), cancellationToken = ct)
                        match resultOpt with
                        | Some (ty, sink) ->
                            let specCnrs = sink.GetResolutions().CapturedNameResolutions
                            getSymbolInfo symbolEnv specCnrs node
                        | _ ->
                            FSharpSymbolInfo.Empty
                    | _ ->
                        FSharpSymbolInfo.Empty
                | _ ->
                    FSharpSymbolInfo.Empty
            | _ ->
                FSharpSymbolInfo.Empty

    member this.LookupSymbols (position: int, ct: CancellationToken) =
        let token = this.FindToken (position, ct)
        if token.IsNone then
            ImmutableArray.Empty
        else
            let checker, tcAcc, resolutions, symbolEnv = Async.RunSynchronously (asyncLazyGetAllSymbols.GetValueAsync (), cancellationToken = ct)

            let range = this.SyntaxTree.ConvertSpanToRange token.Span
            match tryGetBestCapturedTypeCheckEnvEnv range.Start resolutions with
            | Some (m, env) ->
                match env with
                | :? TcEnv as tcEnv ->
                    let symbolsBuilder = ImmutableArray.CreateBuilder ()
                    tcEnv.NameEnv.eUnqualifiedItems.Values
                    |> List.iter (fun item ->
                        symbolsBuilder.Add (FSharpSymbol.Create (symbolEnv, item))
                    )
                    symbolsBuilder.ToImmutable ()
                | _ ->
                    ImmutableArray.Empty
            | _ ->
                ImmutableArray.Empty

    member __.SyntaxTree: FSharpSyntaxTree = lazySyntaxTree.Value

    member __.CompilationObj = compilationObj

    member this.GetSyntaxDiagnostics ?ct =
        let ct = defaultArg ct CancellationToken.None

        this.SyntaxTree.GetDiagnostics ct

    member this.GetSemanticDiagnostics ?ct =
        let ct = defaultArg ct CancellationToken.None

        let text = this.SyntaxTree.GetText ct
        let errors = getErrors ct
        errors.ToDiagnostics (filePath, text)

    member this.GetDiagnostics ?ct =
        let ct = defaultArg ct CancellationToken.None

        let syntaxDiagnostics = this.GetSyntaxDiagnostics ct
        let semanticDiagnostics = this.GetSemanticDiagnostics ct

        let builder = ImmutableArray.CreateBuilder(syntaxDiagnostics.Length + semanticDiagnostics.Length)
        builder.AddRange syntaxDiagnostics
        builder.AddRange semanticDiagnostics
        builder.ToImmutable ()
