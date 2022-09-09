module FSharp.Compiler.Service.Tests.SyntaxTreeTests.TypeTests

open FSharp.Compiler.Service.Tests.Common
open FSharp.Compiler.Syntax
open FSharp.Compiler.SyntaxTrivia
open NUnit.Framework

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
    
[<Test>]
let ``Multiple SynEnumCase contains range of constant`` () =
    let parseResults = 
        getParseResults
            """
type Foo =
    | One =  0x00000001
    | Two = 2
"""

    match parseResults with
    | ParsedInput.ImplFile (ParsedImplFileInput (contents = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
        SynModuleDecl.Types(typeDefns = [
            SynTypeDefn.SynTypeDefn(typeRepr =
                SynTypeDefnRepr.Simple(simpleRepr = SynTypeDefnSimpleRepr.Enum(cases = [ SynEnumCase.SynEnumCase(valueRange = r1)
                                                                                         SynEnumCase.SynEnumCase(valueRange = r2) ])))])
    ]) ])) ->
        assertRange (3, 13) (3, 23) r1
        assertRange (4, 12) (4, 13) r2
    | _ -> Assert.Fail "Could not get valid AST"

[<Test>]
let ``Range of attribute should be included in SynTypeDefn`` () =
    let parseResults = 
        getParseResults
            """
[<Foo>]
type Bar =
    class
    end"""

    match parseResults with
    | ParsedInput.ImplFile (ParsedImplFileInput (contents = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
        SynModuleDecl.Types(typeDefns = [t]) as types
    ]) ])) ->
        assertRange (2, 0) (5, 7) types.Range
        assertRange (2, 0) (5, 7) t.Range
    | _ -> Assert.Fail "Could not get valid AST"

[<Test>]
let ``Range of attributes should be included in recursive types`` () =
    let parseResults = 
        getParseResults
            """
[<NoEquality ; NoComparison>]
type Foo<'context, 'a> =
    | Apply of ApplyCrate<'context, 'a>

and [<CustomEquality ; NoComparison>] Bar<'context, 'a> =
    internal {
        Hash : int
        Foo : Foo<'a, 'b>
    }"""

    match parseResults with
    | ParsedInput.ImplFile (ParsedImplFileInput (contents = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
        SynModuleDecl.Types(typeDefns = [t1;t2]) as types
    ]) ])) ->
        assertRange (2, 0) (10, 5) types.Range
        assertRange (2, 0) (4, 39) t1.Range
        assertRange (6, 4) (10, 5) t2.Range
    | _ -> Assert.Fail "Could not get valid AST"

[<Test>]
let ``SynTypeDefn with ObjectModel Delegate contains the range of the equals sign`` () =
    let parseResults = 
        getParseResults
            """
type X = delegate of string -> string
"""

    match parseResults with
    | ParsedInput.ImplFile (ParsedImplFileInput (contents = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
        SynModuleDecl.Types(
            typeDefns = [ SynTypeDefn(typeRepr = SynTypeDefnRepr.ObjectModel(kind = SynTypeDefnKind.Delegate _)
                                      trivia={ EqualsRange = Some mEquals }) ]
        )
    ]) ])) ->
        assertRange (2, 7) (2, 8) mEquals
    | _ -> Assert.Fail "Could not get valid AST"

[<Test>]
let ``SynTypeDefn with ObjectModel class contains the range of the equals sign`` () =
    let parseResults = 
        getParseResults
            """
type Foobar () =
    class
    end
"""

    match parseResults with
    | ParsedInput.ImplFile (ParsedImplFileInput (contents = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
        SynModuleDecl.Types(
            typeDefns = [ SynTypeDefn(typeRepr = SynTypeDefnRepr.ObjectModel(kind = SynTypeDefnKind.Class)
                                      trivia={ EqualsRange = Some mEquals }) ]
        )
    ]) ])) ->
        assertRange (2, 15) (2, 16) mEquals
    | _ -> Assert.Fail "Could not get valid AST"

[<Test>]
let ``SynTypeDefn with Enum contains the range of the equals sign`` () =
    let parseResults = 
        getParseResults
            """
type Bear =
    | BlackBear = 1
    | PolarBear = 2
"""

    match parseResults with
    | ParsedInput.ImplFile (ParsedImplFileInput (contents = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
        SynModuleDecl.Types(
            typeDefns = [ SynTypeDefn(typeRepr = SynTypeDefnRepr.Simple(simpleRepr =
                                          SynTypeDefnSimpleRepr.Enum(cases = [
                                              SynEnumCase(trivia={ EqualsRange = mEqualsEnumCase1 })
                                              SynEnumCase(trivia={ EqualsRange = mEqualsEnumCase2 })
                                          ]))
                                      trivia={ EqualsRange = Some mEquals }) ]
        )
    ]) ])) ->
        assertRange (2, 10) (2, 11) mEquals
        assertRange (3, 16) (3, 17) mEqualsEnumCase1
        assertRange (4, 16) (4, 17) mEqualsEnumCase2
    | _ -> Assert.Fail "Could not get valid AST"

