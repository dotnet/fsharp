module FSharp.Compiler.Service.Tests.SyntaxTreeTests.MemberFlagTestsTests

open FSharp.Compiler.Service.Tests.Common
open FSharp.Compiler.Syntax
open FSharp.Compiler.SyntaxTrivia
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
    | ParsedInput.SigFile (ParsedSigFileInput (contents = [ SynModuleOrNamespaceSig(decls = [
        SynModuleSigDecl.Types(types =[
            SynTypeDefnSig(typeRepr=SynTypeDefnSigRepr.ObjectModel(memberSigs=[
                SynMemberSig.Member(memberSig = SynValSig(trivia = { LeadingKeyword = SynLeadingKeyword.Abstract mAbstract1 }))
                SynMemberSig.Member(memberSig = SynValSig(trivia = { LeadingKeyword = SynLeadingKeyword.AbstractMember(mAbstract2, mMember1) }))
                SynMemberSig.Member(memberSig = SynValSig(trivia = { LeadingKeyword = SynLeadingKeyword.StaticMember(mStatic3, mMember3) }))
                SynMemberSig.Member(memberSig = SynValSig(trivia = { LeadingKeyword = SynLeadingKeyword.Member mMember4 }))
                SynMemberSig.Member(memberSig = SynValSig(trivia = { LeadingKeyword = SynLeadingKeyword.Override mOverride5 }))
                SynMemberSig.Member(memberSig = SynValSig(trivia = { LeadingKeyword = SynLeadingKeyword.Default mDefault6 }))
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
    | ParsedInput.ImplFile(ParsedImplFileInput(contents = [
        SynModuleOrNamespace.SynModuleOrNamespace(decls = [
            SynModuleDecl.Types ([
                SynTypeDefn.SynTypeDefn (typeRepr = SynTypeDefnRepr.ObjectModel (members=[
                    SynMemberDefn.AbstractSlot(slotSig = SynValSig(trivia = { LeadingKeyword = SynLeadingKeyword.Abstract mAbstract1 }))
                    SynMemberDefn.AbstractSlot(slotSig = SynValSig(trivia = { LeadingKeyword = SynLeadingKeyword.AbstractMember(mAbstract2, mMember2) }))
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

    let (|LeadingKeyword|_|) md =
        match md with
        | SynMemberDefn.AutoProperty(trivia = { LeadingKeyword = lk }) -> Some lk
        | _ -> None
    
    match ast with
    | ParsedInput.ImplFile(ParsedImplFileInput(contents = [
        SynModuleOrNamespace.SynModuleOrNamespace(decls = [
            SynModuleDecl.Types ([
                SynTypeDefn.SynTypeDefn (typeRepr = SynTypeDefnRepr.ObjectModel (members=[
                    LeadingKeyword(SynLeadingKeyword.StaticMemberVal(mStatic1, mMember1, mVal1))
                    LeadingKeyword(SynLeadingKeyword.MemberVal(mMember2, mVal2))
                    LeadingKeyword(SynLeadingKeyword.OverrideVal(mOverride3, mVal3))
                    LeadingKeyword(SynLeadingKeyword.DefaultVal(mDefault4, mVal4))
                ]))
            ], _)
        ])
      ])) ->
        assertRange (3, 4) (3, 10) mStatic1
        assertRange (3, 11) (3, 17) mMember1
        assertRange (3, 18) (3, 21) mVal1

        assertRange (4, 4) (4, 10) mMember2
        assertRange (4, 11) (4, 14) mVal2
        
        assertRange (5, 4) (5, 12) mOverride3
        assertRange (5, 13) (5, 16) mVal3
        
        assertRange (6, 4) (6, 11) mDefault4
        assertRange (6, 12) (6, 15) mVal4
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

    let (|LeadingKeyword|_|) md =
        match md with
        | SynMemberDefn.Member(memberDefn = SynBinding(trivia = { LeadingKeyword = lk })) -> Some lk
        | _ -> None
    
    match ast with
    | ParsedInput.ImplFile(ParsedImplFileInput(contents = [
        SynModuleOrNamespace.SynModuleOrNamespace(decls = [
            SynModuleDecl.Types ([
                SynTypeDefn.SynTypeDefn (typeRepr = SynTypeDefnRepr.ObjectModel (members=[
                    LeadingKeyword(SynLeadingKeyword.StaticMember(mStatic1, mMember1))
                    LeadingKeyword(SynLeadingKeyword.Member(mMember2))
                    LeadingKeyword(SynLeadingKeyword.Override(mOverride3))
                    LeadingKeyword(SynLeadingKeyword.Default mDefaultRange4)
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
    | ParsedInput.ImplFile(ParsedImplFileInput(contents = [
        SynModuleOrNamespace.SynModuleOrNamespace(decls = [
            SynModuleDecl.Let (bindings = [
                SynBinding(expr=SynExpr.ObjExpr(
                    members=[
                        SynMemberDefn.Member(memberDefn=SynBinding(trivia = { LeadingKeyword = SynLeadingKeyword.Override mOverride1 }))
                        SynMemberDefn.Member(memberDefn=SynBinding(trivia = { LeadingKeyword = SynLeadingKeyword.Member mMember2 }))
                    ]
                    extraImpls=[ SynInterfaceImpl(members=[
                        SynMemberDefn.Member(memberDefn=SynBinding(trivia = { LeadingKeyword = SynLeadingKeyword.Member mMember3 }))
                    ]) ]))
            ])
      ]) ])) ->
        assertRange (4, 8) (4, 16) mOverride1
        assertRange (5, 8) (5, 14) mMember2
        assertRange (7, 8) (7, 14) mMember3
    | _ ->
        Assert.Fail "Could not get valid AST"