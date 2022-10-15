module FSharp.Compiler.Service.Tests2.SyntaxTreeTests.TypeTests

open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Service.Tests.Common
open FSharp.Compiler.Syntax
open FSharp.Compiler.SyntaxTrivia
open NUnit.Framework

let unsupported = "unsupported"
type Stuff = SynLongIdent seq

let rec visitSynModuleDecl (decl : SynModuleDecl) : Stuff =
    // TODO
    match decl with
    | SynModuleDecl.Attributes(synAttributeLists, range) ->
        visitSynAttributeLists synAttributeLists
    | SynModuleDecl.Exception(synExceptionDefn, range) ->
        visitSynExceptionDefn synExceptionDefn
    | SynModuleDecl.Expr(synExpr, range) ->
        visitSynExpr synExpr
    | SynModuleDecl.HashDirective(parsedHashDirective, range) ->
        visitHashDirective parsedHashDirective
    | SynModuleDecl.Let(isRecursive, synBindings, range) ->
        visitSynBindings synBindings
    | SynModuleDecl.Open(synOpenDeclTarget, range) -> 
        visitSynOpenDeclTarget synOpenDeclTarget
    | SynModuleDecl.Types(synTypeDefns, range) ->
        visitSynTypeDefns synTypeDefns
    | SynModuleDecl.ModuleAbbrev(ident, longId, range) ->
        visitLongIdent longId
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
        visitSynType typeName

and visitSynTupleTypeSegments (x : SynTupleTypeSegment list) : Stuff =
    Seq.collect visitSynTupleTypeSegment x 

and visitSynTypar (x : SynTypar) : Stuff =
    match x with
    | SynTypar.SynTypar(ident, typarStaticReq, isCompGen) ->
        [] // TODO check

and visitSynRationalConst (x : SynRationalConst) =
    [] // TODO check

and visitSynConst (x : SynConst) : Stuff =
    [] // TODO Check

and visitSynTypes (x : SynType list) : Stuff =
    Seq.collect visitSynType x

and visitTypeConstraints (x : SynTypeConstraint list) : Stuff =
    Seq.collect visitTypeConstraint x

