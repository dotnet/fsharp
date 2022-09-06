module FSharp.Compiler.Service.Tests.SyntaxTreeTests.SynTypeTests

open FSharp.Compiler.Service.Tests.Common
open FSharp.Compiler.Syntax
open NUnit.Framework

[<Test>]
let ``SynType.Tuple does include leading parameter name`` () =
    let parseResults =
        getParseResultsOfSignatureFile
             """
type T =
    member M: p1: a * p2: b -> int
 """

    match parseResults with
    | ParsedInput.SigFile(ParsedSigFileInput(modules = [
        SynModuleOrNamespaceSig(decls = [
            SynModuleSigDecl.Types(types = [
                SynTypeDefnSig(typeRepr = SynTypeDefnSigRepr.ObjectModel(memberSigs = [
                    SynMemberSig.Member(memberSig = SynValSig(synType =
                        SynType.Fun(argType = SynType.Tuple(_, _, mTuple))))
                ]))
            ])
        ])
      ])) ->
        assertRange (3, 14) (3, 27) mTuple
    | _ -> Assert.Fail $"Could not get valid AST, got {parseResults}"

[<Test>]
let ``SynType.Tuple does include leading parameter attributes`` () =
    let parseResults =
        getParseResultsOfSignatureFile
             """
type T =
    member M: [<SomeAttribute>] a * [<OtherAttribute>] b -> int
 """

    match parseResults with
    | ParsedInput.SigFile(ParsedSigFileInput(modules = [
        SynModuleOrNamespaceSig(decls = [
            SynModuleSigDecl.Types(types = [
                SynTypeDefnSig(typeRepr = SynTypeDefnSigRepr.ObjectModel(memberSigs = [
                    SynMemberSig.Member(memberSig = SynValSig(synType =
                        SynType.Fun(argType = SynType.Tuple(_, _, mTuple))))
                ]))
            ])
        ])
      ])) ->
        assertRange (3, 14) (3, 56) mTuple
    | _ -> Assert.Fail $"Could not get valid AST, got {parseResults}"
