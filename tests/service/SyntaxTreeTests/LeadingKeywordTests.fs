module FSharp.Compiler.Service.Tests.SyntaxTreeTests.LeadingKeywordTests

open FSharp.Compiler.Service.Tests.Common
open FSharp.Compiler.Syntax
open FSharp.Compiler.SyntaxTrivia
open NUnit.Framework

[<Test>]
let ``let keyword`` () =
    let parseResults = getParseResults "let a b = b + 1"

    match parseResults with
    | ParsedInput.ImplFile(ParsedImplFileInput(contents = [
                SynModuleOrNamespace.SynModuleOrNamespace(decls = [
                    SynModuleDecl.Let(bindings = [
                        SynBinding(trivia = { LeadingKeyword = SynLeadingKeyword.Let mLet  })
                    ])
                ])
            ])) ->
        assertRange (1, 0) (1, 3) mLet
    | _ -> Assert.Fail $"Could not get valid AST, got {parseResults}"

[<Test>]
let ``let rec keyword`` () =
    let parseResults = getParseResults "let rec a b = b + 1"

    match parseResults with
    | ParsedInput.ImplFile(ParsedImplFileInput(contents = [
                SynModuleOrNamespace.SynModuleOrNamespace(decls = [
                    SynModuleDecl.Let(bindings = [
                        SynBinding(trivia = { LeadingKeyword = SynLeadingKeyword.LetRec(mLet, mRec)  })
                    ])
                ])
            ])) ->
        assertRange (1, 0) (1, 3) mLet
        assertRange (1, 4) (1, 7) mRec
    | _ -> Assert.Fail $"Could not get valid AST, got {parseResults}"

[<Test>]
let ``and keyword`` () =
    let parseResults =
        getParseResults """
let rec a b = b + 1
and d e = e + 1
"""

    match parseResults with
    | ParsedInput.ImplFile(ParsedImplFileInput(contents = [
                SynModuleOrNamespace.SynModuleOrNamespace(decls = [
                    SynModuleDecl.Let(bindings = [
                        SynBinding _
                        SynBinding(trivia = { LeadingKeyword = SynLeadingKeyword.And mAnd  })
                    ])
                ])
            ])) ->
        assertRange (3, 0) (3, 3) mAnd
    | _ -> Assert.Fail $"Could not get valid AST, got {parseResults}"

[<Test>]
let ``use keyword`` () =
    let parseResults =
        getParseResults """
do
    use x = X()
    ()
"""

    match parseResults with
    | ParsedInput.ImplFile(ParsedImplFileInput(contents = [
                SynModuleOrNamespace.SynModuleOrNamespace(decls = [
                    SynModuleDecl.Expr(expr =
                        SynExpr.Do(expr =
                            SynExpr.LetOrUse(bindings = [
                                SynBinding(trivia = { LeadingKeyword = SynLeadingKeyword.Use mUse })
                            ])
                        )
                    )
                ])
            ])) ->
        assertRange (3, 4) (3, 7) mUse
    | _ -> Assert.Fail $"Could not get valid AST, got {parseResults}"

[<Test>]
let ``use rec keyword`` () =
    let parseResults =
        getParseResults """
do
    use rec x = X()
    ()
"""

    match parseResults with
    | ParsedInput.ImplFile(ParsedImplFileInput(contents = [
                SynModuleOrNamespace.SynModuleOrNamespace(decls = [
                    SynModuleDecl.Expr(expr =
                        SynExpr.Do(expr =
                            SynExpr.LetOrUse(bindings = [
                                SynBinding(trivia = { LeadingKeyword = SynLeadingKeyword.UseRec(mUse, mRec) })
                            ])
                        )
                    )
                ])
            ])) ->
        assertRange (3, 4) (3, 7) mUse
        assertRange (3, 8) (3, 11) mRec
    | _ -> Assert.Fail $"Could not get valid AST, got {parseResults}"

[<Test>]
let ``extern keyword`` () =
    let parseResults = getParseResults "extern void Meh()"

    match parseResults with
    | ParsedInput.ImplFile(ParsedImplFileInput(contents = [
                SynModuleOrNamespace.SynModuleOrNamespace(decls = [
                    SynModuleDecl.Let(bindings = [
                        SynBinding(trivia = { LeadingKeyword = SynLeadingKeyword.Extern mExtern  })
                    ])
                ])
            ])) ->
        assertRange (1, 0) (1, 6) mExtern
    | _ -> Assert.Fail $"Could not get valid AST, got {parseResults}"

