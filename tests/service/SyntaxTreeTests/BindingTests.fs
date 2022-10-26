module FSharp.Compiler.Service.Tests.SyntaxTreeTests.BindingTestsTests

open FSharp.Compiler.Service.Tests.Common
open FSharp.Compiler.Syntax
open NUnit.Framework

[<Test>]
let ``Range of attribute should be included in SynModuleDecl.Let`` () =
    let parseResults = 
        getParseResults
            """
[<Foo>]
let a = 0"""

    match parseResults with
    | ParsedInput.ImplFile (ParsedImplFileInput (contents = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
        SynModuleDecl.Let(bindings = [SynBinding(range = mb)]) as lt
    ]) ])) ->
        assertRange (2, 0) (3, 5) mb
        assertRange (2, 0) (3, 9) lt.Range
    | _ -> Assert.Fail "Could not get valid AST"

[<Test>]
let ``Range of attribute between let keyword and pattern should be included in SynModuleDecl.Let`` () =
    let parseResults = 
        getParseResults
            """
let [<Literal>] (A x) = 1"""

    match parseResults with
    | ParsedInput.ImplFile (ParsedImplFileInput (contents = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
        SynModuleDecl.Let(bindings = [SynBinding(range = mb)]) as lt
    ]) ])) ->
        assertRange (2, 4) (2, 21) mb
        assertRange (2, 0) (2, 25) lt.Range
    | _ -> Assert.Fail "Could not get valid AST"

[<Test>]
let ``Range of attribute should be included in SynMemberDefn.LetBindings`` () =
    let parseResults = 
        getParseResults
            """
type Bar =
    [<Foo>]
    let x = 8"""

    match parseResults with
    | ParsedInput.ImplFile (ParsedImplFileInput (contents = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
        SynModuleDecl.Types(typeDefns = [SynTypeDefn(typeRepr = SynTypeDefnRepr.ObjectModel(members = [SynMemberDefn.LetBindings(bindings = [SynBinding(range = mb)]) as m]))])
    ]) ])) ->
        assertRange (3, 4) (4, 9) mb
        assertRange (3, 4) (4, 13) m.Range
    | _ -> Assert.Fail "Could not get valid AST"

[<Test>]
let ``Range of attribute should be included in SynMemberDefn.Member`` () =
    let parseResults = 
        getParseResults
            """
type Bar =
    [<Foo>]
    member this.Something () = ()"""

    match parseResults with
    | ParsedInput.ImplFile (ParsedImplFileInput (contents = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
        SynModuleDecl.Types(typeDefns = [SynTypeDefn(typeRepr = SynTypeDefnRepr.ObjectModel(members = [SynMemberDefn.Member(memberDefn = SynBinding(range = mb)) as m]))])
    ]) ])) ->
        assertRange (3, 4) (4, 28) mb
        assertRange (3, 4) (4, 33) m.Range
    | _ -> Assert.Fail "Could not get valid AST"

[<Test>]
let ``Range of attribute should be included in binding of SynExpr.ObjExpr`` () =
    let parseResults = 
        getParseResults
            """
{ new System.Object() with
    [<Foo>]
    member x.ToString() = "F#" }"""

    match parseResults with
    | ParsedInput.ImplFile (ParsedImplFileInput (contents = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
        SynModuleDecl.Expr(expr = SynExpr.ObjExpr(members = [SynMemberDefn.Member(memberDefn=SynBinding(range = mb))]))
    ]) ])) ->
        assertRange (3, 4) (4, 23) mb
    | _ -> Assert.Fail "Could not get valid AST"

[<Test>]
let ``Range of attribute should be included in constructor SynMemberDefn.Member`` () =
    let parseResults = 
        getParseResults
            """
type Tiger =
    [<Foo>]
    new () = ()"""

    match parseResults with
    | ParsedInput.ImplFile (ParsedImplFileInput (contents = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
        SynModuleDecl.Types(typeDefns = [SynTypeDefn(typeRepr = SynTypeDefnRepr.ObjectModel(members = [SynMemberDefn.Member(memberDefn = SynBinding(range = mb)) as m]))])
    ]) ])) ->
        assertRange (3, 4) (4, 10) mb
        assertRange (3, 4) (4, 15) m.Range
    | _ -> Assert.Fail "Could not get valid AST"

