namespace FSharp.Compiler.Compilation

open System.IO
open System.Threading
open System.Collections.Immutable
open System.Runtime.CompilerServices
open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.Host
open FSharp.Compiler.Compilation.Utilities
open FSharp.Compiler.AbstractIL.Internal.Library
open FSharp.Compiler
open FSharp.Compiler.Text
open FSharp.Compiler.CompileOps
open FSharp.Compiler.Ast
open FSharp.Compiler.Range

[<AutoOpen>]
module SyntaxTreeHelpers =

    let isZeroRange (r: range) =
        posEq r.Start r.End

    let tryVisit p m visit item =
        if rangeContainsPos m p && not (isZeroRange m) then
            visit item
        else
            None

    let tryVisitList p xs : 'T option =
        xs
        |> List.sortWith (fun (getRange1, _) (getRange2, _)->
            let r1 = getRange1 ()
            let r2 = getRange2 ()
            rangeOrder.Compare (r1, r2)
        )
        |> List.tryPick (fun (getRange, visit) ->
            let r = getRange ()
            if rangeContainsPos r p && not (isZeroRange r) then
                visit ()
            else
                None
        )
        |> Option.orElseWith (fun () ->
            xs
            |> List.tryPick (fun (getRange, visit) ->
                let r = getRange ()
                if posGt p r.Start && not (isZeroRange r) then
                    visit ()
                else
                    None
            )
        )

    let mapVisitList getRange visit xs =
        xs
        |> List.map (fun x -> ((fun () -> getRange x), fun () -> visit x))

    type ParsedHashDirective with

        member this.Range =
            match this with
            | ParsedHashDirective (_, _, m) -> m

    type SynTypeConstraint with

        member this.Range =
            match this with
            | SynTypeConstraint.WhereTyparIsValueType (_, m) -> m
            | SynTypeConstraint.WhereTyparIsReferenceType (_, m) -> m
            | SynTypeConstraint.WhereTyparIsUnmanaged (_, m) -> m
            | SynTypeConstraint.WhereTyparSupportsNull (_, m) -> m
            | SynTypeConstraint.WhereTyparIsComparable (_, m) -> m
            | SynTypeConstraint.WhereTyparIsEquatable (_, m) -> m
            | SynTypeConstraint.WhereTyparDefaultsToType (_, _, m) -> m
            | SynTypeConstraint.WhereTyparSubtypeOfType (_, _, m) -> m
            | SynTypeConstraint.WhereTyparSupportsMember (_, _, m) -> m
            | SynTypeConstraint.WhereTyparIsEnum (_, _, m) -> m
            | SynTypeConstraint.WhereTyparIsDelegate (_, _, m) -> m

    type SynMemberSig with

        member this.Range =
            match this with
            | SynMemberSig.Member (_, _, m) -> m
            | SynMemberSig.Interface (_, m) -> m
            | SynMemberSig.Inherit (_, m) -> m
            | SynMemberSig.ValField (_, m) -> m
            | SynMemberSig.NestedType (_, m) -> m

    type SynValSig with

        member this.Range =
            match this with
            | ValSpfn (_, _, _, _, _, _, _, _, _, _, m) -> m

    type SynField with

        member this.Range =
            match this with
            | Field (_, _, _, _, _, _, _, m) -> m

    type SynTypeDefnSig with

        member this.Range =
            match this with
            | TypeDefnSig (_, _, _, m) -> m

    type SynSimplePat with

        member this.Range =
            match this with
            | SynSimplePat.Id (range=m) -> m
            | SynSimplePat.Typed (range=m) -> m
            | SynSimplePat.Attrib (range=m) -> m

    type SynMeasure with

        member this.PossibleRange =
            match this with
            | SynMeasure.Named (range=m) -> m
            | SynMeasure.Product (range=m) -> m
            | SynMeasure.Seq (range=m) -> m
            | SynMeasure.Divide (range=m) -> m
            | SynMeasure.Power (range=m) -> m
            | SynMeasure.One -> range0
            | SynMeasure.Anon (range=m) -> m
            | SynMeasure.Var (range=m) -> m

    type SynRationalConst with

        member this.PossibleRange =
            match this with
            | SynRationalConst.Integer _ -> range0
            | SynRationalConst.Rational (range=m) -> m
            | SynRationalConst.Negate rationalConst -> rationalConst.PossibleRange

    type SynConst with

        member this.PossibleRange =
            this.Range range0

    type SynArgInfo with

        member this.PossibleRange =
            match this with
            | SynArgInfo (attribs, _, idOpt) ->
                let ranges =
                    attribs
                    |> List.map (fun x -> x.Range)
                    |> List.append (match idOpt with | Some id -> [id.idRange] | _ -> [])

                match ranges with
                | [] -> range0
                | _ ->
                    ranges
                    |> List.reduce unionRanges

    type SynValInfo with

        member this.PossibleRange =
            match this with
            | SynValInfo (argInfos, argInfo) ->
                let ranges =
                    argInfos
                    |> List.reduce (@)
                    |> List.append [argInfo]
                    |> List.map (fun x -> x.PossibleRange)
                    |> List.filter (fun x -> not (isZeroRange x))

                match ranges with
                | [] -> range0
                | _ ->
                    ranges
                    |> List.reduce unionRanges

    type SynTypeDefnKind with

        member this.PossibleRange =
            match this with
            | TyconUnspecified
            | TyconClass
            | TyconInterface
            | TyconStruct
            | TyconRecord
            | TyconUnion
            | TyconAbbrev
            | TyconHiddenRepr
            | TyconAugmentation
            | TyconILAssemblyCode ->
                range0
            | TyconDelegate (ty, valInfo) ->
                let valInfoRange = valInfo.PossibleRange
                if isZeroRange valInfoRange then
                    ty.Range
                else
                    unionRanges ty.Range valInfoRange

    type SynTyparDecl with

        member this.Range =
            match this with
            | TyparDecl (attribs, typar) ->
                attribs
                |> List.map (fun x -> x.Range)
                |> List.append [typar.Range]
                |> List.reduce unionRanges

    type SynValTyparDecls with

        member this.PossibleRange =
            match this with
            | SynValTyparDecls (typarDecls, _, constraints) ->
                let ranges =
                    typarDecls
                    |> List.map (fun x -> x.Range)
                    |> List.append (constraints |> List.map (fun x -> x.Range))

                match ranges with
                | [] -> range0
                | _ ->
                    ranges
                    |> List.reduce unionRanges

