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
        []
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
        }

and visitSynIdent (x : SynIdent) : Stuff =
    [] // TODO Correct?

and visitSynType (x : SynType) : Stuff =
    match x with
    | SynType.Anon range ->
        []
    | SynType.App(typeName, rangeOption, typeArgs, commaRanges, greaterRange, isPostfix, range) ->
        failwith unsupported
    | SynType.Array(rank, elementType, range) ->
        failwith unsupported
    | SynType.Fun(argType, returnType, range, synTypeFunTrivia) ->
        failwith unsupported
    | SynType.Or(lhsType, rhsType, range, synTypeOrTrivia) ->
        failwith unsupported
    | SynType.Paren(innerType, range) ->
        failwith unsupported
    | SynType.Tuple(isStruct, synTupleTypeSegments, range) ->
        failwith unsupported
    | SynType.Var(synTypar, range) ->
        failwith unsupported
    | SynType.AnonRecd(isStruct, fields, range) ->
        failwith unsupported
    | SynType.HashConstraint(innerType, range) ->
        failwith unsupported
    | SynType.LongIdent synLongIdent ->
        failwith unsupported
    | SynType.MeasureDivide(synType, divisor, range) ->
        failwith unsupported
    | SynType.MeasurePower(baseMeasure, synRationalConst, range) ->
        failwith unsupported
    | SynType.SignatureParameter(synAttributeLists, optional, identOption, usedType, range) ->
        failwith unsupported
    | SynType.StaticConstant(synConst, range) ->
        failwith unsupported
    | SynType.LongIdentApp(typeName, synLongIdent, rangeOption, typeArgs, commaRanges, greaterRange, range) ->
        failwith unsupported
    | SynType.StaticConstantExpr(synExpr, range) ->
        failwith unsupported
    | SynType.StaticConstantNamed(synType, value, range) ->
        failwith unsupported
    | SynType.WithGlobalConstraints(typeName, synTypeConstraints, range) ->
        failwith unsupported

and visitPreXmlDoc (doc : FSharp.Compiler.Xml.PreXmlDoc) : Stuff =
    [] // TODO Check

and visitSynAccess (x : SynAccess) : Stuff =
    failwith unsupported

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
            // TODO
        }

and visitSynTypeDefnRepr (x : SynTypeDefnRepr) : Stuff =
    match x with
    | SynTypeDefnRepr.Exception synExceptionDefnRepr ->
        visitSynExceptionDefnRepr synExceptionDefnRepr

and visitSynMemberDefn (defn : SynMemberDefn) : Stuff =
    [] // TODO

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

and visitSynExceptionDefn (defn : SynExceptionDefn) : Stuff =
    []

and visitSynBinding (binding : SynBinding) : Stuff =
    [] // TODO

and visitSynBindings (bindings : SynBinding list) : Stuff =
    Seq.collect visitSynBinding bindings

and visitSynOpenDeclTarget (target : SynOpenDeclTarget) : Stuff =
    [] // TODO

and visitSynComponentInfo (info : SynComponentInfo) : Stuff =
    [] // TODO

and visitLongIdent (ident : LongIdent) : Stuff =
    [] // TODO
    
and visitSynLongIdent (ident : SynLongIdent) : Stuff  =
    [ident]
    
and visitSynExpr (expr : SynExpr) =
    // TODO
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
type Foo = One = 0x00000001
"""

    match parseResults with
    | ParsedInput.ImplFile (ParsedImplFileInput (contents = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
        SynModuleDecl.Types(typeDefns = [
            SynTypeDefn.SynTypeDefn(typeRepr =
                SynTypeDefnRepr.Simple(simpleRepr = SynTypeDefnSimpleRepr.Enum(cases = [ SynEnumCase.SynEnumCase(valueRange = r) ])))])
    ]) ])) ->
        assertRange (2, 17) (2, 27) r
    | _ -> Assert.Fail "Could not get valid AST"
