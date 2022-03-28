#if INTERACTIVE
#r "../../artifacts/bin/fcs/net461/FSharp.Compiler.Service.dll" // note, build FSharp.Compiler.Service.Tests.fsproj to generate this, this DLL has a public API so can be used from F# Interactive
#r "../../artifacts/bin/fcs/net461/nunit.framework.dll"
#load "FsUnit.fs"
#load "Common.fs"
#else
module Tests.Service.Symbols
#endif

open System
open FSharp.Compiler.Service.Tests.Common
open FSharp.Compiler.Symbols
open FSharp.Compiler.Syntax
open FSharp.Compiler.SyntaxTrivia
open FsUnit
open NUnit.Framework

module ActivePatterns =

    let completePatternInput = """
let (|True|False|) = function
    | true -> True
    | false -> False

match true with
| True | False -> ()
"""

    let partialPatternInput = """
let (|String|_|) = function
    | :? String -> Some ()
    | _ -> None

match "foo" with
| String
| _ -> ()
"""

    let getCaseUsages source line =
         let fileName, options = mkTestFileAndOptions source [| |]
         let _, checkResults = parseAndCheckFile fileName source options
          
         checkResults.GetAllUsesOfAllSymbolsInFile()
         |> Array.ofSeq
         |> Array.filter (fun su -> su.Range.StartLine = line && su.Symbol :? FSharpActivePatternCase)
         |> Array.map (fun su -> su.Symbol :?> FSharpActivePatternCase)

    [<Test>]
    let ``Active pattern case indices`` () =
        let getIndices = Array.map (fun (case: FSharpActivePatternCase) -> case.Index)

        getCaseUsages completePatternInput 7 |> getIndices |> shouldEqual [| 0; 1 |]
        getCaseUsages partialPatternInput 7 |> getIndices |> shouldEqual [| 0 |]

    [<Test>]
    let ``Active pattern group names`` () =
        let getGroupName (case: FSharpActivePatternCase) = case.Group.Name.Value

        getCaseUsages completePatternInput 7 |> Array.head |> getGroupName |> shouldEqual "|True|False|"
        getCaseUsages partialPatternInput 7 |> Array.head |> getGroupName |> shouldEqual "|String|_|"

module ExternDeclarations =
    [<Test>]
    let ``Access modifier`` () =
        let parseResults, checkResults = getParseAndCheckResults """
extern int a()
extern int public b()
extern int private c()
"""
        let (SynModuleOrNamespace (decls = decls)) = getSingleModuleLikeDecl parseResults.ParseTree

        [ None
          Some SynAccess.Public
          Some SynAccess.Private ]
        |> List.zip decls
        |> List.iter (fun (actual, expected) ->
            match actual with
            | SynModuleDecl.Let (_, [SynBinding (accessibility = access)], _) -> access |> should equal expected
            | decl -> Assert.Fail (sprintf "unexpected decl: %O" decl))

        [ "a", (true, false, false, false)
          "b", (true, false, false, false)
          "c", (false, false, false, true) ]
        |> List.iter (fun (name, expected) ->
            match findSymbolByName name checkResults with
            | :? FSharpMemberOrFunctionOrValue as mfv ->
                let access = mfv.Accessibility
                (access.IsPublic, access.IsProtected, access.IsInternal, access.IsPrivate)
                |> should equal expected
            | _ -> Assert.Fail (sprintf "Couldn't get mfv: %s" name))

    [<Test>]
    let ``Range of attribute should be included in SynDecl.Let and SynBinding`` () =
        let parseResults =
            getParseResults
                """
[<DllImport("oleacc.dll")>]
extern int AccessibleChildren()"""

        match parseResults with
        | ParsedInput.ImplFile (ParsedImplFileInput (modules = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
            SynModuleDecl.Let(false, [ SynBinding(range = mb) ] , ml)
        ]) ])) ->
            assertRange (2, 0) (3, 31) ml
            assertRange (2, 0) (3, 31) mb
        | _ -> Assert.Fail "Could not get valid AST"

module XmlDocSig =

    [<Test>]
    let ``XmlDocSig of modules in namespace`` () =
        let source = """
namespace Ns1
module Mod1 =
    let val1 = 1
    module Mod2 =
       let func2 () = ()
"""
        let fileName, options = mkTestFileAndOptions source [| |]
        let _, checkResults = parseAndCheckFile fileName source options  

        let mod1 = checkResults.PartialAssemblySignature.FindEntityByPath ["Ns1"; "Mod1"] |> Option.get
        let mod2 = checkResults.PartialAssemblySignature.FindEntityByPath ["Ns1"; "Mod1"; "Mod2"] |> Option.get
        let mod1val1 = mod1.MembersFunctionsAndValues |> Seq.find (fun m -> m.DisplayName = "val1")
        let mod2func2 = mod2.MembersFunctionsAndValues |> Seq.find (fun m -> m.DisplayName = "func2")
        mod1.XmlDocSig |> shouldEqual "T:Ns1.Mod1"
        mod2.XmlDocSig |> shouldEqual "T:Ns1.Mod1.Mod2"
        mod1val1.XmlDocSig |> shouldEqual "P:Ns1.Mod1.val1"
        mod2func2.XmlDocSig |> shouldEqual "M:Ns1.Mod1.Mod2.func2"

    [<Test>]
    let ``XmlDocSig of modules`` () =
         let source = """
module Mod1 
let val1 = 1
module Mod2 =
    let func2 () = ()
"""
         let fileName, options = mkTestFileAndOptions source [| |]
         let _, checkResults = parseAndCheckFile fileName source options  

         let mod1 = checkResults.PartialAssemblySignature.FindEntityByPath ["Mod1"] |> Option.get
         let mod2 = checkResults.PartialAssemblySignature.FindEntityByPath ["Mod1"; "Mod2"] |> Option.get
         let mod1val1 = mod1.MembersFunctionsAndValues |> Seq.find (fun m -> m.DisplayName = "val1")
         let mod2func2 = mod2.MembersFunctionsAndValues |> Seq.find (fun m -> m.DisplayName = "func2")
         mod1.XmlDocSig |> shouldEqual "T:Mod1"
         mod2.XmlDocSig |> shouldEqual "T:Mod1.Mod2"
         mod1val1.XmlDocSig |> shouldEqual "P:Mod1.val1"
         mod2func2.XmlDocSig |> shouldEqual "M:Mod1.Mod2.func2"

module Attributes =
    [<Test>]
    let ``Emit conditional attributes`` () =
        let source = """
open System
open System.Diagnostics

[<Conditional("Bar")>]
type FooAttribute() =
    inherit Attribute()

[<Foo>]
let x = 123
"""
        let fileName, options = mkTestFileAndOptions source [| "--noconditionalerasure" |]
        let _, checkResults = parseAndCheckFile fileName source options

        checkResults.GetAllUsesOfAllSymbolsInFile()
        |> Array.ofSeq
        |> Array.tryFind (fun su -> su.Symbol.DisplayName = "x")
        |> Option.orElseWith (fun _ -> failwith "Could not get symbol")
        |> Option.map (fun su -> su.Symbol :?> FSharpMemberOrFunctionOrValue)
        |> Option.iter (fun symbol -> symbol.Attributes.Count |> shouldEqual 1)

