module FSharp.Compiler.Service.Tests.SyntaxTreeTests.MemberFlagTestsTests

open FSharp.Compiler.Service.Tests.Common
open FSharp.Compiler.Syntax
open NUnit.Framework


[<Test>]
let ``SynMemberSig.Member has correct keywords`` () =
    let parseResults = 
        getParseResultsOfSignatureFile
            """
namespace X

type Y =
    abstract A : int
    abstract member B : double
    static member C : string
    member D : int
    override E : int
    default F : int
"""

    match parseResults with
    | ParsedInput.SigFile (ParsedSigFileInput (modules = [ SynModuleOrNamespaceSig(decls = [
        SynModuleSigDecl.Types(types =[
            SynTypeDefnSig(typeRepr=SynTypeDefnSigRepr.ObjectModel(memberSigs=[
                SynMemberSig.Member(flags={ Trivia= { AbstractRange = Some mAbstract1 } })
                SynMemberSig.Member(flags={ Trivia= { AbstractRange = Some mAbstract2
                                                      MemberRange = Some mMember1 } })
                SynMemberSig.Member(flags={ Trivia= { StaticRange = Some mStatic3
                                                      MemberRange = Some mMember3 } })
                SynMemberSig.Member(flags={ Trivia= { MemberRange = Some mMember4 } })
                SynMemberSig.Member(flags={ Trivia= { OverrideRange = Some mOverride5 } })
                SynMemberSig.Member(flags={ Trivia= { DefaultRange = Some mDefault6 } })
            ]))
        ])
    ]) ])) ->
        assertRange (5, 4) (5, 12) mAbstract1
        assertRange (6, 4) (6, 12) mAbstract2
        assertRange (6, 13) (6, 19) mMember1
        assertRange (7, 4) (7, 10) mStatic3
        assertRange (7, 11) (7, 17) mMember3
        assertRange (8, 4) (8, 10) mMember4
        assertRange (9, 4) (9, 12) mOverride5
        assertRange (10, 4) (10, 11) mDefault6
    | _ -> Assert.Fail "Could not get valid AST"

[<Test>]
let ``SynMemberDefn.AbstractSlot has correct keyword`` () =
    let ast = """
type Foo =
    abstract X : int
    abstract member Y: int
"""
                    |> getParseResults

    match ast with
    | ParsedInput.ImplFile(ParsedImplFileInput(modules = [
        SynModuleOrNamespace.SynModuleOrNamespace(decls = [
            SynModuleDecl.Types ([
                SynTypeDefn.SynTypeDefn (typeRepr = SynTypeDefnRepr.ObjectModel (members=[
                    SynMemberDefn.AbstractSlot(flags={ Trivia = { AbstractRange = Some mAbstract1 } })
                    SynMemberDefn.AbstractSlot(flags={ Trivia = { AbstractRange = Some mAbstract2
                                                                  MemberRange = Some mMember2 } })
                ]))
            ], _)
        ])
      ])) ->
        assertRange (3, 4) (3, 12) mAbstract1
        assertRange (4, 4) (4, 12) mAbstract2
        assertRange (4, 13) (4, 19) mMember2
    | _ ->
        Assert.Fail "Could not get valid AST"