[<Test>]
let ``member keyword`` () =
    let parseResults = getParseResults """
type X =
    member this.Y ()  = ()
"""

    match parseResults with
    | ParsedInput.ImplFile(ParsedImplFileInput(contents = [
                SynModuleOrNamespace.SynModuleOrNamespace(decls = [
                    SynModuleDecl.Types(typeDefns = [
                        SynTypeDefn(typeRepr = SynTypeDefnRepr.ObjectModel(members = [
                            SynMemberDefn.Member(memberDefn = SynBinding(trivia = { LeadingKeyword = SynLeadingKeyword.Member mMember }))
                        ]))
                    ])
                ])
            ])) ->
        assertRange (3, 4) (3, 10) mMember
    | _ -> Assert.Fail $"Could not get valid AST, got {parseResults}"

[<Test>]
let ``member val keyword`` () =
    let parseResults = getParseResults  """
type X =
    member val Y : int = 1
"""

    match parseResults with
    | ParsedInput.ImplFile(ParsedImplFileInput(contents = [
                SynModuleOrNamespace.SynModuleOrNamespace(decls = [
                    SynModuleDecl.Types(typeDefns = [
                        SynTypeDefn(typeRepr = SynTypeDefnRepr.ObjectModel(members = [
                            SynMemberDefn.AutoProperty(trivia = { LeadingKeyword = SynLeadingKeyword.MemberVal(mMember, mVal) })
                        ]))
                    ])
                ])
            ])) ->
        assertRange (3, 4) (3, 10) mMember
        assertRange (3, 11) (3, 14) mVal
    | _ -> Assert.Fail $"Could not get valid AST, got {parseResults}"

[<Test>]
let ``override keyword`` () =
    let parseResults = getParseResults  """
type D =
    override E : string = ""
"""

    match parseResults with
    | ParsedInput.ImplFile(ParsedImplFileInput(contents = [
                SynModuleOrNamespace.SynModuleOrNamespace(decls = [
                    SynModuleDecl.Types(typeDefns = [
                        SynTypeDefn(typeRepr = SynTypeDefnRepr.ObjectModel(members = [
                            SynMemberDefn.Member(memberDefn = SynBinding(trivia = { LeadingKeyword = SynLeadingKeyword.Override(mOverride) }))
                        ]))
                    ])
                ])
            ])) ->
        assertRange (3, 4) (3, 12) mOverride
    | _ -> Assert.Fail $"Could not get valid AST, got {parseResults}"

[<Test>]
let ``override val keyword`` () =
    let parseResults = getParseResults  """
type X =
    override val Y : int = 1
"""

    match parseResults with
    | ParsedInput.ImplFile(ParsedImplFileInput(contents = [
                SynModuleOrNamespace.SynModuleOrNamespace(decls = [
                    SynModuleDecl.Types(typeDefns = [
                        SynTypeDefn(typeRepr = SynTypeDefnRepr.ObjectModel(members = [
                            SynMemberDefn.AutoProperty(trivia = { LeadingKeyword = SynLeadingKeyword.OverrideVal(mOverride, mVal) })
                        ]))
                    ])
                ])
            ])) ->
        assertRange (3, 4) (3, 12) mOverride
        assertRange (3, 13) (3, 16) mVal
    | _ -> Assert.Fail $"Could not get valid AST, got {parseResults}"

[<Test>]
let ``abstract keyword`` () =
    let parseResults = getParseResults """
type X =
    abstract Y : int
"""

    match parseResults with
    | ParsedInput.ImplFile(ParsedImplFileInput(contents = [
                SynModuleOrNamespace.SynModuleOrNamespace(decls = [
                    SynModuleDecl.Types(typeDefns = [
                        SynTypeDefn(typeRepr = SynTypeDefnRepr.ObjectModel(members = [
                            SynMemberDefn.AbstractSlot(slotSig = SynValSig(trivia = { LeadingKeyword = SynLeadingKeyword.Abstract mAbstract }))
                        ]))
                    ])
                ])
            ])) ->
        assertRange (3, 4) (3, 12) mAbstract
    | _ -> Assert.Fail $"Could not get valid AST, got {parseResults}"

