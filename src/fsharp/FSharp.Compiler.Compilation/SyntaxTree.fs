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

[<RequireQualifiedAccess; NoEquality; NoComparison>]
type SyntaxNodeKind =
    | Expr of SynExpr
    | ModuleDecl of SynModuleDecl
    | Binding of SynBinding
    | ComponentInfo of SynComponentInfo
    | HashDirective of Range.range
    | ImplicitInherit of SynType * SynExpr * Range.range
    | InheritSynMemberDefn of SynComponentInfo * SynTypeDefnKind * SynType * SynMemberDefns * Range.range
    | InterfaceSynMemberDefnType of SynType
    | LetOrUse of SynBinding list * Range.range
    | MatchClause of SynMatchClause
    | ModuleOrNamespace of SynModuleOrNamespace
    | Pat of SynPat
    | RecordField of SynExpr option * LongIdentWithDots option
    | SimplePats of SynSimplePat list
    | Type of SynType
    | TypeAbbrev of SynType * Range.range

[<Sealed>]
type SyntaxNode (kind, parentOpt: SyntaxNode option) =
    
    member __.Kind = kind

    member __.Parent = parentOpt

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
                let setCurrentParent node =
                    currentParent <- node
                    currentParent
                return FSharp.Compiler.SourceCodeServices.AstTraversal.Traverse(Range.mkPos line column, input, { new FSharp.Compiler.SourceCodeServices.AstTraversal.AstVisitorBase<_>() with 
                    member __.VisitExpr(_path, _traverseSynExpr, _defaultTraverse, expr) =
                        Some (SyntaxNode (SyntaxNodeKind.Expr expr, currentParent))
                        |> setCurrentParent

                    member __.VisitModuleDecl(_defaultTraverse, decl) =
                        Some (SyntaxNode (SyntaxNodeKind.ModuleDecl decl, currentParent))
                        |> setCurrentParent

                    member __.VisitBinding (_, binding) =
                        Some (SyntaxNodeKind.Binding binding |> SyntaxNode)

                    member __.VisitComponentInfo info =
                        Some (SyntaxNodeKind.ComponentInfo info |> SyntaxNode)

                    member __.VisitHashDirective m =
                        Some (SyntaxNodeKind.HashDirective m |> SyntaxNode)

                    member __.VisitImplicitInherit (_, ty, expr, m) =
                        Some (SyntaxNodeKind.ImplicitInherit (ty, expr, m) |> SyntaxNode)

                    member __.VisitInheritSynMemberDefn(info, typeDefnKind, synType, members, m) =
                        Some (SyntaxNodeKind.InheritSynMemberDefn (info, typeDefnKind, synType, members, m) |> SyntaxNode)

                    member __.VisitInterfaceSynMemberDefnType synType =
                        Some (SyntaxNodeKind.InterfaceSynMemberDefnType synType |> SyntaxNode)

                    member __.VisitLetOrUse (_, _, bindings, m) =
                        Some (SyntaxNodeKind.LetOrUse (bindings, m) |> SyntaxNode)

                    member __.VisitMatchClause (_, matchClause) =
                        Some (SyntaxNodeKind.MatchClause matchClause |> SyntaxNode)

                    member __.VisitModuleOrNamespace moduleOrNamespace =
                        Some (SyntaxNodeKind.ModuleOrNamespace moduleOrNamespace |> SyntaxNode)

                    member __.VisitPat (_, pat) =
                        Some (SyntaxNodeKind.Pat pat |> SyntaxNode)

                    member __.VisitRecordField (_, copyOpt, recordFieldOpt) =
                        Some (SyntaxNodeKind.RecordField (copyOpt, recordFieldOpt) |> SyntaxNode)

                    member __.VisitSimplePats simplePats =
                        Some (SyntaxNodeKind.SimplePats simplePats |> SyntaxNode)

                    member this.VisitType (_, ty) =
                        Some (SyntaxNodeKind.Type ty |> SyntaxNode)

                    member __.VisitTypeAbbrev (ty, m) =
                        Some (SyntaxNodeKind.TypeAbbrev (ty, m) |> SyntaxNode)
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
        
