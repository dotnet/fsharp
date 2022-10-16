module FSharp.Compiler.Service.Tests2.SyntaxTreeTests.TypeTests

open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Service.Tests.Common
open FSharp.Compiler.Syntax
open FSharp.Compiler.SyntaxTrivia
open NUnit.Framework

let unsupported = "unsupported"
type Kind =
    | Type
    | ModuleOrNamespace
type Item =
    {
        Ident : LongIdent
        Kind : Kind
    }
type Stuff = Item seq

let rec visitSynModuleDecl (decl : SynModuleDecl) : Stuff =
    // TODO
    match decl with
    | SynModuleDecl.Attributes(synAttributeLists, range) ->
        visitSynAttributeLists synAttributeLists
    | SynModuleDecl.Exception(synExceptionDefn, range) ->
        visitSynExceptionDefn synExceptionDefn
    | SynModuleDecl.Expr(synExpr, range) ->
        visitExpr synExpr
    | SynModuleDecl.HashDirective(parsedHashDirective, range) ->
        visitHashDirective parsedHashDirective
    | SynModuleDecl.Let(isRecursive, synBindings, range) ->
        visitBindings synBindings
    | SynModuleDecl.Open(synOpenDeclTarget, range) -> 
        visitSynOpenDeclTarget synOpenDeclTarget
    | SynModuleDecl.Types(synTypeDefns, range) ->
        visitSynTypeDefns synTypeDefns
    | SynModuleDecl.ModuleAbbrev(ident, longId, range) ->
        // TODO Module abbrevation can break the algorithm.
        // We need to either give up when seeing this or handle it properly.
        //
        // Consider the following:
        // module A = module A1 = let x = 1
        // module B = A
        // let x = B.A1.x
        failwith "Module abbreviations are not currently supported"
        //visitLongIdent longId
    | SynModuleDecl.NamespaceFragment synModuleOrNamespace ->
        visitSynModuleOrNamespace synModuleOrNamespace
    | SynModuleDecl.NestedModule(synComponentInfo, isRecursive, synModuleDecls, isContinuing, range, synModuleDeclNestedModuleTrivia) ->
        seq {
            yield! visitSynComponentInfo synComponentInfo
            yield! Seq.collect visitSynModuleDecl synModuleDecls
            yield! visitSynModuleDeclNestedModuleTrivia synModuleDeclNestedModuleTrivia
        }

and visitSynModuleDeclNestedModuleTrivia (x : SynModuleDeclNestedModuleTrivia) : Stuff =
    [] // TODO check

and visitHashDirective (x : ParsedHashDirective) : Stuff =
    [] // TODO Check

and visitSynIdent (x : SynIdent) : Stuff =
    [] // TODO Check

and visitSynTupleTypeSegment (x : SynTupleTypeSegment) : Stuff =
    match x with
    | SynTupleTypeSegment.Slash range ->
        []
    | SynTupleTypeSegment.Star range ->
        []
    | SynTupleTypeSegment.Type typeName ->
        visitType typeName

and visitSynTupleTypeSegments (x : SynTupleTypeSegment list) : Stuff =
    Seq.collect visitSynTupleTypeSegment x 

and visitTypar (x : SynTypar) : Stuff =
    match x with
    | SynTypar.SynTypar(ident, typarStaticReq, isCompGen) ->
        [] // TODO check

and visitSynRationalConst (x : SynRationalConst) =
    [] // TODO check

and visitSynConst (x : SynConst) : Stuff =
    [] // TODO Check

and visitSynTypes (x : SynType list) : Stuff =
    Seq.collect visitType x

and visitTypeConstraints (x : SynTypeConstraint list) : Stuff =
    Seq.collect visitTypeConstraint x

and visitSynTyparDecl (x : SynTyparDecl) : Stuff =
    match x with
    | SynTyparDecl(synAttributeLists, synTypar) ->
        seq {
            yield! visitSynAttributeLists synAttributeLists
            yield! visitTypar synTypar
        }

and visitSynTyparDeclList (x : SynTyparDecl list) : Stuff =
    Seq.collect visitSynTyparDecl x

and visitSynTyparDecls (x : SynTyparDecls) : Stuff =
    match x with
    | SynTyparDecls.PostfixList(synTyparDecls, synTypeConstraints, range) ->
        seq {
            yield! visitSynTyparDeclList synTyparDecls
            yield! visitTypeConstraints synTypeConstraints
        }
    | SynTyparDecls.PrefixList(synTyparDecls, range) ->
        visitSynTyparDeclList synTyparDecls
    | SynTyparDecls.SinglePrefix(synTyparDecl, range) ->
        visitSynTyparDecl synTyparDecl

and visitSynValTyparDecls (x : SynValTyparDecls) : Stuff =
    match x with
    | SynValTyparDecls(synTyparDeclsOption, canInfer) ->
        match synTyparDeclsOption with
        | Some decls -> visitSynTyparDecls decls
        | None -> []

and visitValSig (x : SynValSig) : Stuff =
    match x with
    | SynValSig(synAttributeLists, synIdent, synValTyparDecls, synType, synValInfo, isInline, isMutable, preXmlDoc, synAccessOption, synExprOption, range, synValSigTrivia) ->
        seq {
            yield! visitSynAttributeLists synAttributeLists
            yield! visitSynIdent synIdent
            yield! visitSynValTyparDecls synValTyparDecls
            yield! visitType synType
            yield! visitSynValInfo synValInfo
            yield! visitPreXmlDoc preXmlDoc
            match synAccessOption with | Some access -> yield! visitSynAccess access | None -> ()
            match synExprOption with | Some expr -> yield! visitExpr expr | None -> ()
        }

and visitSynTypeDefnSignRepr (x : SynTypeDefnSigRepr) : Stuff =
    match x with
    | SynTypeDefnSigRepr.Exception synExceptionDefnRepr ->
        visitSynExceptionDefnRepr synExceptionDefnRepr
    | SynTypeDefnSigRepr.Simple(synTypeDefnSimpleRepr, range) ->
        visitTypeDefnSimpleRepr synTypeDefnSimpleRepr
    | SynTypeDefnSigRepr.ObjectModel(synTypeDefnKind, synMemberSigs, range) ->
        seq {
            yield! visitSynTypeDefnKind synTypeDefnKind
            yield! (Seq.collect visitMemberSig synMemberSigs)
        }    

