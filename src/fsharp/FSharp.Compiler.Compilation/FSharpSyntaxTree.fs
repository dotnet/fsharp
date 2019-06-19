namespace FSharp.Compiler.Compilation

open System
open System.IO
open System.Threading
open System.Collections.Generic
open System.Collections.Immutable
open System.Runtime.CompilerServices
open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.Host
open FSharp.Compiler.Compilation.Utilities
open FSharp.Compiler.AbstractIL.Internal.Library
open FSharp.Compiler
open FSharp.Compiler.Text
open FSharp.Compiler.CompileOps
open FSharp.Compiler.Ast
open FSharp.Compiler.Range
open FSharp.Compiler.SourceCodeServices

[<CustomEquality;NoComparison;RequireQualifiedAccess>]
type FSharpSyntaxNodeKind =
    | ParsedInput of ParsedInput
    | ModuleOrNamespace of SynModuleOrNamespace
    | ModuleDecl of SynModuleDecl
    | LongIdentWithDots of LongIdentWithDots
    | Ident of index: int * Ident
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
            item.AdjustedRange
        | FSharpSyntaxNodeKind.ModuleOrNamespace item ->
            item.AdjustedRange
        | FSharpSyntaxNodeKind.ModuleDecl item ->
            item.Range
        | FSharpSyntaxNodeKind.LongIdentWithDots item ->
            item.Range
        | FSharpSyntaxNodeKind.Ident (_, item) ->
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

    member private this.Item =
        match this with
        | FSharpSyntaxNodeKind.ParsedInput item ->
            item :> obj
        | FSharpSyntaxNodeKind.ModuleOrNamespace item ->
            item :> obj
        | FSharpSyntaxNodeKind.ModuleDecl item ->
            item :> obj
        | FSharpSyntaxNodeKind.LongIdentWithDots item ->
            item :> obj
        | FSharpSyntaxNodeKind.Ident (_, item) ->
            item :> obj
        | FSharpSyntaxNodeKind.ComponentInfo item ->
            item :> obj
        | FSharpSyntaxNodeKind.TypeConstraint item ->
            item :> obj
        | FSharpSyntaxNodeKind.MemberSig item ->
            item :> obj
        | FSharpSyntaxNodeKind.TypeDefnSig item ->
            item :> obj
        | FSharpSyntaxNodeKind.TypeDefnSigRepr item ->
            item :> obj
        | FSharpSyntaxNodeKind.ExceptionDefnRepr item ->
            item :> obj
        | FSharpSyntaxNodeKind.UnionCase item ->
            item :> obj
        | FSharpSyntaxNodeKind.UnionCaseType item ->
            item :> obj
        | FSharpSyntaxNodeKind.ArgInfo item ->
            item :> obj
        | FSharpSyntaxNodeKind.TypeDefnSimpleRepr item ->
            item :> obj
        | FSharpSyntaxNodeKind.SimplePat item ->
            item :> obj
        | FSharpSyntaxNodeKind.EnumCase item ->
            item :> obj
        | FSharpSyntaxNodeKind.Const item ->
            item :> obj
        | FSharpSyntaxNodeKind.Measure item ->
            item :> obj
        | FSharpSyntaxNodeKind.RationalConst item ->
            item :> obj
        | FSharpSyntaxNodeKind.TypeDefnKind item ->
            item :> obj
        | FSharpSyntaxNodeKind.Field item ->
            item :> obj
        | FSharpSyntaxNodeKind.ValSig item ->
            item :> obj
        | FSharpSyntaxNodeKind.ValTyparDecls item ->
            item :> obj
        | FSharpSyntaxNodeKind.Type item ->
            item :> obj
        | FSharpSyntaxNodeKind.SimplePats item ->
            item :> obj
        | FSharpSyntaxNodeKind.Typar item ->
            item :> obj
        | FSharpSyntaxNodeKind.TyparDecl item ->
            item :> obj
        | FSharpSyntaxNodeKind.Binding item ->
            item :> obj
        | FSharpSyntaxNodeKind.ValData item ->
            item :> obj
        | FSharpSyntaxNodeKind.ValInfo item ->
            item :> obj
        | FSharpSyntaxNodeKind.Pat item ->
            item :> obj
        | FSharpSyntaxNodeKind.ConstructorArgs item ->
            item :> obj
        | FSharpSyntaxNodeKind.BindingReturnInfo item ->
            item :> obj
        | FSharpSyntaxNodeKind.Expr item ->
            item :> obj
        | FSharpSyntaxNodeKind.StaticOptimizationConstraint item ->
            item :> obj
        | FSharpSyntaxNodeKind.IndexerArg item ->
            item :> obj
        | FSharpSyntaxNodeKind.SimplePatAlternativeIdInfo item ->
            item :> obj
        | FSharpSyntaxNodeKind.MatchClause item ->
            item :> obj
        | FSharpSyntaxNodeKind.InterfaceImpl item ->
            item :> obj
        | FSharpSyntaxNodeKind.TypeDefn item ->
            item :> obj
        | FSharpSyntaxNodeKind.TypeDefnRepr item ->
            item :> obj
        | FSharpSyntaxNodeKind.MemberDefn item ->
            item :> obj
        | FSharpSyntaxNodeKind.ExceptionDefn item ->
            item :> obj
        | FSharpSyntaxNodeKind.ParsedHashDirective item ->
            item :> obj
        | FSharpSyntaxNodeKind.AttributeList item ->
            item :> obj
        | FSharpSyntaxNodeKind.Attribute item ->
            item :> obj

    override this.GetHashCode () = 0

    override this.Equals target =
        if target = null then
            false
        else
            if obj.ReferenceEquals (this, target) then
                true
            else
                match target with
                | :? FSharpSyntaxNodeKind as targetNodeKind ->
                    obj.ReferenceEquals (this.Item, targetNodeKind.Item)
                | _ ->
                    false

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

    member __.VisitStackCount = visitStack.Count

    abstract OnVisit: visitedNode: FSharpSyntaxNode * resultOpt: FSharpSyntaxNode option -> FSharpSyntaxNode option
    default __.OnVisit (_visitedNode: FSharpSyntaxNode, resultOpt: FSharpSyntaxNode option) =
        resultOpt

    abstract VisitNode: node: FSharpSyntaxNode -> FSharpSyntaxNode option
    default this.VisitNode (node: FSharpSyntaxNode) =
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
        | FSharpSyntaxNodeKind.Ident (index, item) ->
            this.VisitIdent (index, item)
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

    override __.VisitIdent (index, item) =
        let node = createNode (FSharpSyntaxNodeKind.Ident (index, item))
        startVisit node
        let resultOpt = base.VisitIdent (index, item)
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

