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
          Some "Public"
          Some "Private" ]
        |> List.zip decls
        |> List.iter (fun (actual, expected) ->
            match actual with
            | SynModuleDecl.Let (_, [SynBinding (accessibility = access)], _) -> Option.map string access |> should equal expected
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
        | ParsedInput.ImplFile (ParsedImplFileInput (contents = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
            SynModuleDecl.Let(false, [ SynBinding(range = mb) ] , ml)
        ]) ])) ->
            assertRange (2, 0) (3, 31) ml
            assertRange (2, 0) (3, 31) mb
        | _ -> Assert.Fail "Could not get valid AST"

    [<Test>]
    let ``void keyword in extern`` () =
        let ast = getParseResults """
[<DllImport(@"__Internal", CallingConvention = CallingConvention.Cdecl)>]
extern void setCallbridgeSupportTarget(IntPtr newTarget)
"""

        match ast with
        | ParsedInput.ImplFile(ParsedImplFileInput(contents = [
            SynModuleOrNamespace.SynModuleOrNamespace(decls = [
                SynModuleDecl.Let(false, [ SynBinding(returnInfo =
                    Some (SynBindingReturnInfo(typeName =
                        SynType.App(typeName =
                            SynType.LongIdent(SynLongIdent([unitIdent], [], [Some (IdentTrivia.OriginalNotation "void")])))))) ] , _)
                ])
            ])) ->
            Assert.AreEqual("unit", unitIdent.idText)
        | _ ->
            Assert.Fail $"Could not get valid AST, got {ast}"

    [<Test>]
    let ``nativeptr in extern`` () =
        let ast = getParseResults """
[<DllImport(@"__Internal", CallingConvention = CallingConvention.Cdecl)>]
extern int AccessibleChildren(int* x)
"""

        match ast with
        | ParsedInput.ImplFile(ParsedImplFileInput(contents = [
            SynModuleOrNamespace.SynModuleOrNamespace(decls = [
                SynModuleDecl.Let(false, [ SynBinding(headPat =
                    SynPat.LongIdent(argPats = SynArgPats.Pats [
                        SynPat.Tuple(elementPats = [
                            SynPat.Attrib(pat = SynPat.Typed(targetType = SynType.App(typeName = SynType.LongIdent(
                                SynLongIdent([nativeptrIdent], [], [Some (IdentTrivia.OriginalNotation "*")])
                                ))))
                        ])
                    ])) ], _)
                ])
            ])) ->
            Assert.AreEqual("nativeptr", nativeptrIdent.idText)
        | _ ->
            Assert.Fail $"Could not get valid AST, got {ast}"

    [<Test>]
    let ``byref in extern`` () =
        let ast = getParseResults """
[<DllImport(@"__Internal", CallingConvention = CallingConvention.Cdecl)>]
extern int AccessibleChildren(obj& x)
"""

        match ast with
        | ParsedInput.ImplFile(ParsedImplFileInput(contents = [
            SynModuleOrNamespace.SynModuleOrNamespace(decls = [
                SynModuleDecl.Let(false, [ SynBinding(headPat =
                    SynPat.LongIdent(argPats = SynArgPats.Pats [
                        SynPat.Tuple(elementPats = [
                            SynPat.Attrib(pat = SynPat.Typed(targetType = SynType.App(typeName = SynType.LongIdent(
                                SynLongIdent([byrefIdent], [], [Some (IdentTrivia.OriginalNotation "&")])
                                ))))
                        ])
                    ])) ], _)
                ])
            ])) ->
            Assert.AreEqual("byref", byrefIdent.idText)
        | _ ->
            Assert.Fail $"Could not get valid AST, got {ast}"

    [<Test>]
    let ``nativeint in extern`` () =
        let ast = getParseResults """
[<DllImport(@"__Internal", CallingConvention = CallingConvention.Cdecl)>]
extern int AccessibleChildren(void* x)
"""

        match ast with
        | ParsedInput.ImplFile(ParsedImplFileInput(contents = [
            SynModuleOrNamespace.SynModuleOrNamespace(decls = [
                SynModuleDecl.Let(false, [ SynBinding(headPat =
                    SynPat.LongIdent(argPats = SynArgPats.Pats [
                        SynPat.Tuple(elementPats = [
                            SynPat.Attrib(pat = SynPat.Typed(targetType = SynType.App(typeName = SynType.LongIdent(
                                SynLongIdent([nativeintIdent], [], [Some (IdentTrivia.OriginalNotation "void*")])
                                ))))
                        ])
                    ])) ], _)
                ])
            ])) ->
            Assert.AreEqual("nativeint", nativeintIdent.idText)
        | _ ->
            Assert.Fail $"Could not get valid AST, got {ast}"

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
    let ``FsharpType.Format default to arrayNd shorthands for multidimensional arrays`` ([<Values(2,6,32)>]rank) = 
            let commas = System.String(',', rank - 1)
            let _, checkResults = getParseAndCheckResults $""" let myArr : int[{commas}] = Unchecked.defaultOf<_>"""  
            let symbolUse = findSymbolUseByName "myArr" checkResults
            match symbolUse.Symbol  with
            | :? FSharpMemberOrFunctionOrValue as v ->
                v.FullType.Format symbolUse.DisplayContext
                |> shouldEqual $"int array{rank}d"

            | other -> Assert.Fail(sprintf "myArr was supposed to be a value, but is %A"  other)

    [<Test>]
    let ``Unfinished long ident type `` () =
        let _, checkResults = getParseAndCheckResults """
let g (s: string) = ()

let f1 a1 a2 a3 a4 =
    if true then
        a1
        a2

    a3
    a4

    g a2
    g a4

let f2 b1 b2 b3 b4 b5 =
    if true then
        b1.
        b2.
        b5.

    b3.
    b4.

    g b2
    g b4
    g b5.
"""
        let symbolTypes = 
            ["a1", Some "unit"
             "a2", Some "unit"
             "a3", Some "unit"
             "a4", Some "unit"

             "b1", None
             "b2", Some "string"
             "b3", None
             "b4", Some "string"
             "b5", None]
            |> dict

        for symbol in getSymbolUses checkResults |> getSymbols do
            match symbol with
            | :? FSharpMemberOrFunctionOrValue as mfv ->
                match symbolTypes.TryGetValue(mfv.DisplayName) with
                | true, Some expectedType ->
                    mfv.FullType.TypeDefinition.DisplayName |> should equal expectedType
                | true, None ->
                    mfv.FullType.IsGenericParameter |> should equal true
                    mfv.FullType.AllInterfaces.Count |> should equal 0
                | _ -> ()
            | _ -> ()