[<Test>]
let ``Range of attribute should be included in constructor SynMemberDefn.Member, optAsSpec`` () =
    let parseResults = 
        getParseResults
            """
type Tiger =
    [<Foo>]
    new () as tony = ()"""

    match parseResults with
    | ParsedInput.ImplFile (ParsedImplFileInput (contents = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
        SynModuleDecl.Types(typeDefns = [SynTypeDefn(typeRepr = SynTypeDefnRepr.ObjectModel(members = [SynMemberDefn.Member(memberDefn = SynBinding(range = mb)) as m]))])
    ]) ])) ->
        assertRange (3, 4) (4, 18) mb
        assertRange (3, 4) (4, 23) m.Range
    | _ -> Assert.Fail "Could not get valid AST"

[<Test>]
let ``Range of attribute should be included in secondary constructor`` () =
    let parseResults = 
        getParseResults
            """
type T() =
    new () =
        T ()

    internal new () =
        T ()

    [<Foo>]
    new () =
        T ()"""

    match parseResults with
    | ParsedInput.ImplFile (ParsedImplFileInput (contents = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
        SynModuleDecl.Types(typeDefns = [SynTypeDefn(typeRepr = SynTypeDefnRepr.ObjectModel(members = [
            SynMemberDefn.ImplicitCtor _
            SynMemberDefn.Member(memberDefn = SynBinding(range = mb1)) as m1
            SynMemberDefn.Member(memberDefn = SynBinding(range = mb2)) as m2
            SynMemberDefn.Member(memberDefn = SynBinding(range = mb3)) as m3
        ]))])
    ]) ])) ->
        assertRange (3, 4) (3, 10) mb1
        assertRange (3, 4) (4, 12) m1.Range
        assertRange (6, 4) (6, 19) mb2
        assertRange (6, 4) (7, 12) m2.Range
        assertRange (9, 4) (10, 10) mb3
        assertRange (9, 4) (11, 12) m3.Range
    | _ -> Assert.Fail "Could not get valid AST"

[<Test>]
let ``Range of attribute should be included in write only SynMemberDefn.Member property`` () =
    let parseResults = 
        getParseResults
            """
type Crane =
    [<Foo>]
    member this.MyWriteOnlyProperty with set (value) = myInternalValue <- value"""

    match parseResults with
    | ParsedInput.ImplFile (ParsedImplFileInput (contents = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
        SynModuleDecl.Types(typeDefns = [SynTypeDefn(typeRepr = SynTypeDefnRepr.ObjectModel(members =
            [SynMemberDefn.GetSetMember(memberDefnForSet = Some (SynBinding(range = mb))) as m]))])
    ]) ])) ->
        assertRange (3, 4) (4, 52) mb
        assertRange (3, 4) (4, 79) m.Range
    | _ -> Assert.Fail "Could not get valid AST"

[<Test>]
let ``Range of attribute should be included in full SynMemberDefn.Member property`` () =
    let parseResults = 
        getParseResults
            """
type Bird =
    [<Foo>]
    member this.TheWord
        with get () = myInternalValue
        and set (value) = myInternalValue <- value"""

    match parseResults with
    | ParsedInput.ImplFile (ParsedImplFileInput (contents = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
        SynModuleDecl.Types(typeDefns = [SynTypeDefn(typeRepr = SynTypeDefnRepr.ObjectModel(members = [
            SynMemberDefn.GetSetMember(Some (SynBinding(range = mb1)), Some (SynBinding(range = mb2)), m, _)
        ]))])
    ]) ])) ->
        assertRange (3, 4) (5, 19) mb1
        assertRange (3, 4) (6, 23) mb2
        assertRange (3, 4) (6, 50) m
    | _ -> Assert.Fail "Could not get valid AST"

