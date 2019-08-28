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
module AstVisitorHelpers =

    type SynModuleOrNamespace with

        member AdjustedRange: range

    type ParsedInput with

        member AdjustedRange: range

    type ParsedHashDirective with

        member Range: range

    type SynTypeConstraint with

        member Range: range

    type SynMemberSig with

        member Range: range

    type SynValSig with

        member Range: range

    type SynField with

        member Range: range

    type SynTypeDefnSig with

        member Range: range

    type SynMeasure with

        member PossibleRange: range

    type SynRationalConst with

        member PossibleRange: range

    type SynConst with

        member PossibleRange: range

    type SynArgInfo with

        member PossibleRange: range

    type SynValInfo with

        member PossibleRange: range

    type SynTypeDefnKind with

        member PossibleRange: range

    type SynTyparDecl with

        member Range: range

    type SynValTyparDecls with

        member PossibleRange: range

    type SynSimplePat with

        member Range: range

    type SynSimplePats with

        member Range: range

    type SynValData with

        member Range: range

    type SynBindingReturnInfo with

        member Range: range

    type SynConstructorArgs with

        member PossibleRange: range

    type SynInterfaceImpl with

        member Range: range

    type SynSimplePatAlternativeIdInfo with

        member Range: range

    type SynStaticOptimizationConstraint with

        member Range: range

    type SynUnionCaseType with

        member PossibleRange: range

    val longIdentRange: LongIdent -> range

