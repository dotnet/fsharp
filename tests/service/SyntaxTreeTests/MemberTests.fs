module FSharp.Compiler.Service.Tests.SyntaxTreeTests.MemberTests

open FSharp.Compiler.Service.Tests.Common
open FSharp.Compiler.Syntax
open FSharp.Compiler.SyntaxTrivia
open NUnit.Framework

[<Test>]
let ``SynTypeDefn with AutoProperty contains the range of the equals sign`` () =
    let parseResults = 
        getParseResults
            """
/// mutable class with auto-properties
type Person(name : string, age : int) =
    /// Full name
    member val Name = name with get, set
"""

    match parseResults with
    | ParsedInput.ImplFile (ParsedImplFileInput (contents = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
        SynModuleDecl.Types(
            typeDefns = [ SynTypeDefn(typeRepr = SynTypeDefnRepr.ObjectModel(members = [_ ; SynMemberDefn.AutoProperty(trivia = { EqualsRange = Some mEquals })])) ]
        )
    ]) ])) ->
        assertRange (5, 20) (5, 21) mEquals
    | _ -> Assert.Fail "Could not get valid AST"

[<Test>]
let ``SynTypeDefn with AutoProperty contains the range of the with keyword`` () =
    let parseResults = 
        getParseResults
            """
type Foo() =
    member val AutoProperty = autoProp with get, set
    member val AutoProperty2 = autoProp
"""

    match parseResults with
    | ParsedInput.ImplFile (ParsedImplFileInput (contents = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
        SynModuleDecl.Types(
            typeDefns = [ SynTypeDefn(typeRepr = SynTypeDefnRepr.ObjectModel(members = [_
                                                                                        SynMemberDefn.AutoProperty(trivia = { WithKeyword = Some mWith
                                                                                                                              GetSetKeywords = Some (GetSetKeywords.GetSet(mGet, mSet)) })
                                                                                        SynMemberDefn.AutoProperty(trivia = { WithKeyword = None })])) ]
        )
    ]) ])) ->
        assertRange (3, 39) (3, 43) mWith
        assertRange (3, 44) (3, 47) mGet
        assertRange (3, 49) (3, 52) mSet
    | _ -> Assert.Fail "Could not get valid AST"

[<Test>]
let ``SynTypeDefn with AbstractSlot contains the range of the with keyword`` () =
    let parseResults = 
        getParseResults
            """
type Foo() =
    abstract member Bar : int with get,set
"""

    match parseResults with
    | ParsedInput.ImplFile (ParsedImplFileInput (contents = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
        SynModuleDecl.Types(
            typeDefns = [ SynTypeDefn(typeRepr = SynTypeDefnRepr.ObjectModel(members = [_
                                                                                        SynMemberDefn.AbstractSlot(slotSig=SynValSig(trivia = { WithKeyword = Some mWith })
                                                                                                                   trivia = { GetSetKeywords = Some (GetSetKeywords.GetSet(mGet, mSet)) })])) ]
        )
    ]) ])) ->
        assertRange (3, 30) (3, 34) mWith
        assertRange (3, 35) (3, 38) mGet
        assertRange (3, 39) (3, 42) mSet
    | _ -> Assert.Fail "Could not get valid AST"

[<Test>]
let ``read-only property in SynMemberDefn.Member contains the range of the with keyword`` () =
    let parseResults = 
        getParseResults
            """
type Foo() =
    // A read-only property.
    member this.MyReadProperty with get () = myInternalValue
"""

    match parseResults with
    | ParsedInput.ImplFile (ParsedImplFileInput (contents = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
        SynModuleDecl.Types(
            typeDefns = [ SynTypeDefn(typeRepr =
                SynTypeDefnRepr.ObjectModel(members=[
                    _
                    SynMemberDefn.GetSetMember(Some(SynBinding _), None, _, { WithKeyword = mWith }) ])
                ) ])
         ]) ])) ->
        assertRange (4, 31) (4, 35) mWith
    | _ -> Assert.Fail "Could not get valid AST"

[<Test>]
let ``write-only property in SynMemberDefn.Member contains the range of the with keyword`` () =
    let parseResults = 
        getParseResults
            """
type Foo() =
    // A write-only property.
    member this.MyWriteOnlyProperty with set (value) = myInternalValue <- value
"""

    match parseResults with
    | ParsedInput.ImplFile (ParsedImplFileInput (contents = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
        SynModuleDecl.Types(
            typeDefns = [ SynTypeDefn(typeRepr =
                SynTypeDefnRepr.ObjectModel(members=[
                     _
                     SynMemberDefn.GetSetMember(None, Some(SynBinding _), _, { WithKeyword = mWith }) ])
                ) ])
         ]) ])) ->
        assertRange (4, 36) (4, 40) mWith
    | _ -> Assert.Fail "Could not get valid AST"