[<Test>]
let ``SynTypeDefn with Union contains the range of the equals sign`` () =
    let parseResults = 
        getParseResults
            """
type Shape =
    | Square of int 
    | Rectangle of int * int
"""

    match parseResults with
    | ParsedInput.ImplFile (ParsedImplFileInput (contents = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
        SynModuleDecl.Types(
            typeDefns = [ SynTypeDefn(typeRepr = SynTypeDefnRepr.Simple(simpleRepr = SynTypeDefnSimpleRepr.Union _)
                                      trivia={ EqualsRange = Some mEquals }) ]
        )
    ]) ])) ->
        assertRange (2, 11) (2, 12) mEquals
    | _ -> Assert.Fail "Could not get valid AST"

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
            typeDefns = [ SynTypeDefn(typeRepr = SynTypeDefnRepr.ObjectModel(members = [_ ; SynMemberDefn.AutoProperty(equalsRange = mEquals)])) ]
        )
    ]) ])) ->
        assertRange (5, 20) (5, 21) mEquals
    | _ -> Assert.Fail "Could not get valid AST"

[<Test>]
let ``SynTypeDefn with Record contains the range of the with keyword`` () =
    let parseResults = 
        getParseResults
            """
type Foo =
    { Bar : int }
    with
        member this.Meh (v:int) = this.Bar + v
"""

    match parseResults with
    | ParsedInput.ImplFile (ParsedImplFileInput (contents = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
        SynModuleDecl.Types(
            typeDefns = [ SynTypeDefn(typeRepr=SynTypeDefnRepr.Simple(simpleRepr= SynTypeDefnSimpleRepr.Record _)
                                      trivia={ WithKeyword = Some mWithKeyword }) ]
        )
    ]) ])) ->
        assertRange (4, 4) (4, 8) mWithKeyword
    | _ -> Assert.Fail "Could not get valid AST"

[<Test>]
let ``SynTypeDefn with Augmentation contains the range of the with keyword`` () =
    let parseResults = 
        getParseResults
            """
type Int32 with
    member _.Zero = 0
"""

    match parseResults with
    | ParsedInput.ImplFile (ParsedImplFileInput (contents = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
        SynModuleDecl.Types(
            typeDefns = [ SynTypeDefn(typeRepr = SynTypeDefnRepr.ObjectModel(kind=SynTypeDefnKind.Augmentation mWithKeyword)) ]
        )
    ]) ])) ->
        assertRange (2, 11) (2, 15) mWithKeyword
    | _ -> Assert.Fail "Could not get valid AST"

[<Test>]
let ``SynMemberDefn.Interface contains the range of the with keyword`` () =
    let parseResults = 
        getParseResults
            """
type Foo() =
    interface Bar with
        member Meh () = ()
    interface Other
"""

    match parseResults with
    | ParsedInput.ImplFile (ParsedImplFileInput (contents = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
        SynModuleDecl.Types(
            typeDefns = [ SynTypeDefn(typeRepr = SynTypeDefnRepr.ObjectModel(members=[ SynMemberDefn.ImplicitCtor _
                                                                                       SynMemberDefn.Interface(withKeyword=Some mWithKeyword)
                                                                                       SynMemberDefn.Interface(withKeyword=None) ])) ]
        )
    ]) ])) ->
        assertRange (3, 18) (3, 22) mWithKeyword
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
                                                                                        SynMemberDefn.AutoProperty(withKeyword=Some mWith)
                                                                                        SynMemberDefn.AutoProperty(withKeyword=None)])) ]
        )
    ]) ])) ->
        assertRange (3, 39) (3, 43) mWith
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
                                                                                        SynMemberDefn.AbstractSlot(slotSig=SynValSig(trivia = { WithKeyword = Some mWith }))])) ]
        )
    ]) ])) ->
        assertRange (3, 30) (3, 34) mWith
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
let ``SynTypeDefn with XmlDoc contains the range of the type keyword`` () =
    let parseResults = 
        getParseResults
            """
/// Doc
// noDoc
type A = B
and C = D
"""

    match parseResults with
    | ParsedInput.ImplFile (ParsedImplFileInput (contents = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
        SynModuleDecl.Types(
            typeDefns = [ SynTypeDefn(trivia={ TypeKeyword = Some mType })
                          SynTypeDefn(trivia={ TypeKeyword = None }) ]
        )
    ]) ])) ->
        assertRange (4, 0) (4, 4) mType
    | _ -> Assert.Fail "Could not get valid AST"