module Types =
    [<Test>]
    let ``FSharpType.Print parent namespace qualifiers`` () =
        let _, checkResults = getParseAndCheckResults """
namespace Ns1.Ns2
type T() = class end
type A = T

namespace Ns1.Ns3
type B = Ns1.Ns2.T

namespace Ns1.Ns4
open Ns1.Ns2
type C = Ns1.Ns2.T

namespace Ns1.Ns5
open Ns1
type D = Ns1.Ns2.T

namespace Ns1.Ns2.Ns6
type E = Ns1.Ns2.T
"""
        [| "A", "T"
           "B", "Ns1.Ns2.T"
           "C", "T"
           "D", "Ns2.T"
           "E", "Ns1.Ns2.T" |]
        |> Array.iter (fun (symbolName, expectedPrintedType) ->
            let symbolUse = findSymbolUseByName symbolName checkResults
            match symbolUse.Symbol with
            | :? FSharpEntity as entity ->
                entity.AbbreviatedType.Format(symbolUse.DisplayContext)
                |> should equal expectedPrintedType

            | _ -> Assert.Fail (sprintf "Couldn't get entity: %s" symbolName))

    [<Test>]
    let ``FSharpType.Format can use prefix representations`` () =
            let _, checkResults = getParseAndCheckResults """
type 't folks =
| Nil
| Cons of 't * 't folks

let tester: int folks = Cons(1, Nil)
"""
            let prefixForm = "folks<int>"
            let entity = "tester"
            let symbolUse = findSymbolUseByName entity checkResults
            match symbolUse.Symbol with
            | :? FSharpMemberOrFunctionOrValue as v ->
                    v.FullType.Format (symbolUse.DisplayContext.WithPrefixGenericParameters())
                    |> should equal prefixForm
            | _ -> Assert.Fail (sprintf "Couldn't get member: %s" entity)

    [<Test>]
    let ``FSharpType.Format can use suffix representations`` () =
            let _, checkResults = getParseAndCheckResults """
type Folks<'t> =
| Nil
| Cons of 't * Folks<'t>

let tester: Folks<int> = Cons(1, Nil)
"""
            let suffixForm = "int Folks"
            let entity = "tester"
            let symbolUse = findSymbolUseByName entity checkResults
            match symbolUse.Symbol with
            | :? FSharpMemberOrFunctionOrValue as v ->
                    v.FullType.Format (symbolUse.DisplayContext.WithSuffixGenericParameters())
                    |> should equal suffixForm
            | _ -> Assert.Fail (sprintf "Couldn't get member: %s" entity)

    [<Test>]
    let ``FSharpType.Format defaults to derived suffix representations`` () =
            let _, checkResults = getParseAndCheckResults """
type Folks<'t> =
| Nil
| Cons of 't * Folks<'t>

type 't Group = 't list

let tester: Folks<int> = Cons(1, Nil)

let tester2: int Group = []
"""
            let cases =
                ["tester", "Folks<int>"
                 "tester2", "int Group"]
            cases
            |> List.iter (fun (entityName, expectedTypeFormat) ->
                let symbolUse = findSymbolUseByName entityName checkResults
                match symbolUse.Symbol with
                | :? FSharpMemberOrFunctionOrValue as v ->
                        v.FullType.Format symbolUse.DisplayContext
                        |> should equal expectedTypeFormat
                | _ -> Assert.Fail (sprintf "Couldn't get member: %s" entityName)
            )

    [<Test>]
    let ``Single SynEnumCase contains range of constant`` () =
        let parseResults = 
            getParseResults
                """
type Foo = One = 0x00000001
"""

        match parseResults with
        | ParsedInput.ImplFile (ParsedImplFileInput (modules = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
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
        | ParsedInput.ImplFile (ParsedImplFileInput (modules = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
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
        | ParsedInput.ImplFile (ParsedImplFileInput (modules = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
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
        | ParsedInput.ImplFile (ParsedImplFileInput (modules = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
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
        | ParsedInput.ImplFile (ParsedImplFileInput (modules = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
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
        | ParsedInput.ImplFile (ParsedImplFileInput (modules = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
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
        | ParsedInput.ImplFile (ParsedImplFileInput (modules = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
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
        | ParsedInput.ImplFile (ParsedImplFileInput (modules = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
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
        | ParsedInput.ImplFile (ParsedImplFileInput (modules = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
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
        | ParsedInput.ImplFile (ParsedImplFileInput (modules = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
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
        | ParsedInput.ImplFile (ParsedImplFileInput (modules = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
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
        | ParsedInput.ImplFile (ParsedImplFileInput (modules = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
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
        | ParsedInput.ImplFile (ParsedImplFileInput (modules = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
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
        | ParsedInput.ImplFile (ParsedImplFileInput (modules = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
            SynModuleDecl.Types(
                typeDefns = [ SynTypeDefn(typeRepr = SynTypeDefnRepr.ObjectModel(members = [_
                                                                                            SynMemberDefn.AbstractSlot(slotSig=SynValSig(withKeyword=Some mWith))])) ]
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
        | ParsedInput.ImplFile (ParsedImplFileInput (modules = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
            SynModuleDecl.Types(
                typeDefns = [ SynTypeDefn(typeRepr =
                    SynTypeDefnRepr.ObjectModel(members=[ _
                                                          SynMemberDefn.Member(memberDefn=SynBinding(headPat=SynPat.LongIdent(propertyKeyword=Some(PropertyKeyword.With mWith)))) ])
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
        | ParsedInput.ImplFile (ParsedImplFileInput (modules = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
            SynModuleDecl.Types(
                typeDefns = [ SynTypeDefn(typeRepr =
                    SynTypeDefnRepr.ObjectModel(members=[ _
                                                          SynMemberDefn.Member(memberDefn=SynBinding(headPat=SynPat.LongIdent(propertyKeyword=Some(PropertyKeyword.With mWith)))) ])
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
        | ParsedInput.ImplFile (ParsedImplFileInput (modules = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
            SynModuleDecl.Types(
                typeDefns = [ SynTypeDefn(typeRepr =
                    SynTypeDefnRepr.ObjectModel(members=[ _
                                                          SynMemberDefn.Member(memberDefn=SynBinding(headPat=SynPat.LongIdent(propertyKeyword=Some(PropertyKeyword.With mWith))))
                                                          SynMemberDefn.Member(memberDefn=SynBinding(headPat=SynPat.LongIdent(propertyKeyword=Some(PropertyKeyword.And mAnd)))) ])
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
        | ParsedInput.ImplFile (ParsedImplFileInput (modules = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
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
        | ParsedInput.ImplFile (ParsedImplFileInput (modules = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
            SynModuleDecl.Types(
                typeDefns = [ SynTypeDefn(trivia={ TypeKeyword = Some mType }) ]
            )
        ]) ])) ->
            assertRange (4, 0) (4, 4) mType
        | _ -> Assert.Fail "Could not get valid AST"

module SyntaxExpressions =
    [<Test>]
    let ``SynExpr.Do contains the range of the do keyword`` () =
        let ast = """let a =
    do
        foobar
    do!
        foobarBang
"""
                        |> getParseResults

        match ast with
        | ParsedInput.ImplFile(ParsedImplFileInput(modules = [
                    SynModuleOrNamespace.SynModuleOrNamespace(decls = [
                        SynModuleDecl.Let(bindings = [
                            SynBinding(expr = SynExpr.Sequential(expr1 = SynExpr.Do(_, doRange) ; expr2 = SynExpr.DoBang(_, doBangRange)))
                        ])
                    ])
                ])) ->
            assertRange (2, 4) (3, 14) doRange
            assertRange (4, 4) (5, 18) doBangRange
        | _ ->
            Assert.Fail "Could not find SynExpr.Do"

    [<Test>]
    let ``SynExpr.LetOrUseBang contains the range of the equals sign`` () =
        let ast =
            """
comp {
    let! x = y
    and! z = someFunction ()
    return ()
}
"""
            |> getParseResults

        match ast with
        | ParsedInput.ImplFile(ParsedImplFileInput(modules = [
                    SynModuleOrNamespace.SynModuleOrNamespace(decls = [
                        SynModuleDecl.Expr(expr = SynExpr.App(argExpr =
                            SynExpr.ComputationExpr(expr =
                                SynExpr.LetOrUseBang(trivia = { EqualsRange = Some mLetBangEquals }
                                                     andBangs = [ SynExprAndBang(trivia= { EqualsRange = mAndBangEquals }) ]))))
                    ])
                ])) ->
            assertRange (3, 11) (3, 12) mLetBangEquals
            assertRange (4, 11) (4, 12) mAndBangEquals
        | _ -> Assert.Fail "Could not get valid AST"

    [<Test>]
    let ``SynExpr.Record contains the range of the equals sign in SynExprRecordField`` () =
        let ast =
            """
{ V = v
  X      =   // some comment
                someLongFunctionCall
                    a
                    b
                    c }
"""
            |> getParseResults

        match ast with
        | ParsedInput.ImplFile(ParsedImplFileInput(modules = [
                    SynModuleOrNamespace.SynModuleOrNamespace(decls = [
                        SynModuleDecl.Expr(expr =
                            SynExpr.Record(recordFields = [
                                SynExprRecordField(equalsRange = Some mEqualsV)
                                SynExprRecordField(equalsRange = Some mEqualsX)
                            ]))
                    ])
                ])) ->
            assertRange (2, 4) (2, 5) mEqualsV
            assertRange (3, 9) (3, 10) mEqualsX
        | _ -> Assert.Fail "Could not get valid AST"

    [<Test>]
    let ``inherit SynExpr.Record contains the range of the equals sign in SynExprRecordField`` () =
        let ast =
            """
{ inherit Exception(msg); X = 1; }
"""
            |> getParseResults

        match ast with
        | ParsedInput.ImplFile(ParsedImplFileInput(modules = [
                    SynModuleOrNamespace.SynModuleOrNamespace(decls = [
                        SynModuleDecl.Expr(expr =
                            SynExpr.Record(baseInfo = Some _ ; recordFields = [
                                SynExprRecordField(equalsRange = Some mEquals)
                            ]))
                    ])
                ])) ->
            assertRange (2, 28) (2, 29) mEquals
        | _ -> Assert.Fail "Could not get valid AST"

    [<Test>]
    let ``copy SynExpr.Record contains the range of the equals sign in SynExprRecordField`` () =
        let ast =
            """
{ foo with
        X
            =
                12 }
"""
            |> getParseResults

        match ast with
        | ParsedInput.ImplFile(ParsedImplFileInput(modules = [
                    SynModuleOrNamespace.SynModuleOrNamespace(decls = [
                        SynModuleDecl.Expr(expr =
                            SynExpr.Record(copyInfo = Some _ ; recordFields = [
                                SynExprRecordField(equalsRange = Some mEquals)
                            ]))
                    ])
                ])) ->
            assertRange (4, 12) (4, 13) mEquals
        | _ -> Assert.Fail "Could not get valid AST"

    [<Test>]
    let ``SynExpr.AnonRecord contains the range of the equals sign in the fields`` () =
        let ast =
            """
{| X = 5
   Y    = 6
   Z        = 7 |}
"""
            |> getParseResults

        match ast with
        | ParsedInput.ImplFile(ParsedImplFileInput(modules = [
                    SynModuleOrNamespace.SynModuleOrNamespace(decls = [
                        SynModuleDecl.Expr(expr =
                            SynExpr.AnonRecd(recordFields = [
                                (_, Some mEqualsX, _)
                                (_, Some mEqualsY, _)
                                (_, Some mEqualsZ, _)
                            ]))
                    ])
                ])) ->
            assertRange (2, 5) (2, 6) mEqualsX
            assertRange (3, 8) (3, 9) mEqualsY
            assertRange (4, 12) (4, 13) mEqualsZ
        | _ -> Assert.Fail "Could not get valid AST"
        
    [<Test>]
    let ``SynExpr.For contains the range of the equals sign`` () =
        let ast =
            """
for i = 1 to 10 do
    printf "%d " i
"""
            |> getParseResults

        match ast with
        | ParsedInput.ImplFile(ParsedImplFileInput(modules = [
                    SynModuleOrNamespace.SynModuleOrNamespace(decls = [
                        SynModuleDecl.Expr(expr =
                            SynExpr.For(equalsRange = Some mEquals))
                    ])
                ])) ->
            assertRange (2, 6) (2, 7) mEquals
        | _ -> Assert.Fail "Could not get valid AST"

    [<Test>]
    let ``SynExpr.TryWith contains the range of the try and with keyword`` () =
        let ast =
            """
try
    x
with
| ex -> y
"""
            |> getParseResults

        match ast with
        | ParsedInput.ImplFile(ParsedImplFileInput(modules = [
                    SynModuleOrNamespace.SynModuleOrNamespace(decls = [
                        SynModuleDecl.Expr(expr =
                            SynExpr.TryWith(trivia={ TryKeyword = mTry; WithKeyword = mWith }))
                    ])
                ])) ->
            assertRange (2, 0) (2, 3) mTry
            assertRange (4, 0) (4, 4) mWith
        | _ -> Assert.Fail "Could not get valid AST"

    [<Test>]
    let ``SynExpr.TryFinally contains the range of the try and with keyword`` () =
        let ast =
            """
try
    x
finally
    ()
"""
            |> getParseResults

        match ast with
        | ParsedInput.ImplFile(ParsedImplFileInput(modules = [
                    SynModuleOrNamespace.SynModuleOrNamespace(decls = [
                        SynModuleDecl.Expr(expr =
                            SynExpr.TryFinally(trivia={ TryKeyword = mTry; FinallyKeyword = mFinally }))
                    ])
                ])) ->
            assertRange (2, 0) (2, 3) mTry
            assertRange (4, 0) (4, 7) mFinally
        | _ -> Assert.Fail "Could not get valid AST"

    [<Test>]
    let ``SynExpr.Match contains the range of the match and with keyword`` () =
        let ast =
            """
match x with
| y -> z
"""
            |> getParseResults

        match ast with
        | ParsedInput.ImplFile(ParsedImplFileInput(modules = [
                    SynModuleOrNamespace.SynModuleOrNamespace(decls = [
                        SynModuleDecl.Expr(expr =
                            SynExpr.Match(matchKeyword=mMatch; withKeyword=mWith))
                    ])
                ])) ->
            assertRange (2, 0) (2, 5) mMatch
            assertRange (2, 8) (2, 12) mWith
        | _ -> Assert.Fail "Could not get valid AST"

    [<Test>]
    let ``SynExpr.MatchBang contains the range of the match and with keyword`` () =
        let ast =
            """
match! x with
| y -> z
"""
            |> getParseResults

        match ast with
        | ParsedInput.ImplFile(ParsedImplFileInput(modules = [
                    SynModuleOrNamespace.SynModuleOrNamespace(decls = [
                        SynModuleDecl.Expr(expr =
                            SynExpr.MatchBang(matchKeyword=mMatch; withKeyword=mWith))
                    ])
                ])) ->
            assertRange (2, 0) (2, 6) mMatch
            assertRange (2, 9) (2, 13) mWith
        | _ -> Assert.Fail "Could not get valid AST"

    [<Test>]
    let ``SynExpr.ObjExpr contains the range of with keyword`` () =
        let ast =
            """
{ new obj() with
    member x.ToString() = "INotifyEnumerableInternal"
  interface INotifyEnumerableInternal<'T>
  interface IEnumerable<_> with
    member x.GetEnumerator() = null }
"""
            |> getParseResults

        match ast with
        | ParsedInput.ImplFile(ParsedImplFileInput(modules = [
                    SynModuleOrNamespace.SynModuleOrNamespace(decls = [
                        SynModuleDecl.Expr(expr =
                            SynExpr.ObjExpr(withKeyword=Some mWithObjExpr; extraImpls=[ SynInterfaceImpl(withKeyword=None); SynInterfaceImpl(withKeyword=Some mWithSynInterfaceImpl) ]))
                    ])
                ])) ->
            assertRange (2, 12) (2, 16) mWithObjExpr
            assertRange (5, 27) (5, 31) mWithSynInterfaceImpl
        | _ -> Assert.Fail "Could not get valid AST"

    [<Test>]
    let ``SynExpr.LetOrUse contains the range of in keyword`` () =
        let ast =
            getParseResults "let x = 1 in ()"

        match ast with
        | ParsedInput.ImplFile(ParsedImplFileInput(modules = [
                    SynModuleOrNamespace.SynModuleOrNamespace(decls = [
                        SynModuleDecl.Expr(expr =
                            SynExpr.LetOrUse(trivia={ InKeyword = Some mIn }))
                    ])
                ])) ->
            assertRange (1, 10) (1, 12) mIn
        | _ -> Assert.Fail "Could not get valid AST"

    [<Test>]
    let ``SynExpr.LetOrUse with recursive binding contains the range of in keyword`` () =
        let ast =
            getParseResults """
do
    let rec f = ()
    and g = () in
    ()
"""

        match ast with
        | ParsedInput.ImplFile(ParsedImplFileInput(modules = [
                    SynModuleOrNamespace.SynModuleOrNamespace(decls = [
                        SynModuleDecl.Expr(expr =
                            SynExpr.Do(expr = SynExpr.LetOrUse(bindings=[_;_]; trivia={ InKeyword = Some mIn })))
                    ])
                ])) ->
            assertRange (4, 15) (4, 17) mIn
        | _ -> Assert.Fail "Could not get valid AST"

    [<Test>]
    let ``nested SynExpr.LetOrUse contains the range of in keyword`` () =
        let ast =
            getParseResults """
let f () =
    let x = 1 in // the "in" keyword is available in F#
    let y = 2 in
    x + y
"""

        match ast with
        | ParsedInput.ImplFile(ParsedImplFileInput(modules = [
                    SynModuleOrNamespace.SynModuleOrNamespace(decls = [
                        SynModuleDecl.Let(bindings = [
                          SynBinding(expr =
                              SynExpr.LetOrUse(bindings=[_]; trivia={ InKeyword = Some mIn }; body=SynExpr.LetOrUse(trivia={ InKeyword = Some mInnerIn })))
                        ])
                    ])
                ])) ->
            assertRange (3, 14) (3, 16) mIn
            assertRange (4, 14) (4, 16) mInnerIn
        | _ -> Assert.Fail "Could not get valid AST"    

    [<Test>]
    let ``SynExpr.LetOrUse does not contain the range of in keyword`` () =
        let ast =
            getParseResults """
do
    let x = 1     
    ()
"""

        match ast with
        | ParsedInput.ImplFile(ParsedImplFileInput(modules = [
                    SynModuleOrNamespace.SynModuleOrNamespace(decls = [
                        SynModuleDecl.Expr(expr =
                            SynExpr.Do(expr = SynExpr.LetOrUse(trivia={ InKeyword = None })))
                    ])
                ])) ->
            Assert.Pass()
        | _ -> Assert.Fail "Could not get valid AST"

    [<Test>]
    let ``SynExpr.LetOrUse where body expr starts with token of two characters does not contain the range of in keyword`` () =
        let ast =
            getParseResults """
do
    let e1 = e :?> Collections.DictionaryEntry
    e1.Key, e1.Value
"""

        match ast with
        | ParsedInput.ImplFile(ParsedImplFileInput(modules = [
                    SynModuleOrNamespace.SynModuleOrNamespace(decls = [
                        SynModuleDecl.Expr(expr =
                            SynExpr.Do(expr = SynExpr.LetOrUse(trivia={ InKeyword = None })))
                    ])
                ])) ->
            Assert.Pass()
        | _ -> Assert.Fail "Could not get valid AST"

module Strings =
    let getBindingExpressionValue (parseResults: ParsedInput) =
        match parseResults with
        | ParsedInput.ImplFile (ParsedImplFileInput (modules = modules)) ->
            modules |> List.tryPick (fun (SynModuleOrNamespace (decls = decls)) ->
                decls |> List.tryPick (fun decl ->
                    match decl with
                    | SynModuleDecl.Let (bindings = bindings) ->
                        bindings |> List.tryPick (fun binding ->
                            match binding with
                            | SynBinding.SynBinding (headPat=(SynPat.Named _|SynPat.As(_,SynPat.Named _,_)); expr=e) -> Some e
                            | _ -> None)
                    | _ -> None))
        | _ -> None

    let getBindingConstValue parseResults =
        match getBindingExpressionValue parseResults with
        | Some (SynExpr.Const(c,_)) -> Some c
        | _ -> None
    
    [<Test>]
    let ``SynConst.String with SynStringKind.Regular`` () =
        let parseResults =
            getParseResults
                """
 let s = "yo"
 """

        match getBindingConstValue parseResults with
        | Some (SynConst.String (_,  kind, _)) -> kind |> should equal SynStringKind.Regular
        | _ -> Assert.Fail "Couldn't find const"

    [<Test>]
    let ``SynConst.String with SynStringKind.Verbatim`` () =
        let parseResults =
            getParseResults
                """
 let s = @"yo"
 """

        match getBindingConstValue parseResults with
        | Some (SynConst.String (_,  kind, _)) -> kind |> should equal SynStringKind.Verbatim
        | _ -> Assert.Fail "Couldn't find const"

    [<Test>]
    let ``SynConst.String with SynStringKind.TripleQuote`` () =
        let parseResults =
            getParseResults
                "
 let s = \"\"\"yo\"\"\"
 "

        match getBindingConstValue parseResults with
        | Some (SynConst.String (_,  kind, _)) -> kind |> should equal SynStringKind.TripleQuote
        | _ -> Assert.Fail "Couldn't find const"

    [<Test>]
    let ``SynConst.Bytes with SynByteStringKind.Regular`` () =
        let parseResults =
            getParseResults
                """
let bytes = "yo"B
 """

        match getBindingConstValue parseResults with
        | Some (SynConst.Bytes (_,  kind, _)) -> kind |> should equal SynByteStringKind.Regular
        | _ -> Assert.Fail "Couldn't find const"

    [<Test>]
    let ``SynConst.Bytes with SynByteStringKind.Verbatim`` () =
        let parseResults =
            getParseResults
                """
let bytes = @"yo"B
 """

        match getBindingConstValue parseResults with
        | Some (SynConst.Bytes (_,  kind, _)) -> kind |> should equal SynByteStringKind.Verbatim
        | _ -> Assert.Fail "Couldn't find const"

    [<Test>]
    let ``SynExpr.InterpolatedString with SynStringKind.TripleQuote`` () =
        let parseResults =
            getParseResults
                "
 let s = $\"\"\"yo {42}\"\"\"
 "

        match getBindingExpressionValue parseResults with
        | Some (SynExpr.InterpolatedString(_,  kind, _)) -> kind |> should equal SynStringKind.TripleQuote
        | _ -> Assert.Fail "Couldn't find const"

    [<Test>]
    let ``SynExpr.InterpolatedString with SynStringKind.Regular`` () =
        let parseResults =
            getParseResults
                """
 let s = $"yo {42}"
 """

        match getBindingExpressionValue parseResults with
        | Some (SynExpr.InterpolatedString(_,  kind, _)) -> kind |> should equal SynStringKind.Regular
        | _ -> Assert.Fail "Couldn't find const"

    [<Test>]
    let ``SynExpr.InterpolatedString with SynStringKind.Verbatim`` () =
        let parseResults =
            getParseResults
                """
 let s = $@"Migrate notes of file ""{oldId}"" to new file ""{newId}""."
 """

        match getBindingExpressionValue parseResults with
        | Some (SynExpr.InterpolatedString(_,  kind, _)) -> kind |> should equal SynStringKind.Verbatim
        | _ -> Assert.Fail "Couldn't find const"

module SynModuleOrNamespace =
    [<Test>]
    let ``DeclaredNamespace range should start at namespace keyword`` () =
        let parseResults = 
            getParseResults
                """namespace TypeEquality

/// A type for witnessing type equality between 'a and 'b
type Teq<'a, 'b>
"""

        match parseResults with
        | ParsedInput.ImplFile (ParsedImplFileInput (modules = [ SynModuleOrNamespace.SynModuleOrNamespace(kind = SynModuleOrNamespaceKind.DeclaredNamespace; range = r) ])) ->
            assertRange (1, 0) (4, 8) r
        | _ -> Assert.Fail "Could not get valid AST"
        
    [<Test>]
    let ``Multiple DeclaredNamespaces should have a range that starts at the namespace keyword`` () =
        let parseResults = 
            getParseResults
                """namespace TypeEquality

/// A type for witnessing type equality between 'a and 'b
type Teq = class end

namespace Foobar

let x = 42
"""

        match parseResults with
        | ParsedInput.ImplFile (ParsedImplFileInput (modules = [
            SynModuleOrNamespace.SynModuleOrNamespace(kind = SynModuleOrNamespaceKind.DeclaredNamespace; range = r1)
            SynModuleOrNamespace.SynModuleOrNamespace(kind = SynModuleOrNamespaceKind.DeclaredNamespace; range = r2) ])) ->
            assertRange (1, 0) (4, 20) r1
            assertRange (6, 0) (8, 10) r2
        | _ -> Assert.Fail "Could not get valid AST"

    [<Test>]
    let ``GlobalNamespace should start at namespace keyword`` () =
        let parseResults = 
            getParseResults
                """// foo
// bar
namespace  global

type X = int
"""

        match parseResults with
        | ParsedInput.ImplFile (ParsedImplFileInput (modules = [
            SynModuleOrNamespace.SynModuleOrNamespace(kind = SynModuleOrNamespaceKind.GlobalNamespace; range = r) ])) ->
            assertRange (3, 0) (5, 12) r
        | _ -> Assert.Fail "Could not get valid AST"

    [<Test>]
    let ``Module range should start at first attribute`` () =
        let parseResults = 
            getParseResults
                """
[<  Foo  >]
module Bar

let s : string = "s"
"""

        match parseResults with
        | ParsedInput.ImplFile (ParsedImplFileInput (modules = [
            SynModuleOrNamespace.SynModuleOrNamespace(kind = SynModuleOrNamespaceKind.NamedModule; range = r) ])) ->
            assertRange (2, 0) (5, 20) r
        | _ -> Assert.Fail "Could not get valid AST"

module SynConsts =
    [<Test>]
    let ``Measure contains the range of the constant`` () =
        let parseResults = 
            getParseResults
                """
let n = 1.0m<cm>
let m = 7.000<cm>
"""

        match parseResults with
        | ParsedInput.ImplFile (ParsedImplFileInput (modules = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
            SynModuleDecl.Let(bindings = [ SynBinding.SynBinding(expr = SynExpr.Const(SynConst.Measure(constantRange = r1), _)) ])
            SynModuleDecl.Let(bindings = [ SynBinding.SynBinding(expr = SynExpr.Const(SynConst.Measure(constantRange = r2), _)) ])
        ]) ])) ->
            assertRange (2, 8) (2, 12) r1
            assertRange (3, 8) (3, 13) r2
        | _ -> Assert.Fail "Could not get valid AST"

module SynModuleOrNamespaceSig =
    [<Test>]
    let ``Range member returns range of SynModuleOrNamespaceSig`` () =
        let parseResults =
            getParseResultsOfSignatureFile
                """
namespace Foobar

type Bar = | Bar of string * int
"""

        match parseResults with
        | ParsedInput.SigFile(ParsedSigFileInput(modules = [
            SynModuleOrNamespaceSig(kind = SynModuleOrNamespaceKind.DeclaredNamespace) as singleModule
        ])) ->
            assertRange (2,0) (4,32) singleModule.Range
        | _ -> Assert.Fail "Could not get valid AST"

    [<Test>]
    let ``GlobalNamespace should start at namespace keyword`` () =
        let parseResults = 
            getParseResultsOfSignatureFile
                """// foo
// bar
namespace  global

type Bar = | Bar of string * int
"""

        match parseResults with
        | ParsedInput.SigFile (ParsedSigFileInput (modules = [
            SynModuleOrNamespaceSig(kind = SynModuleOrNamespaceKind.GlobalNamespace; range = r) ])) ->
            assertRange (3, 0) (5, 32) r
        | _ -> Assert.Fail "Could not get valid AST"

    [<Test>]
    let ``Module range should start at first attribute`` () =
        let parseResults = 
            getParseResultsOfSignatureFile
                """
 [<  Foo  >]
module Bar

val s : string
"""

        match parseResults with
        | ParsedInput.SigFile (ParsedSigFileInput (modules = [
            SynModuleOrNamespaceSig.SynModuleOrNamespaceSig(kind = SynModuleOrNamespaceKind.NamedModule; range = r) ])) ->
            assertRange (2, 1) (5, 14) r
        | _ -> Assert.Fail "Could not get valid AST"

module SignatureTypes =
    [<Test>]
    let ``Range of Type should end at end keyword`` () =
        let parseResults = 
            getParseResultsOfSignatureFile
                """namespace GreatProjectThing

type Meh =
        class
        end


// foo"""

        match parseResults with
        | ParsedInput.SigFile (ParsedSigFileInput (modules = [
            SynModuleOrNamespaceSig(decls = [SynModuleSigDecl.Types(range = r)]) ])) ->
            assertRange (3, 0) (5,11) r
        | _ -> Assert.Fail "Could not get valid AST"

    [<Test>]
    let ``Range of SynTypeDefnSig record should end at last member`` () =
        let parseResults = 
            getParseResultsOfSignatureFile
                """namespace X
type MyRecord =
    { Level: int }
    member Score : unit -> int"""

        match parseResults with
        | ParsedInput.SigFile (ParsedSigFileInput (modules = [
            SynModuleOrNamespaceSig(decls = [SynModuleSigDecl.Types([SynTypeDefnSig.SynTypeDefnSig(range=mSynTypeDefnSig)], mTypes)]) ])) ->
            assertRange (2, 0) (4, 30) mTypes
            assertRange (2, 5) (4, 30) mSynTypeDefnSig
        | _ -> Assert.Fail "Could not get valid AST"

    [<Test>]
    let ``Range of SynTypeDefnSig object model should end at last member`` () =
        let parseResults = 
            getParseResultsOfSignatureFile
                """namespace X
type MyRecord =
    class
    end
    member Score : unit -> int"""

        match parseResults with
        | ParsedInput.SigFile (ParsedSigFileInput (modules = [
            SynModuleOrNamespaceSig(decls = [SynModuleSigDecl.Types([SynTypeDefnSig.SynTypeDefnSig(range=mSynTypeDefnSig)], mTypes)]) ])) ->
            assertRange (2, 0) (5, 30) mTypes
            assertRange (2, 5) (5, 30) mSynTypeDefnSig
        | _ -> Assert.Fail "Could not get valid AST"

    [<Test>]
    let ``Range of SynTypeDefnSig delegate of should start from name`` () =
        let parseResults = 
            getParseResultsOfSignatureFile
                """namespace Y
type MyFunction =
    delegate of int -> string"""

        match parseResults with
        | ParsedInput.SigFile (ParsedSigFileInput (modules = [
            SynModuleOrNamespaceSig(decls = [SynModuleSigDecl.Types([SynTypeDefnSig.SynTypeDefnSig(range=mSynTypeDefnSig)], mTypes) ]) ])) ->
            assertRange (2, 0) (3, 29) mTypes
            assertRange (2, 5) (3, 29) mSynTypeDefnSig
        | _ -> Assert.Fail "Could not get valid AST"

    [<Test>]
    let ``Range of SynTypeDefnSig simple should end at last val`` () =
        let parseResults = 
            getParseResultsOfSignatureFile
                """namespace Z
type SomeCollection with
    val LastIndex : int
    val SomeThingElse : int -> string"""

        match parseResults with
        | ParsedInput.SigFile (ParsedSigFileInput (modules = [
            SynModuleOrNamespaceSig(decls = [SynModuleSigDecl.Types([SynTypeDefnSig.SynTypeDefnSig(range=mSynTypeDefnSig)], mTypes)]) ])) ->
            assertRange (2, 0) (4, 37) mTypes
            assertRange (2, 5) (4, 37) mSynTypeDefnSig
        | _ -> Assert.Fail "Could not get valid AST"

    [<Test>]
    let ``Range of attribute should be included in SynTypeDefnSig`` () =
        let parseResults = 
            getParseResultsOfSignatureFile
                """
namespace SomeNamespace

[<Foo1>]
type MyType =
    class
    end
"""

        match parseResults with
        | ParsedInput.SigFile (ParsedSigFileInput (modules = [
            SynModuleOrNamespaceSig(decls = [SynModuleSigDecl.Types(types = [SynTypeDefnSig.SynTypeDefnSig(range = r)]) as t]) ])) ->
            assertRange (4, 0) (7, 7) r
            assertRange (4, 0) (7, 7) t.Range
        | _ -> Assert.Fail "Could not get valid AST"

    [<Test>]
    let ``Range of attributes should be included in recursive types`` () =
        let parseResults = 
            getParseResultsOfSignatureFile
                """
namespace SomeNamespace

type Foo =
    | Bar

and [<CustomEquality>] Bang =
    internal
        {
            LongNameBarBarBarBarBarBarBar: int
        }
        override GetHashCode : unit -> int
"""

        match parseResults with
        | ParsedInput.SigFile (ParsedSigFileInput (modules = [
            SynModuleOrNamespaceSig(decls = [SynModuleSigDecl.Types([
                SynTypeDefnSig.SynTypeDefnSig(range = r1)
                SynTypeDefnSig.SynTypeDefnSig(range = r2)
            ], mTypes)]) ])) ->
            assertRange (4, 5) (5, 9) r1
            assertRange (7, 4) (12, 42) r2
            assertRange (4, 0) (12, 42) mTypes
        | _ -> Assert.Fail "Could not get valid AST"

    [<Test>]
    let ``Range of attribute should be included in SynValSpfn and Member`` () =
        let parseResults = 
            getParseResultsOfSignatureFile
                """
namespace SomeNamespace

type FooType =
    [<Foo2>] // ValSpfn
    abstract x : int
"""

        match parseResults with
        | ParsedInput.SigFile (ParsedSigFileInput (modules = [
            SynModuleOrNamespaceSig(decls =
                [ SynModuleSigDecl.Types(types = [
                    SynTypeDefnSig.SynTypeDefnSig(typeRepr =
                        SynTypeDefnSigRepr.ObjectModel(memberSigs = [
                            SynMemberSig.Member(range = mr; memberSig = SynValSig(range = mv)) ]))
                ]) ]) ])) ->
            assertRange (5, 4) (6, 20) mr
            assertRange (5, 4) (6, 20) mv
        | _ -> Assert.Fail "Could not get valid AST"

    [<Test>]
    let ``SynTypeDefnSig with ObjectModel Delegate contains the range of the equals sign`` () =
        let parseResults = 
            getParseResultsOfSignatureFile
                """
namespace Foo

type X = delegate of string -> string
"""

        match parseResults with
        | ParsedInput.SigFile (ParsedSigFileInput (modules = [ SynModuleOrNamespaceSig(decls = [
            SynModuleSigDecl.Types(
                types = [ SynTypeDefnSig(equalsRange = Some mEquals
                                         typeRepr = SynTypeDefnSigRepr.ObjectModel(kind = SynTypeDefnKind.Delegate _)) ]
            )
        ]) ])) ->
            assertRange (4, 7) (4, 8) mEquals
        | _ -> Assert.Fail "Could not get valid AST"

    [<Test>]
    let ``SynTypeDefnSig with ObjectModel class contains the range of the equals sign`` () =
        let parseResults = 
            getParseResultsOfSignatureFile
                """
namespace SomeNamespace

type Foobar =
    class
    end
"""

        match parseResults with
        | ParsedInput.SigFile (ParsedSigFileInput (modules = [ SynModuleOrNamespaceSig(decls = [
            SynModuleSigDecl.Types(
                types = [ SynTypeDefnSig(equalsRange = Some mEquals
                                         typeRepr = SynTypeDefnSigRepr.ObjectModel(kind = SynTypeDefnKind.Class)) ]
            )
        ]) ])) ->
            assertRange (4, 12) (4, 13) mEquals
        | _ -> Assert.Fail "Could not get valid AST"

    [<Test>]
    let ``SynTypeDefnSig with Enum contains the range of the equals sign`` () =
        let parseResults = 
            getParseResultsOfSignatureFile
                """
namespace SomeNamespace

type Bear =
    | BlackBear = 1
    | PolarBear = 2
"""

        match parseResults with
        | ParsedInput.SigFile (ParsedSigFileInput (modules = [ SynModuleOrNamespaceSig(decls = [
            SynModuleSigDecl.Types(
                types = [ SynTypeDefnSig(equalsRange = Some mEquals
                                         typeRepr = SynTypeDefnSigRepr.Simple(repr =
                                             SynTypeDefnSimpleRepr.Enum(cases = [
                                                SynEnumCase(trivia={ EqualsRange = mEqualsEnumCase1 })
                                                SynEnumCase(trivia={ EqualsRange = mEqualsEnumCase2 })
                                         ]) )) ]
            )
        ]) ])) ->
            assertRange (4, 10) (4, 11) mEquals
            assertRange (5, 16) (5, 17) mEqualsEnumCase1
            assertRange (6, 16) (6, 17) mEqualsEnumCase2
        | _ -> Assert.Fail "Could not get valid AST"

    [<Test>]
    let ``SynTypeDefnSig with Union contains the range of the equals sign`` () =
        let parseResults = 
            getParseResultsOfSignatureFile
                """
namespace SomeNamespace

type Shape =
    | Square of int 
    | Rectangle of int * int
"""

        match parseResults with
        | ParsedInput.SigFile (ParsedSigFileInput (modules = [ SynModuleOrNamespaceSig(decls = [
            SynModuleSigDecl.Types(
                types = [ SynTypeDefnSig(equalsRange = Some mEquals
                                         typeRepr = SynTypeDefnSigRepr.Simple(repr = SynTypeDefnSimpleRepr.Union _)) ]
            )
        ]) ])) ->
            assertRange (4, 11) (4, 12) mEquals
        | _ -> Assert.Fail "Could not get valid AST"

    [<Test>]
    let ``SynTypeDefnSig should contains the range of the with keyword`` () =
        let parseResults = 
            getParseResultsOfSignatureFile
                """
namespace X

type Foo with
    member Meh : unit -> unit
"""

        match parseResults with
        | ParsedInput.SigFile (ParsedSigFileInput (modules =[ SynModuleOrNamespaceSig(decls =[
            SynModuleSigDecl.Types(
                types=[ SynTypeDefnSig(typeRepr=SynTypeDefnSigRepr.Simple _
                                       withKeyword=Some mWithKeyword) ]
            )
        ]) ])) ->
            assertRange (4, 9) (4, 13) mWithKeyword
        | _ -> Assert.Fail "Could not get valid AST"

    [<Test>]
    let ``SynExceptionSig should contains the range of the with keyword`` () =
        let parseResults = 
            getParseResultsOfSignatureFile
                """
namespace X

exception Foo with
    member Meh : unit -> unit
"""

        match parseResults with
        | ParsedInput.SigFile (ParsedSigFileInput (modules = [ SynModuleOrNamespaceSig(decls = [
            SynModuleSigDecl.Exception(
                exnSig=SynExceptionSig(withKeyword = Some mWithKeyword)
            )
        ]) ])) ->
            assertRange (4, 14) (4, 18) mWithKeyword
        | _ -> Assert.Fail "Could not get valid AST"

    [<Test>]
    let ``memberSig of SynMemberSig.Member should contains the range of the with keyword`` () =
        let parseResults = 
            getParseResultsOfSignatureFile
                """
namespace X

type Foo =
    abstract member Bar : int with get,set
"""

        match parseResults with
        | ParsedInput.SigFile (ParsedSigFileInput (modules = [ SynModuleOrNamespaceSig(decls = [
            SynModuleSigDecl.Types(
                types=[ SynTypeDefnSig(typeRepr=SynTypeDefnSigRepr.ObjectModel(memberSigs=[SynMemberSig.Member(memberSig=SynValSig(withKeyword=Some mWithKeyword))])) ]
            )
        ]) ])) ->
            assertRange (5, 30) (5, 34) mWithKeyword
        | _ -> Assert.Fail "Could not get valid AST"

    [<Test>]
    let ``Range of attribute should be included in SynExceptionDefnRepr and SynExceptionSig`` () =
        let parseResults = 
            getParseResultsOfSignatureFile
                """
module internal FSharp.Compiler.ParseHelpers

// The error raised by the parse_error_rich function, which is called by the parser engine
[<NoEquality; NoComparison>]
exception SyntaxError of obj * range: range


"""

        match parseResults with
        | ParsedInput.SigFile (ParsedSigFileInput (modules=[
            SynModuleOrNamespaceSig(decls=[
                SynModuleSigDecl.Exception(
                    SynExceptionSig(exnRepr=SynExceptionDefnRepr(range=mSynExceptionDefnRepr); range=mSynExceptionSig), mException)
            ] ) ])) ->
            assertRange (5, 0) (6, 43) mSynExceptionDefnRepr
            assertRange (5, 0) (6, 43) mSynExceptionSig
            assertRange (5, 0) (6, 43) mException
        | _ -> Assert.Fail "Could not get valid AST"

    [<Test>]
    let ``Range of members should be included in SynExceptionSig and SynModuleSigDecl.Exception`` () =
        let parseResults = 
            getParseResultsOfSignatureFile
                """
module internal FSharp.Compiler.ParseHelpers

exception SyntaxError of obj * range: range with
    member Meh : string -> int

open Foo
"""

        match parseResults with
        | ParsedInput.SigFile (ParsedSigFileInput (modules=[
            SynModuleOrNamespaceSig(decls=[
                SynModuleSigDecl.Exception(
                    SynExceptionSig(exnRepr=SynExceptionDefnRepr(range=mSynExceptionDefnRepr); range=mSynExceptionSig), mException)
                SynModuleSigDecl.Open _
            ] ) ])) ->
            assertRange (4, 0) (4, 43) mSynExceptionDefnRepr
            assertRange (4, 0) (5, 30) mSynExceptionSig
            assertRange (4, 0) (5, 30) mException
        | _ -> Assert.Fail "Could not get valid AST"

module SynMatchClause =
    [<Test>]
    let ``Range of single SynMatchClause`` () =
        let parseResults = 
            getParseResults
                """
try
    let content = tryDownloadFile url
    Some content
with ex ->
    Infrastructure.ReportWarning ex
    None"""

        match parseResults with
        | ParsedInput.ImplFile (ParsedImplFileInput (modules = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
            SynModuleDecl.Expr(expr = SynExpr.TryWith(withCases = [ SynMatchClause(range = range) as clause ]))
        ]) ])) ->
            assertRange (5, 5) (7, 8) range
            assertRange (5, 5) (7, 8) clause.Range
        | _ -> Assert.Fail "Could not get valid AST"

    [<Test>]
    let ``Range of multiple SynMatchClause`` () =
        let parseResults = 
            getParseResults
                """
try
    let content = tryDownloadFile url
    Some content
with
| ex ->
    Infrastructure.ReportWarning ex
    None
| exx ->
    None"""

        match parseResults with
        | ParsedInput.ImplFile (ParsedImplFileInput (modules = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
            SynModuleDecl.Expr(expr = SynExpr.TryWith(withCases = [ SynMatchClause(range = r1) as clause1
                                                                    SynMatchClause(range = r2) as clause2 ]))
        ]) ])) ->
            assertRange (6, 2) (8, 8) r1
            assertRange (6, 2) (8, 8) clause1.Range
            
            assertRange (9, 2) (10, 8) r2
            assertRange (9, 2) (10, 8) clause2.Range
        | _ -> Assert.Fail "Could not get valid AST"

    [<Test>]
    let ``Range of single SynMatchClause followed by bar`` () =
        let parseResults = 
            getParseResults
                """
try
    let content = tryDownloadFile url
    Some content
with
| ex ->
    ()
| """

        match parseResults with
        | ParsedInput.ImplFile (ParsedImplFileInput (modules = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
            SynModuleDecl.Expr(expr = SynExpr.TryWith(withCases = [ SynMatchClause(range = range) as clause ]))
        ]) ])) ->
            assertRange (6, 2) (7, 6) range
            assertRange (6, 2) (7, 6) clause.Range
        | _ -> Assert.Fail "Could not get valid AST"
    
    [<Test>]
    let ``Range of single SynMatchClause with missing body`` () =
        let parseResults = 
            getParseResults
                """
try
    let content = tryDownloadFile url
    Some content
with
| ex ->"""

        match parseResults with
        | ParsedInput.ImplFile (ParsedImplFileInput (modules = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
            SynModuleDecl.Expr(expr = SynExpr.TryWith(withCases = [ SynMatchClause(range = range) as clause ]))
        ]) ])) ->
            assertRange (6, 2) (6, 4) range
            assertRange (6, 2) (6, 4) clause.Range
        | _ -> Assert.Fail "Could not get valid AST"

    [<Test>]
    let ``Range of single SynMatchClause with missing body and when expr`` () =
        let parseResults = 
            getParseResults
                """
try
    let content = tryDownloadFile url
    Some content
with
| ex when (isNull ex) ->"""

        match parseResults with
        | ParsedInput.ImplFile (ParsedImplFileInput (modules = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
            SynModuleDecl.Expr(expr = SynExpr.TryWith(withCases = [ SynMatchClause(range = range) as clause ]))
        ]) ])) ->
            assertRange (6, 2) (6, 21) range
            assertRange (6, 2) (6, 21) clause.Range
        | _ -> Assert.Fail "Could not get valid AST"

    [<Test>]
    let ``Range of arrow in SynMatchClause`` () =
        let parseResults = 
            getParseResults
                """
match foo with
| Bar bar -> ()"""

        match parseResults with
        | ParsedInput.ImplFile (ParsedImplFileInput (modules = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
            SynModuleDecl.Expr(expr = SynExpr.Match(clauses = [ SynMatchClause(trivia={ ArrowRange = Some mArrow }) ]))
        ]) ])) ->
            assertRange (3, 10) (3, 12) mArrow
        | _ -> Assert.Fail "Could not get valid AST"

    [<Test>]
    let ``Range of arrow in SynMatchClause with when clause`` () =
        let parseResults = 
            getParseResults
                """
match foo with
| Bar bar when (someCheck bar) -> ()"""

        match parseResults with
        | ParsedInput.ImplFile (ParsedImplFileInput (modules = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
            SynModuleDecl.Expr(expr = SynExpr.Match(clauses = [ SynMatchClause(trivia={ ArrowRange = Some mArrow }) ]))
        ]) ])) ->
            assertRange (3, 31) (3, 33) mArrow
        | _ -> Assert.Fail "Could not get valid AST"

    [<Test>]
    let ``Range of bar in a single SynMatchClause in SynExpr.Match`` () =
        let parseResults = 
            getParseResults
                """
match foo with
| Bar bar when (someCheck bar) -> ()"""

        match parseResults with
        | ParsedInput.ImplFile (ParsedImplFileInput (modules = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
            SynModuleDecl.Expr(expr = SynExpr.Match(clauses = [ SynMatchClause(trivia={ BarRange = Some mBar }) ]))
        ]) ])) ->
            assertRange (3, 0) (3, 1) mBar
        | _ -> Assert.Fail "Could not get valid AST"

    [<Test>]
    let ``Range of bar in multiple SynMatchClauses in SynExpr.Match`` () =
        let parseResults = 
            getParseResults
                """
match foo with
| Bar bar when (someCheck bar) -> ()
| Far too -> near ()"""

        match parseResults with
        | ParsedInput.ImplFile (ParsedImplFileInput (modules = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
            SynModuleDecl.Expr(expr = SynExpr.Match(clauses = [ SynMatchClause(trivia={ BarRange = Some mBar1 })
                                                                SynMatchClause(trivia={ BarRange = Some mBar2 }) ]))
        ]) ])) ->
            assertRange (3, 0) (3, 1) mBar1
            assertRange (4, 0) (4, 1) mBar2
        | _ -> Assert.Fail "Could not get valid AST"

    [<Test>]
    let ``Range of bar in a single SynMatchClause in SynExpr.TryWith`` () =
        let parseResults = 
            getParseResults
                """
try
    foo ()
with
| exn -> ()"""

        match parseResults with
        | ParsedInput.ImplFile (ParsedImplFileInput (modules = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
            SynModuleDecl.Expr(expr = SynExpr.TryWith(withCases = [ SynMatchClause(trivia={ BarRange = Some mBar }) ]))
        ]) ])) ->
            assertRange (5, 0) (5, 1) mBar
        | _ -> Assert.Fail "Could not get valid AST"

    [<Test>]
    let ``No range of bar in a single SynMatchClause in SynExpr.TryWith`` () =
        let parseResults = 
            getParseResults
                """
try
    foo ()
with exn ->
    // some comment
    ()"""

        match parseResults with
        | ParsedInput.ImplFile (ParsedImplFileInput (modules = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
            SynModuleDecl.Expr(expr = SynExpr.TryWith(withCases = [ SynMatchClause(trivia={ BarRange = None }) ]))
        ]) ])) ->
            Assert.Pass()
        | _ -> Assert.Fail "Could not get valid AST"

    [<Test>]
    let ``Range of bar in a multiple SynMatchClauses in SynExpr.TryWith`` () =
        let parseResults = 
            getParseResults
                """
try
    foo ()
with
| IOException as ioex ->
    // some comment
    ()
| ex -> ()"""

        match parseResults with
        | ParsedInput.ImplFile (ParsedImplFileInput (modules = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
            SynModuleDecl.Expr(expr = SynExpr.TryWith(withCases = [ SynMatchClause(trivia={ BarRange = Some mBar1 })
                                                                    SynMatchClause(trivia={ BarRange = Some mBar2 }) ]))
        ]) ])) ->
            assertRange (5, 0) (5, 1) mBar1
            assertRange (8, 0) (8, 1) mBar2
        | _ -> Assert.Fail "Could not get valid AST"

module SourceIdentifiers =
    [<Test>]
    let ``__LINE__`` () =
        let parseResults = 
            getParseResults
                """
__LINE__"""

        match parseResults with
        | ParsedInput.ImplFile (ParsedImplFileInput (modules = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
            SynModuleDecl.Expr(expr = SynExpr.Const(SynConst.SourceIdentifier("__LINE__", "2", range), _))
        ]) ])) ->
            assertRange (2, 0) (2, 8) range
        | _ -> Assert.Fail "Could not get valid AST"

    [<Test>]
    let ``__SOURCE_DIRECTORY__`` () =
        let parseResults = 
            getParseResults
                """
__SOURCE_DIRECTORY__"""

        match parseResults with
        | ParsedInput.ImplFile (ParsedImplFileInput (modules = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
            SynModuleDecl.Expr(expr = SynExpr.Const(SynConst.SourceIdentifier("__SOURCE_DIRECTORY__", _, range), _))
        ]) ])) ->
            assertRange (2, 0) (2, 20) range
        | _ -> Assert.Fail "Could not get valid AST"
        
    [<Test>]
    let ``__SOURCE_FILE__`` () =
        let parseResults = 
            getParseResults
                """
__SOURCE_FILE__"""

        match parseResults with
        | ParsedInput.ImplFile (ParsedImplFileInput (modules = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
            SynModuleDecl.Expr(expr = SynExpr.Const(SynConst.SourceIdentifier("__SOURCE_FILE__", _, range), _))
        ]) ])) ->
            assertRange (2, 0) (2, 15) range
        | _ -> Assert.Fail "Could not get valid AST"