[<Test>]
let ``abstract member keyword`` () =
    let parseResults = getParseResults """
type X =
    abstract member Y : int
"""

    match parseResults with
    | ParsedInput.ImplFile(ParsedImplFileInput(contents = [
                SynModuleOrNamespace.SynModuleOrNamespace(decls = [
                    SynModuleDecl.Types(typeDefns = [
                        SynTypeDefn(typeRepr = SynTypeDefnRepr.ObjectModel(members = [
                            SynMemberDefn.AbstractSlot(slotSig = SynValSig(trivia = { LeadingKeyword = SynLeadingKeyword.AbstractMember(mAbstract, mMember) }))
                        ]))
                    ])
                ])
            ])) ->
        assertRange (3, 4) (3, 12) mAbstract
        assertRange (3, 13) (3, 19) mMember
    | _ -> Assert.Fail $"Could not get valid AST, got {parseResults}"

[<Test>]
let ``static member keyword`` () =
    let parseResults = getParseResults """
type X =
    static member Y : int = 1
"""

    match parseResults with
    | ParsedInput.ImplFile(ParsedImplFileInput(contents = [
                SynModuleOrNamespace.SynModuleOrNamespace(decls = [
                    SynModuleDecl.Types(typeDefns = [
                        SynTypeDefn(typeRepr = SynTypeDefnRepr.ObjectModel(members = [
                            SynMemberDefn.Member(memberDefn = SynBinding(trivia = { LeadingKeyword = SynLeadingKeyword.StaticMember(mStatic, mMember) }))
                        ]))
                    ])
                ])
            ])) ->
        assertRange (3, 4) (3, 10) mStatic
        assertRange (3, 11) (3, 17) mMember
    | _ -> Assert.Fail $"Could not get valid AST, got {parseResults}"

[<Test>]
let ``static member val keyword`` () =
    let parseResults = getParseResults """
type X =
    static member val Y : int = 1
"""

    match parseResults with
    | ParsedInput.ImplFile(ParsedImplFileInput(contents = [
                SynModuleOrNamespace.SynModuleOrNamespace(decls = [
                    SynModuleDecl.Types(typeDefns = [
                        SynTypeDefn(typeRepr = SynTypeDefnRepr.ObjectModel(members = [
                            SynMemberDefn.AutoProperty(trivia = { LeadingKeyword = SynLeadingKeyword.StaticMemberVal(mStatic, mMember, mVal) })
                        ]))
                    ])
                ])
            ])) ->
        assertRange (3, 4) (3, 10) mStatic
        assertRange (3, 11) (3, 17) mMember
        assertRange (3, 18) (3, 21) mVal
    | _ -> Assert.Fail $"Could not get valid AST, got {parseResults}"

[<Test>]
let ``static abstract keyword`` () =
    let parseResults = getParseResults """
type X =
    static abstract Y : int -> int
"""

    match parseResults with
    | ParsedInput.ImplFile(ParsedImplFileInput(contents = [
                SynModuleOrNamespace.SynModuleOrNamespace(decls = [
                    SynModuleDecl.Types(typeDefns = [
                        SynTypeDefn(typeRepr = SynTypeDefnRepr.ObjectModel(members = [
                            SynMemberDefn.AbstractSlot(slotSig = SynValSig(trivia = { LeadingKeyword = SynLeadingKeyword.StaticAbstract(mStatic, mAbstract) }))
                        ]))
                    ])
                ])
            ])) ->
        assertRange (3, 4) (3, 10) mStatic
        assertRange (3, 11) (3, 19) mAbstract
    | _ -> Assert.Fail $"Could not get valid AST, got {parseResults}"

[<Test>]
let ``static abstract member keyword`` () =
    let parseResults = getParseResults """
type X =
    static abstract member Y : int -> int
"""

    match parseResults with
    | ParsedInput.ImplFile(ParsedImplFileInput(contents = [
                SynModuleOrNamespace.SynModuleOrNamespace(decls = [
                    SynModuleDecl.Types(typeDefns = [
                        SynTypeDefn(typeRepr = SynTypeDefnRepr.ObjectModel(members = [
                            SynMemberDefn.AbstractSlot(slotSig = SynValSig(trivia = {
                                LeadingKeyword = SynLeadingKeyword.StaticAbstractMember(mStatic, mAbstract, mMember)
                            }))
                        ]))
                    ])
                ])
            ])) ->
        assertRange (3, 4) (3, 10) mStatic
        assertRange (3, 11) (3, 19) mAbstract
        assertRange (3,20) (3, 26) mMember 
    | _ -> Assert.Fail $"Could not get valid AST, got {parseResults}"

