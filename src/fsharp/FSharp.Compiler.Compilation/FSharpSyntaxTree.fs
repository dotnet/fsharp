namespace FSharp.Compiler.Compilation

open System.IO
open System.Threading
open System.Collections.Generic
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
module Lexer =

    open FSharp.Compiler.Lexhelp

    let Lex (pConfig: ParsingConfig) sourceValue tokenCallback =
        let skipWhitespace = true
        let tcConfig = pConfig.tcConfig
        let filePath = pConfig.filePath
        let errorLogger = CompilationErrorLogger("Lex", tcConfig.errorSeverityOptions)

        let lightSyntaxStatus = LightSyntaxStatus (tcConfig.ComputeLightSyntaxInitialStatus filePath, true) 
        let conditionalCompilationDefines = pConfig.conditionalCompilationDefines
        let lexargs = mkLexargs (filePath, conditionalCompilationDefines@tcConfig.conditionalCompilationDefines, lightSyntaxStatus, Lexhelp.LexResourceManager (), ref [], errorLogger, tcConfig.pathMap)

        match sourceValue with
        | SourceValue.SourceText sourceText ->
            let lexbuf = UnicodeLexing.SourceTextAsLexbuf (sourceText.ToFSharpSourceText ())
            usingLexbufForParsing (lexbuf, filePath) (fun lexbuf ->
                while not lexbuf.IsPastEndOfStream do
                    tokenCallback (Lexer.token lexargs skipWhitespace lexbuf, lexbuf.LexemeRange)
            )
        | SourceValue.Stream stream ->
            let streamReader = new StreamReader(stream) // don't dispose of stream reader
            let lexbuf = 
                UnicodeLexing.FunctionAsLexbuf (fun (chars, start, length) ->
                    streamReader.ReadBlock (chars, start, length)
                )
            usingLexbufForParsing (lexbuf, filePath) (fun lexbuf ->
                while not lexbuf.IsPastEndOfStream do
                    tokenCallback (Lexer.token lexargs skipWhitespace lexbuf, lexbuf.LexemeRange)
            )

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

[<NoEquality;NoComparison;RequireQualifiedAccess>]
type FSharpSyntaxNodeKind =
    | ParsedInput of ParsedInput
    | ModuleOrNamespace of SynModuleOrNamespace
    | ModuleDecl of SynModuleDecl
    | LongIdentWithDots of LongIdentWithDots
    | Ident of Ident
    | ComponentInfo of SynComponentInfo
    | TypeConstraint of SynTypeConstraint
    | MemberSig of SynMemberSig
    | TypeDefnSig of SynTypeDefnSig
    | TypeDefnSigRepr of SynTypeDefnSigRepr
    | ExceptionDefnRepr of SynExceptionDefnRepr
    | UnionCase of SynUnionCase
    | UnionCaseType of SynUnionCaseType
    | ArgInfo of SynArgInfo
    | TypeDefnSimpleRepr of SynTypeDefnSimpleRepr
    | SimplePat of SynSimplePat
    | EnumCase of SynEnumCase
    | Const of SynConst
    | Measure of SynMeasure
    | RationalConst of SynRationalConst
    | TypeDefnKind of SynTypeDefnKind
    | Field of SynField
    | ValSig of SynValSig
    | ValTyparDecls of SynValTyparDecls
    | Type of SynType
    | SimplePats of SynSimplePats
    | Typar of SynTypar
    | TyparDecl of SynTyparDecl
    | Binding of SynBinding
    | ValData of SynValData
    | ValInfo of SynValInfo
    | Pat of SynPat
    | ConstructorArgs of SynConstructorArgs
    | BindingReturnInfo of SynBindingReturnInfo
    | Expr of SynExpr
    | StaticOptimizationConstraint of SynStaticOptimizationConstraint
    | IndexerArg of SynIndexerArg
    | SimplePatAlternativeIdInfo of SynSimplePatAlternativeIdInfo
    | MatchClause of SynMatchClause
    | InterfaceImpl of SynInterfaceImpl
    | TypeDefn of SynTypeDefn
    | TypeDefnRepr of SynTypeDefnRepr
    | MemberDefn of SynMemberDefn
    | ExceptionDefn of SynExceptionDefn
    | ParsedHashDirective of ParsedHashDirective
    | AttributeList of SynAttributeList
    | Attribute of SynAttribute