module NestedModules =

    [<Test>]
    let ``Range of attribute should be included in SynModuleSigDecl.NestedModule`` () =
        let parseResults = 
            getParseResultsOfSignatureFile
                """
namespace SomeNamespace

[<Foo>]
module Nested =
    val x : int
"""

        match parseResults with
        | ParsedInput.SigFile (ParsedSigFileInput (modules = [ SynModuleOrNamespaceSig(decls = [
            SynModuleSigDecl.NestedModule _ as nm
        ]) as sigModule ])) ->
            assertRange (4, 0) (6, 15) nm.Range
            assertRange (2, 0) (6, 15) sigModule.Range
        | _ -> Assert.Fail "Could not get valid AST"

    [<Test>]
    let ``Range of attribute should be included in SynModuleDecl.NestedModule`` () =
        let parseResults = 
            getParseResults
                """
module TopLevel

[<Foo>]
module Nested =
    ()"""

        match parseResults with
        | ParsedInput.ImplFile (ParsedImplFileInput (modules = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
            SynModuleDecl.NestedModule _ as nm
        ]) ])) ->
            assertRange (4, 0) (6, 6) nm.Range
        | _ -> Assert.Fail "Could not get valid AST"
        
    [<Test>]
    let ``Range of equal sign should be present`` () =
        let parseResults = 
            getParseResults
                """
module X =
    ()
"""

        match parseResults with
        | ParsedInput.ImplFile (ParsedImplFileInput (modules = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
            SynModuleDecl.NestedModule(trivia = { ModuleKeyword = Some mModule; EqualsRange = Some mEquals })
        ]) ])) ->
            assertRange (2, 0) (2, 6) mModule
            assertRange (2, 9) (2, 10) mEquals
        | _ -> Assert.Fail "Could not get valid AST"

    [<Test>]
    let ``Range of equal sign should be present, signature file`` () =
        let parseResults = 
            getParseResultsOfSignatureFile
                """
namespace Foo

module X =
    val bar : int
"""

        match parseResults with
        | ParsedInput.SigFile (ParsedSigFileInput (modules = [ SynModuleOrNamespaceSig(decls = [
            SynModuleSigDecl.NestedModule(trivia = { ModuleKeyword = Some mModule; EqualsRange = Some mEquals })
        ]) ])) ->
            assertRange (4, 0) (4, 6) mModule
            assertRange (4, 9) (4, 10) mEquals
        | _ -> Assert.Fail "Could not get valid AST"

    [<Test>]
    let ``Range of nested module in signature file should end at the last SynModuleSigDecl`` () =
        let parseResults =
            getParseResultsOfSignatureFile
                """namespace Microsoft.FSharp.Core

open System
open System.Collections.Generic
open Microsoft.FSharp.Core
open Microsoft.FSharp.Collections
open System.Collections


module Tuple =

    type Tuple<'T1,'T2,'T3,'T4> =
        interface IStructuralEquatable
        interface IStructuralComparable
        interface IComparable
        new : 'T1 * 'T2 * 'T3 * 'T4 -> Tuple<'T1,'T2,'T3,'T4>
        member Item1 : 'T1 with get
        member Item2 : 'T2 with get
        member Item3 : 'T3 with get
        member Item4 : 'T4 with get


module Choice =

    /// <summary>Helper types for active patterns with 6 choices.</summary>
    [<StructuralEquality; StructuralComparison>]
    [<CompiledName("FSharpChoice`6")>]
    type Choice<'T1,'T2,'T3,'T4,'T5,'T6> =
      /// <summary>Choice 1 of 6 choices</summary>
      | Choice1Of6 of 'T1
      /// <summary>Choice 2 of 6 choices</summary>
      | Choice2Of6 of 'T2
      /// <summary>Choice 3 of 6 choices</summary>
      | Choice3Of6 of 'T3
      /// <summary>Choice 4 of 6 choices</summary>
      | Choice4Of6 of 'T4
      /// <summary>Choice 5 of 6 choices</summary>
      | Choice5Of6 of 'T5
      /// <summary>Choice 6 of 6 choices</summary>
      | Choice6Of6 of 'T6



/// <summary>Basic F# Operators. This module is automatically opened in all F# code.</summary>
[<AutoOpen>]
module Operators =

    type ``[,]``<'T> with
        [<CompiledName("Length1")>]
        /// <summary>Get the length of an array in the first dimension  </summary>
        member Length1 : int
        [<CompiledName("Length2")>]
        /// <summary>Get the length of the array in the second dimension  </summary>
        member Length2 : int
        [<CompiledName("Base1")>]
        /// <summary>Get the lower bound of the array in the first dimension  </summary>
        member Base1 : int
        [<CompiledName("Base2")>]
        /// <summary>Get the lower bound of the array in the second dimension  </summary>
        member Base2 : int
"""

        match parseResults with
        | ParsedInput.SigFile (ParsedSigFileInput (modules = [ SynModuleOrNamespaceSig(decls = [
              SynModuleSigDecl.Open _
              SynModuleSigDecl.Open _
              SynModuleSigDecl.Open _
              SynModuleSigDecl.Open _
              SynModuleSigDecl.Open _
              SynModuleSigDecl.NestedModule(range=mTupleModule; moduleDecls=[ SynModuleSigDecl.Types([
                  SynTypeDefnSig(typeRepr=SynTypeDefnSigRepr.ObjectModel(range=mTupleObjectModel); range=mTupleType)
              ], mTupleTypes) ])
              SynModuleSigDecl.NestedModule(range=mChoiceModule)
              SynModuleSigDecl.NestedModule(range=mOperatorsModule; moduleDecls=[ SynModuleSigDecl.Types([
                  SynTypeDefnSig(typeRepr=SynTypeDefnSigRepr.Simple(range=mAugmentationSimple); range=mAugmentation)
              ], mOperatorsTypes) ])
          ]) ])) ->
            assertRange (10, 0) (20, 35) mTupleModule
            assertRange (12, 4) (20, 35) mTupleTypes
            assertRange (12, 9) (20, 35) mTupleType
            assertRange (13, 8) (20, 35) mTupleObjectModel
            assertRange (23, 0) (40, 25) mChoiceModule
            assertRange (44, 0) (60, 26) mOperatorsModule
            assertRange (48, 4) (60, 26) mOperatorsTypes
            assertRange (48, 9) (60, 26) mAugmentation
            assertRange (48, 9) (60, 26) mAugmentationSimple
        | _ -> Assert.Fail "Could not get valid AST"

