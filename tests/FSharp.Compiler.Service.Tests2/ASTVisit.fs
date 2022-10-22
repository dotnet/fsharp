module FSharp.Compiler.Service.Tests2.ASTVisit

open FSharp.Compiler.Syntax
open FSharp.Compiler.SyntaxTrivia

let unsupported = "unsupported"
type ReferenceKind =
    | Type
    | ModuleOrNamespace

/// Reference to a module or type, found in the AST
type Reference =
    {
        Ident : LongIdent
        Kind : ReferenceKind
    }
    
type Abbreviation =
    {
        Alias : Ident
        Target : LongIdent
    }
    
/// Reference to a module or type, found in the AST
type ReferenceOrAbbreviation =
    | Reference of Reference
    | Abbreviation of Abbreviation
    
type private References = ReferenceOrAbbreviation seq

let rec visitSynModuleDecl (decl : SynModuleDecl) : References =
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
        [ReferenceOrAbbreviation.Abbreviation({Alias = ident; Target = longId})]
    | SynModuleDecl.NamespaceFragment synModuleOrNamespace ->
        visitSynModuleOrNamespace synModuleOrNamespace
    | SynModuleDecl.NestedModule(synComponentInfo, isRecursive, synModuleDecls, isContinuing, range, synModuleDeclNestedModuleTrivia) ->
        seq {
            yield! visitSynComponentInfo synComponentInfo
            yield! Seq.collect visitSynModuleDecl synModuleDecls
            yield! visitSynModuleDeclNestedModuleTrivia synModuleDeclNestedModuleTrivia
        }

and visitSynModuleDeclNestedModuleTrivia (x : SynModuleDeclNestedModuleTrivia) : References =
    [] // TODO check

and visitHashDirective (x : ParsedHashDirective) : References =
    [] // TODO Check

and visitSynIdent (x : SynIdent) : References =
    [] // TODO Check

and visitSynTupleTypeSegment (x : SynTupleTypeSegment) : References =
    match x with
    | SynTupleTypeSegment.Slash range ->
        []
    | SynTupleTypeSegment.Star range ->
        []
    | SynTupleTypeSegment.Type typeName ->
        visitType typeName

and visitSynTupleTypeSegments (x : SynTupleTypeSegment list) : References =
    Seq.collect visitSynTupleTypeSegment x 

and visitTypar (x : SynTypar) : References =
    match x with
    | SynTypar.SynTypar(ident, typarStaticReq, isCompGen) ->
        [] // TODO check

and visitSynRationalConst (x : SynRationalConst) =
    [] // TODO check

and visitSynConst (x : SynConst) : References =
    [] // TODO Check

and visitSynTypes (x : SynType list) : References =
    Seq.collect visitType x

and visitTypeConstraints (x : SynTypeConstraint list) : References =
    Seq.collect visitTypeConstraint x

and visitSynTyparDecl (x : SynTyparDecl) : References =
    match x with
    | SynTyparDecl(synAttributeLists, synTypar) ->
        seq {
            yield! visitSynAttributeLists synAttributeLists
            yield! visitTypar synTypar
        }

and visitSynTyparDeclList (x : SynTyparDecl list) : References =
    Seq.collect visitSynTyparDecl x

and visitSynTyparDecls (x : SynTyparDecls) : References =
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

and visitSynValTyparDecls (x : SynValTyparDecls) : References =
    match x with
    | SynValTyparDecls(synTyparDeclsOption, canInfer) ->
        match synTyparDeclsOption with
        | Some decls -> visitSynTyparDecls decls
        | None -> []

and visitValSig (x : SynValSig) : References =
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

and visitSynTypeDefnSignRepr (x : SynTypeDefnSigRepr) : References =
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

and visitSynTypeDefnSign (x : SynTypeDefnSig) : References =
    match x with
    | SynTypeDefnSig(synComponentInfo, synTypeDefnSigRepr, synMemberSigs, range, synTypeDefnSigTrivia) ->
        seq {
            yield! visitSynComponentInfo synComponentInfo
            yield! visitSynTypeDefnSignRepr synTypeDefnSigRepr
        }

and visitMemberSig (x : SynMemberSig) : References =
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

and visitTypeConstraint (x : SynTypeConstraint) : References =
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

and visitType (x : SynType) : References =
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