and FSharpSyntaxNodeVisitor (findm: range, syntaxTree) =
    inherit FSharpSyntaxVisitor (syntaxTree)

    let isZeroRange (r: range) =
        posEq r.Start r.End

    override __.CanVisit m = rangeContainsRange m findm && not (isZeroRange m)

    override __.OnVisit (visitedNode, resultOpt) =
        match resultOpt with
        | Some _ -> resultOpt
        | _ -> Some visitedNode

and FSharpSyntaxNodeDescendantVisitor (findm, syntaxTree) =
    inherit FSharpSyntaxNodeVisitor (findm, syntaxTree)

    let isZeroRange (r: range) =
        posEq r.Start r.End
    
    let descendants = ImmutableArray.CreateBuilder ()

    member __.GetDescendantNodes () = descendants.ToImmutable ()

    override __.CanVisit m = rangeContainsRange findm m && not (isZeroRange m)

    override this.OnVisit (visitedNode, _) =
        if this.VisitStackCount > 1 then
            descendants.Add visitedNode
        None

and FSharpSyntaxNodeChildVisitor (findm, syntaxTree) =
    inherit FSharpSyntaxNodeDescendantVisitor (findm, syntaxTree)

    override this.CanVisit m =
        // We want to visit direct children only.
        if this.VisitStackCount > 2 then
            false
        else
            base.CanVisit m