module SynBindings =
    [<Test>]
    let ``Range of attribute should be included in SynModuleDecl.Let`` () =
        let parseResults = 
            getParseResults
                """
[<Foo>]
let a = 0"""

        match parseResults with
        | ParsedInput.ImplFile (ParsedImplFileInput (modules = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
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
        | ParsedInput.ImplFile (ParsedImplFileInput (modules = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
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
        | ParsedInput.ImplFile (ParsedImplFileInput (modules = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
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
        | ParsedInput.ImplFile (ParsedImplFileInput (modules = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
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
        | ParsedInput.ImplFile (ParsedImplFileInput (modules = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
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
        | ParsedInput.ImplFile (ParsedImplFileInput (modules = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
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
        | ParsedInput.ImplFile (ParsedImplFileInput (modules = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
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
        | ParsedInput.ImplFile (ParsedImplFileInput (modules = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
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
        | ParsedInput.ImplFile (ParsedImplFileInput (modules = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
            SynModuleDecl.Types(typeDefns = [SynTypeDefn(typeRepr = SynTypeDefnRepr.ObjectModel(members = [SynMemberDefn.Member(memberDefn = SynBinding(range = mb)) as m]))])
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
        | ParsedInput.ImplFile (ParsedImplFileInput (modules = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
            SynModuleDecl.Types(typeDefns = [SynTypeDefn(typeRepr = SynTypeDefnRepr.ObjectModel(members = [
                SynMemberDefn.Member(memberDefn = SynBinding(range = mb1)) as getter
                SynMemberDefn.Member(memberDefn = SynBinding(range = mb2)) as setter
            ]))])
        ]) ])) ->
            assertRange (3, 4) (5, 19) mb1
            assertRange (3, 4) (6, 50) getter.Range
            assertRange (3, 4) (6, 23) mb2
            assertRange (3, 4) (6, 50) setter.Range
        | _ -> Assert.Fail "Could not get valid AST"

    [<Test>]
    let ``Range of equal sign should be present in SynModuleDecl.Let binding`` () =
        let parseResults = 
            getParseResults "let v = 12"

        match parseResults with
        | ParsedInput.ImplFile (ParsedImplFileInput (modules = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
            SynModuleDecl.Let(bindings = [SynBinding(trivia={ EqualsRange = Some mEquals })])
        ]) ])) ->
            assertRange (1, 6) (1, 7) mEquals
        | _ -> Assert.Fail "Could not get valid AST"

    [<Test>]
    let ``Range of equal sign should be present in SynModuleDecl.Let binding, typed`` () =
        let parseResults = 
            getParseResults "let v : int = 12"

        match parseResults with
        | ParsedInput.ImplFile (ParsedImplFileInput (modules = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
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
        | ParsedInput.ImplFile (ParsedImplFileInput (modules = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
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
        | ParsedInput.ImplFile (ParsedImplFileInput (modules = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
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
        | ParsedInput.ImplFile (ParsedImplFileInput (modules = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
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
        | ParsedInput.ImplFile (ParsedImplFileInput (modules = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
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
        | ParsedInput.ImplFile (ParsedImplFileInput (modules = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
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
        | ParsedInput.ImplFile (ParsedImplFileInput (modules = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
            SynModuleDecl.Types(typeDefns = [SynTypeDefn(typeRepr = SynTypeDefnRepr.ObjectModel(members = [
                _
                SynMemberDefn.Member(memberDefn = SynBinding(trivia={ EqualsRange = Some eqGetM }))
                SynMemberDefn.Member(memberDefn = SynBinding(trivia={ EqualsRange = Some eqSetM }))
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
        | ParsedInput.ImplFile (ParsedImplFileInput (modules = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
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
        | ParsedInput.ImplFile (ParsedImplFileInput (modules = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
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
        | ParsedInput.ImplFile (ParsedImplFileInput (modules = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
            SynModuleDecl.Let(bindings = [SynBinding(expr=SynExpr.LetOrUse(bindings=[SynBinding(trivia={ LetKeyword = Some mLet })]))])
        ]) ])) ->
            assertRange (3, 4) (3, 7) mLet
        | _ -> Assert.Fail "Could not get valid AST"

module ParsedHashDirective =
    [<Test>]
    let ``SourceIdentifier as ParsedHashDirectiveArgument`` () =
        let parseResults = 
            getParseResults
                "#I __SOURCE_DIRECTORY__"

        match parseResults with
        | ParsedInput.ImplFile (ParsedImplFileInput (modules = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
            SynModuleDecl.HashDirective(ParsedHashDirective("I", [ ParsedHashDirectiveArgument.SourceIdentifier(c,_,m) ] , _), _)
        ]) ])) ->
            Assert.AreEqual("__SOURCE_DIRECTORY__", c)
            assertRange (1, 3) (1, 23) m
        | _ -> Assert.Fail "Could not get valid AST"

    [<Test>]
    let ``Regular String as ParsedHashDirectiveArgument`` () =
        let parseResults = 
            getParseResults
                "#I \"/tmp\""

        match parseResults with
        | ParsedInput.ImplFile (ParsedImplFileInput (modules = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
            SynModuleDecl.HashDirective(ParsedHashDirective("I", [ ParsedHashDirectiveArgument.String(v, SynStringKind.Regular, m) ] , _), _)
        ]) ])) ->
            Assert.AreEqual("/tmp", v)
            assertRange (1, 3) (1, 9) m
        | _ -> Assert.Fail "Could not get valid AST"

    [<Test>]
    let ``Verbatim String as ParsedHashDirectiveArgument`` () =
        let parseResults = 
            getParseResults
                "#I @\"C:\\Temp\""

        match parseResults with
        | ParsedInput.ImplFile (ParsedImplFileInput (modules = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
            SynModuleDecl.HashDirective(ParsedHashDirective("I", [ ParsedHashDirectiveArgument.String(v, SynStringKind.Verbatim, m) ] , _), _)
        ]) ])) ->
            Assert.AreEqual("C:\\Temp", v)
            assertRange (1, 3) (1, 13) m
        | _ -> Assert.Fail "Could not get valid AST"

    [<Test>]
    let ``Triple quote String as ParsedHashDirectiveArgument`` () =
        let parseResults = 
            getParseResults
                "#nowarn \"\"\"40\"\"\""

        match parseResults with
        | ParsedInput.ImplFile (ParsedImplFileInput (modules = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
            SynModuleDecl.HashDirective(ParsedHashDirective("nowarn", [ ParsedHashDirectiveArgument.String(v, SynStringKind.TripleQuote, m) ] , _), _)
        ]) ])) ->
            Assert.AreEqual("40", v)
            assertRange (1, 8) (1, 16) m
        | _ -> Assert.Fail "Could not get valid AST"

module Lambdas =
    [<Test>]
    let ``Lambda with two parameters gives correct body`` () =
        let parseResults = 
            getParseResults
                "fun a b -> x"

        match parseResults with
        | ParsedInput.ImplFile (ParsedImplFileInput (modules = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
            SynModuleDecl.Expr(
                expr = SynExpr.Lambda(parsedData = Some([SynPat.Named _; SynPat.Named _], SynExpr.Ident(ident)))
            )
        ]) ])) ->
            Assert.AreEqual("x", ident.idText)
        | _ -> Assert.Fail "Could not get valid AST"

    [<Test>]
    let ``Lambda with wild card parameter gives correct body`` () =
        let parseResults = 
            getParseResults
                "fun a _ b -> x"

        match parseResults with
        | ParsedInput.ImplFile (ParsedImplFileInput (modules = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
            SynModuleDecl.Expr(
                expr = SynExpr.Lambda(parsedData = Some([SynPat.Named _; SynPat.Wild _; SynPat.Named _], SynExpr.Ident(ident)))
            )
        ]) ])) ->
            Assert.AreEqual("x", ident.idText)
        | _ -> Assert.Fail "Could not get valid AST"

    [<Test>]
    let ``Lambda with tuple parameter with wild card gives correct body`` () =
        let parseResults = 
            getParseResults
                "fun a (b, _) c -> x"

        match parseResults with
        | ParsedInput.ImplFile (ParsedImplFileInput (modules = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
            SynModuleDecl.Expr(
                expr = SynExpr.Lambda(parsedData = Some([SynPat.Named _; SynPat.Paren(SynPat.Tuple _,_); SynPat.Named _], SynExpr.Ident(ident)))
            )
        ]) ])) ->
            Assert.AreEqual("x", ident.idText)
        | _ -> Assert.Fail "Could not get valid AST"

    [<Test>]
    let ``Lambda with wild card that returns a lambda gives correct body`` () =
        let parseResults = 
            getParseResults
                "fun _ -> fun _ -> x"

        match parseResults with
        | ParsedInput.ImplFile (ParsedImplFileInput (modules = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
            SynModuleDecl.Expr(
                expr = SynExpr.Lambda(parsedData = Some([SynPat.Wild _], SynExpr.Lambda(parsedData = Some([SynPat.Wild _], SynExpr.Ident(ident)))))
            )
        ]) ])) ->
            Assert.AreEqual("x", ident.idText)
        | _ -> Assert.Fail "Could not get valid AST"

    [<Test>]
    let ``Simple lambda has arrow range`` () =
        let parseResults = 
            getParseResults
                "fun x -> x"

        match parseResults with
        | ParsedInput.ImplFile (ParsedImplFileInput (modules = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
            SynModuleDecl.Expr(
                expr = SynExpr.Lambda(trivia={ ArrowRange = Some mArrow })
            )
        ]) ])) ->
            assertRange (1, 6) (1, 8) mArrow
        | _ -> Assert.Fail "Could not get valid AST"

    [<Test>]
    let ``Multiline lambda has arrow range`` () =
        let parseResults = 
            getParseResults
                "fun x y z
                            ->
                                x * y * z"

        match parseResults with
        | ParsedInput.ImplFile (ParsedImplFileInput (modules = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
            SynModuleDecl.Expr(
                expr = SynExpr.Lambda(trivia={ ArrowRange = Some mArrow })
            )
        ]) ])) ->
            assertRange (2, 28) (2, 30) mArrow
        | _ -> Assert.Fail "Could not get valid AST"

    [<Test>]
    let ``Destructed lambda has arrow range`` () =
        let parseResults = 
            getParseResults
                "fun { X = x } -> x * 2"

        match parseResults with
        | ParsedInput.ImplFile (ParsedImplFileInput (modules = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
            SynModuleDecl.Expr(
                expr = SynExpr.Lambda(trivia={ ArrowRange = Some mArrow })
            )
        ]) ])) ->
            assertRange (1, 14) (1, 16) mArrow
        | _ -> Assert.Fail "Could not get valid AST"

    [<Test>]
    let ``Tuple in lambda has arrow range`` () =
        let parseResults = 
            getParseResults
                "fun (x, _) -> x * 3"

        match parseResults with
        | ParsedInput.ImplFile (ParsedImplFileInput (modules = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
            SynModuleDecl.Expr(
                expr = SynExpr.Lambda(trivia={ ArrowRange = Some mArrow })
            )
        ]) ])) ->
            assertRange (1, 11) (1, 13) mArrow
        | _ -> Assert.Fail "Could not get valid AST"

    [<Test>]
    let ``Complex arguments lambda has arrow range`` () =
        let parseResults = 
            getParseResults
                "fun (x, _) 
    ({ Y = h::_ }) 
    (SomePattern(z)) 
    -> 
    x * y + z"

        match parseResults with
        | ParsedInput.ImplFile (ParsedImplFileInput (modules = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
            SynModuleDecl.Expr(
                expr = SynExpr.Lambda(trivia={ ArrowRange = Some mArrow })
            )
        ]) ])) ->
            assertRange (4, 4) (4, 6) mArrow
        | _ -> Assert.Fail "Could not get valid AST"