and visitSynTypeDefnSign (x : SynTypeDefnSig) : Stuff =
    match x with
    | SynTypeDefnSig(synComponentInfo, synTypeDefnSigRepr, synMemberSigs, range, synTypeDefnSigTrivia) ->
        seq {
            yield! visitSynComponentInfo synComponentInfo
            yield! visitSynTypeDefnSignRepr synTypeDefnSigRepr
        }

and visitMemberSig (x : SynMemberSig) : Stuff =
    match x with
    | SynMemberSig.Inherit(inheritedType, range) ->
        visitType inheritedType
    | SynMemberSig.Interface(interfaceType, range) ->
        visitType interfaceType
    | SynMemberSig.Member(synValSig, synMemberFlags, range) ->
        seq {
            yield! visitValSig synValSig
        }
    | SynMemberSig.NestedType(synTypeDefnSig, range) ->
        visitSynTypeDefnSign synTypeDefnSig
    | SynMemberSig.ValField(synField, range) ->
        visitSynField synField

and visitTypeConstraint (x : SynTypeConstraint) : Stuff =
    match x with
    | SynTypeConstraint.WhereTyparIsValueType(typar, range) ->
        visitTypar typar
    | SynTypeConstraint.WhereTyparIsReferenceType(typar, range) ->
        visitTypar typar
    | SynTypeConstraint.WhereTyparIsUnmanaged(typar, range) ->
        visitTypar typar
    | SynTypeConstraint.WhereTyparSupportsNull(typar, range) ->
        visitTypar typar
    | SynTypeConstraint.WhereTyparIsComparable(typar, range) ->
        visitTypar typar
    | SynTypeConstraint.WhereTyparIsEquatable(typar, range) ->
        visitTypar typar
    | SynTypeConstraint.WhereTyparDefaultsToType(typar, typeName: SynType, range) ->
        seq {
            yield! visitTypar typar
            yield! visitType typeName
        }
    | SynTypeConstraint.WhereTyparSubtypeOfType(typar, typeName: SynType, range) ->
        seq {
            yield! visitTypar typar
            yield! visitType typeName
        }
    | SynTypeConstraint.WhereTyparSupportsMember(typars: SynType, memberSig: SynMemberSig, range) ->
        seq {
            yield! visitType typars
            yield! visitMemberSig memberSig
        }
    | SynTypeConstraint.WhereTyparIsEnum(typar, typeArgs: SynType list, range) ->
        seq {
            yield! visitTypar typar
            yield! visitSynTypes typeArgs
        }
    | SynTypeConstraint.WhereTyparIsDelegate(typar, typeArgs: SynType list, range) ->
        seq {
            yield! visitTypar typar
            yield! visitSynTypes typeArgs
        }
    | SynTypeConstraint.WhereSelfConstrained(selfConstraint, range) ->
        visitType selfConstraint

and visitType (x : SynType) : Stuff =
    match x with
    | SynType.Anon range ->
        []
    | SynType.App(typeName, rangeOption, typeArgs, commaRanges, greaterRange, isPostfix, range) ->
        seq {
            yield! visitType typeName
            yield! typeArgs |> Seq.collect visitType
        }
    | SynType.Array(rank, elementType, range) ->
        visitType elementType
    | SynType.Fun(argType, returnType, range, synTypeFunTrivia) ->
        seq {
            yield! visitType argType
            yield! visitType returnType
        }
    | SynType.Or(lhsType, rhsType, range, synTypeOrTrivia) ->
        seq {
            yield! visitType lhsType
            yield! visitType rhsType
        }
    | SynType.Paren(innerType, range) ->
        visitType innerType
    | SynType.Tuple(isStruct, synTupleTypeSegments, range) ->
        visitSynTupleTypeSegments synTupleTypeSegments
    | SynType.Var(synTypar, range) ->
        visitTypar synTypar
    | SynType.AnonRecd(isStruct, fields, range) ->
        fields |> Seq.collect (fun (id, f) -> visitType f)
    | SynType.HashConstraint(innerType, range) ->
        visitType innerType
    | SynType.LongIdent synLongIdent ->
        visitSynLongIdent synLongIdent
    | SynType.MeasureDivide(synType, divisor, range) ->
        seq {
            yield! visitType synType
            yield! visitType divisor
        }
    | SynType.MeasurePower(baseMeasure, synRationalConst, range) ->
        seq {
            yield! visitType baseMeasure
            yield! visitSynRationalConst synRationalConst
        }
    | SynType.SignatureParameter(synAttributeLists, optional, identOption, usedType, range) ->
        seq {
            yield! visitSynAttributeLists synAttributeLists
            yield! visitType usedType
        }
    | SynType.StaticConstant(synConst, range) ->
        visitSynConst synConst
    | SynType.LongIdentApp(typeName, synLongIdent, rangeOption, typeArgs, commaRanges, greaterRange, range) ->
        seq {
            yield! visitType typeName
            yield! visitSynLongIdent synLongIdent
            yield! visitSynTypes typeArgs
        }
    | SynType.StaticConstantExpr(synExpr, range) ->
        seq {
            yield! visitExpr synExpr
        }
    | SynType.StaticConstantNamed(synType, value, range) ->
        seq {
            yield! visitType synType
            yield! visitType value
        }
    | SynType.WithGlobalConstraints(typeName, synTypeConstraints, range) ->
        seq {
            yield! visitType typeName
            yield! visitTypeConstraints synTypeConstraints
        }

and visitPreXmlDoc (doc : FSharp.Compiler.Xml.PreXmlDoc) : Stuff =
    [] // TODO Check

and visitSynAccess (x : SynAccess) : Stuff =
    [] // TODO check

and visitSynField (x : SynField) : Stuff =
    match x with
    | SynField.SynField(synAttributeLists, isStatic, identOption, fieldType, isMutable, preXmlDoc, synAccessOption, range, synFieldTrivia) ->
        seq {
            yield! visitSynAttributeLists synAttributeLists
            yield! visitType fieldType
            yield! visitPreXmlDoc preXmlDoc
            match synAccessOption with | Some access -> yield! visitSynAccess access | None -> ()
        }