and visitPreXmlDoc (doc : FSharp.Compiler.Xml.PreXmlDoc) : References =
    [] // TODO Check

and visitSynAccess (x : SynAccess) : References =
    [] // TODO check

and visitSynField (x : SynField) : References =
    match x with
    | SynField.SynField(synAttributeLists, isStatic, identOption, fieldType, isMutable, preXmlDoc, synAccessOption, range, synFieldTrivia) ->
        seq {
            yield! visitSynAttributeLists synAttributeLists
            yield! visitType fieldType
            yield! visitPreXmlDoc preXmlDoc
            match synAccessOption with | Some access -> yield! visitSynAccess access | None -> ()
        }

and visitSynFields (x : SynField list) : References =
    Seq.collect visitSynField x

and visitSynUnionCaseKind (x : SynUnionCaseKind) : References =
    match x with
    | SynUnionCaseKind.Fields synFields ->
        
        [] // TODO
    | SynUnionCaseKind.FullType(fullType, fullTypeInfo) ->
        [] // TODO

and visitSynUnionCase (x : SynUnionCase) : References =
    match x with
    | SynUnionCase.SynUnionCase(synAttributeLists, synIdent, synUnionCaseKind, preXmlDoc, synAccessOption, range, synUnionCaseTrivia) ->
        seq {
            yield! visitSynAttributeLists synAttributeLists
            yield! visitSynIdent synIdent
            yield! visitSynUnionCaseKind synUnionCaseKind
        }

and visitSynExceptionDefnRepr (x : SynExceptionDefnRepr) : References =
    match x with
    | SynExceptionDefnRepr.SynExceptionDefnRepr(synAttributeLists, synUnionCase, identsOption, preXmlDoc, synAccessOption, range) ->
        seq {
            yield! visitSynAttributeLists synAttributeLists
            yield! visitSynUnionCase synUnionCase
            match identsOption with | Some ident -> yield! visitLongIdent ident | None -> ()
            yield! visitPreXmlDoc preXmlDoc
            match synAccessOption with | Some synAccess -> yield! visitSynAccess synAccess | None -> ()
        }

and visitEnumCase (x : SynEnumCase) : References =
    match x with
    | SynEnumCase(synAttributeLists, synIdent, synConst, valueRange, preXmlDoc, range, synEnumCaseTrivia) ->
        seq {
            yield! visitSynAttributeLists synAttributeLists
            yield! visitSynIdent synIdent
            yield! visitSynConst synConst
            yield! visitPreXmlDoc preXmlDoc
        }

and visitMulti (f) (items) : References = Seq.collect f items

and visitEnumCases = visitMulti visitEnumCase

and visitSynUnionCases = visitMulti visitSynUnionCase

and visitParserDetail (x : ParserDetail) : References =
    []

and visitTypeDefnSimpleRepr (x : SynTypeDefnSimpleRepr) : References =
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

and visitSynArgInfo (x : SynArgInfo) : References =
    match x with
    | SynArgInfo(synAttributeLists, optional, identOption) ->
        visitSynAttributeLists synAttributeLists

and visitSynValInfo (x : SynValInfo) : References =
    match x with
    | SynValInfo(curriedArgInfos, synArgInfo) ->
        seq {
            yield! curriedArgInfos |> Seq.concat |> Seq.collect visitSynArgInfo
            yield! visitSynArgInfo synArgInfo
        }

and visitSynTypeDefnKind (x : SynTypeDefnKind) : References =
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

and visitSynTypeDefnRepr (x : SynTypeDefnRepr) : References =
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

and visitSynValSig (x : SynValSig) : References =
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

and visitSynMemberKind (x : SynMemberKind) : References =
    []

and visitSynMemberFlags (x : SynMemberFlags) : References =
    []
    
and visitSynSimplePats (x : SynSimplePats) : References =
    match x with
    | SynSimplePats.Typed(synSimplePats, targetType, range) ->
        seq {
            yield! visitSynSimplePats synSimplePats
            yield! visitType targetType
        }
    | SynSimplePats.SimplePats(synSimplePats, range) ->
        Seq.collect visitSynSimplePat synSimplePats

and visitSynSimplePatAlternativeIdInfoRef (x : SynSimplePatAlternativeIdInfo ref) : References =
    [] // TODO Check

and visitSynSimplePat (x : SynSimplePat) : References =
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