module IfThenElse =
    [<Test>]
    let ``If keyword in IfThenElse`` () =
        let parseResults = 
            getParseResults
                "if a then b"

        match parseResults with
        | ParsedInput.ImplFile (ParsedImplFileInput (modules = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
            SynModuleDecl.Expr(
                expr = SynExpr.IfThenElse(trivia={ IfKeyword = mIfKw; IsElif = false; ThenKeyword = mThenKw; ElseKeyword = None })
            )
        ]) ])) ->
            assertRange (1, 0) (1, 2) mIfKw
            assertRange (1, 5) (1, 9) mThenKw
        | _ -> Assert.Fail "Could not get valid AST"

    [<Test>]
    let ``Else keyword in simple IfThenElse`` () =
        let parseResults = 
            getParseResults
                "if a then b else c"

        match parseResults with
        | ParsedInput.ImplFile (ParsedImplFileInput (modules = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
            SynModuleDecl.Expr(
                expr =SynExpr.IfThenElse(trivia={ IfKeyword = mIfKw; IsElif = false; ThenKeyword = mThenKw; ElseKeyword = Some mElse })
            )
        ]) ])) ->
            assertRange (1, 0) (1, 2) mIfKw
            assertRange (1, 5) (1, 9) mThenKw
            assertRange (1, 12) (1, 16) mElse
        | _ -> Assert.Fail "Could not get valid AST"

    [<Test>]
    let ``If, Then and Else keyword on separate lines`` () =
        let parseResults = 
            getParseResults
                """
if a
then b
else c"""

        match parseResults with
        | ParsedInput.ImplFile (ParsedImplFileInput (modules = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
            SynModuleDecl.Expr(
                expr = SynExpr.IfThenElse(trivia={ IfKeyword = mIfKw; IsElif = false; ThenKeyword = mThenKw; ElseKeyword = Some mElse })
            )
        ]) ])) ->
            assertRange (2, 0) (2, 2) mIfKw
            assertRange (3, 0) (3, 4) mThenKw
            assertRange (4, 0) (4, 4) mElse
        | _ -> Assert.Fail "Could not get valid AST"

    [<Test>]
    let ``Nested elif in IfThenElse`` () =
        let parseResults = 
            getParseResults
                """
if a then
    b
elif c then d"""

        match parseResults with
        | ParsedInput.ImplFile (ParsedImplFileInput (modules = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
            SynModuleDecl.Expr(
                expr = SynExpr.IfThenElse(trivia={ IfKeyword = mIfKw; IsElif=false; ThenKeyword = mThenKw; ElseKeyword = None }
                                          elseExpr = Some (SynExpr.IfThenElse(trivia={ IfKeyword = mElif; IsElif = true })))
            )
        ]) ])) ->
            assertRange (2, 0) (2, 2) mIfKw
            assertRange (2, 5) (2, 9) mThenKw
            assertRange (4, 0) (4, 4) mElif
        | _ -> Assert.Fail "Could not get valid AST"

    [<Test>]
    let ``Nested else if in IfThenElse`` () =
        let parseResults = 
            getParseResults
                """
if a then
    b
else
    if c then d"""

        match parseResults with
        | ParsedInput.ImplFile (ParsedImplFileInput (modules = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
            SynModuleDecl.Expr(
                expr = SynExpr.IfThenElse(trivia={ IfKeyword = mIfKw; IsElif = false; ThenKeyword = mThenKw; ElseKeyword = Some mElse }
                                          elseExpr = Some (SynExpr.IfThenElse(trivia={ IfKeyword = mElseIf; IsElif = false })))
            )
        ]) ])) ->
            assertRange (2, 0) (2, 2) mIfKw
            assertRange (2, 5) (2, 9) mThenKw
            assertRange (4, 0) (4, 4) mElse
            assertRange (5, 4) (5, 6) mElseIf
        | _ -> Assert.Fail "Could not get valid AST"

    [<Test>]
    let ``Nested else if on the same line in IfThenElse`` () =
        let parseResults = 
            getParseResults
                """
if a then
    b
else if c then
    d"""

        match parseResults with
        | ParsedInput.ImplFile (ParsedImplFileInput (modules = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
            SynModuleDecl.Expr(
                expr = SynExpr.IfThenElse(trivia={ IfKeyword = mIfKw; IsElif=false; ThenKeyword = mThenKw; ElseKeyword = Some mElse }
                                          elseExpr = Some (SynExpr.IfThenElse(trivia={ IfKeyword = mElseIf; IsElif = false })))
            )
        ]) ])) ->
            assertRange (2, 0) (2, 2) mIfKw
            assertRange (2, 5) (2, 9) mThenKw
            assertRange (4, 0) (4, 4) mElse
            assertRange (4, 5) (4, 7) mElseIf
        | _ -> Assert.Fail "Could not get valid AST"
    
    [<Test>]
    let ``Deeply nested IfThenElse`` () =
        let parseResults = 
            getParseResults
                """
if a then
    b
elif c then
    d
else
        if e then
            f
        else
            g"""

        match parseResults with
        | ParsedInput.ImplFile (ParsedImplFileInput (modules = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
            SynModuleDecl.Expr(
                expr = SynExpr.IfThenElse(trivia={ IfKeyword = mIf1; IsElif = false; ElseKeyword = None }
                                          elseExpr = Some (SynExpr.IfThenElse(trivia={ IfKeyword = mElif; IsElif = true; ElseKeyword = Some mElse1 }
                                                                              elseExpr = Some (SynExpr.IfThenElse(trivia={ IfKeyword = mIf2; IsElif = false; ElseKeyword = Some mElse2 }))))))
        ]) ])) ->
            assertRange (2, 0) (2, 2) mIf1
            assertRange (4, 0) (4, 4) mElif
            assertRange (6, 0) (6, 4) mElse1
            assertRange (7, 8) (7, 10) mIf2
            assertRange (9, 8) (9, 12) mElse2

        | _ -> Assert.Fail "Could not get valid AST"
        
    [<Test>]
    let ``Comment between else and if`` () =
        let parseResults = 
            getParseResults
                """
if a then
    b
else (* some long comment here *) if c then
    d"""

        match parseResults with
        | ParsedInput.ImplFile (ParsedImplFileInput (modules = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
            SynModuleDecl.Expr(
                expr = SynExpr.IfThenElse(trivia={ IfKeyword = mIf1; IsElif = false; ElseKeyword = Some mElse }
                                          elseExpr = Some (SynExpr.IfThenElse(trivia = { IfKeyword = mIf2; IsElif = false }))))
        ]) ])) ->
            assertRange (2, 0) (2, 2) mIf1
            assertRange (4, 0) (4, 4) mElse
            assertRange (4, 34) (4, 36) mIf2

        | _ -> Assert.Fail "Could not get valid AST"