and [<Struct;NoEquality;NoComparison>] FSharpSyntaxToken (syntaxTree: FSharpSyntaxTree, token: Parser.token, span: TextSpan) =

    member __.IsNone = obj.ReferenceEquals (syntaxTree, null)

    member this.GetParentNode ?ct =
        let ct = defaultArg ct CancellationToken.None

        if this.IsNone then
            failwith "Token's kind is None and will not have a parent. Check if the token's kind is None before calling GetParentNode."

        let rootNode: FSharpSyntaxNode = syntaxTree.GetRootNode ct
        match rootNode.TryFindNode span with
        | Some node -> node
        | _ -> failwith "should not happen"

    member __.Span = span

    member this.IsKeyword =
        if this.IsNone then false
        else

        match token with
        | Parser.token.ABSTRACT
        | Parser.token.AND
        | Parser.token.AS
        | Parser.token.ASSERT
        | Parser.token.BASE
        | Parser.token.BEGIN
        | Parser.token.CLASS
        | Parser.token.DEFAULT
        | Parser.token.DELEGATE
        | Parser.token.DO
        | Parser.token.DONE
        | Parser.token.DOWNCAST
        | Parser.token.DOWNTO
        | Parser.token.ELIF
        | Parser.token.ELSE
        | Parser.token.END
        | Parser.token.EXCEPTION
        | Parser.token.EXTERN
        | Parser.token.FALSE
        | Parser.token.FINALLY
        | Parser.token.FIXED
        | Parser.token.FOR
        | Parser.token.FUN
        | Parser.token.FUNCTION
        | Parser.token.GLOBAL
        | Parser.token.IF
        | Parser.token.IN
        | Parser.token.INHERIT
        | Parser.token.INLINE
        | Parser.token.INTERFACE
        | Parser.token.INTERNAL
        | Parser.token.LAZY
        | Parser.token.LET _ // "let" and "use"
        | Parser.token.DO_BANG //  "let!", "use!" and "do!"
        | Parser.token.MATCH
        | Parser.token.MATCH_BANG
        | Parser.token.MEMBER
        | Parser.token.MODULE
        | Parser.token.MUTABLE
     
        | Parser.token.NAMESPACE
        | Parser.token.NEW
        // | Parser.token.NOT // Not actually a keyword. However, not struct in combination is used as a generic parameter constraint.
        | Parser.token.NULL
        | Parser.token.OF
        | Parser.token.OPEN
        | Parser.token.OR
        | Parser.token.OVERRIDE
        | Parser.token.PRIVATE
        | Parser.token.PUBLIC
        | Parser.token.REC
        | Parser.token.YIELD _ // "yield" and "return"
        | Parser.token.YIELD_BANG _ // "yield!" and "return!"
        | Parser.token.STATIC
        | Parser.token.STRUCT
        | Parser.token.THEN
        | Parser.token.TO
        | Parser.token.TRUE
        | Parser.token.TRY
        | Parser.token.TYPE
        | Parser.token.UPCAST
        | Parser.token.VAL
        | Parser.token.VOID
        | Parser.token.WHEN
        | Parser.token.WHILE
        | Parser.token.WITH

        // * Reserved - from OCAML *
        | Parser.token.ASR
        | Parser.token.INFIX_STAR_STAR_OP "asr"
        | Parser.token.INFIX_STAR_DIV_MOD_OP "land"
        | Parser.token.INFIX_STAR_DIV_MOD_OP "lor"
        | Parser.token.INFIX_STAR_STAR_OP "lsl"
        | Parser.token.INFIX_STAR_STAR_OP "lsr"
        | Parser.token.INFIX_STAR_DIV_MOD_OP "lxor"
        | Parser.token.INFIX_STAR_DIV_MOD_OP "mod"
        | Parser.token.SIG

        // * Reserved - for future *
        // atomic
        // break
        // checked
        // component
        // const
        // constraint
        // constructor
        // continue
        // eager
        // event
        // external
        // functor
        // include
        // method
        // mixin
        // object
        // parallel
        // process
        // protected
        // pure
        // sealed
        // tailcall
        // trait
        // virtual
        // volatile
        | Parser.token.RESERVED
        | Parser.token.KEYWORD_STRING _
            -> true
        | _ -> false

    member this.IsIdentifier =
        if this.IsNone then false
        else

        match token with
        | Parser.token.IDENT _ -> true
        | _ -> false

    member this.IsString =
        if this.IsNone then false
        else

        match token with
        | Parser.token.STRING _ -> true
        | _ -> false

    member this.Value =
        if this.IsNone then 
            ValueNone
        else
            let value =
                match token with
                | Parser.token.STRING value -> value :> obj
                | Parser.token.UINT8 value -> value :> obj
                | Parser.token.INT8 value -> value :> obj
                | Parser.token.UINT16 value -> value :> obj
                | Parser.token.INT16 value -> value :> obj
                | Parser.token.UINT32 value -> value :> obj
                | Parser.token.INT32 value -> value :> obj
                | Parser.token.UINT64 value -> value :> obj
                | Parser.token.INT64 value -> value :> obj
                | Parser.token.TRUE -> true :> obj
                | Parser.token.FALSE -> false :> obj
                | Parser.token.BYTEARRAY value -> value :> obj
                | Parser.token.NATIVEINT value -> value :> obj
                | Parser.token.UNATIVEINT value -> value :> obj
                | Parser.token.KEYWORD_STRING value -> value :> obj
                | Parser.token.IDENT value -> value :> obj
                | _ -> null
            if value = null then
                ValueNone
            else
                ValueSome value

    member this.ValueText =
        match this.Value with
        | ValueSome value -> ValueSome (string value)
        | _ -> ValueNone

    static member None = Unchecked.defaultof<FSharpSyntaxToken>

    // TODO: Implement TryGetNextToken / TryGetPreviousToken