[<AbstractClass>]
type FSharpSyntaxVisitor (syntaxTree: FSharpSyntaxTree) as this =
    inherit AstVisitor<FSharpSyntaxNode> ()

    let visitStack = Stack<FSharpSyntaxNode> ()

    let startVisit node =
        visitStack.Push node

    let endVisit resultOpt =
        let visitedNode = visitStack.Pop ()
        this.OnVisit (visitedNode, resultOpt)

    let createNode kind =
        let parent =
            if visitStack.Count > 0 then
                Some (visitStack.Peek ())
            else
                None
        FSharpSyntaxNode (parent, syntaxTree, kind)

    abstract OnVisit: visitedNode: FSharpSyntaxNode * resultOpt: FSharpSyntaxNode option -> FSharpSyntaxNode option
    default __.OnVisit (_visitedNode: FSharpSyntaxNode, resultOpt: FSharpSyntaxNode option) =
        resultOpt

    member this.VisitNode (node: FSharpSyntaxNode) =
        node.Parent |> Option.iter visitStack.Push
        match node.Kind with
        | FSharpSyntaxNodeKind.ParsedInput item ->
            this.VisitParsedInput item
        | FSharpSyntaxNodeKind.ModuleOrNamespace item ->
            this.VisitModuleOrNamespace item
        | FSharpSyntaxNodeKind.ModuleDecl item ->
            this.VisitModuleDecl item
        | FSharpSyntaxNodeKind.LongIdentWithDots item ->
            this.VisitLongIdentWithDots item
        | FSharpSyntaxNodeKind.Ident item ->
            this.VisitIdent item
        | FSharpSyntaxNodeKind.ComponentInfo item ->
            this.VisitComponentInfo item
        | FSharpSyntaxNodeKind.TypeConstraint item ->
            this.VisitTypeConstraint item
        | FSharpSyntaxNodeKind.MemberSig item ->
            this.VisitMemberSig item
        | FSharpSyntaxNodeKind.TypeDefnSig item ->
            this.VisitTypeDefnSig item
        | FSharpSyntaxNodeKind.TypeDefnSigRepr item ->
            this.VisitTypeDefnSigRepr item
        | FSharpSyntaxNodeKind.ExceptionDefnRepr item ->
            this.VisitExceptionDefnRepr item
        | FSharpSyntaxNodeKind.UnionCase item ->
            this.VisitUnionCase item
        | FSharpSyntaxNodeKind.UnionCaseType item ->
            this.VisitUnionCaseType item
        | FSharpSyntaxNodeKind.ArgInfo item ->
            this.VisitArgInfo item
        | FSharpSyntaxNodeKind.TypeDefnSimpleRepr item ->
            this.VisitTypeDefnSimpleRepr item
        | FSharpSyntaxNodeKind.SimplePat item ->
            this.VisitSimplePat item
        | FSharpSyntaxNodeKind.EnumCase item ->
            this.VisitEnumCase item
        | FSharpSyntaxNodeKind.Const item ->
            this.VisitConst item
        | FSharpSyntaxNodeKind.Measure item ->
            this.VisitMeasure item
        | FSharpSyntaxNodeKind.RationalConst item ->
            this.VisitRationalConst item
        | FSharpSyntaxNodeKind.TypeDefnKind item ->
            this.VisitTypeDefnKind item
        | FSharpSyntaxNodeKind.Field item ->
            this.VisitField item
        | FSharpSyntaxNodeKind.ValSig item ->
            this.VisitValSig item
        | FSharpSyntaxNodeKind.ValTyparDecls item ->
            this.VisitValTyparDecls item
        | FSharpSyntaxNodeKind.Type item ->
            this.VisitType item
        | FSharpSyntaxNodeKind.SimplePats item ->
            this.VisitSimplePats item
        | FSharpSyntaxNodeKind.Typar item ->
            this.VisitTypar item
        | FSharpSyntaxNodeKind.TyparDecl item ->
            this.VisitTyparDecl item
        | FSharpSyntaxNodeKind.Binding item ->
            this.VisitBinding item
        | FSharpSyntaxNodeKind.ValData item ->
            this.VisitValData item
        | FSharpSyntaxNodeKind.ValInfo item ->
            this.VisitValInfo item
        | FSharpSyntaxNodeKind.Pat item ->
            this.VisitPat item
        | FSharpSyntaxNodeKind.ConstructorArgs item ->
            this.VisitConstructorArgs item
        | FSharpSyntaxNodeKind.BindingReturnInfo item ->
            this.VisitBindingReturnInfo item
        | FSharpSyntaxNodeKind.Expr item ->
            this.VisitExpr item
        | FSharpSyntaxNodeKind.StaticOptimizationConstraint item ->
            this.VisitStaticOptimizationConstraint item
        | FSharpSyntaxNodeKind.IndexerArg item ->
            this.VisitIndexerArg item
        | FSharpSyntaxNodeKind.SimplePatAlternativeIdInfo item ->
            this.VisitSimplePatAlternativeIdInfo item
        | FSharpSyntaxNodeKind.MatchClause item ->
            this.VisitMatchClause item
        | FSharpSyntaxNodeKind.InterfaceImpl item ->
            this.VisitInterfaceImpl item
        | FSharpSyntaxNodeKind.TypeDefn item ->
            this.VisitTypeDefn item
        | FSharpSyntaxNodeKind.TypeDefnRepr item ->
            this.VisitTypeDefnRepr item
        | FSharpSyntaxNodeKind.MemberDefn item ->
            this.VisitMemberDefn item
        | FSharpSyntaxNodeKind.ExceptionDefn item ->
            this.VisitExceptionDefn item
        | FSharpSyntaxNodeKind.ParsedHashDirective item ->
            this.VisitParsedHashDirective item
        | FSharpSyntaxNodeKind.AttributeList item ->
            this.VisitAttributeList item
        | FSharpSyntaxNodeKind.Attribute item ->
            this.VisitAttribute item

    override __.VisitParsedInput item =
        let node = createNode (FSharpSyntaxNodeKind.ParsedInput item)
        startVisit node
        let resultOpt = base.VisitParsedInput item
        endVisit resultOpt

    override __.VisitModuleOrNamespace item =
        let node = createNode (FSharpSyntaxNodeKind.ModuleOrNamespace item)
        startVisit node
        let resultOpt = base.VisitModuleOrNamespace item
        endVisit resultOpt

    override __.VisitModuleDecl item =
        let node = createNode (FSharpSyntaxNodeKind.ModuleDecl item)
        startVisit node
        let resultOpt = base.VisitModuleDecl item
        endVisit resultOpt

    override __.VisitLongIdentWithDots item =
        let node = createNode (FSharpSyntaxNodeKind.LongIdentWithDots item)
        visit node (fun () -> base.VisitLongIdentWithDots item)

    override __.VisitIdent item =
        let node = createNode (FSharpSyntaxNodeKind.Ident item)
        visit node (fun () -> base.VisitIdent item)

    override __.VisitComponentInfo item =
        let node = createNode (FSharpSyntaxNodeKind.ComponentInfo item)
        visit node (fun () -> base.VisitComponentInfo item)

    override __.VisitTypeConstraint item =
        let node = createNode (FSharpSyntaxNodeKind.TypeConstraint item)
        visit node (fun () -> base.VisitTypeConstraint item)

    override __.VisitMemberSig item =
        let node = createNode (FSharpSyntaxNodeKind.MemberSig item)
        visit node (fun () -> base.VisitMemberSig item)

    override __.VisitTypeDefnSig item =
        let node = createNode (FSharpSyntaxNodeKind.TypeDefnSig item)
        visit node (fun () -> base.VisitTypeDefnSig item)

    override __.VisitTypeDefnSigRepr item =
        let node = createNode (FSharpSyntaxNodeKind.TypeDefnSigRepr item)
        visit node (fun () -> base.VisitTypeDefnSigRepr item)

    override __.VisitExceptionDefnRepr item =
        let node = createNode (FSharpSyntaxNodeKind.ExceptionDefnRepr item)
        visit node (fun () -> base.VisitExceptionDefnRepr item)

    override __.VisitUnionCase item =
        let node = createNode (FSharpSyntaxNodeKind.UnionCase item)
        visit node (fun () -> base.VisitUnionCase item)

    override __.VisitUnionCaseType item =
        let node = createNode (FSharpSyntaxNodeKind.UnionCaseType item)
        visit node (fun () -> base.VisitUnionCaseType item)

    override __.VisitArgInfo item =
        let node = createNode (FSharpSyntaxNodeKind.ArgInfo item)
        visit node (fun () -> base.VisitArgInfo item)

    override __.VisitTypeDefnSimpleRepr item =
        let node = createNode (FSharpSyntaxNodeKind.TypeDefnSimpleRepr item)
        visit node (fun () -> base.VisitTypeDefnSimpleRepr item)

    override __.VisitSimplePat item =
        let node = createNode (FSharpSyntaxNodeKind.SimplePat item)
        visit node (fun () -> base.VisitSimplePat item)

    override __.VisitEnumCase item =
        let node = createNode (FSharpSyntaxNodeKind.EnumCase item)
        visit node (fun () -> base.VisitEnumCase item)

    override __.VisitConst item =
        let node = createNode (FSharpSyntaxNodeKind.Const item)
        visit node (fun () -> base.VisitConst item)

    override __.VisitMeasure item =
        let node = createNode (FSharpSyntaxNodeKind.Measure item)
        visit node (fun () -> base.VisitMeasure item)

    override __.VisitRationalConst item =
        let node = createNode (FSharpSyntaxNodeKind.RationalConst item)
        visit node (fun () -> base.VisitRationalConst item)

    override __.VisitTypeDefnKind item =
        let node = createNode (FSharpSyntaxNodeKind.TypeDefnKind item)
        visit node (fun () -> base.VisitTypeDefnKind item)

    override __.VisitField item =
        let node = createNode (FSharpSyntaxNodeKind.Field item)
        visit node (fun () -> base.VisitField item)

    override __.VisitValSig item =
        let node = createNode (FSharpSyntaxNodeKind.ValSig item)
        visit node (fun () -> base.VisitValSig item)

    override __.VisitValTyparDecls item =
        let node = createNode (FSharpSyntaxNodeKind.ValTyparDecls item)
        visit node (fun () -> base.VisitValTyparDecls item)

    override __.VisitType item =
        let node = createNode (FSharpSyntaxNodeKind.Type item)
        visit node (fun () -> base.VisitType item)

    override __.VisitSimplePats item =
        let node = createNode (FSharpSyntaxNodeKind.SimplePats item)
        visit node (fun () -> base.VisitSimplePats item)

    override __.VisitTypar item =
        let node = createNode (FSharpSyntaxNodeKind.Typar item)
        visit node (fun () -> base.VisitTypar item)

    override __.VisitTyparDecl item =
        let node = createNode (FSharpSyntaxNodeKind.TyparDecl item)
        visit node (fun () -> base.VisitTyparDecl item)

    override __.VisitBinding item =
        let node = createNode (FSharpSyntaxNodeKind.Binding item)
        visit node (fun () -> base.VisitBinding item)

    override __.VisitValData item =
        let node = createNode (FSharpSyntaxNodeKind.ValData item)
        visit node (fun () -> base.VisitValData item)

    override __.VisitValInfo item =
        let node = createNode (FSharpSyntaxNodeKind.ValInfo item)
        visit node (fun () -> base.VisitValInfo item)

    override __.VisitPat item =
        let node = createNode (FSharpSyntaxNodeKind.Pat item)
        visit node (fun () -> base.VisitPat item)

    override __.VisitConstructorArgs item =
        let node = createNode (FSharpSyntaxNodeKind.ConstructorArgs item)
        visit node (fun () -> base.VisitConstructorArgs item)

    override __.VisitBindingReturnInfo item =
        let node = createNode (FSharpSyntaxNodeKind.BindingReturnInfo item)
        visit node (fun () -> base.VisitBindingReturnInfo item)

    override __.VisitExpr item =
        let node = createNode (FSharpSyntaxNodeKind.Expr item)
        visit node (fun () -> base.VisitExpr item)

    override __.VisitStaticOptimizationConstraint item =
        let node = createNode (FSharpSyntaxNodeKind.StaticOptimizationConstraint item)
        visit node (fun () -> base.VisitStaticOptimizationConstraint item)

    override __.VisitIndexerArg item =
        let node = createNode (FSharpSyntaxNodeKind.IndexerArg item)
        visit node (fun () -> base.VisitIndexerArg item)

    override __.VisitSimplePatAlternativeIdInfo item =
        let node = createNode (FSharpSyntaxNodeKind.SimplePatAlternativeIdInfo item)
        visit node (fun () -> base.VisitSimplePatAlternativeIdInfo item)

    override __.VisitMatchClause item =
        let node = createNode (FSharpSyntaxNodeKind.MatchClause item)
        visit node (fun () -> base.VisitMatchClause item)

    override __.VisitInterfaceImpl item =
        let node = createNode (FSharpSyntaxNodeKind.InterfaceImpl item)
        visit node (fun () -> base.VisitInterfaceImpl item)

    override __.VisitTypeDefn item =
        let node = createNode (FSharpSyntaxNodeKind.TypeDefn item)
        visit node (fun () -> base.VisitTypeDefn item)

    override __.VisitTypeDefnRepr item =
        let node = createNode (FSharpSyntaxNodeKind.TypeDefnRepr item)
        visit node (fun () -> base.VisitTypeDefnRepr item)

    override __.VisitMemberDefn item =
        let node = createNode (FSharpSyntaxNodeKind.MemberDefn item)
        visit node (fun () -> base.VisitMemberDefn item)

    override __.VisitExceptionDefn item =
        let node = createNode (FSharpSyntaxNodeKind.ExceptionDefn item)
        visit node (fun () -> base.VisitExceptionDefn item)

    override __.VisitParsedHashDirective item =
        let node = createNode (FSharpSyntaxNodeKind.ParsedHashDirective item)
        visit node (fun () -> base.VisitParsedHashDirective item)

    override __.VisitAttributeList item =
        let node = createNode (FSharpSyntaxNodeKind.AttributeList item)
        visit node (fun () -> base.VisitAttributeList item)

    override __.VisitAttribute item =
        let node = createNode (FSharpSyntaxNodeKind.Attribute item)
        visit node (fun () -> base.VisitAttribute item)