module UnionCases =
    [<Test>]
    let ``Union Case fields can have comments`` () =
        let ast = """
type Foo =
/// docs for Thing
| Thing of
  /// docs for first
  first: string *
  /// docs for anon field
  bool
"""
                        |> getParseResults

        match ast with
        | ParsedInput.ImplFile(ParsedImplFileInput(modules = [
            SynModuleOrNamespace.SynModuleOrNamespace(decls = [
                SynModuleDecl.Types ([
                    SynTypeDefn.SynTypeDefn (typeRepr = SynTypeDefnRepr.Simple (simpleRepr = SynTypeDefnSimpleRepr.Union(unionCases = [
                        SynUnionCase.SynUnionCase (caseType = SynUnionCaseKind.Fields [
                            SynField.SynField(xmlDoc = firstXml)
                            SynField.SynField(xmlDoc = anonXml)
                        ])
                    ])))
                ], _)
            ])
          ])) ->
            let firstDocs = firstXml.ToXmlDoc(false, None).GetXmlText()
            let anonDocs = anonXml.ToXmlDoc(false, None).GetXmlText()

            let nl = Environment.NewLine

            Assert.AreEqual($"<summary>{nl} docs for first{nl}</summary>", firstDocs)
            Assert.AreEqual($"<summary>{nl} docs for anon field{nl}</summary>", anonDocs)

        | _ ->
            failwith "Could not find SynExpr.Do"

    [<Test>]
    let ``single SynUnionCase has bar range`` () =
        let ast = """
type Foo = | Bar of string
"""
                        |> getParseResults

        match ast with
        | ParsedInput.ImplFile(ParsedImplFileInput(modules = [
            SynModuleOrNamespace.SynModuleOrNamespace(decls = [
                SynModuleDecl.Types ([
                    SynTypeDefn.SynTypeDefn (typeRepr = SynTypeDefnRepr.Simple (simpleRepr = SynTypeDefnSimpleRepr.Union(unionCases = [
                        SynUnionCase.SynUnionCase (trivia = { BarRange = Some mBar })
                    ])))
                ], _)
            ])
          ])) ->
            assertRange (2, 11) (2, 12) mBar
        | _ ->
            Assert.Fail "Could not get valid AST"

    [<Test>]
    let ``multiple SynUnionCases have bar range`` () =
        let ast = """
type Foo =
    | Bar of string
    | Bear of int
"""
                        |> getParseResults

        match ast with
        | ParsedInput.ImplFile(ParsedImplFileInput(modules = [
            SynModuleOrNamespace.SynModuleOrNamespace(decls = [
                SynModuleDecl.Types ([
                    SynTypeDefn.SynTypeDefn (typeRepr = SynTypeDefnRepr.Simple (simpleRepr = SynTypeDefnSimpleRepr.Union(unionCases = [
                        SynUnionCase.SynUnionCase (trivia = { BarRange = Some mBar1 })
                        SynUnionCase.SynUnionCase (trivia = { BarRange = Some mBar2 })
                    ])))
                ], _)
            ])
          ])) ->
            assertRange (3, 4) (3, 5) mBar1
            assertRange (4, 4) (4, 5) mBar2
        | _ ->
            Assert.Fail "Could not get valid AST"

    [<Test>]
    let ``single SynUnionCase without bar`` () =
        let ast = """
type Foo = Bar of string
"""
                        |> getParseResults

        match ast with
        | ParsedInput.ImplFile(ParsedImplFileInput(modules = [
            SynModuleOrNamespace.SynModuleOrNamespace(decls = [
                SynModuleDecl.Types ([
                    SynTypeDefn.SynTypeDefn (typeRepr = SynTypeDefnRepr.Simple (simpleRepr = SynTypeDefnSimpleRepr.Union(unionCases = [
                        SynUnionCase.SynUnionCase (trivia = { BarRange = None })
                    ])))
                ], _)
            ])
          ])) ->
            Assert.Pass()
        | _ ->
            Assert.Fail "Could not get valid AST"