and [<Sealed;System.Diagnostics.DebuggerDisplay("{DebugString}")>] FSharpSyntaxNode (parent: FSharpSyntaxNode option, syntaxTree: FSharpSyntaxTree, kind: FSharpSyntaxNodeKind) =

    let mutable lazySpan = TextSpan ()

    member __.Text: SourceText = 
        syntaxTree.GetText CancellationToken.None

    member __.Parent = parent

    member __.SyntaxTree = syntaxTree
    
    member __.Kind = kind

    member this.Span =
        if lazySpan.Length = 0 then
            lazySpan <-
                match kind with
                | FSharpSyntaxNodeKind.ParsedInput _ -> TextSpan (0, this.Text.Length)
                | _ ->
                    match this.Text.TryRangeToSpan kind.Range with
                    | ValueSome span -> span
                    | _ -> failwith "should not happen"
        lazySpan
    
    member __.GetAncestors () =            
        seq {
            let mutable nodeOpt = parent
            while nodeOpt.IsSome do
                yield nodeOpt.Value
                nodeOpt <- nodeOpt.Value.Parent
        }

    member this.GetAncestorsAndSelf () =
        seq {
            let mutable nodeOpt = Some this
            while nodeOpt.IsSome do
                yield nodeOpt.Value
                nodeOpt <- nodeOpt.Value.Parent
        }

    member this.GetDescendantTokens () =
        syntaxTree.GetTokens this.Span

    member this.GetChildTokens () =
        this.GetDescendantTokens ()
        |> Seq.filter (fun (token: FSharpSyntaxToken) ->
            obj.ReferenceEquals (token.GetParentNode (), this)
        )

    member this.GetDescendants (span: TextSpan) =
        let text: SourceText = this.Text
        match text.TrySpanToRange (syntaxTree.FilePath, span) with
        | ValueSome m ->
            // TODO: This isn't lazy, make it lazy so we don't try to evaluate everything. Will require work here and in the visitor.
            let visitor = FSharpSyntaxNodeDescendantVisitor (m, syntaxTree)
            visitor.VisitNode this |> ignore
            visitor.GetDescendantNodes() :> FSharpSyntaxNode seq
        | _ ->
            Seq.empty

    member this.GetDescendants () =
        this.GetDescendants this.Span

    member this.GetChildren (span: TextSpan) =
        let text: SourceText = this.Text
        match text.TrySpanToRange (syntaxTree.FilePath, span) with
        | ValueSome m ->
            // TODO: This isn't lazy, make it lazy so we don't try to evaluate everything. Will require work here and in the visitor.
            let visitor = FSharpSyntaxNodeChildVisitor (m, syntaxTree)
            visitor.VisitNode this |> ignore
            visitor.GetDescendantNodes() :> FSharpSyntaxNode seq
        | _ ->
            Seq.empty

    member this.GetChildren () =
        this.GetChildren this.Span

    member this.GetRoot () =
        this.GetAncestorsAndSelf ()
        |> Seq.last

    member this.FindToken (position: int) =
        let text: SourceText = this.Text

        let nodeSpan = this.Span
        let line = text.Lines.GetLineFromPosition position

        syntaxTree.GetTokens (line.Span)
        |> Seq.filter (fun (token: FSharpSyntaxToken) ->
            (token.Span.Contains position || token.Span.End = position) && nodeSpan.Contains token.Span
        )
        // Pick the innermost one.
        |> Seq.sortByDescending (fun (token: FSharpSyntaxToken) -> 
            let parentNode: FSharpSyntaxNode = token.GetParentNode ()
            Seq.length (parentNode.GetAncestors ())
        )
        |> Seq.tryHead
        |> Option.defaultValue FSharpSyntaxToken.None

    member this.TryFindNode (span: TextSpan) =
        let text = this.Text

        match text.TrySpanToRange (syntaxTree.FilePath, span) with
        | ValueSome m ->
            let visitor = FSharpSyntaxNodeVisitor (m, syntaxTree)
            visitor.VisitNode this
        | _ ->
            None

    member private this.DebugString = 
        let span =
            match this.Text.TryRangeToSpan kind.Range with
            | ValueSome span -> span
            | _ -> failwith "should not happen"
        this.Text.GetSubText(span).ToString () 

    override this.Equals target =
        if target = null then 
            false
        else
            if obj.ReferenceEquals (this, target) then 
                true
            else
                match target with
                | :? FSharpSyntaxNode as targetNode ->
                    this.Kind = targetNode.Kind
                | _ ->
                    false

    override this.GetHashCode () =
        this.Span.GetHashCode ()

    override this.ToString () =
        let index = this.DebugString.IndexOf ("\n")
        this.Kind.GetType().Name + ": " + if index <> -1 then this.DebugString.Substring(0, index) else this.DebugString