[<Test>]
let ``SynTypeDefn with attribute contains the range of the type keyword`` () =
    let parseResults = 
        getParseResults
            """
[<MyAttribute>]
// noDoc
type A = B
"""

    match parseResults with
    | ParsedInput.ImplFile (ParsedImplFileInput (contents = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
        SynModuleDecl.Types(
            typeDefns = [ SynTypeDefn(trivia={ TypeKeyword = Some mType }) ]
        )
    ]) ])) ->
        assertRange (4, 0) (4, 4) mType
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
let ``SynType.Fun has range of arrow`` () =
    let parseResults =
        getParseResults
             """
     type X = string -> // after a tuple, mixed needs an indent 
                 int
 """

    match parseResults with
    | ParsedInput.ImplFile (ParsedImplFileInput(contents = [
        SynModuleOrNamespace.SynModuleOrNamespace(decls = [
            SynModuleDecl.Types(typeDefns = [
                SynTypeDefn(typeRepr = SynTypeDefnRepr.Simple(simpleRepr =
                    SynTypeDefnSimpleRepr.TypeAbbrev(rhsType =
                        SynType.Fun(trivia = { ArrowRange = mArrow }))))
            ])
        ])
    ])) ->
        assertRange (2, 21) (2, 23) mArrow
    | _ -> Assert.Fail $"Could not get valid AST, got {parseResults}"

[<Test>]
let ``SynType.Tuple with struct`` () =
    let parseResults =
        getParseResults
            """
let _: struct (int * int) = ()
"""

    match parseResults with
    | ParsedInput.ImplFile (ParsedImplFileInput(contents = [
        SynModuleOrNamespace.SynModuleOrNamespace(decls = [
            SynModuleDecl.Let(bindings = [ SynBinding(returnInfo = Some (SynBindingReturnInfo(typeName =
                SynType.Tuple(true, [ SynTupleTypeSegment.Type _ ; SynTupleTypeSegment.Star _ ; SynTupleTypeSegment.Type _ ], mTuple)))) ])
            ])
        ])
     ) ->
        assertRange (2, 7) (2, 25) mTuple
        
    | _ -> Assert.Fail $"Could not get valid AST, got {parseResults}"

[<Test>]
let ``SynType.Tuple with struct, recovery`` () =
    let parseResults =
        getParseResults
            """
let _: struct (int * int = ()
"""

    match parseResults with
    | ParsedInput.ImplFile (ParsedImplFileInput(contents = [
        SynModuleOrNamespace.SynModuleOrNamespace(decls = [
            SynModuleDecl.Let(bindings = [ SynBinding(returnInfo = Some (SynBindingReturnInfo(typeName =
                SynType.Tuple(true, [ SynTupleTypeSegment.Type _ ; SynTupleTypeSegment.Star _ ; SynTupleTypeSegment.Type _ ], mTuple)))) ])
            ])
        ])
     ) ->
        assertRange (2, 7) (2, 24) mTuple
        
    | _ -> Assert.Fail $"Could not get valid AST, got {parseResults}"

[<Test>]
let ``Named parameters in delegate type`` () =
    let parseResults =
        getParseResults
             """
type Foo = delegate of a: A * b: B -> c:C -> D
 """

    match parseResults with
    | ParsedInput.ImplFile (ParsedImplFileInput(modules = [
        SynModuleOrNamespace.SynModuleOrNamespace(decls = [
            SynModuleDecl.Types(typeDefns = [
                SynTypeDefn(typeRepr = SynTypeDefnRepr.ObjectModel(kind =
                    SynTypeDefnKind.Delegate(signature = SynType.Fun(
                        argType =
                            SynType.Tuple(path = [
                                SynTupleTypeSegment.Type(SynType.SignatureParameter(id = Some a))
                                SynTupleTypeSegment.Star _
                                SynTupleTypeSegment.Type(SynType.SignatureParameter(id = Some b))
                            ])
                        returnType =
                            SynType.Fun(
                                argType = SynType.SignatureParameter(id = Some c)
                                returnType = SynType.LongIdent _
                            )
                    ))))
            ])
        ])
    ])) ->
        Assert.AreEqual("a", a.idText)
        Assert.AreEqual("b", b.idText)
        Assert.AreEqual("c", c.idText)
    | _ -> Assert.Fail $"Could not get valid AST, got {parseResults}"

[<Test>]
let ``Attributes in optional named member parameter`` () =
    let parseResults =
        getParseResults
             """
type X =
    abstract member Y: [<Foo; Bar>] ?a: A -> B
 """

    match parseResults with
    | ParsedInput.ImplFile (ParsedImplFileInput(modules = [
        SynModuleOrNamespace.SynModuleOrNamespace(decls = [
            SynModuleDecl.Types(typeDefns = [
                SynTypeDefn(typeRepr = SynTypeDefnRepr.ObjectModel(
                    members = [
                        SynMemberDefn.AbstractSlot(slotSig = SynValSig(synType =
                            SynType.Fun(
                                argType = SynType.SignatureParameter(
                                    [ { Attributes = [ _ ; _ ] } ],
                                    true,
                                    Some a,
                                    SynType.LongIdent _,
                                    m
                                )
                                returnType = SynType.LongIdent _
                            )
                        ))
                    ]
                ))
            ])
        ])
    ])) ->
        Assert.AreEqual("a", a.idText)
        assertRange (3, 23) (3, 41) m
    | _ -> Assert.Fail $"Could not get valid AST, got {parseResults}"