and visitSynTyparDecl (x : SynTyparDecl) : Stuff =
    match x with
    | SynTyparDecl(synAttributeLists, synTypar) ->
        seq {
            yield! visitSynAttributeLists synAttributeLists
            yield! visitSynTypar synTypar
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
        visitSynType inheritedType
    | SynMemberSig.Interface(interfaceType, range) ->
        visitSynType interfaceType
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
        visitSynTypar typar
    | SynTypeConstraint.WhereTyparIsReferenceType(typar, range) ->
        visitSynTypar typar
    | SynTypeConstraint.WhereTyparIsUnmanaged(typar, range) ->
        visitSynTypar typar
    | SynTypeConstraint.WhereTyparSupportsNull(typar, range) ->
        visitSynTypar typar
    | SynTypeConstraint.WhereTyparIsComparable(typar, range) ->
        visitSynTypar typar
    | SynTypeConstraint.WhereTyparIsEquatable(typar, range) ->
        visitSynTypar typar
    | SynTypeConstraint.WhereTyparDefaultsToType(typar, typeName: SynType, range) ->
        seq {
            yield! visitSynTypar typar
            yield! visitSynType typeName
        }
    | SynTypeConstraint.WhereTyparSubtypeOfType(typar, typeName: SynType, range) ->
        seq {
            yield! visitSynTypar typar
            yield! visitSynType typeName
        }
    | SynTypeConstraint.WhereTyparSupportsMember(typars: SynType, memberSig: SynMemberSig, range) ->
        seq {
            yield! visitSynType typars
            yield! visitMemberSig memberSig
        }
    | SynTypeConstraint.WhereTyparIsEnum(typar, typeArgs: SynType list, range) ->
        seq {
            yield! visitSynTypar typar
            yield! visitSynTypes typeArgs
        }
    | SynTypeConstraint.WhereTyparIsDelegate(typar, typeArgs: SynType list, range) ->
        seq {
            yield! visitSynTypar typar
            yield! visitSynTypes typeArgs
        }
    | SynTypeConstraint.WhereSelfConstrained(selfConstraint, range) ->
        visitSynType selfConstraint

and visitSynType (x : SynType) : Stuff =
    match x with
    | SynType.Anon range ->
        []
    | SynType.App(typeName, rangeOption, typeArgs, commaRanges, greaterRange, isPostfix, range) ->
        seq {
            yield! visitSynType typeName
            yield! typeArgs |> Seq.collect visitSynType
        }
    | SynType.Array(rank, elementType, range) ->
        visitSynType elementType
    | SynType.Fun(argType, returnType, range, synTypeFunTrivia) ->
        seq {
            yield! visitSynType argType
            yield! visitSynType returnType
        }
    | SynType.Or(lhsType, rhsType, range, synTypeOrTrivia) ->
        seq {
            yield! visitSynType lhsType
            yield! visitSynType rhsType
        }
    | SynType.Paren(innerType, range) ->
        visitSynType innerType
    | SynType.Tuple(isStruct, synTupleTypeSegments, range) ->
        visitSynTupleTypeSegments synTupleTypeSegments
    | SynType.Var(synTypar, range) ->
        visitSynTypar synTypar
    | SynType.AnonRecd(isStruct, fields, range) ->
        fields |> Seq.collect (fun (id, f) -> visitSynType f)
    | SynType.HashConstraint(innerType, range) ->
        visitSynType innerType
    | SynType.LongIdent synLongIdent ->
        visitSynLongIdent synLongIdent
    | SynType.MeasureDivide(synType, divisor, range) ->
        seq {
            yield! visitSynType synType
            yield! visitSynType divisor
        }
    | SynType.MeasurePower(baseMeasure, synRationalConst, range) ->
        seq {
            yield! visitSynType baseMeasure
            yield! visitSynRationalConst synRationalConst
        }
    | SynType.SignatureParameter(synAttributeLists, optional, identOption, usedType, range) ->
        seq {
            yield! visitSynAttributeLists synAttributeLists
            yield! visitSynType usedType
        }
    | SynType.StaticConstant(synConst, range) ->
        visitSynConst synConst
    | SynType.LongIdentApp(typeName, synLongIdent, rangeOption, typeArgs, commaRanges, greaterRange, range) ->
        seq {
            yield! visitSynType typeName
            yield! visitSynLongIdent synLongIdent
            yield! visitSynTypes typeArgs
        }
    | SynType.StaticConstantExpr(synExpr, range) ->
        seq {
            yield! visitSynExpr synExpr
        }
    | SynType.StaticConstantNamed(synType, value, range) ->
        seq {
            yield! visitSynType synType
            yield! visitSynType value
        }
    | SynType.WithGlobalConstraints(typeName, synTypeConstraints, range) ->
        seq {
            yield! visitSynType typeName
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
            yield! visitSynType fieldType
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
            yield! visitSynType rhsType
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
            yield! visitSynType synType
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
            yield! visitSynType synType
            yield! visitSynValInfo synValInfo
            yield! visitPreXmlDoc preXmlDoc
            match synAccessOption with | Some access -> yield! visitSynAccess access | None -> ()
            match synExprOption with | Some expr -> yield! visitSynExpr expr | None -> ()
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
            yield! visitSynType targetType
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
            yield! visitSynType targetType
        }

and visitSynMemberDefn (defn : SynMemberDefn) : Stuff =
    match defn with
    | SynMemberDefn.Inherit(baseType, identOption, range) ->
        visitSynType baseType
    | SynMemberDefn.Interface(interfaceType, withKeyword, synMemberDefnsOption, range) ->
        seq {
            yield! visitSynType interfaceType
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
            match synTypeOption with | Some synType -> yield! visitSynType synType | None -> ()
            yield! visitSynMemberKind synMemberKind
            yield! visitSynMemberFlags synMemberFlags
            yield! visitSynMemberFlags memberFlagsForSet
            yield! visitPreXmlDoc preXmlDoc
            match synAccessOption with | Some synAccess -> yield! visitSynAccess synAccess | None -> ()
            yield! visitSynExpr synExpr
        }
    | SynMemberDefn.ImplicitCtor(synAccessOption, synAttributeLists, synSimplePats, selfIdentifier, preXmlDoc, range) ->
        seq {
            match synAccessOption with | Some synAccess -> yield! visitSynAccess synAccess | None -> ()
            yield! visitSynAttributeLists synAttributeLists
            yield! visitSynSimplePats synSimplePats
        }
    | SynMemberDefn.ImplicitInherit(inheritType, inheritArgs, inheritAlias, range) ->
        seq {
            yield! visitSynType inheritType
            yield! visitSynExpr inheritArgs
        }
    | SynMemberDefn.LetBindings(synBindings, isStatic, isRecursive, range) ->
        visitSynBindings synBindings
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
    Seq.collect visitSynPat x

and visitSynPat (x : SynPat) : Stuff =
    match x with
    | SynPat.Ands(synPats, range) ->
        visitSynPats synPats
    | SynPat.As(lhsPat, rhsPat, range) ->
        visitSynPats [lhsPat; rhsPat]

and visitSynBinding (x : SynBinding) : Stuff =
    match x with
    | SynBinding.SynBinding(synAccessOption, synBindingKind, isInline, isMutable, synAttributeLists, preXmlDoc, synValData, headPat, synBindingReturnInfoOption, synExpr, range, debugPointAtBinding, synBindingTrivia) ->
        seq {
            match synAccessOption with | Some access -> yield! visitSynAccess access | None -> ()
            yield! visitSynAttributeLists synAttributeLists
            yield! visitPreXmlDoc preXmlDoc
            yield! visitSynValData synValData
            yield! visitSynPat headPat
        }
    failwith unsupported

and visitSynBindings (bindings : SynBinding list) : Stuff =
    Seq.collect visitSynBinding bindings

and visitSynOpenDeclTarget (target : SynOpenDeclTarget) : Stuff =
    failwith unsupported

and visitSynComponentInfo (info : SynComponentInfo) : Stuff =
    failwith unsupported

and visitLongIdent (ident : LongIdent) : Stuff =
    failwith unsupported
    
and visitSynLongIdent (ident : SynLongIdent) : Stuff  =
    [ident]
    
and visitSynExpr (expr : SynExpr) =
    failwith unsupported
    match expr with
    | _ -> []
    
and visitSynAttribute (attribute : SynAttribute) : Stuff  =
    seq {
        yield! visitSynLongIdent attribute.TypeName
        yield! visitSynExpr attribute.ArgExpr
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
            yield! synModuleDecls |> Seq.collect visitSynModuleDecl
            yield! visitSynAttributeLists synAttributeLists 
        }

and visit (input : ParsedInput) : Stuff  =
    match input with
    | ParsedInput.SigFile _ -> failwith unsupported
    | ParsedInput.ImplFile(ParsedImplFileInput(fileName, isScript, qualifiedNameOfFile, scopedPragmas, parsedHashDirectives, synModuleOrNamespaces, flags, parsedImplFileInputTrivia)) ->
        synModuleOrNamespaces
        |> Seq.collect visitSynModuleOrNamespace

[<Test>]
let ``Single SynEnumCase contains range of constant`` () =
    let parseResults = 
        getParseResults
            """
module A1 = let a = 3
module B =
    let b = [|
        A1.a
    |]
"""

    printfn $"%+A{parseResults}"
    let stuff = visit parseResults
    printfn $"%+A{stuff}"
    ()

module A1 = let a = 3
module A2 = let a = 3
module A3 = let a = 3
module A4 =
    let a = 3
    module A1 =
        let a = 3

module B =
    open A2
    let b = [|
        A1.a
        A2.a
        A3.a
    |]

let c = A4.a
let d = A4.A1.a
open A4
let e = A1.a
open A1
let f = a