[<Test>]
let ``Range of equal sign should be present in SynModuleDecl.Let binding`` () =
    let parseResults = 
        getParseResults "let v = 12"

    match parseResults with
    | ParsedInput.ImplFile (ParsedImplFileInput (contents = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
        SynModuleDecl.Let(bindings = [SynBinding(trivia={ EqualsRange = Some mEquals })])
    ]) ])) ->
        assertRange (1, 6) (1, 7) mEquals
    | _ -> Assert.Fail "Could not get valid AST"

[<Test>]
let ``Range of equal sign should be present in SynModuleDecl.Let binding, typed`` () =
    let parseResults = 
        getParseResults "let v : int = 12"

    match parseResults with
    | ParsedInput.ImplFile (ParsedImplFileInput (contents = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
        SynModuleDecl.Let(bindings = [SynBinding(trivia={ EqualsRange = Some mEquals })])
    ]) ])) ->
        assertRange (1, 12) (1, 13) mEquals
    | _ -> Assert.Fail "Could not get valid AST"

[<Test>]
let ``Range of equal sign should be present in local Let binding`` () =
    let parseResults = 
        getParseResults
            """
do
    let z = 2
    ()
"""

    match parseResults with
    | ParsedInput.ImplFile (ParsedImplFileInput (contents = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
        SynModuleDecl.Expr(expr = SynExpr.Do(expr = SynExpr.LetOrUse(bindings = [SynBinding(trivia={ EqualsRange = Some mEquals })])))
    ]) ])) ->
        assertRange (3, 10) (3, 11) mEquals
    | _ -> Assert.Fail "Could not get valid AST"

[<Test>]
let ``Range of equal sign should be present in local Let binding, typed`` () =
    let parseResults = 
        getParseResults
            """
do
    let z: int = 2
    ()
"""

    match parseResults with
    | ParsedInput.ImplFile (ParsedImplFileInput (contents = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
        SynModuleDecl.Expr(expr = SynExpr.Do(expr = SynExpr.LetOrUse(bindings = [SynBinding(trivia={ EqualsRange = Some mEquals })])))
    ]) ])) ->
        assertRange (3, 15) (3, 16) mEquals
    | _ -> Assert.Fail "Could not get valid AST"

[<Test>]
let ``Range of equal sign should be present in member binding`` () =
    let parseResults = 
        getParseResults
            """
type X() =
    member this.Y = z
"""

    match parseResults with
    | ParsedInput.ImplFile (ParsedImplFileInput (contents = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
        SynModuleDecl.Types(typeDefns = [SynTypeDefn(typeRepr = SynTypeDefnRepr.ObjectModel(members = [ _; SynMemberDefn.Member(memberDefn = SynBinding(trivia={ EqualsRange = Some mEquals }))]))])
    ]) ])) ->
        assertRange (3, 18) (3, 19) mEquals
    | _ -> Assert.Fail "Could not get valid AST"

[<Test>]
let ``Range of equal sign should be present in member binding, with parameters`` () =
    let parseResults = 
        getParseResults
            """
type X() =
    member this.Y () = z
"""

    match parseResults with
    | ParsedInput.ImplFile (ParsedImplFileInput (contents = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
        SynModuleDecl.Types(typeDefns = [SynTypeDefn(typeRepr = SynTypeDefnRepr.ObjectModel(members = [ _; SynMemberDefn.Member(memberDefn = SynBinding(trivia={ EqualsRange = Some mEquals }))]))])
    ]) ])) ->
        assertRange (3, 21) (3, 22) mEquals
    | _ -> Assert.Fail "Could not get valid AST"

[<Test>]
let ``Range of equal sign should be present in member binding, with return type`` () =
    let parseResults = 
        getParseResults
            """
type X() =
    member this.Y () : string = z
"""

    match parseResults with
    | ParsedInput.ImplFile (ParsedImplFileInput (contents = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
        SynModuleDecl.Types(typeDefns = [SynTypeDefn(typeRepr = SynTypeDefnRepr.ObjectModel(members = [ _; SynMemberDefn.Member(memberDefn = SynBinding(trivia={ EqualsRange = Some mEquals }))]))])
    ]) ])) ->
        assertRange (3, 30) (3, 31) mEquals
    | _ -> Assert.Fail "Could not get valid AST"