[<Test>]
let ``read/write property in SynMemberDefn.Member contains the range of the with keyword`` () =
    let parseResults = 
        getParseResults
            """
type Foo() =
    // A read-write property.
    member this.MyReadWriteProperty
        with get () = myInternalValue
        and set (value) = myInternalValue <- value
"""

    match parseResults with
    | ParsedInput.ImplFile (ParsedImplFileInput (contents = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
        SynModuleDecl.Types(
            typeDefns = [ SynTypeDefn(typeRepr =
                SynTypeDefnRepr.ObjectModel(members=[
                   _
                   SynMemberDefn.GetSetMember(Some _, Some _, _, { WithKeyword = mWith; AndKeyword = Some mAnd }) ])
                ) ])
         ]) ])) ->
        assertRange (5, 8) (5, 12) mWith
        assertRange (6, 8) (6, 11) mAnd
    | _ -> Assert.Fail "Could not get valid AST"

[<Test>]
let ``SynTypeDefn with static member with get/set`` () =
    let parseResults = 
        getParseResults
            """
type Foo =
    static member ReadWrite2 
        with set  x = lastUsed <- ("ReadWrite2", x)
        and  get () = lastUsed <- ("ReadWrite2", 0); 4
"""

    match parseResults with
    | ParsedInput.ImplFile (ParsedImplFileInput (contents = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
        SynModuleDecl.Types(
            typeDefns = [ SynTypeDefn(typeRepr = SynTypeDefnRepr.ObjectModel(members = [
                SynMemberDefn.GetSetMember(Some _, Some _, m, { WithKeyword = mWith
                                                                GetKeyword = Some mGet
                                                                AndKeyword = Some mAnd
                                                                SetKeyword = Some mSet })
            ])) ]
        )
    ]) ])) ->
        assertRange (4, 8) (4, 12) mWith
        assertRange (4, 13) (4, 16) mSet
        assertRange (5, 8) (5, 11) mAnd
        assertRange (5, 13) (5, 16) mGet
        assertRange (3, 4) (5, 54) m
    | _ -> Assert.Fail "Could not get valid AST"

[<Test>]
let ``SynTypeDefn with member with set/get`` () =
    let parseResults = 
        getParseResults
            """
type A() =
    member this.Z with set (_:int):unit = () and get():int = 1
"""

    match parseResults with
    | ParsedInput.ImplFile (ParsedImplFileInput (contents = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
        SynModuleDecl.Types(
            typeDefns = [ SynTypeDefn(typeRepr = SynTypeDefnRepr.ObjectModel(members = [
                SynMemberDefn.ImplicitCtor _
                SynMemberDefn.GetSetMember(Some (SynBinding(headPat = SynPat.LongIdent(extraId = Some getIdent))),
                                           Some (SynBinding(headPat = SynPat.LongIdent(extraId = Some setIdent))),
                                           m,
                                           { WithKeyword = mWith
                                             GetKeyword = Some mGet
                                             AndKeyword = Some mAnd
                                             SetKeyword = Some mSet })
            ])) ]
        )
    ]) ])) ->
        Assert.AreEqual("get", getIdent.idText)
        Assert.AreEqual("set", setIdent.idText)
        assertRange (3, 18) (3, 22) mWith
        assertRange (3, 23) (3, 26) mSet
        assertRange (3, 23) (3, 26) setIdent.idRange
        assertRange (3, 45) (3, 48) mAnd
        assertRange (3, 49) (3, 52) mGet
        assertRange (3, 49) (3, 52) getIdent.idRange
        assertRange (3, 4) (3, 62) m
    | _ -> Assert.Fail "Could not get valid AST"

[<Test>]
let ``SynTypeDefn with member with get has xml comment`` () =
    let parseResults = 
        getParseResults
            """
type A =
    /// B
    member x.B with get() = 5
"""

    match parseResults with
    | ParsedInput.ImplFile (ParsedImplFileInput (contents = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
        SynModuleDecl.Types(
            typeDefns = [ SynTypeDefn(typeRepr = SynTypeDefnRepr.ObjectModel(members = [
                SynMemberDefn.GetSetMember(Some (SynBinding(xmlDoc = preXmlDoc)),
                                           None,
                                           _,
                                           _)
            ])) ]
        )
    ]) ])) ->
        Assert.False preXmlDoc.IsEmpty
        let comment = preXmlDoc.ToXmlDoc(false, None).GetXmlText()
        Assert.False (System.String.IsNullOrWhiteSpace(comment))
    | _ -> Assert.Fail "Could not get valid AST"