and visitSynMemberDefn (defn : SynMemberDefn) : References =
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

and visitSynMemberDefns (defns : SynMemberDefn list) : References =
    Seq.collect visitSynMemberDefn defns

and visitSynTypeDefn (defn : SynTypeDefn) : References =
    match defn with
    | SynTypeDefn.SynTypeDefn(synComponentInfo, synTypeDefnRepr, synMemberDefns, synMemberDefnOption, range, synTypeDefnTrivia) ->
        seq {
            yield! visitSynComponentInfo synComponentInfo
            yield! visitSynTypeDefnRepr synTypeDefnRepr
            yield! visitSynMemberDefns synMemberDefns
            match synMemberDefnOption with Some defn -> yield! visitSynMemberDefn defn | None -> ()
        }

and visitSynTypeDefns (defns : SynTypeDefn list) : References =
    Seq.collect visitSynTypeDefn defns

and visitSynExceptionDefn (x : SynExceptionDefn) : References =
    match x with
    | SynExceptionDefn(synExceptionDefnRepr, withKeyword, synMemberDefns, range) ->
        seq {
            yield! visitSynExceptionDefnRepr synExceptionDefnRepr
            yield! visitSynMemberDefns synMemberDefns
        }

and visitSynValData (x : SynValData) : References =
    match x with
    | SynValData(synMemberFlagsOption, synValInfo, thisIdOpt) ->
        seq {
            match synMemberFlagsOption with | Some flags -> yield! visitSynMemberFlags flags | None -> ()
            yield! visitSynValInfo synValInfo
        } 

and visitSynPats (x : SynPat list) : References =
    Seq.collect visitPat x

and visitPat (x : SynPat) : References =
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

and visitBindingReturnInfo (x : SynBindingReturnInfo) : References =
    match x with
    | SynBindingReturnInfo(typeName, range, synAttributeLists) ->
        seq {
            yield! visitType typeName
            yield! visitSynAttributeLists synAttributeLists
        }

and visitSynBinding (x : SynBinding) : References =
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

and visitBindings (bindings : SynBinding list) : References =
    Seq.collect visitSynBinding bindings

and visitSynOpenDeclTarget (target : SynOpenDeclTarget) : References =
    match target with
    | SynOpenDeclTarget.Type(typeName, range) ->
        visitType typeName
    | SynOpenDeclTarget.ModuleOrNamespace(synLongIdent, range) ->
        [ReferenceOrAbbreviation.Reference {Ident = synLongIdent.LongIdent; Kind = ReferenceKind.ModuleOrNamespace}] 

and visitSynComponentInfo (info : SynComponentInfo) : References =
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

and visitLongIdent (ident : LongIdent) : References =
    [ReferenceOrAbbreviation.Reference {Ident = ident; Kind = ReferenceKind.Type}]
    
and visitSynLongIdent (ident : SynLongIdent) : References  =
    [ReferenceOrAbbreviation.Reference {Ident = ident.LongIdent; Kind = ReferenceKind.Type}]
   
and visitSynMatchClause (x : SynMatchClause) : References =
    match x with
    | SynMatchClause(synPat, synExprOption, resultExpr, range, debugPointAtTarget, synMatchClauseTrivia) ->
        seq {
            yield! visitPat synPat
            match synExprOption with | Some expr -> yield! visitExpr expr | None -> ()
            yield! visitExpr resultExpr
        }
   
and visitSynMatchClauses = visitMulti visitSynMatchClause
      
and visitExprOnly (x : SeqExprOnly) : References =
    []
      
and visitSynInterpolatedStringPart (x : SynInterpolatedStringPart) : References =
    match x with
    | SynInterpolatedStringPart.String(value, range) ->
        []
    | SynInterpolatedStringPart.FillExpr(fillExpr, identOption) ->
        visitExpr fillExpr
      
and visitSynInterpolatedStringParts (x : SynInterpolatedStringPart list) : References =
    Seq.collect visitSynInterpolatedStringPart x
      
and visitSynInterfaceImpl (x : SynInterfaceImpl) : References =
    match x with
    | SynInterfaceImpl(interfaceTy, withKeyword, synBindings, synMemberDefns, range) ->
        seq {
            yield! visitType interfaceTy
            yield! visitBindings synBindings
            yield! visitSynMemberDefns synMemberDefns
        }
      