module EnumCases =
    [<Test>]
    let ``single SynEnumCase has bar range`` () =
        let ast = """
type Foo = | Bar = 1
"""
                        |> getParseResults

        match ast with
        | ParsedInput.ImplFile(ParsedImplFileInput(modules = [
            SynModuleOrNamespace.SynModuleOrNamespace(decls = [
                SynModuleDecl.Types ([
                    SynTypeDefn.SynTypeDefn (typeRepr = SynTypeDefnRepr.Simple (simpleRepr = SynTypeDefnSimpleRepr.Enum(cases = [
                        SynEnumCase.SynEnumCase (trivia = { BarRange = Some mBar; EqualsRange = mEquals })
                    ])))
                ], _)
            ])
          ])) ->
            assertRange (2, 11) (2, 12) mBar
            assertRange (2, 17) (2, 18) mEquals
        | _ ->
            Assert.Fail "Could not get valid AST"

    [<Test>]
    let ``multiple SynEnumCases have bar range`` () =
        let ast = """
type Foo =
    | Bar = 1
    | Bear = 2
"""
                        |> getParseResults

        match ast with
        | ParsedInput.ImplFile(ParsedImplFileInput(modules = [
            SynModuleOrNamespace.SynModuleOrNamespace(decls = [
                SynModuleDecl.Types ([
                    SynTypeDefn.SynTypeDefn (typeRepr = SynTypeDefnRepr.Simple (simpleRepr = SynTypeDefnSimpleRepr.Enum(cases = [
                        SynEnumCase.SynEnumCase (trivia = { BarRange = Some mBar1; EqualsRange = mEquals1 })
                        SynEnumCase.SynEnumCase (trivia = { BarRange = Some mBar2; EqualsRange = mEquals2 })
                    ])))
                ], _)
            ])
          ])) ->
            assertRange (3, 4) (3, 5) mBar1
            assertRange (3, 10) (3, 11) mEquals1
            assertRange (4, 4) (4, 5) mBar2
            assertRange (4, 11) (4, 12) mEquals2
        | _ ->
            Assert.Fail "Could not get valid AST"

    [<Test>]
    let ``single SynEnumCase without bar`` () =
        let ast = """
type Foo = Bar = 1
"""
                        |> getParseResults

        match ast with
        | ParsedInput.ImplFile(ParsedImplFileInput(modules = [
            SynModuleOrNamespace.SynModuleOrNamespace(decls = [
                SynModuleDecl.Types ([
                    SynTypeDefn.SynTypeDefn (typeRepr = SynTypeDefnRepr.Simple (simpleRepr = SynTypeDefnSimpleRepr.Enum(cases = [
                        SynEnumCase.SynEnumCase (trivia = { BarRange = None; EqualsRange = mEquals })
                    ])))
                ], _)
            ])
          ])) ->
            assertRange (2, 15) (2, 16) mEquals
        | _ ->
            Assert.Fail "Could not get valid AST"

module Patterns =
    [<Test>]
    let ``SynPat.Record contains the range of the equals sign`` () =
        let parseResults = 
            getParseResults
                """
match x with
| { Foo = bar } -> ()
| _ -> ()
"""

        match parseResults with
        | ParsedInput.ImplFile (ParsedImplFileInput (modules = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
            SynModuleDecl.Expr(
                expr = SynExpr.Match(clauses = [ SynMatchClause(pat = SynPat.Record(fieldPats = [ (_, mEquals, _) ])) ; _ ])
            )
        ]) ])) ->
            assertRange (3, 8) (3, 9) mEquals
        | _ -> Assert.Fail "Could not get valid AST"

    [<Test>]
    let ``SynArgPats.NamePatPairs contains the range of the equals sign`` () =
        let parseResults = 
            getParseResults
                """
match x with
| X(Y  = y) -> y
"""

        match parseResults with
        | ParsedInput.ImplFile (ParsedImplFileInput (modules = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
            SynModuleDecl.Expr(
                expr = SynExpr.Match(clauses = [ SynMatchClause(pat = SynPat.LongIdent(argPats = SynArgPats.NamePatPairs(pats = [ _, mEquals ,_ ])))])
            )
        ]) ])) ->
            assertRange (3, 7) (3, 8) mEquals
        | _ -> Assert.Fail "Could not get valid AST"

    [<Test>]
    let ``SynPat.Or contains the range of the bar`` () =
        let parseResults = 
            getParseResults
                """
match x with
| A
| B -> ()
| _ -> ()
"""

        match parseResults with
        | ParsedInput.ImplFile (ParsedImplFileInput (modules = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
            SynModuleDecl.Expr(
                expr = SynExpr.Match(clauses = [ SynMatchClause(pat = SynPat.Or(trivia={ BarRange = mBar })) ; _ ])
            )
        ]) ])) ->
            assertRange (4, 0) (4, 1) mBar
        | _ -> Assert.Fail "Could not get valid AST"

module Exceptions =
    [<Test>]
    let ``SynExceptionDefn should contains the range of the with keyword`` () =
        let parseResults = 
            getParseResults
                """
namespace X

exception Foo with
    member Meh () = ()
"""

        match parseResults with
        | ParsedInput.ImplFile (ParsedImplFileInput (modules = [ SynModuleOrNamespace(decls = [
            SynModuleDecl.Exception(
                exnDefn=SynExceptionDefn(withKeyword = Some mWithKeyword)
            )
        ]) ])) ->
            assertRange (4, 14) (4, 18) mWithKeyword
        | _ -> Assert.Fail "Could not get valid AST"

module SynMemberFlags =
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

module ComputationExpressions =
    [<Test>]
    let ``SynExprAndBang range starts at and! and ends after expression`` () =
        let ast =
            getParseResults """
async {
    let! bar = getBar ()

    and! foo = getFoo ()

    return bar
}
"""

        match ast with
        | ParsedInput.ImplFile(ParsedImplFileInput(modules = [
            SynModuleOrNamespace.SynModuleOrNamespace(decls = [
                SynModuleDecl.Expr (expr = SynExpr.App(argExpr = SynExpr.ComputationExpr(expr = SynExpr.LetOrUseBang(andBangs = [
                    SynExprAndBang(range = mAndBang)
                    ]))))
                ])
            ])) ->
            assertRange (5, 4) (5, 24) mAndBang
        | _ ->
            Assert.Fail "Could not get valid AST"

    [<Test>]
    let ``multiple SynExprAndBang have range that starts at and! and ends after expression`` () =
        let ast =
            getParseResults """
async {
    let! bar = getBar ()
    and! foo = getFoo () in
    and! meh = getMeh ()
    return bar
}
"""

        match ast with
        | ParsedInput.ImplFile(ParsedImplFileInput(modules = [
            SynModuleOrNamespace.SynModuleOrNamespace(decls = [
                SynModuleDecl.Expr (expr = SynExpr.App(argExpr = SynExpr.ComputationExpr(expr = SynExpr.LetOrUseBang(andBangs = [
                    SynExprAndBang(range = mAndBang1; trivia={ InKeyword = Some mIn })
                    SynExprAndBang(range = mAndBang2)
                    ]))))
                ])
            ])) ->
            assertRange (4, 4) (4, 24) mAndBang1
            assertRange (4, 25) (4, 27) mIn
            assertRange (5, 4) (5, 24) mAndBang2
        | _ ->
            Assert.Fail "Could not get valid AST"