and [<Sealed>] FSharpSyntaxTree (filePath: string, pConfig: ParsingConfig, textSnapshot: FSharpSourceSnapshot, lexer: IncrementalLexer) =

    let gate = obj ()
    let mutable lazyText = ValueNone
    let mutable lazyRootNode = ValueNone

    let mutable parseResult = ValueStrength.None

    member this.ConvertSpanToRange (span: TextSpan) =
        let text = this.GetText CancellationToken.None
        match text.TrySpanToRange (filePath, span) with
        | ValueSome range -> range
        | _ -> failwith "Invalid span to range conversion."

    member __.FilePath = filePath

    member __.ParsingConfig = pConfig

    member __.GetParseResult ct =
        lock gate (fun () ->
            match parseResult.TryGetTarget () with
            | true, parseResult -> parseResult
            | _ ->
                if lexer.AreTokensCached then
                        let tcConfig = pConfig.tcConfig
                        let isLastCompiland = (pConfig.isLastFileOrScript, pConfig.isExecutable)
                        let errorLogger = CompilationErrorLogger("Parse", tcConfig.errorSeverityOptions)
                        let mutable result = Unchecked.defaultof<_>

                        let result =
                            try
                                let parsedInput =
                                    let mutable parsedResult = Unchecked.defaultof<_>
                                    printfn "using incremental lexer"
                                    lexer.LexFilter (errorLogger, ((fun lexbuf getNextToken ->
                                        let parsedInput = ParseInput(getNextToken, errorLogger, lexbuf, None, filePath, isLastCompiland)
                                        parsedResult <- parsedInput
                                    )), ct)
                                    parsedResult

                                (Some parsedInput, errorLogger.GetErrorInfos ())
                            with
                            | _ -> (None, errorLogger.GetErrorInfos ())
                        parseResult <- ValueStrength.Weak (WeakReference<_> result)
                        result
                else
                    let result =
                        match lazyText with
                        | ValueSome text ->
                            Parser.Parse pConfig (SourceValue.SourceText text) ct
                        | _ ->
                            match textSnapshot.TryGetStream ct with
                            | Some stream ->
                                Parser.Parse pConfig (SourceValue.Stream stream) ct
                            | _ ->
                                let text = textSnapshot.GetText ct
                                lazyText <- ValueSome text
                                Parser.Parse pConfig (SourceValue.SourceText text) ct
                    parseResult <- ValueStrength.Weak (WeakReference<_> result)
                    result
        )

    member __.GetText ?ct =
        let ct = defaultArg ct CancellationToken.None

        match lazyText with
        | ValueSome text -> text
        | _ ->
            lock gate (fun () ->
                match lazyText with
                | ValueSome text -> text
                | _ ->
                    let text = textSnapshot.GetText ct
                    lazyText <- ValueSome text
                    text
            )

    member private this.GetAllTokens (span: TextSpan, ct) =
        lexer.GetTokens (span, ct)
        |> Seq.map (fun (TokenItem (t, m, _)) ->
            FSharpSyntaxToken (this, t, m)
        )

    member this.GetTokens (span: TextSpan, ?ct) =
        let ct = defaultArg ct CancellationToken.None

        this.GetAllTokens (span, ct)

    member this.GetTokens ?ct =
        let ct = defaultArg ct CancellationToken.None

        let text = this.GetText ct
        let span = TextSpan (0, text.Length)
        this.GetTokens (span, ct)

    member this.GetRootNode ?ct =
        let ct = defaultArg ct CancellationToken.None

        lock gate (fun () ->
            let inputOpt, _ = this.GetParseResult ct
            if inputOpt.IsNone then failwith "parsed input does not exist"
            let input = inputOpt.Value
            let rootNode = FSharpSyntaxNode (None, this, FSharpSyntaxNodeKind.ParsedInput input)
            lazyRootNode <- ValueSome rootNode
            rootNode
        )

    member this.WithChangedTextSnapshot (newTextSnapshot: FSharpSourceSnapshot) =
        if obj.ReferenceEquals (textSnapshot, newTextSnapshot) then
            this
        else
            FSharpSyntaxTree (filePath, pConfig, newTextSnapshot, lexer.WithChangedTextSnapshot newTextSnapshot)

    member this.GetDiagnostics ?ct =
        let ct = defaultArg ct CancellationToken.None

        let text = this.GetText ct
        let _, errors = this.GetParseResult ct
        errors.ToDiagnostics (filePath, text)

    static member Create (filePath, pConfig, textSnapshot) =
        FSharpSyntaxTree (filePath, pConfig, textSnapshot, IncrementalLexer.Create (pConfig, textSnapshot))