module FSharpMemberOrFunctionOrValue =
    [<Test>]
    let ``Both Set and Get symbols are present`` () =
        let _, checkResults = getParseAndCheckResults """
namespace Foo

type Foo =
    member _.X
            with get (y: int) : string = ""
            and set (a: int) (b: float) = ()
"""

        // "X" resolves a symbol but it will either be the get or set symbol.
        // Use get_ or set_ to differentiate.
        let xSymbol = checkResults.GetSymbolUseAtLocation(5, 14, "    member _.X", [ "X" ])
        Assert.True xSymbol.IsSome
        
        let getSymbol = findSymbolUseByName "get_X" checkResults
        match getSymbol.Symbol with
        | :? FSharpMemberOrFunctionOrValue as mfv ->
            Assert.AreEqual(1, mfv.CurriedParameterGroups.[0].Count)
        | symbol -> Assert.Fail $"Expected {symbol} to be FSharpMemberOrFunctionOrValue"

        let setSymbol = findSymbolUseByName "set_X" checkResults
        match setSymbol.Symbol with
        | :? FSharpMemberOrFunctionOrValue as mfv ->
            Assert.AreEqual(2, mfv.CurriedParameterGroups.[0].Count)
        | symbol -> Assert.Fail $"Expected {symbol} to be FSharpMemberOrFunctionOrValue"

    [<Test>]
    let ``AutoProperty with get,set has two symbols`` () =
        let _, checkResults = getParseAndCheckResults """
namespace Foo

type Foo =
    member val AutoPropGetSet = 0 with get, set
"""

        let getSymbol = findSymbolUseByName "get_AutoPropGetSet" checkResults
        let setSymbol = findSymbolUseByName "set_AutoPropGetSet" checkResults

        match getSymbol.Symbol, setSymbol.Symbol with
        | :? FSharpMemberOrFunctionOrValue as getMfv,
          (:? FSharpMemberOrFunctionOrValue as setMfv) ->
            Assert.AreNotEqual(getMfv.CurriedParameterGroups, setMfv.CurriedParameterGroups)
        | _ -> Assert.Fail "Expected symbols to be FSharpMemberOrFunctionOrValue"
        
    [<Test>]
    let ``Multiple symbols are resolved for property`` () =
        let source = """
type X(y: string) =
    member val Y = y with get, set
"""

        let _, checkResults = getParseAndCheckResults source
        let symbolUses =
            checkResults.GetSymbolUsesAtLocation(3, 16, "    member val Y = y with get, set", [ "Y" ])
            |> List.map (fun su -> su.Symbol)

        match symbolUses with
        | [ :? FSharpMemberOrFunctionOrValue as setMfv
            :? FSharpMemberOrFunctionOrValue as getMfv ] ->
            Assert.AreEqual("set_Y", setMfv.CompiledName)
            Assert.AreEqual("get_Y", getMfv.CompiledName)
        | _ -> Assert.Fail "Expected symbols"

    [<Test>]
    let ``Multiple relevant symbols for type name`` () =
        let _, checkResults = getParseAndCheckResults """
// This is a generated file; the original input is 'FSInteractiveSettings.txt'
namespace FSInteractiveSettings

type internal SR () =

    static let mutable swallowResourceText = false

    /// If set to true, then all error messages will just return the filled 'holes' delimited by ',,,'s - this is for language-neutral testing (e.g. localization-invariant baselines).
    static member SwallowResourceText with get () = swallowResourceText
                                        and set (b) = swallowResourceText <- b
    // END BOILERPLATE
"""

        let symbols =
            checkResults.GetSymbolUsesAtLocation(5, 16, "type internal SR () =", [ "" ])
            |> List.map (fun su -> su.Symbol)

        match symbols with
        | [ :? FSharpMemberOrFunctionOrValue as cctor
            :? FSharpMemberOrFunctionOrValue as ctor
            :? FSharpEntity as entity  ] ->
            Assert.AreEqual(".cctor", cctor.CompiledName)
            Assert.AreEqual(".ctor", ctor.CompiledName)
            Assert.AreEqual("SR", entity.DisplayName)
        | _ -> Assert.Fail "Expected symbols"