[<AbstractClass>]
type AstVisitor<'T> =

    new: unit -> AstVisitor<'T>

    abstract CanVisit: range -> bool
    default CanVisit: range -> bool

    abstract VisitParsedInput: ParsedInput -> 'T option
    default VisitParsedInput: ParsedInput -> 'T option

    abstract VisitModuleOrNamespace: SynModuleOrNamespace -> 'T option
    default VisitModuleOrNamespace: SynModuleOrNamespace -> 'T option

    abstract VisitModuleDecl: SynModuleDecl -> 'T option
    default VisitModuleDecl: SynModuleDecl -> 'T option

    abstract VisitLongIdentWithDots: LongIdentWithDots -> 'T option
    default VisitLongIdentWithDots: LongIdentWithDots -> 'T option

    abstract VisitLongIdent: LongIdent -> 'T option
    default VisitLongIdent: LongIdent -> 'T option

    abstract VisitIdent: index: int * Ident -> 'T option
    default VisitIdent: index: int * Ident -> 'T option

    abstract VisitComponentInfo: SynComponentInfo -> 'T option
    default VisitComponentInfo: SynComponentInfo -> 'T option

    abstract VisitTypeConstraint: SynTypeConstraint -> 'T option
    default VisitTypeConstraint: SynTypeConstraint -> 'T option

    abstract VisitMemberSig: SynMemberSig -> 'T option
    default VisitMemberSig: SynMemberSig -> 'T option

    abstract VisitTypeDefnSig: SynTypeDefnSig -> 'T option
    default VisitTypeDefnSig: SynTypeDefnSig -> 'T option

    abstract VisitTypeDefnSigRepr: SynTypeDefnSigRepr -> 'T option
    default VisitTypeDefnSigRepr: SynTypeDefnSigRepr -> 'T option

    abstract VisitExceptionDefnRepr: SynExceptionDefnRepr -> 'T option
    default VisitExceptionDefnRepr: SynExceptionDefnRepr -> 'T option

    abstract VisitUnionCase: SynUnionCase -> 'T option
    default VisitUnionCase: SynUnionCase -> 'T option

    abstract VisitUnionCaseType: SynUnionCaseType -> 'T option
    default VisitUnionCaseType: SynUnionCaseType -> 'T option

    abstract VisitArgInfo: SynArgInfo -> 'T option
    default VisitArgInfo: SynArgInfo -> 'T option

    abstract VisitTypeDefnSimpleRepr: SynTypeDefnSimpleRepr -> 'T option
    default VisitTypeDefnSimpleRepr: SynTypeDefnSimpleRepr -> 'T option

    abstract VisitSimplePat: SynSimplePat -> 'T option
    default VisitSimplePat: SynSimplePat -> 'T option

    abstract VisitEnumCase: SynEnumCase -> 'T option
    default VisitEnumCase: SynEnumCase -> 'T option

    abstract VisitConst: SynConst -> 'T option
    default VisitConst: SynConst -> 'T option

    abstract VisitMeasure: SynMeasure -> 'T option
    default VisitMeasure: SynMeasure -> 'T option

    abstract VisitRationalConst: SynRationalConst -> 'T option
    default VisitRationalConst: SynRationalConst -> 'T option

    abstract VisitTypeDefnKind: SynTypeDefnKind -> 'T option
    default VisitTypeDefnKind: SynTypeDefnKind -> 'T option

    abstract VisitField: SynField -> 'T option
    default VisitField: SynField -> 'T option

    abstract VisitValSig: SynValSig -> 'T option
    default VisitValSig: SynValSig -> 'T option

    abstract VisitValTyparDecls: SynValTyparDecls -> 'T option
    default VisitValTyparDecls: SynValTyparDecls -> 'T option

    abstract VisitType: SynType -> 'T option
    default VisitType: SynType -> 'T option

    abstract VisitSimplePats: SynSimplePats -> 'T option
    default VisitSimplePats: SynSimplePats -> 'T option

    abstract VisitTypar: SynTypar -> 'T option
    default VisitTypar: SynTypar -> 'T option

    abstract VisitTyparDecl: SynTyparDecl -> 'T option
    default VisitTyparDecl: SynTyparDecl -> 'T option

    abstract VisitBinding: SynBinding -> 'T option
    default VisitBinding: SynBinding -> 'T option

    abstract VisitValData: SynValData -> 'T option
    default VisitValData: SynValData -> 'T option

    abstract VisitValInfo: SynValInfo -> 'T option
    default VisitValInfo: SynValInfo -> 'T option

    abstract VisitPat: SynPat -> 'T option
    default VisitPat: SynPat -> 'T option

    abstract VisitConstructorArgs: SynConstructorArgs -> 'T option
    default VisitConstructorArgs: SynConstructorArgs -> 'T option

    abstract VisitBindingReturnInfo: SynBindingReturnInfo -> 'T option
    default VisitBindingReturnInfo: SynBindingReturnInfo -> 'T option

    abstract VisitExpr: SynExpr -> 'T option
    default VisitExpr: SynExpr -> 'T option

    abstract VisitStaticOptimizationConstraint: SynStaticOptimizationConstraint -> 'T option
    default VisitStaticOptimizationConstraint: SynStaticOptimizationConstraint -> 'T option

    abstract VisitIndexerArg: SynIndexerArg -> 'T option
    default VisitIndexerArg: SynIndexerArg -> 'T option

    abstract VisitSimplePatAlternativeIdInfo: SynSimplePatAlternativeIdInfo -> 'T option
    default VisitSimplePatAlternativeIdInfo: SynSimplePatAlternativeIdInfo -> 'T option

    abstract VisitMatchClause: SynMatchClause -> 'T option
    default VisitMatchClause: SynMatchClause -> 'T option

    abstract VisitInterfaceImpl: SynInterfaceImpl -> 'T option
    default VisitInterfaceImpl: SynInterfaceImpl -> 'T option

    abstract VisitTypeDefn: SynTypeDefn -> 'T option
    default VisitTypeDefn: SynTypeDefn -> 'T option

    abstract VisitTypeDefnRepr: SynTypeDefnRepr -> 'T option
    default VisitTypeDefnRepr: SynTypeDefnRepr -> 'T option

    abstract VisitMemberDefn: SynMemberDefn -> 'T option
    default VisitMemberDefn: SynMemberDefn -> 'T option

    abstract VisitExceptionDefn: SynExceptionDefn -> 'T option
    default VisitExceptionDefn: SynExceptionDefn -> 'T option

    abstract VisitParsedHashDirective: ParsedHashDirective -> 'T option
    default VisitParsedHashDirective: ParsedHashDirective -> 'T option

    abstract VisitAttributeList: SynAttributeList -> 'T option
    default VisitAttributeList: SynAttributeList -> 'T option

    abstract VisitAttribute: SynAttribute -> 'T option
    default VisitAttribute: SynAttribute -> 'T option