type ParsingConfig =
    {
        tcConfig: TcConfig
        isLastFileOrScript: bool
        isExecutable: bool
        conditionalCompilationDefines: string list
        filePath: string
    }

[<RequireQualifiedAccess>]
type SourceValue =
    | SourceText of SourceText
    | Stream of Stream

[<RequireQualifiedAccess>]
module Parser =

    let Parse (pConfig: ParsingConfig) sourceValue =
        let tcConfig = pConfig.tcConfig
        let filePath = pConfig.filePath
        let errorLogger = CompilationErrorLogger("ParseFile", tcConfig.errorSeverityOptions)

        let input =
            match sourceValue with
            | SourceValue.SourceText sourceText ->
                ParseOneInputSourceText (pConfig.tcConfig, Lexhelp.LexResourceManager (), pConfig.conditionalCompilationDefines, filePath, sourceText.ToFSharpSourceText (), (pConfig.isLastFileOrScript, pConfig.isExecutable), errorLogger)
            | SourceValue.Stream stream ->
                ParseOneInputStream (pConfig.tcConfig, Lexhelp.LexResourceManager (), pConfig.conditionalCompilationDefines, filePath, stream, (pConfig.isLastFileOrScript, pConfig.isExecutable), errorLogger)
        (input, errorLogger.GetErrors ())

[<RequireQualifiedAccess>]
type SourceStorage =
    | SourceText of ITemporaryTextStorage 
    | Stream of ITemporaryStreamStorage

[<Sealed>]
type SourceSnapshot (filePath: string, sourceStorage: SourceStorage) =

    member __.FilePath = filePath

    member __.SourceStorage = sourceStorage