and visitSynFields (x : SynField list) : Stuff =
    Seq.collect visitSynField x

and visitSynUnionCaseKind (x : SynUnionCaseKind) : Stuff =
    match x with
    | SynUnionCaseKind.Fields synFields ->
        
        [] // TODO
    | SynUnionCaseKind.FullType(fullType, fullTypeInfo) ->
        [] // TODO

and visitSynUnionCase (x : SynUnionCase) : Stuff =
    match x with
    | SynUnionCase.SynUnionCase(synAttributeLists, synIdent, synUnionCaseKind, preXmlDoc, synAccessOption, range, synUnionCaseTrivia) ->
        seq {
            yield! visitSynAttributeLists synAttributeLists
            yield! visitSynIdent synIdent
            yield! visitSynUnionCaseKind synUnionCaseKind
        }

and visitSynExceptionDefnRepr (x : SynExceptionDefnRepr) : Stuff =
    match x with
    | SynExceptionDefnRepr.SynExceptionDefnRepr(synAttributeLists, synUnionCase, identsOption, preXmlDoc, synAccessOption, range) ->
        seq {
            yield! visitSynAttributeLists synAttributeLists
            yield! visitSynUnionCase synUnionCase
            match identsOption with | Some ident -> yield! visitLongIdent ident | None -> ()
            yield! visitPreXmlDoc preXmlDoc
            match synAccessOption with | Some synAccess -> yield! visitSynAccess synAccess | None -> ()
        }

and visitEnumCase (x : SynEnumCase) : Stuff =
    match x with
    | SynEnumCase(synAttributeLists, synIdent, synConst, valueRange, preXmlDoc, range, synEnumCaseTrivia) ->
        seq {
            yield! visitSynAttributeLists synAttributeLists
            yield! visitSynIdent synIdent
            yield! visitSynConst synConst
            yield! visitPreXmlDoc preXmlDoc
        }

and visitMulti (f) (items) : Stuff = Seq.collect f items

and visitEnumCases = visitMulti visitEnumCase

and visitSynUnionCases = visitMulti visitSynUnionCase

and visitParserDetail (x : ParserDetail) : Stuff =
    []

and visitTypeDefnSimpleRepr (x : SynTypeDefnSimpleRepr) : Stuff =
    match x with
    | SynTypeDefnSimpleRepr.Enum(synEnumCases, range) ->
        visitEnumCases synEnumCases
    | SynTypeDefnSimpleRepr.Exception synExceptionDefnRepr ->
        visitSynExceptionDefnRepr synExceptionDefnRepr
    | SynTypeDefnSimpleRepr.General(synTypeDefnKind, inherits, slotsigs, synFields, isConcrete, isIncrClass, implicitCtorSynPats, range) ->
        seq {
            yield! visitSynTypeDefnKind synTypeDefnKind
            let inheritTypes = inherits |> List.map (fun (t, range, ident) -> t)
            yield! visitSynTypes inheritTypes
        }
    | SynTypeDefnSimpleRepr.None range ->
        []
    | SynTypeDefnSimpleRepr.Record(synAccessOption, recordFields, range) ->
        seq {
            match synAccessOption with | Some access -> yield! visitSynAccess access | None -> ()
            yield! visitSynFields recordFields
        }
    | SynTypeDefnSimpleRepr.Union(synAccessOption, synUnionCases, range) ->
        seq {
            match synAccessOption with | Some access -> yield! visitSynAccess access | None -> ()
            yield! visitSynUnionCases synUnionCases
        }
    | SynTypeDefnSimpleRepr.TypeAbbrev(parserDetail, rhsType, range) ->
        seq {
            yield! visitParserDetail parserDetail
            yield! visitType rhsType
        }
    | SynTypeDefnSimpleRepr.LibraryOnlyILAssembly(ilType, range) ->
        []

and visitSynArgInfo (x : SynArgInfo) : Stuff =
    match x with
    | SynArgInfo(synAttributeLists, optional, identOption) ->
        visitSynAttributeLists synAttributeLists

and visitSynValInfo (x : SynValInfo) : Stuff =
    match x with
    | SynValInfo(curriedArgInfos, synArgInfo) ->
        seq {
            yield! curriedArgInfos |> Seq.concat |> Seq.collect visitSynArgInfo
            yield! visitSynArgInfo synArgInfo
        }

and visitSynTypeDefnKind (x : SynTypeDefnKind) : Stuff =
    match x with
    | SynTypeDefnKind.Delegate(synType, synValInfo) ->
        seq {
            yield! visitType synType
            yield! visitSynValInfo synValInfo
        }
    | SynTypeDefnKind.Abbrev
    | SynTypeDefnKind.Augmentation _
    | SynTypeDefnKind.Class
    | SynTypeDefnKind.Interface
    | SynTypeDefnKind.Opaque
    | SynTypeDefnKind.Record
    | SynTypeDefnKind.Struct
    | SynTypeDefnKind.Union
    | SynTypeDefnKind.Unspecified
    | SynTypeDefnKind.IL ->
        []

and visitSynTypeDefnRepr (x : SynTypeDefnRepr) : Stuff =
    match x with
    | SynTypeDefnRepr.Exception synExceptionDefnRepr ->
        visitSynExceptionDefnRepr synExceptionDefnRepr
    | SynTypeDefnRepr.Simple(synTypeDefnSimpleRepr, range) ->
        visitTypeDefnSimpleRepr synTypeDefnSimpleRepr
    | SynTypeDefnRepr.ObjectModel(synTypeDefnKind, synMemberDefns, range) ->
        seq {
            yield! visitSynTypeDefnKind synTypeDefnKind
            yield! visitSynMemberDefns synMemberDefns
        }

and visitSynValSig (x : SynValSig) : Stuff =
    match x with
    | SynValSig(synAttributeLists, synIdent, synValTyparDecls, synType, synValInfo, isInline, isMutable, preXmlDoc, synAccessOption, synExprOption, range, synValSigTrivia) ->
        seq {
            yield! visitSynAttributeLists synAttributeLists
            yield! visitSynIdent synIdent
            yield! visitSynValTyparDecls synValTyparDecls
            yield! visitType synType
            yield! visitSynValInfo synValInfo
            yield! visitPreXmlDoc preXmlDoc
            match synAccessOption with | Some access -> yield! visitSynAccess access | None -> ()
            match synExprOption with | Some expr -> yield! visitExpr expr | None -> ()
        }

