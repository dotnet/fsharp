﻿namespace FSharp.Compiler.Compilation

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

    member this.Range =
        match this with
        | FSharpSyntaxNodeKind.ParsedInput item ->
            item.Range
        | FSharpSyntaxNodeKind.ModuleOrNamespace item ->
            item.Range
        | FSharpSyntaxNodeKind.ModuleDecl item ->
            item.Range
        | FSharpSyntaxNodeKind.LongIdentWithDots item ->
            item.Range
        | FSharpSyntaxNodeKind.Ident item ->
            item.idRange
        | FSharpSyntaxNodeKind.ComponentInfo item ->
            item.Range
        | FSharpSyntaxNodeKind.TypeConstraint item ->
            item.Range
        | FSharpSyntaxNodeKind.MemberSig item ->
            item.Range
        | FSharpSyntaxNodeKind.TypeDefnSig item ->
            item.Range
        | FSharpSyntaxNodeKind.TypeDefnSigRepr item ->
            item.Range
        | FSharpSyntaxNodeKind.ExceptionDefnRepr item ->
            item.Range
        | FSharpSyntaxNodeKind.UnionCase item ->
            item.Range
        | FSharpSyntaxNodeKind.UnionCaseType item ->
            item.PossibleRange
        | FSharpSyntaxNodeKind.ArgInfo item ->
            item.PossibleRange
        | FSharpSyntaxNodeKind.TypeDefnSimpleRepr item ->
            item.Range
        | FSharpSyntaxNodeKind.SimplePat item ->
            item.Range
        | FSharpSyntaxNodeKind.EnumCase item ->
            item.Range
        | FSharpSyntaxNodeKind.Const item ->
            item.PossibleRange
        | FSharpSyntaxNodeKind.Measure item ->
            item.PossibleRange
        | FSharpSyntaxNodeKind.RationalConst item ->
            item.PossibleRange
        | FSharpSyntaxNodeKind.TypeDefnKind item ->
            item.PossibleRange
        | FSharpSyntaxNodeKind.Field item ->
            item.Range
        | FSharpSyntaxNodeKind.ValSig item ->
            item.Range
        | FSharpSyntaxNodeKind.ValTyparDecls item ->
            item.PossibleRange
        | FSharpSyntaxNodeKind.Type item ->
            item.Range
        | FSharpSyntaxNodeKind.SimplePats item ->
            item.Range
        | FSharpSyntaxNodeKind.Typar item ->
            item.Range
        | FSharpSyntaxNodeKind.TyparDecl item ->
            item.Range
        | FSharpSyntaxNodeKind.Binding item ->
            item.RangeOfBindingAndRhs
        | FSharpSyntaxNodeKind.ValData item ->
            item.Range
        | FSharpSyntaxNodeKind.ValInfo item ->
            item.PossibleRange
        | FSharpSyntaxNodeKind.Pat item ->
            item.Range
        | FSharpSyntaxNodeKind.ConstructorArgs item ->
            item.PossibleRange
        | FSharpSyntaxNodeKind.BindingReturnInfo item ->
            item.Range
        | FSharpSyntaxNodeKind.Expr item ->
            item.Range
        | FSharpSyntaxNodeKind.StaticOptimizationConstraint item ->
            item.Range
        | FSharpSyntaxNodeKind.IndexerArg item ->
            item.Range
        | FSharpSyntaxNodeKind.SimplePatAlternativeIdInfo item ->
            item.Range
        | FSharpSyntaxNodeKind.MatchClause item ->
            item.Range
        | FSharpSyntaxNodeKind.InterfaceImpl item ->
            item.Range
        | FSharpSyntaxNodeKind.TypeDefn item ->
            item.Range
        | FSharpSyntaxNodeKind.TypeDefnRepr item ->
            item.Range
        | FSharpSyntaxNodeKind.MemberDefn item ->
            item.Range
        | FSharpSyntaxNodeKind.ExceptionDefn item ->
            item.Range
        | FSharpSyntaxNodeKind.ParsedHashDirective item ->
            item.Range
        | FSharpSyntaxNodeKind.AttributeList item ->
            item.Range
        | FSharpSyntaxNodeKind.Attribute item ->
            item.Range

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
        startVisit node
        let resultOpt = base.VisitLongIdentWithDots item
        endVisit resultOpt

    override __.VisitIdent item =
        let node = createNode (FSharpSyntaxNodeKind.Ident item)
        startVisit node
        let resultOpt = base.VisitIdent item
        endVisit resultOpt

    override __.VisitComponentInfo item =
        let node = createNode (FSharpSyntaxNodeKind.ComponentInfo item)
        startVisit node
        let resultOpt = base.VisitComponentInfo item
        endVisit resultOpt

    override __.VisitTypeConstraint item =
        let node = createNode (FSharpSyntaxNodeKind.TypeConstraint item)
        startVisit node
        let resultOpt = base.VisitTypeConstraint item
        endVisit resultOpt

    override __.VisitMemberSig item =
        let node = createNode (FSharpSyntaxNodeKind.MemberSig item)
        startVisit node
        let resultOpt = base.VisitMemberSig item
        endVisit resultOpt

    override __.VisitTypeDefnSig item =
        let node = createNode (FSharpSyntaxNodeKind.TypeDefnSig item)
        startVisit node
        let resultOpt = base.VisitTypeDefnSig item
        endVisit resultOpt

    override __.VisitTypeDefnSigRepr item =
        let node = createNode (FSharpSyntaxNodeKind.TypeDefnSigRepr item)
        startVisit node
        let resultOpt = base.VisitTypeDefnSigRepr item
        endVisit resultOpt

    override __.VisitExceptionDefnRepr item =
        let node = createNode (FSharpSyntaxNodeKind.ExceptionDefnRepr item)
        startVisit node
        let resultOpt = base.VisitExceptionDefnRepr item
        endVisit resultOpt

    override __.VisitUnionCase item =
        let node = createNode (FSharpSyntaxNodeKind.UnionCase item)
        startVisit node
        let resultOpt = base.VisitUnionCase item
        endVisit resultOpt

    override __.VisitUnionCaseType item =
        let node = createNode (FSharpSyntaxNodeKind.UnionCaseType item)
        startVisit node
        let resultOpt = base.VisitUnionCaseType item
        endVisit resultOpt

    override __.VisitArgInfo item =
        let node = createNode (FSharpSyntaxNodeKind.ArgInfo item)
        startVisit node
        let resultOpt = base.VisitArgInfo item
        endVisit resultOpt

    override __.VisitTypeDefnSimpleRepr item =
        let node = createNode (FSharpSyntaxNodeKind.TypeDefnSimpleRepr item)
        startVisit node
        let resultOpt = base.VisitTypeDefnSimpleRepr item
        endVisit resultOpt

    override __.VisitSimplePat item =
        let node = createNode (FSharpSyntaxNodeKind.SimplePat item)
        startVisit node
        let resultOpt = base.VisitSimplePat item
        endVisit resultOpt

    override __.VisitEnumCase item =
        let node = createNode (FSharpSyntaxNodeKind.EnumCase item)
        startVisit node
        let resultOpt = base.VisitEnumCase item
        endVisit resultOpt

    override __.VisitConst item =
        let node = createNode (FSharpSyntaxNodeKind.Const item)
        startVisit node
        let resultOpt = base.VisitConst item
        endVisit resultOpt

    override __.VisitMeasure item =
        let node = createNode (FSharpSyntaxNodeKind.Measure item)
        startVisit node
        let resultOpt = base.VisitMeasure item
        endVisit resultOpt

    override __.VisitRationalConst item =
        let node = createNode (FSharpSyntaxNodeKind.RationalConst item)
        startVisit node
        let resultOpt = base.VisitRationalConst item
        endVisit resultOpt

    override __.VisitTypeDefnKind item =
        let node = createNode (FSharpSyntaxNodeKind.TypeDefnKind item)
        startVisit node
        let resultOpt = base.VisitTypeDefnKind item
        endVisit resultOpt

    override __.VisitField item =
        let node = createNode (FSharpSyntaxNodeKind.Field item)
        startVisit node
        let resultOpt = base.VisitField item
        endVisit resultOpt

    override __.VisitValSig item =
        let node = createNode (FSharpSyntaxNodeKind.ValSig item)
        startVisit node
        let resultOpt = base.VisitValSig item
        endVisit resultOpt

    override __.VisitValTyparDecls item =
        let node = createNode (FSharpSyntaxNodeKind.ValTyparDecls item)
        startVisit node
        let resultOpt = base.VisitValTyparDecls item
        endVisit resultOpt

    override __.VisitType item =
        let node = createNode (FSharpSyntaxNodeKind.Type item)
        startVisit node
        let resultOpt = base.VisitType item
        endVisit resultOpt

    override __.VisitSimplePats item =
        let node = createNode (FSharpSyntaxNodeKind.SimplePats item)
        startVisit node
        let resultOpt = base.VisitSimplePats item
        endVisit resultOpt

    override __.VisitTypar item =
        let node = createNode (FSharpSyntaxNodeKind.Typar item)
        startVisit node
        let resultOpt = base.VisitTypar item
        endVisit resultOpt

    override __.VisitTyparDecl item =
        let node = createNode (FSharpSyntaxNodeKind.TyparDecl item)
        startVisit node
        let resultOpt = base.VisitTyparDecl item
        endVisit resultOpt

    override __.VisitBinding item =
        let node = createNode (FSharpSyntaxNodeKind.Binding item)
        startVisit node
        let resultOpt = base.VisitBinding item
        endVisit resultOpt

    override __.VisitValData item =
        let node = createNode (FSharpSyntaxNodeKind.ValData item)
        startVisit node
        let resultOpt = base.VisitValData item
        endVisit resultOpt

    override __.VisitValInfo item =
        let node = createNode (FSharpSyntaxNodeKind.ValInfo item)
        startVisit node
        let resultOpt = base.VisitValInfo item
        endVisit resultOpt

    override __.VisitPat item =
        let node = createNode (FSharpSyntaxNodeKind.Pat item)
        startVisit node
        let resultOpt = base.VisitPat item
        endVisit resultOpt

    override __.VisitConstructorArgs item =
        let node = createNode (FSharpSyntaxNodeKind.ConstructorArgs item)
        startVisit node
        let resultOpt = base.VisitConstructorArgs item
        endVisit resultOpt

    override __.VisitBindingReturnInfo item =
        let node = createNode (FSharpSyntaxNodeKind.BindingReturnInfo item)
        startVisit node
        let resultOpt = base.VisitBindingReturnInfo item
        endVisit resultOpt

    override __.VisitExpr item =
        let node = createNode (FSharpSyntaxNodeKind.Expr item)
        startVisit node
        let resultOpt = base.VisitExpr item
        endVisit resultOpt

    override __.VisitStaticOptimizationConstraint item =
        let node = createNode (FSharpSyntaxNodeKind.StaticOptimizationConstraint item)
        startVisit node
        let resultOpt = base.VisitStaticOptimizationConstraint item
        endVisit resultOpt

    override __.VisitIndexerArg item =
        let node = createNode (FSharpSyntaxNodeKind.IndexerArg item)
        startVisit node
        let resultOpt = base.VisitIndexerArg item
        endVisit resultOpt

    override __.VisitSimplePatAlternativeIdInfo item =
        let node = createNode (FSharpSyntaxNodeKind.SimplePatAlternativeIdInfo item)
        startVisit node
        let resultOpt = base.VisitSimplePatAlternativeIdInfo item
        endVisit resultOpt

    override __.VisitMatchClause item =
        let node = createNode (FSharpSyntaxNodeKind.MatchClause item)
        startVisit node
        let resultOpt = base.VisitMatchClause item
        endVisit resultOpt

    override __.VisitInterfaceImpl item =
        let node = createNode (FSharpSyntaxNodeKind.InterfaceImpl item)
        startVisit node
        let resultOpt = base.VisitInterfaceImpl item
        endVisit resultOpt

    override __.VisitTypeDefn item =
        let node = createNode (FSharpSyntaxNodeKind.TypeDefn item)
        startVisit node
        let resultOpt = base.VisitTypeDefn item
        endVisit resultOpt

    override __.VisitTypeDefnRepr item =
        let node = createNode (FSharpSyntaxNodeKind.TypeDefnRepr item)
        startVisit node
        let resultOpt = base.VisitTypeDefnRepr item
        endVisit resultOpt

    override __.VisitMemberDefn item =
        let node = createNode (FSharpSyntaxNodeKind.MemberDefn item)
        startVisit node
        let resultOpt = base.VisitMemberDefn item
        endVisit resultOpt

    override __.VisitExceptionDefn item =
        let node = createNode (FSharpSyntaxNodeKind.ExceptionDefn item)
        startVisit node
        let resultOpt = base.VisitExceptionDefn item
        endVisit resultOpt

    override __.VisitParsedHashDirective item =
        let node = createNode (FSharpSyntaxNodeKind.ParsedHashDirective item)
        startVisit node
        let resultOpt = base.VisitParsedHashDirective item
        endVisit resultOpt

    override __.VisitAttributeList item =
        let node = createNode (FSharpSyntaxNodeKind.AttributeList item)
        startVisit node
        let resultOpt = base.VisitAttributeList item
        endVisit resultOpt

    override __.VisitAttribute item =
        let node = createNode (FSharpSyntaxNodeKind.Attribute item)
        startVisit node
        let resultOpt = base.VisitAttribute item
        endVisit resultOpt

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

    member __.Range = range

    member __.IsKeyword =
        match token with
        | Parser.token.ABSTRACT -> true
        | _ -> false