[<Sealed;AbstractClass;Extension>]
type ITemporaryStorageServiceExtensions =

    [<Extension>]
    static member CreateSourceSnapshot (this: ITemporaryStorageService, filePath: string, sourceText: SourceText) =
        cancellable {
            let! ct = Cancellable.token ()
            let storage = this.CreateTemporaryTextStorage ct
            storage.WriteText (sourceText, ct)
            let sourceStorage = SourceStorage.SourceText storage
            return SourceSnapshot (filePath, sourceStorage)
        }

    [<Extension>]
    static member CreateSourceSnapshot (this: ITemporaryStorageService, filePath: string) =
        cancellable {
            let! ct = Cancellable.token ()
            let storage = this.CreateTemporaryStreamStorage ct
            use stream = File.OpenRead filePath
            storage.WriteStream (stream, ct)
            let sourceStorage = SourceStorage.Stream storage
            return SourceSnapshot (filePath, sourceStorage)
        }

[<Sealed>]
type SyntaxTree (filePath: string, pConfig: ParsingConfig, sourceSnapshot: SourceSnapshot) =

    let asyncLazyWeakGetSourceTextFromSourceTextStorage =
        AsyncLazyWeak(async {
            let! ct = Async.CancellationToken
            match sourceSnapshot.SourceStorage with
            | SourceStorage.SourceText storage ->
                return storage.ReadText ct
            | _ ->
                return failwith "SyntaxTree Error: Expected SourceStorage.SourceText"
        })

    let asyncLazyWeakGetSourceText =
        AsyncLazyWeak(async {
            let! ct = Async.CancellationToken
            // If we already have a weakly cached source text as a result of calling GetParseResult, just use that value.
            match asyncLazyWeakGetSourceTextFromSourceTextStorage.TryGetValue () with
            | ValueSome sourceText ->
                return sourceText
            | _ ->
                match sourceSnapshot.SourceStorage with
                | SourceStorage.SourceText storage ->
                    return storage.ReadText ct
                | SourceStorage.Stream storage ->
                    use stream = storage.ReadStream ct
                    return SourceText.From stream
        })

    let asyncLazyWeakGetParseResult =
        AsyncLazyWeak(async {
            let! ct = Async.CancellationToken
            // If we already have a weakly cached source text as a result of calling GetSourceText, just use that value.
            match asyncLazyWeakGetSourceText.TryGetValue () with
            | ValueSome sourceText ->
                return Parser.Parse pConfig (SourceValue.SourceText sourceText)
            | _ ->
                match sourceSnapshot.SourceStorage with
                | SourceStorage.SourceText _ ->
                    let! sourceText = asyncLazyWeakGetSourceTextFromSourceTextStorage.GetValueAsync ()
                    return Parser.Parse pConfig (SourceValue.SourceText sourceText)
                | SourceStorage.Stream storage ->
                    let stream = storage.ReadStream ct
                    return Parser.Parse pConfig (SourceValue.Stream stream)
        })

    member __.FilePath = filePath

    member __.ParsingConfig = pConfig

    member __.GetParseResultAsync () =
        asyncLazyWeakGetParseResult.GetValueAsync ()

    member __.GetSourceTextAsync () =
        asyncLazyWeakGetSourceText.GetValueAsync ()

    member this.TryFindNodeAsync (line: int, column: int) =
        async {
            ()
            //match! this.GetParseResultAsync () with
            //| Some input, _ ->
            //    let mutable currentParent = None
            //    let setCurrentParent node f =
            //        let prev = currentParent
            //        currentParent <- node
            //        let result = f ()
            //        currentParent <- prev
            //        result
            //    return FSharp.Compiler.SourceCodeServices.AstTraversal.Traverse(Range.mkPos line column, input, { new FSharp.Compiler.SourceCodeServices.AstTraversal.AstVisitorBase<_>() with 
            //        member __.VisitExpr(_path, _traverseSynExpr, defaultTraverse, expr) =
            //            let node = Some (SyntaxNode (SyntaxNodeKind.Expr expr, currentParent))
            //            setCurrentParent node (fun () -> defaultTraverse expr)

            //        member __.VisitModuleDecl(defaultTraverse, decl) =
            //            let node = Some (SyntaxNode (SyntaxNodeKind.ModuleDecl decl, currentParent))
            //            setCurrentParent node (fun () -> defaultTraverse decl)

            //        member __.VisitBinding (_, binding) =
            //            Some (SyntaxNode (SyntaxNodeKind.Binding binding, currentParent))

            //        member __.VisitComponentInfo info =
            //            Some (SyntaxNode (SyntaxNodeKind.ComponentInfo info, currentParent))

            //        member __.VisitHashDirective m =
            //            Some (SyntaxNode (SyntaxNodeKind.HashDirective m, currentParent))

            //        member __.VisitImplicitInherit (defaultTraverse, ty, expr, m) =
            //            let node = Some (SyntaxNode (SyntaxNodeKind.ImplicitInherit (ty, expr, m), currentParent))
            //            setCurrentParent node (fun () -> defaultTraverse expr)

            //        member __.VisitInheritSynMemberDefn(info, typeDefnKind, synType, members, m) =
            //            Some (SyntaxNode (SyntaxNodeKind.InheritSynMemberDefn (info, typeDefnKind, synType, members, m), currentParent))

            //        member __.VisitInterfaceSynMemberDefnType synType =
            //            Some (SyntaxNode (SyntaxNodeKind.InterfaceSynMemberDefnType synType, currentParent))

            //        member __.VisitLetOrUse (_, defaultTraverse, bindings, m) =
            //            let node = Some (SyntaxNode (SyntaxNodeKind.LetOrUse (bindings, m), currentParent))
            //            bindings
            //            |> List.tryPick (fun binding ->
            //                setCurrentParent node (fun () -> defaultTraverse binding)
            //            )

            //        member __.VisitMatchClause (_, matchClause) =
            //            Some (SyntaxNode (SyntaxNodeKind.MatchClause matchClause, currentParent))

            //        member __.VisitModuleOrNamespace moduleOrNamespace =
            //            Some (SyntaxNode (SyntaxNodeKind.ModuleOrNamespace moduleOrNamespace, currentParent))

            //        member __.VisitPat (defaultTraverse, pat) =
            //            let node = Some (SyntaxNode (SyntaxNodeKind.Pat pat, currentParent))
            //            setCurrentParent node (fun () -> defaultTraverse pat)

            //        member __.VisitRecordField (_, copyOpt, recordFieldOpt) =
            //            Some (SyntaxNode (SyntaxNodeKind.RecordField (copyOpt, recordFieldOpt), currentParent))

            //        member __.VisitSimplePats simplePats =
            //            Some (SyntaxNode (SyntaxNodeKind.SimplePats simplePats, currentParent))

            //        member this.VisitType (defaultTraverse, ty) =
            //            let node = Some (SyntaxNode (SyntaxNodeKind.Type ty, currentParent))
            //            setCurrentParent node (fun () -> defaultTraverse ty)

            //        member __.VisitTypeAbbrev (ty, m) =
            //            Some (SyntaxNode (SyntaxNodeKind.TypeAbbrev (ty, m), currentParent))
            //    })
            //| _ ->
            //    return None
        }

    //member this.GetTokensAsync (line: int) =
    //    if line <= 0 then
    //        invalidArg "line" "Specified line is less than or equal to 0. F# uses line counting starting at 1."
    //    async {
    //        let! sourceText = this.GetSourceTextAsync ()
    //        let tokenizer = FSharpSourceTokenizer (pConfig.conditionalCompilationDefines, Some filePath)
    //        let lines = sourceText.Lines
    //        if line > lines.Count then
    //            invalidArg "line" "Specified line does not exist in source."
    //        let lineTokenizer = tokenizer.CreateLineTokenizer (sourceText.Lines.[line - 1].Text.ToString())

    //        let tokens = ImmutableArray.CreateBuilder ()

    //        let mutable state = FSharpTokenizerLexState.Initial
    //        let mutable canStop = false
    //        while not canStop do
    //            let infoOpt, newState = lineTokenizer.ScanToken (FSharpTokenizerLexState.Initial)
    //            state <- newState
    //            match infoOpt with
    //            | Some info ->
    //                tokens.Add info
    //            | _ ->
    //                canStop <- true

    //        return tokens.ToImmutable ()
    //    }

    //member this.TryGetToken (line: int, column: int) =
    //    if line <= 0 then
    //        invalidArg "line" "Specified line is less than or equal to zero. F# uses line counting starting at 1."
    //    if column < 0 then
    //        invalidArg "column" "Specified column is less than zero."
    //    async {
    //        let! tokens = this.GetTokensAsync line
    //        if tokens.Length > 0 then
    //            return
    //                tokens
    //                |> Seq.tryFind (fun x -> column >= x.LeftColumn && column <= x.RightColumn)
    //        else
    //            return None
    //    }
        