and visitSynMemberKind (x : SynMemberKind) : Stuff =
    []

and visitSynMemberFlags (x : SynMemberFlags) : Stuff =
    []
    
and visitSynSimplePats (x : SynSimplePats) : Stuff =
    match x with
    | SynSimplePats.Typed(synSimplePats, targetType, range) ->
        seq {
            yield! visitSynSimplePats synSimplePats
            yield! visitType targetType
        }
    | SynSimplePats.SimplePats(synSimplePats, range) ->
        Seq.collect visitSynSimplePat synSimplePats

and visitSynSimplePatAlternativeIdInfoRef (x : SynSimplePatAlternativeIdInfo ref) : Stuff =
    [] // TODO Check

and visitSynSimplePat (x : SynSimplePat) : Stuff =
    match x with
    | SynSimplePat.Attrib(synSimplePat, synAttributeLists, range) ->
        seq {
            yield! visitSynSimplePat synSimplePat
            yield! visitSynAttributeLists synAttributeLists
        }
    | SynSimplePat.Id(ident, synSimplePatAlternativeIdInfoRefOption, isCompilerGenerated, isThisVal, isOptional, range) ->
        seq {
            match synSimplePatAlternativeIdInfoRefOption with | Some info -> yield! visitSynSimplePatAlternativeIdInfoRef info | None -> ()
        }
    | SynSimplePat.Typed(synSimplePat, targetType, range) ->
        seq {
            yield! visitSynSimplePat synSimplePat
            yield! visitType targetType
        }

and visitSynMemberDefn (defn : SynMemberDefn) : Stuff =
    match defn with
    | SynMemberDefn.Inherit(baseType, identOption, range) ->
        visitType baseType
    | SynMemberDefn.Interface(interfaceType, withKeyword, synMemberDefnsOption, range) ->
        seq {
            yield! visitType interfaceType
            match synMemberDefnsOption with | Some defns -> yield! visitSynMemberDefns defns | None -> ()
        }
    | SynMemberDefn.Member(memberDefn, range) ->
        visitSynBinding memberDefn
    | SynMemberDefn.Open(synOpenDeclTarget, range) ->
        visitSynOpenDeclTarget synOpenDeclTarget
    | SynMemberDefn.AbstractSlot(synValSig, synMemberFlags, range) ->
        seq {
            yield! visitSynValSig synValSig
            yield! visitSynMemberFlags synMemberFlags
        }
    | SynMemberDefn.AutoProperty(synAttributeLists, isStatic, ident, synTypeOption, synMemberKind, synMemberFlags, memberFlagsForSet, preXmlDoc, synAccessOption, synExpr, range, synMemberDefnAutoPropertyTrivia) ->
        seq {
            yield! visitSynAttributeLists synAttributeLists
            match synTypeOption with | Some synType -> yield! visitType synType | None -> ()
            yield! visitSynMemberKind synMemberKind
            yield! visitSynMemberFlags synMemberFlags
            yield! visitSynMemberFlags memberFlagsForSet
            yield! visitPreXmlDoc preXmlDoc
            match synAccessOption with | Some synAccess -> yield! visitSynAccess synAccess | None -> ()
            yield! visitExpr synExpr
        }
    | SynMemberDefn.ImplicitCtor(synAccessOption, synAttributeLists, synSimplePats, selfIdentifier, preXmlDoc, range) ->
        seq {
            match synAccessOption with | Some synAccess -> yield! visitSynAccess synAccess | None -> ()
            yield! visitSynAttributeLists synAttributeLists
            yield! visitSynSimplePats synSimplePats
        }
    | SynMemberDefn.ImplicitInherit(inheritType, inheritArgs, inheritAlias, range) ->
        seq {
            yield! visitType inheritType
            yield! visitExpr inheritArgs
        }
    | SynMemberDefn.LetBindings(synBindings, isStatic, isRecursive, range) ->
        visitBindings synBindings
    | SynMemberDefn.NestedType(synTypeDefn, synAccessOption, range) ->
        seq {
            yield! visitSynTypeDefn synTypeDefn
            match synAccessOption with | Some access -> yield! visitSynAccess access | None -> ()
        }
    | SynMemberDefn.ValField(fieldInfo, range) ->
        visitSynField fieldInfo
    | SynMemberDefn.GetSetMember(memberDefnForGet, memberDefnForSet, range, synMemberGetSetTrivia) ->
        seq {
            match memberDefnForGet with | Some binding -> yield! visitSynBinding binding | None -> ()
            match memberDefnForSet with | Some binding -> yield! visitSynBinding binding | None -> ()
        }

and visitSynMemberDefns (defns : SynMemberDefn list) : Stuff =
    Seq.collect visitSynMemberDefn defns

and visitSynTypeDefn (defn : SynTypeDefn) : Stuff =
    match defn with
    | SynTypeDefn.SynTypeDefn(synComponentInfo, synTypeDefnRepr, synMemberDefns, synMemberDefnOption, range, synTypeDefnTrivia) ->
        seq {
            yield! visitSynComponentInfo synComponentInfo
            yield! visitSynTypeDefnRepr synTypeDefnRepr
            yield! visitSynMemberDefns synMemberDefns
            match synMemberDefnOption with Some defn -> yield! visitSynMemberDefn defn | None -> ()
        }

and visitSynTypeDefns (defns : SynTypeDefn list) : Stuff =
    Seq.collect visitSynTypeDefn defns

and visitSynExceptionDefn (x : SynExceptionDefn) : Stuff =
    match x with
    | SynExceptionDefn(synExceptionDefnRepr, withKeyword, synMemberDefns, range) ->
        seq {
            yield! visitSynExceptionDefnRepr synExceptionDefnRepr
            yield! visitSynMemberDefns synMemberDefns
        }

and visitSynValData (x : SynValData) : Stuff =
    match x with
    | SynValData(synMemberFlagsOption, synValInfo, thisIdOpt) ->
        seq {
            match synMemberFlagsOption with | Some flags -> yield! visitSynMemberFlags flags | None -> ()
            yield! visitSynValInfo synValInfo
        } 