and visitSynInterfaceImpls (x : SynInterfaceImpl list) : References =
    Seq.collect visitSynInterfaceImpl x
      
and visitSynArgPats (x : SynArgPats) : References =
    match x with
    | SynArgPats.Pats synPats ->
        visitSynPats synPats
    | SynArgPats.NamePatPairs(tuples, range, synArgPatsNamePatPairsTrivia) ->
        tuples
        |> Seq.collect (fun (ident, range, pat) -> visitPat pat)
      
and visitSynMemberSig (x : SynMemberSig) : References =
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
    let l = System.Collections.Generic.List<ReferenceOrAbbreviation>()
    let go (items : References) =
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
        let mutable expr = expr2
        let mutable stop = false
        while not stop do
            match expr with
            | SynExpr.Sequential(debugPointAtSequential, isTrueSeq, synExpr, expr2, range) ->
                visitExpr synExpr |> go
                expr <- expr2
            | _ ->
                stop <- true
        visitExpr expr |> go
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
    
and visitStaticOptimizationConstraint (x : SynStaticOptimizationConstraint) : References =
    match x with
    | SynStaticOptimizationConstraint.WhenTyparIsStruct(synTypar, range) ->
        visitTypar synTypar
    | SynStaticOptimizationConstraint.WhenTyparTyconEqualsTycon(synTypar, rhsType, range) ->
        seq {
            yield! visitTypar synTypar
            yield! visitType rhsType
        }
    
and visitStaticOptimizationConstraints = visitMulti visitStaticOptimizationConstraint
    
and visitExprAndBang (x : SynExprAndBang) : References =
    match x with
    | SynExprAndBang(debugPointAtBinding, isUse, isFromSource, synPat, synExpr, range, synExprAndBangTrivia) ->
        seq {
            yield! visitPat synPat
            yield! visitExpr synExpr
        }
    
and visitExprs = visitMulti visitExpr
    
and visitSynAttribute (attribute : SynAttribute) : References  =
    seq {
        yield! visitSynLongIdent attribute.TypeName
        yield! visitExpr attribute.ArgExpr
    }
                
and visitSynAttributeList (attributeList : SynAttributeList) : References  =
    attributeList.Attributes
    |> Seq.collect visitSynAttribute

and visitSynAttributeLists (attributeLists : SynAttributeList list) : References  =
    Seq.collect visitSynAttributeList attributeLists

and visitSynModuleOrNamespace (x : SynModuleOrNamespace) : References  =
    match x with
    | SynModuleOrNamespace.SynModuleOrNamespace(longId, isRecursive, synModuleOrNamespaceKind, synModuleDecls, preXmlDoc, synAttributeLists, synAccessOption, range, synModuleOrNamespaceTrivia) ->
        seq {
            // Don't include 'longId' as that's module definition rather than reference
            yield! synModuleDecls |> Seq.collect visitSynModuleDecl
            yield! visitSynAttributeLists synAttributeLists 
        }
        
// Sigs

and visitSynMemberSigs = visitMulti visitSynMemberSig

and visitSynExceptionSig (x : SynExceptionSig) : References =
    match x with
    | SynExceptionSig(synExceptionDefnRepr, withKeyword, synMemberSigs, range) ->
        seq {
            yield! visitSynExceptionDefnRepr synExceptionDefnRepr
            yield! visitSynMemberSigs synMemberSigs
        }

and visitSynTypeDefnSigs = visitMulti visitSynTypeDefnSign

and visitSynModuleSigDecl (x : SynModuleSigDecl) : References =
    match x with
    | SynModuleSigDecl.Exception(synExceptionSig, range) ->
        visitSynExceptionSig synExceptionSig
    | SynModuleSigDecl.Open(synOpenDeclTarget, range) ->
        visitSynOpenDeclTarget synOpenDeclTarget
    | SynModuleSigDecl.Types(synTypeDefnSigs, range) ->
        visitSynTypeDefnSigs synTypeDefnSigs
    | SynModuleSigDecl.Val(synValSig, range) ->
        visitSynValSig synValSig
    | SynModuleSigDecl.HashDirective(parsedHashDirective, range) ->
        []
    | SynModuleSigDecl.ModuleAbbrev(ident, longId, range) ->
        // TODO Module abbrevation can break the algorithm.
        // We need to either give up when seeing this or handle it properly.
        //
        // Consider the following:
        // module A = module A1 = let x = 1
        // module B = A
        // let x = B.A1.x
        failwith "Module abbreviations are not currently supported"
    | SynModuleSigDecl.NamespaceFragment synModuleOrNamespaceSig ->
        visitSynModuleOrNamespaceSig synModuleOrNamespaceSig
    | SynModuleSigDecl.NestedModule(synComponentInfo, isRecursive, synModuleSigDecls, range, synModuleSigDeclNestedModuleTrivia) ->
        seq {
            yield! visitSynComponentInfo synComponentInfo
            yield! visitSynModuleSigDecls synModuleSigDecls
        }