and [<Sealed>] FSharpSyntaxFinder (findm: range, syntaxTree) =
    inherit FSharpSyntaxVisitor (syntaxTree)

    let isZeroRange (r: range) =
        posEq r.Start r.End

    override __.CanVisit m = rangeContainsRange m findm && not (isZeroRange m)

    override __.OnVisit (visitedNode, resultOpt) =
        match resultOpt with
        | Some _ -> resultOpt
        | _ -> Some visitedNode

and [<Sealed>] FSharpSyntaxToken (parent: FSharpSyntaxNode, token: Parser.token, range: range) =

    member __.Parent = parent

    member __.Token = token

    member __.Range = range

and [<Sealed>] FSharpSyntaxNode (parent: FSharpSyntaxNode option, syntaxTree: FSharpSyntaxTree, kind: FSharpSyntaxNodeKind) =

    member __.Parent = parent
    
    member __.Kind = kind

and [<Sealed>] FSharpSyntaxTree (filePath: string, pConfig: ParsingConfig, sourceSnapshot: FSharpSourceSnapshot) =

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

    let asyncLazyWeakGetTokens =
        AsyncLazyWeak(async {
            let! ct = Async.CancellationToken
            // If we already have a weakly cached source text as a result of calling GetSourceText, just use that value.
            match asyncLazyWeakGetSourceText.TryGetValue () with
            | ValueSome sourceText ->
                let tokens = ImmutableArray.CreateBuilder ()
                Lexer.Lex pConfig (SourceValue.SourceText sourceText) tokens.Add
                return tokens.ToImmutable () |> ref
            | _ ->
                match sourceSnapshot.SourceStorage with
                | SourceStorage.SourceText _ ->
                    let! sourceText = asyncLazyWeakGetSourceTextFromSourceTextStorage.GetValueAsync ()
                    let tokens = ImmutableArray.CreateBuilder ()
                    Lexer.Lex pConfig (SourceValue.SourceText sourceText) tokens.Add
                    return tokens.ToImmutable () |> ref
                | SourceStorage.Stream storage ->
                    let stream = storage.ReadStream ct
                    let tokens = ImmutableArray.CreateBuilder ()
                    Lexer.Lex pConfig (SourceValue.Stream stream) tokens.Add
                    return tokens.ToImmutable () |> ref
        })

    member __.FilePath = filePath

    member __.ParsingConfig = pConfig

    member __.GetParseResultAsync () =
        asyncLazyWeakGetParseResult.GetValueAsync ()

    member __.GetSourceTextAsync () =
        asyncLazyWeakGetSourceText.GetValueAsync ()

    member this.TryFindTokenAsync (line: int, column: int) =
        async {
            let! inputOpt, _ = asyncLazyWeakGetParseResult.GetValueAsync ()

            match inputOpt with
            | Some input ->
                let rootNode = FSharpSyntaxNode (None, this, FSharpSyntaxNodeKind.ParsedInput input)
                let! tokens = asyncLazyWeakGetTokens.GetValueAsync ()
                let tokens = !tokens
                let p = mkPos line column
                return
                    tokens
                    |> Seq.tryPick (fun (t, m) ->
                        if rangeContainsPos m p then
                            let finder = FSharpSyntaxFinder (m, this)
                            let parent = finder.VisitNode rootNode
                            if parent.IsNone then failwith "not possible"
                            Some (FSharpSyntaxToken (parent.Value, t, m))
                        else
                            None
                    )
            | _ ->
                return None
        }

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
        