and visitSynPats (x : SynPat list) : Stuff =
    Seq.collect visitPat x

and visitPat (x : SynPat) : Stuff =
    match x with
    | SynPat.Ands(synPats, range) ->
        visitSynPats synPats
    | SynPat.As(lhsPat, rhsPat, range) ->
        visitSynPats [lhsPat; rhsPat]
    | SynPat.Attrib(synPat, synAttributeLists, range) ->
        seq {
            yield! visitPat synPat
            yield! visitSynAttributeLists synAttributeLists
        }
    | SynPat.Const(synConst, range) ->
        visitSynConst synConst
    | SynPat.Named(synIdent, isThisVal, synAccessOption, range) ->
        seq {
            yield! visitSynIdent synIdent
            match synAccessOption with | Some access -> yield! visitSynAccess access | None -> ()
        }
    | SynPat.Null range ->
        []
    | SynPat.Or(lhsPat, rhsPat, range, synPatOrTrivia) ->
        seq {
            yield! visitSynPats [lhsPat; rhsPat]
        }
    | SynPat.Paren(synPat, range) ->
        visitPat synPat
    | SynPat.Record(fieldPats, range) ->
        fieldPats
        |> Seq.collect (fun ((longId, id), range, pat) ->
            seq {
                yield! visitLongIdent longId
                yield! visitPat pat
            }
        )
    | SynPat.Tuple(isStruct, elementPats, range) ->
        visitSynPats elementPats
    | SynPat.Typed(synPat, targetType, range) ->
        seq {
            yield! visitPat synPat
            yield! visitType targetType
        }
    | SynPat.Wild range ->
        []
    | SynPat.InstanceMember(thisId, memberId, identOption, synAccessOption, range) ->
        seq {
            match synAccessOption with | Some access -> yield! visitSynAccess access | None -> ()
        }
    | SynPat.IsInst(synType, range) ->
        visitType synType
    | SynPat.ListCons(lhsPat, rhsPat, range, synPatListConsTrivia) ->
        seq {
            yield! visitPat lhsPat
            yield! visitPat rhsPat
        }
    | SynPat.LongIdent(synLongIdent, identOption, synValTyparDeclsOption, synArgPats, synAccessOption, range) ->
        seq {
            yield! visitSynLongIdent synLongIdent
            match synValTyparDeclsOption with | Some decls -> yield! visitSynValTyparDecls decls | None -> ()
            yield! visitSynArgPats synArgPats
            match synAccessOption with | Some access -> yield! visitSynAccess access | None -> ()
        }
    | SynPat.OptionalVal(ident, range) ->
        []
    | SynPat.QuoteExpr(synExpr, range) ->
        visitExpr synExpr
    | SynPat.ArrayOrList(isArray, elementPats, range) ->
        visitSynPats elementPats
    | SynPat.DeprecatedCharRange(startChar, endChar, range) ->
        []
    | SynPat.FromParseError(synPat, range) ->
        visitPat synPat

and visitBindingReturnInfo (x : SynBindingReturnInfo) : Stuff =
    match x with
    | SynBindingReturnInfo(typeName, range, synAttributeLists) ->
        seq {
            yield! visitType typeName
            yield! visitSynAttributeLists synAttributeLists
        }

and visitSynBinding (x : SynBinding) : Stuff =
    match x with
    | SynBinding.SynBinding(synAccessOption, synBindingKind, isInline, isMutable, synAttributeLists, preXmlDoc, synValData, headPat, synBindingReturnInfoOption, synExpr, range, debugPointAtBinding, synBindingTrivia) ->
        seq {
            match synAccessOption with | Some access -> yield! visitSynAccess access | None -> ()
            yield! visitSynAttributeLists synAttributeLists
            yield! visitPreXmlDoc preXmlDoc
            yield! visitSynValData synValData
            yield! visitPat headPat
            match synBindingReturnInfoOption with | Some info -> yield! visitBindingReturnInfo info | None -> ()
            yield! visitExpr synExpr
        }

and visitBindings (bindings : SynBinding list) : Stuff =
    Seq.collect visitSynBinding bindings

and visitSynOpenDeclTarget (target : SynOpenDeclTarget) : Stuff =
    match target with
    | SynOpenDeclTarget.Type(typeName, range) ->
        visitType typeName
    | SynOpenDeclTarget.ModuleOrNamespace(synLongIdent, range) ->
        visitSynLongIdent synLongIdent
        |> Seq.map (fun s -> {s with Kind = Kind.ModuleOrNamespace})

and visitSynComponentInfo (info : SynComponentInfo) : Stuff =
    match info with
    | SynComponentInfo(synAttributeLists, synTyparDeclsOption, synTypeConstraints, longId, preXmlDoc, preferPostfix, synAccessOption, range) ->
        seq {
            yield! visitSynAttributeLists synAttributeLists
            match synTyparDeclsOption with | Some decls -> yield! visitSynTyparDecls decls | None -> ()
            // Don't include this as it's a module definition rather than reference
            // yield! visitLongIdent longId
            yield! visitPreXmlDoc preXmlDoc
            match synAccessOption with | Some access -> yield! visitSynAccess access | None -> ()
        }

and visitLongIdent (ident : LongIdent) : Stuff =
    [{Ident = ident; Kind = Kind.Type}]
    
and visitSynLongIdent (ident : SynLongIdent) : Stuff  =
    [{Ident = ident.LongIdent; Kind = Kind.Type}]
   
and visitSynMatchClause (x : SynMatchClause) : Stuff =
    match x with
    | SynMatchClause(synPat, synExprOption, resultExpr, range, debugPointAtTarget, synMatchClauseTrivia) ->
        seq {
            yield! visitPat synPat
            match synExprOption with | Some expr -> yield! visitExpr expr | None -> ()
            yield! visitExpr resultExpr
        }
   
and visitSynMatchClauses = visitMulti visitSynMatchClause
      
and visitExprOnly (x : SeqExprOnly) : Stuff =
    []
      
and visitSynInterpolatedStringPart (x : SynInterpolatedStringPart) : Stuff =
    match x with
    | SynInterpolatedStringPart.String(value, range) ->
        []
    | SynInterpolatedStringPart.FillExpr(fillExpr, identOption) ->
        visitExpr fillExpr
      