and visitSynModuleSigDecls = visitMulti visitSynModuleSigDecl

and visitSynModuleOrNamespaceSig (x : SynModuleOrNamespaceSig) : References  =
    match x with
    | SynModuleOrNamespaceSig.SynModuleOrNamespaceSig(longId, isRecursive, synModuleOrNamespaceKind, synModuleDecls, preXmlDoc, synAttributeLists, synAccessOption, range, synModuleOrNamespaceTrivia) ->
        seq {
            // Don't include 'longId' as that's module definition rather than reference
            yield! synModuleDecls |> Seq.collect visitSynModuleSigDecl
            yield! visitSynAttributeLists synAttributeLists 
        }

and extractModuleRefs (input : ParsedInput) =
    match input with
    | ParsedInput.SigFile(ParsedSigFileInput(fileName, qualifiedNameOfFile, scopedPragmas, parsedHashDirectives, synModuleOrNamespaceSigs, parsedSigFileInputTrivia)) ->
        synModuleOrNamespaceSigs
        |> Seq.collect visitSynModuleOrNamespaceSig
        |> Seq.toArray
    | ParsedInput.ImplFile(ParsedImplFileInput(fileName, isScript, qualifiedNameOfFile, scopedPragmas, parsedHashDirectives, synModuleOrNamespaces, flags, parsedImplFileInputTrivia)) ->
        synModuleOrNamespaces
        |> Seq.collect visitSynModuleOrNamespace
        |> Seq.toArray

let mightHaveAutoOpen (synAttributeLists : SynAttributeList list) : bool =
    let attributes =
        synAttributeLists
        |> List.collect (fun attributes -> attributes.Attributes)
    match attributes with
    // No attributes found - no [<AutoOpen>] possible
    | [] -> false
    // Some attributes found - we can't know for sure if one of them is the AutoOpenAttribute (possibly hidden with a type alias), so we say 'yes'.
    | _ -> true

/// Extract the top-level module/namespaces from the AST
let topModuleOrNamespaces (input : ParsedInput) =
    match input with
    | ParsedInput.ImplFile f ->
        match f.Contents with
        | [] -> failwith $"No modules or namespaces found in file '{f.FileName}'"
        | items ->
            items
            |> List.map (fun item ->
                match item with
                | SynModuleOrNamespace(longId, isRecursive, synModuleOrNamespaceKind, synModuleDecls, preXmlDoc, synAttributeLists, synAccessOption, range, synModuleOrNamespaceTrivia) ->
                    if mightHaveAutoOpen synAttributeLists then
                        // Contents of a module that's potentially AutoOpen are available everywhere, so treat it as if it had no name ('root' module).
                        // This makes the dependency tracking algorithm detect it as a dependency for all further files.
                        LongIdent.Empty
                    else
                        longId
            )
    | ParsedInput.SigFile f ->
        match f.Contents with
        | [] -> failwith $"No modules or namespaces found in file '{f.FileName}'"
        | items ->
            items
            |> List.map (fun item ->
                match item with
                | SynModuleOrNamespaceSig(longId, isRecursive, synModuleOrNamespaceKind, synModuleDecls, preXmlDoc, synAttributeLists, synAccessOption, range, synModuleOrNamespaceTrivia) ->
                    if mightHaveAutoOpen synAttributeLists then
                        // Contents of a module that's potentially AutoOpen are available everywhere, so treat it as if it had no name ('root' module).
                        // This makes the dependency tracking algorithm detect it as a dependency for all further files.
                        LongIdent.Empty
                    else
                        longId
            )
    |> List.toArray