[<Test>]
let ``Range of equal sign should be present in property`` () =
    let parseResults = 
        getParseResults
            """
type Y() =
    member this.MyReadWriteProperty
        with get () = myInternalValue
        and set (value) = myInternalValue <- value
"""

    match parseResults with
    | ParsedInput.ImplFile (ParsedImplFileInput (contents = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
        SynModuleDecl.Types(typeDefns = [SynTypeDefn(typeRepr = SynTypeDefnRepr.ObjectModel(members = [
            _
            SynMemberDefn.GetSetMember(
                Some(SynBinding(trivia={ EqualsRange = Some eqGetM })),
                Some(SynBinding(trivia={ EqualsRange = Some eqSetM })), _, _)
        ]))])
    ]) ])) ->
        assertRange (4, 20) (4, 21) eqGetM
        assertRange (5, 24) (5, 25) eqSetM
    | _ -> Assert.Fail "Could not get valid AST"

[<Test>]
let ``Range of let keyword should be present in SynModuleDecl.Let binding`` () =
    let parseResults = 
        getParseResults "let v = 12"

    match parseResults with
    | ParsedInput.ImplFile (ParsedImplFileInput (contents = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
        SynModuleDecl.Let(bindings = [SynBinding(trivia={ LetKeyword = Some mLet })])
    ]) ])) ->
        assertRange (1, 0) (1, 3) mLet
    | _ -> Assert.Fail "Could not get valid AST"

[<Test>]
let ``Range of let keyword should be present in SynModuleDecl.Let binding with attributes`` () =
    let parseResults = 
        getParseResults """
/// XmlDoc
[<SomeAttribute>]
// some comment
let v = 12
"""

    match parseResults with
    | ParsedInput.ImplFile (ParsedImplFileInput (contents = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
        SynModuleDecl.Let(bindings = [SynBinding(trivia={ LetKeyword = Some mLet })])
    ]) ])) ->
        assertRange (5, 0) (5, 3) mLet
    | _ -> Assert.Fail "Could not get valid AST"

[<Test>]
let ``Range of let keyword should be present in SynExpr.LetOrUse binding`` () =
    let parseResults = 
        getParseResults """
let a =
    let b c = d
    ()
"""

    match parseResults with
    | ParsedInput.ImplFile (ParsedImplFileInput (contents = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
        SynModuleDecl.Let(bindings = [SynBinding(expr=SynExpr.LetOrUse(bindings=[SynBinding(trivia={ LetKeyword = Some mLet })]))])
    ]) ])) ->
        assertRange (3, 4) (3, 7) mLet
    | _ -> Assert.Fail "Could not get valid AST"

[<Test>]
let ``Tuple return type of binding should contain stars`` () =
    let parseResults = 
        getParseResults """
let a : int * string = failwith "todo"
let b : int * string * bool = 1, "", false
"""

    match parseResults with
    | ParsedInput.ImplFile (ParsedImplFileInput (contents = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
        SynModuleDecl.Let(bindings = [
            SynBinding(returnInfo =
                Some (SynBindingReturnInfo(typeName = SynType.Tuple(path = [
                    SynTupleTypeSegment.Type _
                    SynTupleTypeSegment.Star _
                    SynTupleTypeSegment.Type _
                ]))))
        ])
        SynModuleDecl.Let(bindings = [
            SynBinding(returnInfo =
                Some (SynBindingReturnInfo(typeName = SynType.Tuple(path = [
                    SynTupleTypeSegment.Type _
                    SynTupleTypeSegment.Star _
                    SynTupleTypeSegment.Type _
                    SynTupleTypeSegment.Star _
                    SynTupleTypeSegment.Type _
                ]))))
        ])
    ]) ])) ->
        Assert.Pass ()
    | _ -> Assert.Fail $"Could not get valid AST, got {parseResults}"