[<Test>]
let ``static val keyword`` () =
    let parseResults = getParseResultsOfSignatureFile """
namespace Meh

type X =
    static val Y : int -> int
"""

    match parseResults with
    | ParsedInput.SigFile(ParsedSigFileInput(contents = [
                SynModuleOrNamespaceSig(decls = [
                    SynModuleSigDecl.Types(types = [
                        SynTypeDefnSig(typeRepr = SynTypeDefnSigRepr.ObjectModel(memberSigs = [
                            SynMemberSig.ValField(field =
                                SynField(trivia = { LeadingKeyword = Some (SynLeadingKeyword.StaticVal(mStatic, mVal)) }))
                        ]))
                    ])
                ])
            ])) ->
        assertRange (5, 4) (5, 10) mStatic
        assertRange (5, 11) (5, 14) mVal
    | _ -> Assert.Fail $"Could not get valid AST, got {parseResults}"

[<Test>]
let ``default keyword`` () =
    let parseResults = 
        getParseResultsOfSignatureFile
            """
namespace X

type Y =
    default F : int
"""

    match parseResults with
    | ParsedInput.SigFile (ParsedSigFileInput (contents = [ SynModuleOrNamespaceSig(decls = [
        SynModuleSigDecl.Types(types =[
            SynTypeDefnSig(typeRepr=SynTypeDefnSigRepr.ObjectModel(memberSigs=[
                SynMemberSig.Member(memberSig = SynValSig(trivia = { LeadingKeyword = SynLeadingKeyword.Default mDefault }))
            ]))
        ])
    ]) ])) ->
        assertRange (5, 4) (5, 11) mDefault
    | _ -> Assert.Fail "Could not get valid AST"

[<Test>]
let ``default val keyword`` () =
    let ast = """
type Foo =
    default val A : int = 1
"""
                    |> getParseResults

    match ast with
    | ParsedInput.ImplFile(ParsedImplFileInput(contents = [
        SynModuleOrNamespace.SynModuleOrNamespace(decls = [
            SynModuleDecl.Types ([
                SynTypeDefn.SynTypeDefn (typeRepr = SynTypeDefnRepr.ObjectModel (members=[
                    SynMemberDefn.AutoProperty(trivia = { LeadingKeyword = SynLeadingKeyword.DefaultVal(mDefault , mVal) })
                ]))
            ], _)
        ])
      ])) ->        
        assertRange (3, 4) (3, 11) mDefault
        assertRange (3, 12) (3, 15) mVal
    | _ ->
        Assert.Fail "Could not get valid AST"

[<Test>]
let ``val keyword`` () =
    let parseResults = 
        getParseResultsOfSignatureFile
            """
namespace X

type Y =
    val F : int
"""

    match parseResults with
    | ParsedInput.SigFile (ParsedSigFileInput (contents = [ SynModuleOrNamespaceSig(decls = [
        SynModuleSigDecl.Types(types =[
            SynTypeDefnSig(typeRepr=SynTypeDefnSigRepr.ObjectModel(memberSigs=[
                SynMemberSig.ValField(field = SynField(trivia = { LeadingKeyword = Some (SynLeadingKeyword.Val mVal) }))
            ]))
        ])
    ]) ])) ->
        assertRange (5, 4) (5, 7) mVal
    | _ -> Assert.Fail "Could not get valid AST"

[<Test>]
let ``new keyword`` () =
    let parseResults = getParseResults """
type Y() =
    new (message:string) = Y()
"""

    match parseResults with
    | ParsedInput.ImplFile(ParsedImplFileInput(contents = [
                SynModuleOrNamespace.SynModuleOrNamespace(decls = [
                    SynModuleDecl.Types(typeDefns = [
                        SynTypeDefn(typeRepr = SynTypeDefnRepr.ObjectModel(members = [
                            SynMemberDefn.ImplicitCtor _
                            SynMemberDefn.Member(memberDefn = SynBinding(trivia = { LeadingKeyword = SynLeadingKeyword.New mNew }))
                        ]))
                    ])
                ])
            ])) ->
        assertRange (3, 4) (3, 7) mNew
    | _ -> Assert.Fail $"Could not get valid AST, got {parseResults}"