module Expressions =
    [<Test>]
    let ``Unresolved record field 01`` () =
        let _, checkResults = getParseAndCheckResults """
type R =
    { F1: int
      F2: int }

{ F = 1
  F2 = 1 }
"""
        getSymbolUses checkResults
        |> Seq.exists (fun symbolUse -> symbolUse.IsFromUse && symbolUse.Symbol.DisplayName = "F2")
        |> shouldEqual true

    [<Test>]
    let ``Unresolved record field 02`` () =
        let _, checkResults = getParseAndCheckResults """
[<RequireQualifiedAccess>]
type R =
    { F1: int
      F2: int }

{ F1 = 1
  R.F2 = 1 }
"""
        getSymbolUses checkResults
        |> Seq.exists (fun symbolUse -> symbolUse.IsFromUse && symbolUse.Symbol.DisplayName = "F2")
        |> shouldEqual true

    [<Test>]
    let ``Unresolved record field 03`` () =
        let _, checkResults = getParseAndCheckResults """
[<RequireQualifiedAccess>]
type R =
    { F1: int
      F2: int }

{ R.F2 = 1
  F1 = 1 }
"""
        getSymbolUses checkResults
        |> Seq.exists (fun symbolUse -> symbolUse.IsFromUse && symbolUse.Symbol.DisplayName = "F2")
        |> shouldEqual true

    [<Test>]
    let ``Unresolved record field 04`` () =
        let _, checkResults = getParseAndCheckResults """
type R =
    { F1: int
      F2: int }

match Unchecked.defaultof<R> with
{ F = 1
  F2 = 1 } -> ()
"""
        getSymbolUses checkResults
        |> Seq.exists (fun symbolUse -> symbolUse.IsFromUse && symbolUse.Symbol.DisplayName = "F2")
        |> shouldEqual true

    [<Test>]
    let ``Unresolved record field 05`` () =
        let _, checkResults = getParseAndCheckResults """
[<RequireQualifiedAccess>]
type R =
    { F1: int
      F2: int }

match Unchecked.defaultof<R> with
{ F = 1
  R.F2 = 1 } -> ()
"""
        getSymbolUses checkResults
        |> Seq.exists (fun symbolUse -> symbolUse.IsFromUse && symbolUse.Symbol.DisplayName = "F2")
        |> shouldEqual true


    [<Test>]
    let ``Unresolved record field 06`` () =
        let _, checkResults = getParseAndCheckResults """
[<RequireQualifiedAccess>]
type R =
    { F1: int
      F2: int }

match Unchecked.defaultof<R> with
{ R.F2 = 1
  F = 1 } -> ()
"""
        getSymbolUses checkResults
        |> Seq.exists (fun symbolUse -> symbolUse.IsFromUse && symbolUse.Symbol.DisplayName = "F2")
        |> shouldEqual true

