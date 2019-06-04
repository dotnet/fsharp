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
            match! this.GetParseResultAsync () with
            | Some input, _ ->
                let mutable currentParent = None
                let setCurrentParent node f =
                    let prev = currentParent
                    currentParent <- node
                    let result = f ()
                    currentParent <- prev
                    result
                return FSharp.Compiler.SourceCodeServices.AstTraversal.Traverse(Range.mkPos line column, input, { new FSharp.Compiler.SourceCodeServices.AstTraversal.AstVisitorBase<_>() with 
                    member __.VisitExpr(_path, _traverseSynExpr, defaultTraverse, expr) =
                        let node = Some (SyntaxNode (SyntaxNodeKind.Expr expr, currentParent))
                        setCurrentParent node (fun () -> defaultTraverse expr)

                    member __.VisitModuleDecl(defaultTraverse, decl) =
                        let node = Some (SyntaxNode (SyntaxNodeKind.ModuleDecl decl, currentParent))
                        setCurrentParent node (fun () -> defaultTraverse decl)

                    member __.VisitBinding (_, binding) =
                        Some (SyntaxNode (SyntaxNodeKind.Binding binding, currentParent))

                    member __.VisitComponentInfo info =
                        Some (SyntaxNode (SyntaxNodeKind.ComponentInfo info, currentParent))

                    member __.VisitHashDirective m =
                        Some (SyntaxNode (SyntaxNodeKind.HashDirective m, currentParent))

                    member __.VisitImplicitInherit (defaultTraverse, ty, expr, m) =
                        let node = Some (SyntaxNode (SyntaxNodeKind.ImplicitInherit (ty, expr, m), currentParent))
                        setCurrentParent node (fun () -> defaultTraverse expr)

                    member __.VisitInheritSynMemberDefn(info, typeDefnKind, synType, members, m) =
                        Some (SyntaxNode (SyntaxNodeKind.InheritSynMemberDefn (info, typeDefnKind, synType, members, m), currentParent))

                    member __.VisitInterfaceSynMemberDefnType synType =
                        Some (SyntaxNode (SyntaxNodeKind.InterfaceSynMemberDefnType synType, currentParent))

                    member __.VisitLetOrUse (_, defaultTraverse, bindings, m) =
                        let node = Some (SyntaxNode (SyntaxNodeKind.LetOrUse (bindings, m), currentParent))
                        bindings
                        |> List.tryPick (fun binding ->
                            setCurrentParent node (fun () -> defaultTraverse binding)
                        )

                    member __.VisitMatchClause (_, matchClause) =
                        Some (SyntaxNode (SyntaxNodeKind.MatchClause matchClause, currentParent))

                    member __.VisitModuleOrNamespace moduleOrNamespace =
                        Some (SyntaxNode (SyntaxNodeKind.ModuleOrNamespace moduleOrNamespace, currentParent))

                    member __.VisitPat (defaultTraverse, pat) =
                        let node = Some (SyntaxNode (SyntaxNodeKind.Pat pat, currentParent))
                        setCurrentParent node (fun () -> defaultTraverse pat)

                    member __.VisitRecordField (_, copyOpt, recordFieldOpt) =
                        Some (SyntaxNode (SyntaxNodeKind.RecordField (copyOpt, recordFieldOpt), currentParent))

                    member __.VisitSimplePats simplePats =
                        Some (SyntaxNode (SyntaxNodeKind.SimplePats simplePats, currentParent))

                    member this.VisitType (defaultTraverse, ty) =
                        let node = Some (SyntaxNode (SyntaxNodeKind.Type ty, currentParent))
                        setCurrentParent node (fun () -> defaultTraverse ty)

                    member __.VisitTypeAbbrev (ty, m) =
                        Some (SyntaxNode (SyntaxNodeKind.TypeAbbrev (ty, m), currentParent))
                })
            | _ ->
                return None
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
        