module ConditionalDirectives =
    let private getDirectiveTrivia isSignatureFile source =
        let ast = (if isSignatureFile then getParseResultsOfSignatureFile else getParseResults) source
        match ast with
        | ParsedInput.ImplFile(ParsedImplFileInput(trivia = { ConditionalDirectives = trivia }))
        | ParsedInput.SigFile(ParsedSigFileInput(trivia = { ConditionalDirectives = trivia })) -> trivia

    [<Test>]
    let ``single #if / #endif`` () =
        let trivia =
            getDirectiveTrivia false """
let v =
    #if DEBUG
    ()
    #endif
    42
"""

        match trivia with
        | [ ConditionalDirectiveTrivia.If(expr, mIf)
            ConditionalDirectiveTrivia.EndIf mEndif ] ->
            assertRange (3, 4) (3, 13) mIf
            assertRange (5, 4) (5, 10) mEndif
            
            match expr with
            | IfDirectiveExpression.Ident "DEBUG" -> ()
            | _ -> Assert.Fail $"Expected different expression, got {expr}"
        | _ ->
            Assert.Fail $"Unexpected trivia, got {trivia}"

    [<Test>]
    let ``single #if / #else / #endif`` () =
        let trivia =
            getDirectiveTrivia false """
let v =
    #if DEBUG
    30
    #else
    42
    #endif
"""

        match trivia with
        | [ ConditionalDirectiveTrivia.If(expr, mIf)
            ConditionalDirectiveTrivia.Else mElse
            ConditionalDirectiveTrivia.EndIf mEndif ] ->
            assertRange (3, 4) (3, 13) mIf
            assertRange (5, 4) (5, 9) mElse
            assertRange (7, 4) (7, 10) mEndif
            
            match expr with
            | IfDirectiveExpression.Ident "DEBUG" -> ()
            | _ -> Assert.Fail $"Expected different expression, got {expr}"
        | _ ->
            Assert.Fail $"Unexpected trivia, got {trivia}"

    [<Test>]
    let ``nested #if / #else / #endif`` () =
        let trivia =
            getDirectiveTrivia false """
let v =
    #if FOO
        #if MEH
        1
        #else
        2
        #endif
    #else
        3
    #endif
"""

        match trivia with
        | [ ConditionalDirectiveTrivia.If(expr1, mIf1)
            ConditionalDirectiveTrivia.If(expr2, mIf2)
            ConditionalDirectiveTrivia.Else mElse1
            ConditionalDirectiveTrivia.EndIf mEndif1
            ConditionalDirectiveTrivia.Else mElse2
            ConditionalDirectiveTrivia.EndIf mEndif2 ] ->
            assertRange (3, 4) (3, 11) mIf1
            assertRange (4, 8) (4, 15) mIf2
            assertRange (6, 8) (6, 13) mElse1
            assertRange (8, 8) (8, 14) mEndif1
            assertRange (9, 4) (9, 9) mElse2
            assertRange (11, 4) (11, 10) mEndif2
            
            match expr1 with
            | IfDirectiveExpression.Ident "FOO" -> ()
            | _ -> Assert.Fail $"Expected different expression, got {expr1}"

            match expr2 with
            | IfDirectiveExpression.Ident "MEH" -> ()
            | _ -> Assert.Fail $"Expected different expression, got {expr2}"
        | _ ->
            Assert.Fail $"Unexpected trivia, got {trivia}"

    [<Test>]
    let ``nested #if / #endif with complex expressions`` () =
        let trivia =
            getDirectiveTrivia false """
let v =
    #if !DEBUG
        #if FOO && BAR
            #if MEH || HMM
                printfn "oh some logging"
            #endif
        #endif
    #endif
"""

        match trivia with
        | [ ConditionalDirectiveTrivia.If(expr1, mIf1)
            ConditionalDirectiveTrivia.If(expr2, mIf2)
            ConditionalDirectiveTrivia.If(expr3, mIf3)
            ConditionalDirectiveTrivia.EndIf mEndif1
            ConditionalDirectiveTrivia.EndIf mEndif2
            ConditionalDirectiveTrivia.EndIf mEndif3 ] ->
            assertRange (3, 4) (3, 14) mIf1
            assertRange (4, 8) (4, 22) mIf2
            assertRange (5, 12) (5, 26) mIf3
            assertRange (7, 12) (7, 18) mEndif1
            assertRange (8, 8) (8, 14) mEndif2
            assertRange (9, 4) (9, 10) mEndif3
            
            match expr1 with
            | IfDirectiveExpression.Not (IfDirectiveExpression.Ident "DEBUG") -> ()
            | _ -> Assert.Fail $"Expected different expression, got {expr1}"

            match expr2 with
            | IfDirectiveExpression.And(IfDirectiveExpression.Ident "FOO", IfDirectiveExpression.Ident "BAR") -> ()
            | _ -> Assert.Fail $"Expected different expression, got {expr2}"

            match expr3 with
            | IfDirectiveExpression.Or(IfDirectiveExpression.Ident "MEH", IfDirectiveExpression.Ident "HMM") -> ()
            | _ -> Assert.Fail $"Expected different expression, got {expr3}"
        | _ ->
            Assert.Fail $"Unexpected trivia, got {trivia}"

    [<Test>]
    let ``directives in multiline comment are not reported as trivia`` () =
        let trivia =
            getDirectiveTrivia false """
let v =
(*
    #if DEBUG
    ()
    #endif
*)
    42
"""

        match trivia with
        | [] -> Assert.Pass()
        | _ ->
            Assert.Fail $"Unexpected trivia, got {trivia}"

    [<Test>]
    let ``directives in multiline string are not reported as trivia`` () =
        let trivia =
            getDirectiveTrivia false "
let v = \"\"\"
    #if DEBUG
    ()
    #endif
    42
\"\"\"
"

        match trivia with
        | [] -> Assert.Pass()
        | _ ->
            Assert.Fail $"Unexpected trivia, got {trivia}"

    [<Test>]
    let ``single #if / #endif, signature file`` () =
        let trivia =
            getDirectiveTrivia true """
namespace Foobar

val v: int =
    #if DEBUG
    1
    #endif
    42
"""

        match trivia with
        | [ ConditionalDirectiveTrivia.If(expr, mIf)
            ConditionalDirectiveTrivia.EndIf mEndif ] ->
            assertRange (5, 4) (5, 13) mIf
            assertRange (7, 4) (7, 10) mEndif
            
            match expr with
            | IfDirectiveExpression.Ident "DEBUG" -> ()
            | _ -> Assert.Fail $"Expected different expression, got {expr}"
        | _ ->
            Assert.Fail $"Unexpected trivia, got {trivia}"

    [<Test>]
    let ``single #if / #else / #endif, signature file`` () =
        let trivia =
            getDirectiveTrivia true """
namespace Foobar

val v : int =
    #if DEBUG
    30
    #else
    42
    #endif
"""

        match trivia with
        | [ ConditionalDirectiveTrivia.If(expr, mIf)
            ConditionalDirectiveTrivia.Else mElse
            ConditionalDirectiveTrivia.EndIf mEndif ] ->
            assertRange (5, 4) (5, 13) mIf
            assertRange (7, 4) (7, 9) mElse
            assertRange (9, 4) (9, 10) mEndif
            
            match expr with
            | IfDirectiveExpression.Ident "DEBUG" -> ()
            | _ -> Assert.Fail $"Expected different expression, got {expr}"
        | _ ->
            Assert.Fail $"Unexpected trivia, got {trivia}"

    [<Test>]
    let ``nested #if / #else / #endif, signature file`` () =
        let trivia =
            getDirectiveTrivia true """
namespace Foobar

val v : int =
    #if FOO
        #if MEH
        1
        #else
        2
        #endif
    #else
        3
    #endif
"""

        match trivia with
        | [ ConditionalDirectiveTrivia.If(expr1, mIf1)
            ConditionalDirectiveTrivia.If(expr2, mIf2)
            ConditionalDirectiveTrivia.Else mElse1
            ConditionalDirectiveTrivia.EndIf mEndif1
            ConditionalDirectiveTrivia.Else mElse2
            ConditionalDirectiveTrivia.EndIf mEndif2 ] ->
            assertRange (5, 4) (5, 11) mIf1
            assertRange (6, 8) (6, 15) mIf2
            assertRange (8, 8) (8, 13) mElse1
            assertRange (10, 8) (10, 14) mEndif1
            assertRange (11, 4) (11, 9) mElse2
            assertRange (13, 4) (13, 10) mEndif2
            
            match expr1 with
            | IfDirectiveExpression.Ident "FOO" -> ()
            | _ -> Assert.Fail $"Expected different expression, got {expr1}"

            match expr2 with
            | IfDirectiveExpression.Ident "MEH" -> ()
            | _ -> Assert.Fail $"Expected different expression, got {expr2}"
        | _ ->
            Assert.Fail $"Unexpected trivia, got {trivia}"

    [<Test>]
    let ``nested #if / #endif with complex expressions, signature file`` () =
        let trivia =
            getDirectiveTrivia true """
namespace Foobar

val v : int =
    #if !DEBUG
        #if FOO && BAR
            #if MEH || HMM
                9
            #endif
        #endif
    #endif
    10
"""

        match trivia with
        | [ ConditionalDirectiveTrivia.If(expr1, mIf1)
            ConditionalDirectiveTrivia.If(expr2, mIf2)
            ConditionalDirectiveTrivia.If(expr3, mIf3)
            ConditionalDirectiveTrivia.EndIf mEndif1
            ConditionalDirectiveTrivia.EndIf mEndif2
            ConditionalDirectiveTrivia.EndIf mEndif3 ] ->
            assertRange (5, 4) (5, 14) mIf1
            assertRange (6, 8) (6, 22) mIf2
            assertRange (7, 12) (7, 26) mIf3
            assertRange (9, 12) (9, 18) mEndif1
            assertRange (10, 8) (10, 14) mEndif2
            assertRange (11, 4) (11, 10) mEndif3
            
            match expr1 with
            | IfDirectiveExpression.Not (IfDirectiveExpression.Ident "DEBUG") -> ()
            | _ -> Assert.Fail $"Expected different expression, got {expr1}"

            match expr2 with
            | IfDirectiveExpression.And(IfDirectiveExpression.Ident "FOO", IfDirectiveExpression.Ident "BAR") -> ()
            | _ -> Assert.Fail $"Expected different expression, got {expr2}"

            match expr3 with
            | IfDirectiveExpression.Or(IfDirectiveExpression.Ident "MEH", IfDirectiveExpression.Ident "HMM") -> ()
            | _ -> Assert.Fail $"Expected different expression, got {expr3}"
        | _ ->
            Assert.Fail $"Unexpected trivia, got {trivia}"

    [<Test>]
    let ``directives in multiline comment are not reported as trivia, signature file`` () =
        let trivia =
            getDirectiveTrivia true """
namespace Foobar

val v : int =
(*
    #if DEBUG
    ()
    #endif
*)
    42
"""

        match trivia with
        | [] -> Assert.Pass()
        | _ ->
            Assert.Fail $"Unexpected trivia, got {trivia}"

    [<Test>]
    let ``directives in multiline string are not reported as trivia, signature file`` () =
        let trivia =
            getDirectiveTrivia true "
namespace Foobar

let v : string = \"\"\"
    #if DEBUG
    ()
    #endif
    42
\"\"\"
"

        match trivia with
        | [] -> Assert.Pass()
        | _ ->
            Assert.Fail $"Unexpected trivia, got {trivia}"

module CodeComments =
    let private getCommentTrivia isSignatureFile source =
        let ast = (if isSignatureFile then getParseResultsOfSignatureFile else getParseResults) source
        match ast with
        | ParsedInput.ImplFile(ParsedImplFileInput(trivia = { CodeComments = trivia }))
        | ParsedInput.SigFile(ParsedSigFileInput(trivia = { CodeComments = trivia })) -> trivia
    
    [<Test>]
    let ``comment on single line`` () =
        let trivia =
            getCommentTrivia false """
// comment!
foo()
"""

        match trivia with
        | [ CommentTrivia.LineComment mComment ] ->
            assertRange (2, 0) (2, 11) mComment
        | _ ->
            Assert.Fail "Could not get valid AST"

    [<Test>]
    let ``comment on single line, signature file`` () =
        let trivia =
            getCommentTrivia true """
namespace Meh
// comment!
foo()
"""

        match trivia with
        | [ CommentTrivia.LineComment mComment ] ->
            assertRange (3, 0) (3, 11) mComment
        | _ ->
            Assert.Fail "Could not get valid AST"

    [<Test>]
    let ``comment after source code`` () =
        let trivia =
            getCommentTrivia false """
foo() // comment!
"""

        match trivia with
        | [ CommentTrivia.LineComment mComment ] ->
            assertRange (2, 6) (2, 17) mComment
        | _ ->
            Assert.Fail "Could not get valid AST"

    [<Test>]
    let ``comment after source code, signature file`` () =
        let trivia =
            getCommentTrivia true """
namespace Meh

val foo : int // comment!
"""

        match trivia with
        | [ CommentTrivia.LineComment mComment ] ->
            assertRange (4, 14) (4, 25) mComment
        | _ ->
            Assert.Fail "Could not get valid AST"

    [<Test>]
    let ``block comment in source code`` () =
        let trivia =
            getCommentTrivia false """
let a (* b *)  c = c + 42
"""

        match trivia with
        | [ CommentTrivia.BlockComment mComment ] ->
            assertRange (2, 6) (2, 13) mComment
        | _ ->
            Assert.Fail "Could not get valid AST"

    [<Test>]
    let ``block comment in source code, signature file`` () =
        let trivia =
            getCommentTrivia true """
namespace Meh

val a (* b *) : int
"""

        match trivia with
        | [ CommentTrivia.BlockComment mComment ] ->
            assertRange (4, 6) (4, 13) mComment
        | _ ->
            Assert.Fail "Could not get valid AST"