module GetValSignatureText =
    let private assertSignature (expected:string) source (lineNumber, column, line, identifier) =
        let _, checkResults = getParseAndCheckResults source
        let symbolUseOpt = checkResults.GetSymbolUseAtLocation(lineNumber, column, line, [ identifier ])
        match symbolUseOpt with
        | None -> Assert.Fail "Expected symbol"
        | Some symbolUse ->
            match symbolUse.Symbol with
            | :? FSharpMemberOrFunctionOrValue as mfv ->
                let expected = expected.Replace("\r", "")
                let signature = mfv.GetValSignatureText(symbolUse.DisplayContext, symbolUse.Range)
                Assert.AreEqual(expected, signature.Value)
            | symbol -> Assert.Fail $"Expected FSharpMemberOrFunctionOrValue, got %A{symbol}"

    [<Test>]
    let ``Signature text for let binding`` () =
        assertSignature
            "val a: b: int -> c: int -> int"
            "let a b c = b + c"
            (1, 4, "let a b c = b + c", "a")

    [<Test>]
    let ``Signature text for member binding`` () =
        assertSignature
            "member Bar: a: int -> b: int -> int"
            """
type Foo() =
    member this.Bar (a:int) (b:int) : int = 0
"""
            (3, 19, "    member this.Bar (a:int) (b:int) : int = 0", "Bar")

#if NETCOREAPP
    [<Test>]
    let ``Signature text for type with generic parameter in path`` () =
        assertSignature
            "new: builder: ImmutableArray<'T>.Builder -> ImmutableArrayViaBuilder<'T>"
            """
module Telplin

open System
open System.Collections.Generic
open System.Collections.Immutable

type ImmutableArrayViaBuilder<'T>(builder: ImmutableArray<'T>.Builder) =
    class end
"""
            (8, 29, "type ImmutableArrayViaBuilder<'T>(builder: ImmutableArray<'T>.Builder) =", ".ctor")
#endif

    [<Test>]
    let ``Includes attribute for parameter`` () =
        assertSignature
            "val a: [<B>] c: int -> int"
            """
module Telplin

type BAttribute() =
    inherit System.Attribute()

let a ([<B>] c: int) : int = 0
"""
            (7, 5, "let a ([<B>] c: int) : int = 0", "a")