and [<Sealed>] FSharpSyntaxNode (parent: FSharpSyntaxNode option, syntaxTree: FSharpSyntaxTree, kind: FSharpSyntaxNodeKind) as this =

    let lazyRange = lazy kind.Range

    let asyncLazyGetAllTokens =
        AsyncLazy(async {
            let! tokens = syntaxTree.GetAllTokensAsync ()
            let r = this.Range
            return
                tokens
                |> Seq.choose (fun (t, m) ->
                    if rangeContainsRange r m then
                        let parentTokenNode = syntaxTree.GetTokenParentAsync m |> Async.RunSynchronously
                        Some (FSharpSyntaxToken (parentTokenNode, t, m))
                    else
                        None
                )
        })

    member __.Parent = parent

    member __.SyntaxTree = syntaxTree
    
    member __.Kind = kind

    member __.Range = lazyRange.Value
    
    member __.GetAncestors () =            
        seq {
            let mutable nodeOpt = parent
            while nodeOpt.IsSome do
                nodeOpt <- nodeOpt.Value.Parent
                yield parent
        }

    member this.GetAllTokensAsync () =
        asyncLazyGetAllTokens.GetValueAsync ()

and [<Sealed>] FSharpSyntaxTree (filePath: string, pConfig: ParsingConfig, sourceSnapshot: FSharpSourceSnapshot, changes: IReadOnlyList<TextChangeRange>) as this =

    let asyncLazyWeakGetSourceText =
        AsyncLazyWeak(async {
            let! ct = Async.CancellationToken
            return sourceSnapshot.GetText ct
        })

    let asyncLazyWeakGetParseResult =
        AsyncLazyWeak(async {
            let! ct = Async.CancellationToken
            // If we already have a weakly cached source text as a result of calling GetSourceText, just use that value.
            match asyncLazyWeakGetSourceText.TryGetValue () with
            | ValueSome sourceText ->
                return Parser.Parse pConfig (SourceValue.SourceText sourceText)
            | _ ->
                match sourceSnapshot.TryGetStream ct with
                | Some stream ->
                    let result = Parser.Parse pConfig (SourceValue.Stream stream)
                    stream.Dispose ()
                    return result
                | _ ->
                    let! text = asyncLazyWeakGetSourceText.GetValueAsync ()
                    return Parser.Parse pConfig (SourceValue.SourceText text)
        })

    let getTokenParent input m =
        let rootNode = FSharpSyntaxNode (None, this, FSharpSyntaxNodeKind.ParsedInput input)
        let finder = FSharpSyntaxFinder (m, this)
        let result = finder.VisitNode rootNode
        if result.IsNone then failwith "should not happen"
        result.Value

    let lex text =
        let tokens = ResizeArray ()
        Lexer.Lex pConfig (SourceValue.SourceText text) (fun t m -> tokens.Add (t, m))
        tokens

    let asyncLazyGetAllTokens =
        AsyncLazy(async {
           let! text = asyncLazyWeakGetSourceText.GetValueAsync ()
           return lex text
        })

    member __.GetAllTokensAsync () =
        asyncLazyGetAllTokens.GetValueAsync ()

    member __.FilePath = filePath

    member __.ParsingConfig = pConfig

    member __.GetParseResultAsync () =
        asyncLazyWeakGetParseResult.GetValueAsync ()

    member __.GetSourceTextAsync () =
        asyncLazyWeakGetSourceText.GetValueAsync ()

    member __.GetTokenParentAsync m =
        async {
            let! inputOpt, _ = this.GetParseResultAsync ()
            match inputOpt with
            | Some input ->
                return getTokenParent input m
            | _ ->
                return failwith "should not happen"
        }

    member this.TryFindTokenAsync (line: int, column: int) =
        async {
            let! inputOpt, _ = this.GetParseResultAsync ()
            match inputOpt with
            | Some input ->
                let! tokens = this.GetAllTokensAsync ()
                let p = mkPos line column
                return
                    tokens
                    |> Seq.tryPick (fun (t, m) ->
                        if rangeContainsPos m p then
                            Some (FSharpSyntaxToken (getTokenParent input m, t, m))
                        else
                            None
                    )
            | _ ->
                return None
        }

    member this.WithChangedTextSnapshot (newTextSnapshot: FSharpSourceSnapshot) =
        if obj.ReferenceEquals (sourceSnapshot, newTextSnapshot) then
            this
        else
            match sourceSnapshot.TryGetText (), newTextSnapshot.TryGetText () with
            | Some oldText, Some text ->
                let changes = text.GetChangeRanges oldText
                if changes.Count = 0 || (changes.[0].Span.Start = 0 && changes.[0].Span.Length = oldText.Length) then
                    FSharpSyntaxTree (filePath, pConfig, newTextSnapshot, [])
                else
                    FSharpSyntaxTree (filePath, pConfig, newTextSnapshot, changes)
            | _ ->
                FSharpSyntaxTree (filePath, pConfig, newTextSnapshot, [])