[<Test>]
let ``Signature member with set,get`` () =
    let parseResults = 
        getParseResultsOfSignatureFile
            """
module Meh

type X =
    // MemberSig.Member
    member Y : int
                    with
                            set  ,  get
"""

    match parseResults with
    | ParsedInput.SigFile (ParsedSigFileInput (contents=[
        SynModuleOrNamespaceSig(decls=[
            SynModuleSigDecl.Types(types = [
                SynTypeDefnSig(typeRepr = SynTypeDefnSigRepr.ObjectModel(memberSigs = [
                    SynMemberSig.Member(memberSig = SynValSig(trivia = { WithKeyword = Some mWith })
                                        trivia = { GetSetKeywords = Some (GetSetKeywords.GetSet(mGet, mSet)) })
                ]))
            ])
        ] ) ])) ->
        assertRange (7, 20) (7, 24) mWith
        assertRange (8, 28) (8, 31) mSet
        assertRange (8, 36) (8, 39) mGet
    | _ -> Assert.Fail "Could not get valid AST"

[<Test>]
let ``Signature member with set`` () =
    let parseResults = 
        getParseResultsOfSignatureFile
            """
module Meh

type X =
    // MemberSig.Member
    member Y : int
                    with
                            set
"""

    match parseResults with
    | ParsedInput.SigFile (ParsedSigFileInput (contents=[
        SynModuleOrNamespaceSig(decls=[
            SynModuleSigDecl.Types(types = [
                SynTypeDefnSig(typeRepr = SynTypeDefnSigRepr.ObjectModel(memberSigs = [
                    SynMemberSig.Member(memberSig = SynValSig(trivia = { WithKeyword = Some mWith })
                                        trivia = { GetSetKeywords = Some (GetSetKeywords.Set(mSet)) })
                ]))
            ])
        ] ) ])) ->
        assertRange (7, 20) (7, 24) mWith
        assertRange (8, 28) (8, 31) mSet
    | _ -> Assert.Fail "Could not get valid AST"


[<Test>]
let ``Signature member with get`` () =
    let parseResults = 
        getParseResultsOfSignatureFile
            """
module Meh

type X =
    // MemberSig.Member
    member Y : int
                    with
                                get
"""

    match parseResults with
    | ParsedInput.SigFile (ParsedSigFileInput (contents=[
        SynModuleOrNamespaceSig(decls=[
            SynModuleSigDecl.Types(types = [
                SynTypeDefnSig(typeRepr = SynTypeDefnSigRepr.ObjectModel(memberSigs = [
                    SynMemberSig.Member(memberSig = SynValSig(trivia = { WithKeyword = Some mWith })
                                        trivia = { GetSetKeywords = Some (GetSetKeywords.Get(mGet)) })
                ]))
            ])
        ] ) ])) ->
        assertRange (7, 20) (7, 24) mWith
        assertRange (8, 32) (8, 35) mGet
    | _ -> Assert.Fail "Could not get valid AST"

[<Test>]
let ``Member with inline keyword`` () =
    let parseResults = 
        getParseResults
            """
type X =
    member inline x.Y () = ()
"""

    match parseResults with
    | ParsedInput.ImplFile (ParsedImplFileInput (contents = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
        SynModuleDecl.Types(
            typeDefns = [ SynTypeDefn(typeRepr = SynTypeDefnRepr.ObjectModel(members = [
                SynMemberDefn.Member(memberDefn = SynBinding(trivia = { InlineKeyword = Some mInline }))
            ])) ]
        )
    ]) ])) ->
        assertRange (3, 11) (3, 17) mInline
    | ast -> Assert.Fail $"Could not get valid AST, got {ast}"

[<Test>]
let ``Get/Set member with inline keyword`` () =
    let parseResults = 
        getParseResults
            """
type X =
    member inline x.Y 
        with inline get () = 4
        and inline set y = ()
"""

    match parseResults with
    | ParsedInput.ImplFile (ParsedImplFileInput (contents = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
        SynModuleDecl.Types(
            typeDefns = [ SynTypeDefn(typeRepr = SynTypeDefnRepr.ObjectModel(members = [
                SynMemberDefn.GetSetMember(Some (SynBinding(trivia = { InlineKeyword = Some mInlineGet })),
                                           Some (SynBinding(trivia = { InlineKeyword = Some mInlineSet })),
                                           _,
                                           { InlineKeyword = Some mInlineGetSetMember })
            ])) ]
        )
    ]) ])) ->
        assertRange (3, 11) (3, 17) mInlineGetSetMember
        assertRange (4, 13) (4, 19) mInlineGet
        assertRange (5, 12) (5, 18) mInlineSet
    | ast -> Assert.Fail $"Could not get valid AST, got {ast}"