[<Test>]
let ``SynMemberDefn.AutoProperty has correct keyword`` () =
    let ast = """
type Foo =
    static member val W : int = 1
    member val X : int = 1
    override val Y : int = 2
    default val Z : int = 1
"""
                    |> getParseResults

    match ast with
    | ParsedInput.ImplFile(ParsedImplFileInput(modules = [
        SynModuleOrNamespace.SynModuleOrNamespace(decls = [
            SynModuleDecl.Types ([
                SynTypeDefn.SynTypeDefn (typeRepr = SynTypeDefnRepr.ObjectModel (members=[
                    SynMemberDefn.AutoProperty(memberFlags= mkFlags1)
                    SynMemberDefn.AutoProperty(memberFlags= mkFlags2)
                    SynMemberDefn.AutoProperty(memberFlags= mkFlags3)
                    SynMemberDefn.AutoProperty(memberFlags= mkFlags4)
                ]))
            ], _)
        ])
      ])) ->
        let ({ Trivia = flagsTrivia1 } : SynMemberFlags) = mkFlags1 SynMemberKind.Member
        assertRange (3, 4) (3, 10) flagsTrivia1.StaticRange.Value
        assertRange (3, 11) (3, 17) flagsTrivia1.MemberRange.Value

        let ({ Trivia = flagsTrivia2 } : SynMemberFlags) = mkFlags2 SynMemberKind.Member
        assertRange (4, 4) (4, 10) flagsTrivia2.MemberRange.Value
        
        let ({ Trivia = flagsTrivia3 } : SynMemberFlags) = mkFlags3 SynMemberKind.Member
        assertRange (5, 4) (5, 12) flagsTrivia3.OverrideRange.Value
        
        let ({ Trivia = flagsTrivia4 } : SynMemberFlags) = mkFlags4 SynMemberKind.Member
        assertRange (6, 4) (6, 11) flagsTrivia4.DefaultRange.Value
    | _ ->
        Assert.Fail "Could not get valid AST"

[<Test>]
let ``SynMemberDefn.Member SynValData has correct keyword`` () =
    let ast = """
type Foo =
    static member this.B() = ()
    member this.A() = ()
    override this.C() = ()
    default this.D() = ()
"""
                    |> getParseResults

    match ast with
    | ParsedInput.ImplFile(ParsedImplFileInput(modules = [
        SynModuleOrNamespace.SynModuleOrNamespace(decls = [
            SynModuleDecl.Types ([
                SynTypeDefn.SynTypeDefn (typeRepr = SynTypeDefnRepr.ObjectModel (members=[
                    SynMemberDefn.Member(memberDefn=SynBinding(valData=SynValData(memberFlags=Some { Trivia = { StaticRange = Some mStatic1
                                                                                                                MemberRange = Some mMember1 } })))
                    SynMemberDefn.Member(memberDefn=SynBinding(valData=SynValData(memberFlags=Some { Trivia = { MemberRange = Some mMember2 } })))
                    SynMemberDefn.Member(memberDefn=SynBinding(valData=SynValData(memberFlags=Some { Trivia = { OverrideRange = Some mOverride3 } })))
                    SynMemberDefn.Member(memberDefn=SynBinding(valData=SynValData(memberFlags=Some { Trivia = { DefaultRange = Some mDefaultRange4 } })))
                ]))
            ], _)
        ])
      ])) ->
        assertRange (3, 4) (3, 10) mStatic1
        assertRange (3, 11) (3, 17) mMember1
        assertRange (4, 4) (4, 10) mMember2
        assertRange (5, 4) (5, 12) mOverride3
        assertRange (6, 4) (6, 11) mDefaultRange4
    | _ ->
        Assert.Fail "Could not get valid AST"

[<Test>]
let ``SynExpr.Obj members have correct keywords`` () =
    let ast = """
let meh =
    { new Interface with
        override this.Foo () = ()
        member this.Bar () = ()
      interface SomethingElse with
        member this.Blah () = () }
"""
                    |> getParseResults

    match ast with
    | ParsedInput.ImplFile(ParsedImplFileInput(modules = [
        SynModuleOrNamespace.SynModuleOrNamespace(decls = [
            SynModuleDecl.Let (bindings = [
                SynBinding(expr=SynExpr.ObjExpr(
                    members=[
                        SynMemberDefn.Member(memberDefn=SynBinding(valData=SynValData(memberFlags=Some { Trivia = { OverrideRange = Some mOverride1 } })))
                        SynMemberDefn.Member(memberDefn=SynBinding(valData=SynValData(memberFlags=Some { Trivia = { MemberRange = Some mMember2 } })))
                    ]
                    extraImpls=[ SynInterfaceImpl(members=[
                        SynMemberDefn.Member(memberDefn=SynBinding(valData=SynValData(memberFlags=Some { Trivia = { MemberRange = Some mMember3 } })))
                    ]) ]))
            ])
      ]) ])) ->
        assertRange (4, 8) (4, 16) mOverride1
        assertRange (5, 8) (5, 14) mMember2
        assertRange (7, 8) (7, 14) mMember3
    | _ ->
        Assert.Fail "Could not get valid AST"