[<Test>]
let ``synthetic keyword`` () =
    let parseResults =
        getParseResults """
{ new ISomething with
    a = () }
"""

    match parseResults with
    | ParsedInput.ImplFile(ParsedImplFileInput(contents = [
                SynModuleOrNamespace.SynModuleOrNamespace(decls = [
                    SynModuleDecl.Expr(expr =
                        SynExpr.ObjExpr(bindings = [
                            SynBinding(trivia = { LeadingKeyword = SynLeadingKeyword.Synthetic })
                        ])
                    )
                ])
            ])) ->
        Assert.Pass()
    | _ -> Assert.Fail $"Could not get valid AST, got {parseResults}"

[<Test>]
let `` static let keyword`` () =
    let parseResults =
        getParseResults """
type X =
    static let PI = 3.14
"""

    match parseResults with
    | ParsedInput.ImplFile(ParsedImplFileInput(contents = [
                SynModuleOrNamespace.SynModuleOrNamespace(decls = [
                    SynModuleDecl.Types(typeDefns = [
                        SynTypeDefn(typeRepr = SynTypeDefnRepr.ObjectModel(members = [
                            SynMemberDefn.LetBindings(bindings = [
                                SynBinding(trivia = { LeadingKeyword = SynLeadingKeyword.StaticLet(mStatic, mLet) })
                            ])
                        ]))
                    ])
                ])
            ])) ->
        assertRange (3, 4) (3, 10) mStatic
        assertRange (3, 11) (3, 14) mLet
    | _ -> Assert.Fail $"Could not get valid AST, got {parseResults}"

[<Test>]
let `` static let rec keyword`` () =
    let parseResults =
        getParseResults """
type X =
    static let rec forever () = forever()
"""

    match parseResults with
    | ParsedInput.ImplFile(ParsedImplFileInput(contents = [
                SynModuleOrNamespace.SynModuleOrNamespace(decls = [
                    SynModuleDecl.Types(typeDefns = [
                        SynTypeDefn(typeRepr = SynTypeDefnRepr.ObjectModel(members = [
                            SynMemberDefn.LetBindings(bindings = [
                                SynBinding(trivia = { LeadingKeyword = SynLeadingKeyword.StaticLetRec(mStatic, mLet, mRec) })
                            ])
                        ]))
                    ])
                ])
            ])) ->
        assertRange (3, 4) (3, 10) mStatic
        assertRange (3, 11) (3, 14) mLet
        assertRange (3, 15) (3, 18) mRec
    | _ -> Assert.Fail $"Could not get valid AST, got {parseResults}"

[<Test>]
let `` do keyword`` () =
    let parseResults =
        getParseResults """
type X =
    do ()
"""

    match parseResults with
    | ParsedInput.ImplFile(ParsedImplFileInput(contents = [
                SynModuleOrNamespace.SynModuleOrNamespace(decls = [
                    SynModuleDecl.Types(typeDefns = [
                        SynTypeDefn(typeRepr = SynTypeDefnRepr.ObjectModel(members = [
                            SynMemberDefn.LetBindings(bindings = [
                                SynBinding(trivia = { LeadingKeyword = SynLeadingKeyword.Do(mDo) })
                            ])
                        ]))
                    ])
                ])
            ])) ->
        assertRange (3, 4) (3, 6) mDo
    | _ -> Assert.Fail $"Could not get valid AST, got {parseResults}"

[<Test>]
let `` do static keyword`` () =
    let parseResults =
        getParseResults """
type X =
    static do ()
"""

    match parseResults with
    | ParsedInput.ImplFile(ParsedImplFileInput(contents = [
                SynModuleOrNamespace.SynModuleOrNamespace(decls = [
                    SynModuleDecl.Types(typeDefns = [
                        SynTypeDefn(typeRepr = SynTypeDefnRepr.ObjectModel(members = [
                            SynMemberDefn.LetBindings(bindings = [
                                SynBinding(trivia = { LeadingKeyword = SynLeadingKeyword.StaticDo(mStatic, mDo) })
                            ])
                        ]))
                    ])
                ])
            ])) ->
        assertRange (3, 4) (3, 10) mStatic
        assertRange (3, 11) (3, 13) mDo
    | _ -> Assert.Fail $"Could not get valid AST, got {parseResults}"