and visitSynInterpolatedStringParts (x : SynInterpolatedStringPart list) : Stuff =
    Seq.collect visitSynInterpolatedStringPart x
      
and visitSynInterfaceImpl (x : SynInterfaceImpl) : Stuff =
    match x with
    | SynInterfaceImpl(interfaceTy, withKeyword, synBindings, synMemberDefns, range) ->
        seq {
            yield! visitType interfaceTy
            yield! visitBindings synBindings
            yield! visitSynMemberDefns synMemberDefns
        }
      
and visitSynInterfaceImpls (x : SynInterfaceImpl list) : Stuff =
    Seq.collect visitSynInterfaceImpl x
      
and visitSynArgPats (x : SynArgPats) : Stuff =
    match x with
    | SynArgPats.Pats synPats ->
        visitSynPats synPats
    | SynArgPats.NamePatPairs(tuples, range, synArgPatsNamePatPairsTrivia) ->
        tuples
        |> Seq.collect (fun (ident, range, pat) -> visitPat pat)
      
and visitSynMemberSig (x : SynMemberSig) : Stuff =
    match x with
    | SynMemberSig.Inherit(inheritedType, range) ->
        visitType inheritedType
    | SynMemberSig.Interface(interfaceType, range) ->
        visitType interfaceType
    | SynMemberSig.Member(synValSig, synMemberFlags, range) ->
        visitValSig synValSig
    | SynMemberSig.NestedType(synTypeDefnSig, range) ->
        visitSynTypeDefnSign synTypeDefnSig
    | SynMemberSig.ValField(synField, range) ->
        visitSynField synField
      
and visitExpr (expr : SynExpr) =
    let l = System.Collections.Generic.List<Item>()
    let go (items : Stuff) =
        l.AddRange(items)
        
    match expr with
    | SynExpr.App(exprAtomicFlag, isInfix, funcExpr, argExpr, range) ->
        visitExpr funcExpr |> go
        visitExpr argExpr |> go
    | SynExpr.Assert(synExpr, range) ->
        visitExpr synExpr |> go
    | SynExpr.Const(synConst, range) ->
        visitSynConst synConst |> go
    | SynExpr.Do(synExpr, range) ->
        visitExpr synExpr |> go
    | SynExpr.Downcast(synExpr, targetType, range) ->
        visitExpr synExpr |> go
        visitType targetType |> go
    | SynExpr.Dynamic(funcExpr, qmark, argExpr, range) ->
        visitExpr funcExpr |> go
        visitExpr argExpr |> go
    | SynExpr.Fixed(synExpr, range) ->
        visitExpr synExpr |> go
    | SynExpr.For(debugPointAtFor, debugPointAtInOrTo, ident, equalsRange, identBody, direction, synExpr, doBody, range) ->
        visitExpr identBody |> go
        visitExpr synExpr |> go
        visitExpr doBody |> go
    | SynExpr.Ident ident ->
        ()
    | SynExpr.Lambda(fromMethod, inLambdaSeq, synSimplePats, synExpr, tupleOption, range, synExprLambdaTrivia) ->
        visitSynSimplePats synSimplePats |> go
        visitExpr synExpr |> go
        match tupleOption with | Some (simplePats, expr) -> visitSynPats simplePats |> go; visitExpr expr |> go | None -> ()
    | SynExpr.Lazy(synExpr, range) ->
        visitExpr synExpr |> go
    | SynExpr.Match(debugPointAtBinding, synExpr, synMatchClauses, range, synExprMatchTrivia) ->
        visitExpr synExpr |> go
        visitSynMatchClauses synMatchClauses |> go
    | SynExpr.New(isProtected, targetType, synExpr, range) ->
        visitType targetType |> go
        visitExpr synExpr |> go
    | SynExpr.Null range ->
        ()
    | SynExpr.Paren(synExpr, leftParenRange, rightParenRange, range) ->
        visitExpr synExpr |> go
    | SynExpr.Quote(synExpr, isRaw, quotedExpr, isFromQueryExpression, range) ->
        visitExpr synExpr |> go
        visitExpr quotedExpr |> go
    | SynExpr.Record(tupleOption, copyInfo, synExprRecordFields, range) ->
        match tupleOption with
        | Some(synType, synExpr, range, tupleOption, range1) ->
            seq {
                yield! visitType synType
                yield! visitExpr synExpr
            }
            |> go
        | None ->
            ()
        match copyInfo with
        | Some(synExpr, tuple) ->
            visitExpr synExpr |> go
        | None ->
            ()
    | SynExpr.Sequential(debugPointAtSequential, isTrueSeq, synExpr, expr2, range) ->
        visitExpr synExpr |> go
        visitExpr expr2 |> go
    | SynExpr.Set(targetExpr, rhsExpr, range) ->
        visitExpr targetExpr |> go
        visitExpr rhsExpr |> go
    | SynExpr.Tuple(isStruct, synExprs, commaRanges, range) ->
        visitExprs synExprs |> go
    | SynExpr.Typar(synTypar, range) ->
        visitTypar synTypar |> go
    | SynExpr.Typed(synExpr, targetType, range) ->
        visitExpr synExpr |> go
        visitType targetType |> go
    | SynExpr.Upcast(synExpr, targetType, range) ->
        visitExpr synExpr |> go
        visitType targetType |> go
    | SynExpr.While(debugPointAtWhile, whileExpr, synExpr, range) ->
        visitExpr whileExpr |> go
        visitExpr synExpr |> go
    | SynExpr.AddressOf(isByref, synExpr, opRange, range) ->
        visitExpr synExpr |> go
    | SynExpr.AnonRecd(isStruct, tupleOption, recordFields, range) ->
        match tupleOption with
        | Some(synExpr, tuple) ->
            ()
        | None ->
            ()
        recordFields
        |> Seq.collect (fun (ident, range, f) -> visitExpr f)
        |> go
    | SynExpr.ComputationExpr(hasSeqBuilder, synExpr, range) ->
        visitExpr synExpr |> go
    | SynExpr.DebugPoint(debugPointAtLeafExpr, isControlFlow, innerExpr) ->
        visitExpr innerExpr |> go
    | SynExpr.DoBang(synExpr, range) ->
        visitExpr synExpr |> go
    | SynExpr.DotGet(synExpr, rangeOfDot, synLongIdent, range) ->
        visitExpr synExpr |> go
        visitSynLongIdent synLongIdent |> go
    | SynExpr.DotSet(targetExpr, synLongIdent, rhsExpr, range) ->
        visitExpr targetExpr |> go
        visitSynLongIdent synLongIdent |> go
        visitExpr rhsExpr |> go
    | SynExpr.ForEach(debugPointAtFor, debugPointAtInOrTo, seqExprOnly, isFromSource, synPat, enumExpr, bodyExpr, range) ->
        visitExprOnly seqExprOnly |> go
        visitPat synPat |> go
        visitExpr enumExpr |> go
        visitExpr bodyExpr |> go
    | SynExpr.ImplicitZero range ->
        ()
    | SynExpr.IndexRange(synExprOption, range, exprOption, range1, range2, range3) ->
        match synExprOption with | Some expr -> visitExpr expr |> go | None -> ()
        match exprOption with | Some expr -> visitExpr expr |> go | None -> ()
    | SynExpr.InferredDowncast(synExpr, range) ->
        visitExpr synExpr |> go
    | SynExpr.InferredUpcast(synExpr, range) ->
        visitExpr synExpr |> go
    | SynExpr.InterpolatedString(synInterpolatedStringParts, synStringKind, range) ->
        visitSynInterpolatedStringParts synInterpolatedStringParts |> go
    | SynExpr.JoinIn(lhsExpr, lhsRange, rhsExpr, range) ->
        visitExpr lhsExpr |> go
        visitExpr rhsExpr |> go
    | SynExpr.LongIdent(isOptional, synLongIdent, synSimplePatAlternativeIdInfoRefOption, range) ->
        visitSynLongIdent synLongIdent |> go
        match synSimplePatAlternativeIdInfoRefOption with
        | Some info -> visitSynSimplePatAlternativeIdInfoRef info |> go
        | None -> ()
    | SynExpr.MatchBang(debugPointAtBinding, synExpr, synMatchClauses, range, synExprMatchBangTrivia) ->
        visitExpr synExpr |> go
        visitSynMatchClauses synMatchClauses |> go
    | SynExpr.MatchLambda(isExnMatch, keywordRange, synMatchClauses, debugPointAtBinding, range) ->
        visitSynMatchClauses synMatchClauses |> go
    | SynExpr.ObjExpr(objType, tupleOption, withKeyword, synBindings, synMemberDefns, synInterfaceImpls, newExprRange, range) ->
        visitType objType |> go
        match tupleOption with
        | Some(synExpr, identOption) ->
            visitExpr synExpr |> go
        | None ->
            ()
        visitSynMemberDefns synMemberDefns |> go
        visitSynInterfaceImpls synInterfaceImpls |> go
    | SynExpr.TraitCall(supportTys, synMemberSig, argExpr, range) ->
        visitType supportTys |> go
        visitSynMemberSig synMemberSig |> go
        visitExpr argExpr |> go
    | SynExpr.TryFinally(tryExpr, finallyExpr, range, debugPointAtTry, debugPointAtFinally, synExprTryFinallyTrivia) ->
        visitExpr tryExpr |> go
        visitExpr finallyExpr |> go
    | SynExpr.TryWith(tryExpr, synMatchClauses, range, debugPointAtTry, debugPointAtWith, synExprTryWithTrivia) ->
        visitExpr tryExpr |> go
        visitSynMatchClauses synMatchClauses |> go
    | SynExpr.TypeApp(synExpr, lessRange, typeArgs, commaRanges, greaterRange, typeArgsRange, range) ->
        visitExpr synExpr |> go
        visitSynTypes typeArgs |> go
    | SynExpr.TypeTest(synExpr, targetType, range) ->
        visitExpr synExpr |> go
        visitType targetType |> go
    | SynExpr.ArbitraryAfterError(debugStr, range) ->
        ()
    | SynExpr.ArrayOrList(isArray, synExprs, range) ->
        visitExprs synExprs |> go
    | SynExpr.DotIndexedGet(objectExpr, indexArgs, dotRange, range) ->
        visitExpr objectExpr |> go
        visitExpr indexArgs |> go
    | SynExpr.DotIndexedSet(objectExpr, indexArgs, valueExpr, leftOfSetRange, dotRange, range) ->
        visitExpr objectExpr |> go
        visitExpr indexArgs |> go
        visitExpr valueExpr |> go
    | SynExpr.FromParseError(synExpr, range) ->
        visitExpr synExpr |> go
    | SynExpr.IfThenElse(synExpr, thenExpr, synExprOption, debugPointAtBinding, isFromErrorRecovery, range, synExprIfThenElseTrivia) ->
        visitExpr synExpr |> go
        visitExpr thenExpr |> go
        match synExprOption with | Some expr -> visitExpr expr |> go | None -> ()
    | SynExpr.IndexFromEnd(synExpr, range) ->
        visitExpr synExpr |> go
    | SynExpr.LetOrUse(isRecursive, isUse, synBindings, synExpr, range, synExprLetOrUseTrivia) ->
        visitBindings synBindings |> go
        visitExpr synExpr |> go
    | SynExpr.LongIdentSet(synLongIdent, synExpr, range) ->
        visitSynLongIdent synLongIdent |> go
        visitExpr synExpr |> go
    | SynExpr.YieldOrReturn(flags, synExpr, range) ->
        visitExpr synExpr |> go
    | SynExpr.ArrayOrListComputed(isArray, synExpr, range) ->
        visitExpr synExpr |> go
    | SynExpr.LetOrUseBang(debugPointAtBinding, isUse, isFromSource, synPat, synExpr, synExprAndBangs, body, range, synExprLetOrUseBangTrivia) ->
        visitPat synPat |> go
        visitExpr synExpr |> go
        Seq.collect visitExprAndBang synExprAndBangs |> go
        visitExpr body |> go
    | SynExpr.LibraryOnlyStaticOptimization(synStaticOptimizationConstraints, synExpr, optimizedExpr, range) ->
        visitStaticOptimizationConstraints synStaticOptimizationConstraints |> go
        visitExpr synExpr |> go
        visitExpr optimizedExpr |> go
    | SynExpr.NamedIndexedPropertySet(synLongIdent, synExpr, expr2, range) ->
        visitSynLongIdent synLongIdent |> go
        visitExpr synExpr |> go
        visitExpr expr2 |> go
        failwith unsupported
    | SynExpr.SequentialOrImplicitYield(debugPointAtSequential, synExpr, expr2, ifNotStmt, range) ->
        visitExpr synExpr |> go
        visitExpr expr2 |> go
        visitExpr ifNotStmt |> go
    | SynExpr.YieldOrReturnFrom(flags, synExpr, range) ->
        visitExpr synExpr |> go
    | SynExpr.DotNamedIndexedPropertySet(targetExpr, synLongIdent, argExpr, rhsExpr, range) ->
        visitExpr targetExpr |> go
        visitSynLongIdent synLongIdent |> go
        visitExpr argExpr |> go
        visitExpr rhsExpr |> go
    | SynExpr.LibraryOnlyILAssembly(ilCode, typeArgs, synExprs, synTypes, range) ->
        visitSynTypes typeArgs |> go
        visitExprs synExprs |> go
        visitSynTypes synTypes |> go
    | SynExpr.DiscardAfterMissingQualificationAfterDot(synExpr, range) ->
        visitExpr synExpr |> go
    | SynExpr.LibraryOnlyUnionCaseFieldGet(synExpr, longId, fieldNum, range) ->
        visitExpr synExpr |> go
        visitLongIdent longId |> go
    | SynExpr.LibraryOnlyUnionCaseFieldSet(synExpr, longId, fieldNum, rhsExpr, range) ->
        visitExpr synExpr |> go
        visitLongIdent longId |> go
        visitExpr rhsExpr |> go
    
    l
    
and visitStaticOptimizationConstraint (x : SynStaticOptimizationConstraint) : Stuff =
    match x with
    | SynStaticOptimizationConstraint.WhenTyparIsStruct(synTypar, range) ->
        visitTypar synTypar
    | SynStaticOptimizationConstraint.WhenTyparTyconEqualsTycon(synTypar, rhsType, range) ->
        seq {
            yield! visitTypar synTypar
            yield! visitType rhsType
        }
    
and visitStaticOptimizationConstraints = visitMulti visitStaticOptimizationConstraint
    
and visitExprAndBang (x : SynExprAndBang) : Stuff =
    match x with
    | SynExprAndBang(debugPointAtBinding, isUse, isFromSource, synPat, synExpr, range, synExprAndBangTrivia) ->
        seq {
            yield! visitPat synPat
            yield! visitExpr synExpr
        }
    
and visitExprs = visitMulti visitExpr
    
and visitSynAttribute (attribute : SynAttribute) : Stuff  =
    seq {
        yield! visitSynLongIdent attribute.TypeName
        yield! visitExpr attribute.ArgExpr
    }
                
and visitSynAttributeList (attributeList : SynAttributeList) : Stuff  =
    attributeList.Attributes
    |> Seq.collect visitSynAttribute

and visitSynAttributeLists (attributeLists : SynAttributeList list) : Stuff  =
    Seq.collect visitSynAttributeList attributeLists

and visitSynModuleOrNamespace (x : SynModuleOrNamespace) : Stuff  =
    match x with
    | SynModuleOrNamespace.SynModuleOrNamespace(longId, isRecursive, synModuleOrNamespaceKind, synModuleDecls, preXmlDoc, synAttributeLists, synAccessOption, range, synModuleOrNamespaceTrivia) ->
        seq {
            // Don't include 'longId' as that's module definition rather than reference
            yield! synModuleDecls |> Seq.collect visitSynModuleDecl
            yield! visitSynAttributeLists synAttributeLists 
        }

and visit (input : ParsedInput) : Stuff  =
    match input with
    | ParsedInput.SigFile _ -> failwith "Signature files are not currently supported"
    | ParsedInput.ImplFile(ParsedImplFileInput(fileName, isScript, qualifiedNameOfFile, scopedPragmas, parsedHashDirectives, synModuleOrNamespaces, flags, parsedImplFileInputTrivia)) ->
        synModuleOrNamespaces
        |> Seq.collect visitSynModuleOrNamespace

let topModuleOrNamespace (input : ParsedInput) =
    match input with
    | ParsedInput.ImplFile f ->
        match f.Contents with
        | [] -> failwith "No modules or namespaces"
        | first :: rest ->
            match first with
            | SynModuleOrNamespace(longId, isRecursive, synModuleOrNamespaceKind, synModuleDecls, preXmlDoc, synAttributeLists, synAccessOption, range, synModuleOrNamespaceTrivia) ->
                longId
    | ParsedInput.SigFile _ ->
        failwith "Sig files not supported atm"

[<Test>]
let ``Single SynEnumCase contains range of constant`` () =
    let parseResults = 
        getParseResults
            """

module A1 = let a = 3
module A2 = let a = 3
module A3 = let a = 3
module A4 =
    
    type AAttribute(name : string) =
        inherit System.Attribute()
    
    let a = 3
    module A1 =
        let a = 3
    type X = int * int
    type Y = Y of int

module B =
    open A2
    let b = [|
        A1.a
        A2.a
        A3.a
    |]
    let c : A4.X = 2,2
    [<A4.A("name")>]
    let d : A4.Y = A4.Y 2
    type Z =
        {
            X : A4.X
            Y : A4.Y
        }

let c = A4.a
let d = A4.A1.a
open A4
let e = A1.a
open A1
let f = a
"""

    let stuff = visit parseResults
    let top = topModuleOrNamespace parseResults
    printfn $"%+A{top}"
    printfn $"%+A{stuff}"
    ()

[<Test>]
let ``Test two`` () =
    
    let A =
        """
module A
open B
let x = B.x
"""
    let B =
        """
module B
let x = 3
"""
    
    let parsedA = getParseResults A
    let visitedA = visit parsedA
    let parsedB = getParseResults B
    let topB = topModuleOrNamespace parsedB 
    printfn $"Top B: %+A{topB}"
    printfn $"A refs: %+A{visitedA}"
    ()


[<Test>]
let ``Test big`` () =
    let code = System.IO.File.ReadAllText("Big.fs")
    let parsedA = getParseResults code
    let visitedA = visit parsedA
    printfn $"A refs: %+A{visitedA}"
