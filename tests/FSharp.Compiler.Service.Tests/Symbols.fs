module FSharp.Compiler.Service.Tests.Symbols

open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Service.Tests.Common
open FSharp.Compiler.Symbols
open FSharp.Compiler.Syntax
open FSharp.Compiler.SyntaxTrivia
open FSharp.Test.Assert
open Xunit

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

    [<Fact>]
    let ``Active pattern case indices`` () =
        let getIndices = Array.map (fun (case: FSharpActivePatternCase) -> case.Index)

        getCaseUsages completePatternInput 7 |> getIndices |> shouldEqual [| 0; 1 |]
        getCaseUsages partialPatternInput 7 |> getIndices |> shouldEqual [| 0 |]

    [<Fact>]
    let ``Active pattern group names`` () =
        let getGroupName (case: FSharpActivePatternCase) = case.Group.Name.Value

        getCaseUsages completePatternInput 7 |> Array.head |> getGroupName |> shouldEqual "|True|False|"
        getCaseUsages partialPatternInput 7 |> Array.head |> getGroupName |> shouldEqual "|String|_|"

module ExternDeclarations =
    [<Fact>]
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
            | SynModuleDecl.Let (_, [SynBinding (accessibility = access)], _) -> Option.map string access |> shouldEqual expected
            | decl -> failwithf "unexpected decl: %O" decl)

        [ "a", (true, false, false, false)
          "b", (true, false, false, false)
          "c", (false, false, false, true) ]
        |> List.iter (fun (name, expected) ->
            match findSymbolByName name checkResults with
            | :? FSharpMemberOrFunctionOrValue as mfv ->
                let access = mfv.Accessibility
                (access.IsPublic, access.IsProtected, access.IsInternal, access.IsPrivate)
                |> shouldEqual expected
            | _ -> failwithf "Couldn't get mfv: %s" name)

    [<Fact>]
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
        | _ -> failwith "Could not get valid AST"

    [<Fact>]
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
            Assert.Equal("unit", unitIdent.idText)
        | _ ->
            failwith $"Could not get valid AST, got {ast}"

    [<Fact>]
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
            Assert.Equal("nativeptr", nativeptrIdent.idText)
        | _ ->
            failwith $"Could not get valid AST, got {ast}"

    [<Fact>]
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
            Assert.Equal("byref", byrefIdent.idText)
        | _ ->
            failwith $"Could not get valid AST, got {ast}"

    [<Fact>]
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
            Assert.Equal("nativeint", nativeintIdent.idText)
        | _ ->
            failwith $"Could not get valid AST, got {ast}"

module XmlDocSig =

    [<Fact>]
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

    [<Fact>]
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
    [<Fact>]
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
    [<Fact>]
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
                |> shouldEqual expectedPrintedType

            | _ -> failwithf "Couldn't get entity: %s" symbolName)

    [<Fact>]
    let ``Interface 01`` () =
        let _, checkResults = getParseAndCheckResults """
open System

IDisposable
"""
        findSymbolUseByName "IDisposable" checkResults |> ignore

    [<Fact>]
    let ``Interface 02`` () =
        let _, checkResults = getParseAndCheckResults """
System.IDisposable
"""
        findSymbolUseByName "IDisposable" checkResults |> ignore

    [<Fact>]
    let ``Interface 03`` () =
        let _, checkResults = getParseAndCheckResults """
open System

{ new IDisposable with }
"""
        findSymbolUseByName "IDisposable" checkResults |> ignore

    
    [<Fact>]
    let ``Interface 04 - Type arg`` () =
        let _, checkResults = getParseAndCheckResults """
open System.Collections.Generic

IList<int>
"""
        let symbolUse = findSymbolUseByName "IList`1" checkResults
        let symbol = symbolUse.Symbol :?> FSharpEntity
        let typeArg = symbol.GenericArguments[0]
        typeArg.Format(symbolUse.DisplayContext) |> shouldEqual "int"

    [<Fact>]
    let ``Interface 05 - Type arg`` () =
        let _, checkResults = getParseAndCheckResults """
type I<'T> =
    abstract M: 'T -> unit

{ new I<_> with
      member this.M(i: int) = () }
"""
        let symbolUse =
            getSymbolUses checkResults
            |> Seq.findBack (fun symbolUse -> symbolUse.Symbol.DisplayName = "I")

        let symbol = symbolUse.Symbol :?> FSharpEntity
        let typeArg = symbol.GenericArguments[0]
        typeArg.Format(symbolUse.DisplayContext) |> shouldEqual "int"

    [<Fact>]
    let ``Interface 06 - Type arg`` () =
        let _, checkResults = getParseAndCheckResults """
type I<'T> =
    abstract M: 'T -> unit

{ new I<int> with
      member this.M _ = () }
"""
        let symbolUse =
            getSymbolUses checkResults
            |> Seq.findBack (fun symbolUse -> symbolUse.Symbol.DisplayName = "I")

        let symbol = symbolUse.Symbol :?> FSharpEntity
        let typeArg = symbol.GenericArguments[0]
        typeArg.Format(symbolUse.DisplayContext) |> shouldEqual "int"

    [<Fact>]
    let ``Operator 01 - Type arg`` () =
        let _, checkResults = getParseAndCheckResults """
[1] |> ignore
"""
        let symbolUses = checkResults.GetAllUsesOfAllSymbolsInFile()
        ()

    [<Fact>]
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
                    |> shouldEqual prefixForm
            | _ -> failwithf "Couldn't get member: %s" entity

    [<Fact>]
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
                    |> shouldEqual suffixForm
            | _ -> failwithf "Couldn't get member: %s" entity

    [<Fact>]
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
                        |> shouldEqual expectedTypeFormat
                | _ -> failwithf "Couldn't get member: %s" entityName
            )

    [<Theory>]
    [<InlineData 2>]
    [<InlineData 6>]
    [<InlineData 32>]
    let ``FsharpType.Format default to arrayNd shorthands for multidimensional arrays`` rank = 
            let commas = System.String(',', rank - 1)
            let _, checkResults = getParseAndCheckResults $""" let myArr : int[{commas}] = Unchecked.defaultOf<_>"""  
            let symbolUse = findSymbolUseByName "myArr" checkResults
            match symbolUse.Symbol  with
            | :? FSharpMemberOrFunctionOrValue as v ->
                v.FullType.Format symbolUse.DisplayContext
                |> shouldEqual $"int array{rank}d"

            | other -> failwithf "myArr was supposed to be a value, but is %A"  other

    [<Fact>]
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
                    mfv.FullType.TypeDefinition.DisplayName |> shouldEqual expectedType
                | true, None ->
                    mfv.FullType.IsGenericParameter |> shouldEqual true
                    mfv.FullType.AllInterfaces.Count |> shouldEqual 0
                | _ -> ()
            | _ -> ()

module FSharpMemberOrFunctionOrValue =
    let private chooseMemberOrFunctionOrValue (su: FSharpSymbolUse) =
        match su.Symbol with :? FSharpMemberOrFunctionOrValue as mfv -> Some mfv | _ -> None

    let private pickPropertySymbol (su: FSharpSymbolUse) =
        match su.Symbol with
        | :? FSharpMemberOrFunctionOrValue as mfv when mfv.IsProperty -> Some (mfv, su.Range)
        | _ -> None
    
    [<Fact>]
    let ``Both Set and Get symbols are present`` () =
        let _, checkResults = getParseAndCheckResults """
namespace Foo

type Foo =
    member _.X
            with get (y: int) : string = ""
            and set (a: int) (b: float) = ()
"""

        // "X" resolves a symbol but it will either be the property, get or set symbol.
        // Use get_ or set_ to differentiate.
        let xPropertySymbol, _ = checkResults.GetSymbolUsesAtLocation(5, 14, "    member _.X", [ "X" ]) |> List.pick pickPropertySymbol

        Assert.True xPropertySymbol.IsProperty
        Assert.True xPropertySymbol.HasGetterMethod
        Assert.True xPropertySymbol.HasSetterMethod

        let getSymbol = findSymbolUseByName "get_X" checkResults
        match getSymbol.Symbol with
        | :? FSharpMemberOrFunctionOrValue as mfv ->
            Assert.Equal(1, mfv.CurriedParameterGroups.[0].Count)
        | symbol -> failwith $"Expected {symbol} to be FSharpMemberOrFunctionOrValue"

        let setSymbol = findSymbolUseByName "set_X" checkResults
        match setSymbol.Symbol with
        | :? FSharpMemberOrFunctionOrValue as mfv ->
            Assert.Equal(2, mfv.CurriedParameterGroups.[0].Count)
        | symbol -> failwith $"Expected {symbol} to be FSharpMemberOrFunctionOrValue"

    [<Fact>]
    let ``AutoProperty with get,set has property symbol!`` () =
        let _, checkResults = getParseAndCheckResults """
namespace Foo

type Foo =
    member val AutoPropGetSet = 0 with get, set
"""

        let symbols = checkResults.GetSymbolUsesAtLocation(5, 29, "    member val AutoPropGetSet = 0 with get, set", ["AutoPropGetSet"])

        let autoPropertySymbol, mAutoPropSymbolUse =
            symbols
            |> List.pick pickPropertySymbol

        Assert.True autoPropertySymbol.IsProperty
        Assert.True autoPropertySymbol.HasGetterMethod
        Assert.True autoPropertySymbol.HasSetterMethod
        Assert.True (autoPropertySymbol.GetterMethod.CompiledName.StartsWith("get_"))
        Assert.True (autoPropertySymbol.SetterMethod.CompiledName.StartsWith("set_"))
        assertRange (5, 15) (5, 29) mAutoPropSymbolUse

        let symbols =
            symbols
            |> List.choose chooseMemberOrFunctionOrValue
            
        match symbols with
        | [ propMfv
            setMfv
            getMfv ] ->
            Assert.True propMfv.IsProperty
            Assert.True(getMfv.CompiledName.StartsWith("get_"))
            Assert.True(setMfv.CompiledName.StartsWith("set_"))
        | _ -> failwith $"Expected three symbols, got %A{symbols}"
        
        // The setter should have a symbol for the generated parameter `v`.
        let setVMfv =
            checkResults.GetSymbolUsesAtLocation(5, 29, "    member val AutoPropGetSet = 0 with get, set", ["v"])
            |> List.tryExactlyOne
            |> Option.map chooseMemberOrFunctionOrValue

        if Option.isNone setVMfv then failwith "No generated v symbol for the setter was found"

    [<Fact>]
    let ``Property symbol is resolved for property`` () =
        let source = """
type X(y: string) =
    member val Y = y with get, set
"""

        let _, checkResults = getParseAndCheckResults source
        let propSymbol, _ =
            checkResults.GetSymbolUsesAtLocation(3, 16, "    member val Y = y with get, set", [ "Y" ])
            |> List.pick pickPropertySymbol

        Assert.True propSymbol.IsProperty
        Assert.True propSymbol.HasGetterMethod
        Assert.True propSymbol.HasSetterMethod
        assertRange (3, 15) (3, 16) propSymbol.SignatureLocation.Value

    [<Fact>]
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
            Assert.Equal(".cctor", cctor.CompiledName)
            Assert.Equal(".ctor", ctor.CompiledName)
            Assert.Equal("SR", entity.DisplayName)
        | _ -> failwith "Expected symbols"

    [<Fact>]
    let ``AutoProperty with get has get symbol attached to property name`` () =
        let _, checkResults = getParseAndCheckResults """
namespace Foo

type Foo() =
    member val Bar = 0 with get
"""

        let autoPropertySymbolUses =
            checkResults.GetSymbolUsesAtLocation(5, 18, "    member val Bar = 0 with get", ["Bar"])
            |> List.map (fun su -> su.Symbol)

        match autoPropertySymbolUses with
        | [ :? FSharpMemberOrFunctionOrValue as mfv ] ->
            Assert.True mfv.IsPropertyGetterMethod
            assertRange (5, 15) (5, 18) mfv.SignatureLocation.Value
        | symbols -> failwith $"Unexpected symbols, got %A{symbols}"

    [<Fact>]
    let ``Property with get has symbol attached to property name`` () =
        let _, checkResults = getParseAndCheckResults """
namespace F

type Foo() =
    let mutable b = 0
    member this.Count with get () = b
"""

        let getSymbolUses =
            checkResults.GetSymbolUsesAtLocation(6, 21, "    member this.Count with get () = b", ["Count"])
            |> List.map (fun su -> su.Symbol)

        match getSymbolUses with
        | [ :? FSharpMemberOrFunctionOrValue as mfv ] ->
            Assert.True mfv.IsPropertyGetterMethod
            assertRange (6, 16) (6, 21) mfv.SignatureLocation.Value
        | symbols -> failwith $"Unexpected symbols, got %A{symbols}"

    [<Fact>]
    let ``Property with set has symbol attached to property name`` () =
        let _, checkResults = getParseAndCheckResults """
namespace F

type Foo() =
    let mutable b = 0
    member this.Count with set (v:int) = b <- v
"""

        let _all = checkResults.GetAllUsesOfAllSymbolsInFile()

        let getSymbolUses =
            checkResults.GetSymbolUsesAtLocation(6, 21, "    member this.Count with set (v:int) = b <- v", ["Count"])
            |> List.map (fun su -> su.Symbol)

        match getSymbolUses with
        | [ :? FSharpMemberOrFunctionOrValue as mfv ] ->
            Assert.True mfv.IsPropertySetterMethod
            assertRange (6, 16) (6, 21) mfv.SignatureLocation.Value
        | symbols -> failwith $"Unexpected symbols, got %A{symbols}"
        
    [<Fact>]
    let ``Property with set/get has property symbol`` () =
        let _, checkResults = getParseAndCheckResults """
namespace F

type Foo() =
    let mutable b = 0
    member this.Count with set (v:int) = b <- v and get () = b
"""

        let propSymbol, _ =
            checkResults.GetSymbolUsesAtLocation(6, 21, "    member this.Count with set (v:int) = b <- v", ["Count"])
            |> List.pick pickPropertySymbol

        Assert.True propSymbol.IsProperty
        Assert.True propSymbol.HasGetterMethod
        Assert.True propSymbol.HasSetterMethod
        assertRange (6, 16) (6, 21) propSymbol.SignatureLocation.Value

    [<Fact>]
    let ``Property usage is reported properly`` () =
        let _, checkResults = getParseAndCheckResults """
module X

type Foo() =
    let mutable b = 0
    member x.Name
        with get() = 0
        and set (v: int) = ()

ignore (Foo().Name)
"""

        let propertySymbolUse, _ =
            checkResults.GetSymbolUsesAtLocation(6, 17, "    member x.Name", ["Name"])
            |> List.pick pickPropertySymbol

        let usages =  checkResults.GetUsesOfSymbolInFile(propertySymbolUse)
        Assert.Equal(2, usages.Length)
        Assert.True usages.[0].IsFromDefinition
        Assert.True usages.[1].IsFromUse

    [<Fact>]
    let ``Property symbol is present after critical error`` () =
        let _, checkResults = getParseAndCheckResults """
module X

let _ = UnresolvedName

type X() =
    member val Y = 1 with get, set
"""

        let _propertySymbolUse, _ =
            checkResults.GetSymbolUsesAtLocation(7, 16, "    member val Y = 1 with get, set", ["Y"])
            |> List.pick pickPropertySymbol

        Assert.False (Array.isEmpty checkResults.Diagnostics)

    [<Fact>]
    let ``Property symbol is present after critical error in property`` () =
        let _, checkResults = getParseAndCheckResults """
module Z

type X() =
    member val Y = UnresolvedName with get, set
"""

        let _propertySymbolUse, _ =
            checkResults.GetSymbolUsesAtLocation(5, 16, "    member val Y = UnresolvedName with get, set", ["Y"])
            |> List.pick pickPropertySymbol

        Assert.False (Array.isEmpty checkResults.Diagnostics)

    [<Fact>]
    let ``Property symbol on interface implementation`` () =
        let _, checkResults = getParseAndCheckResults """
module Z

type I =
    abstract P: int with get, set

type T() =
    interface I with
        member val P = 1 with get, set
"""

        let _propertySymbolUse, mProp =
            checkResults.GetSymbolUsesAtLocation(9, 20, "        member val P = 1 with get, set", ["P"])
            |> List.pick pickPropertySymbol

        assertRange (9, 19) (9, 20) mProp


    [<Fact>]
    let ``Repr info 01`` () =
        let _, checkResults =
            getParseAndCheckResults """
module Module

let f x = ()
"""
        let mfv = findSymbolByName "f" checkResults :?> FSharpMemberOrFunctionOrValue
        let param = mfv.CurriedParameterGroups[0][0]
        param.Name.Value |> shouldEqual "x"

    [<Fact>]
    let ``Repr info 02`` () =
        let _, checkResults =
            getParseAndCheckResults """
module Module

do
    let f x = ()
    ()
"""
        let mfv = findSymbolByName "f" checkResults :?> FSharpMemberOrFunctionOrValue
        let param = mfv.CurriedParameterGroups[0][0]
        param.Name.Value |> shouldEqual "x"

    [<Fact>]
    let ``Repr info 03`` () =
        let _, checkResults =
            getParseAndCheckResults """
module Module

type T() =
    let f x = ()
"""
        let mfv = findSymbolByName "f" checkResults :?> FSharpMemberOrFunctionOrValue
        let param = mfv.CurriedParameterGroups[0][0]
        param.Name.Value |> shouldEqual "x"

module GetValSignatureText =
    let private assertSignature (expected:string) source (lineNumber, column, line, identifier) =
        let _, checkResults = getParseAndCheckResults source
        let symbolUseOpt = checkResults.GetSymbolUseAtLocation(lineNumber, column, line, [ identifier ])
        match symbolUseOpt with
        | None -> failwith "Expected symbol"
        | Some symbolUse ->
            match symbolUse.Symbol with
            | :? FSharpMemberOrFunctionOrValue as mfv ->
                let expected = expected.Replace("\r", "")
                let signature = mfv.GetValSignatureText(symbolUse.DisplayContext, symbolUse.Range)
                Assert.Equal(expected, signature.Value)
            | symbol -> failwith $"Expected FSharpMemberOrFunctionOrValue, got %A{symbol}"

    [<Fact>]
    let ``Signature text for let binding`` () =
        assertSignature
            "val a: b: int -> c: int -> int"
            "let a b c = b + c"
            (1, 4, "let a b c = b + c", "a")

    [<Fact>]
    let ``Signature text for member binding`` () =
        assertSignature
            "member Bar: a: int -> b: int -> int"
            """
type Foo() =
    member this.Bar (a:int) (b:int) : int = 0
"""
            (3, 19, "    member this.Bar (a:int) (b:int) : int = 0", "Bar")

#if NETCOREAPP
    [<Fact>]
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

    [<Fact>]
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

    [<Fact>]
    let ``Signature text for auto property`` () =
        assertSignature
            "member AutoPropGetSet: int with get, set"
            """
module T

type Foo() =
    member val AutoPropGetSet = 0 with get, set
"""
            (5, 29, "    member val AutoPropGetSet = 0 with get, set", "AutoPropGetSet")

    [<Fact>]
    let ``Signature text for property`` () =
        assertSignature
            "member X: y: int -> string with get\nmember X: a: int -> float with set"
            """
module T

type Foo() =
    member _.X
            with get (y: int) : string = ""
            and set (a: int) (b: float) = ()
"""
            (5, 14, "    member _.X", "X")

    [<Fact>]
    let ``Signature text for inline property`` () =
        assertSignature
            "member inline Item: i: int * j: char -> string with get\nmember inline Item: i: int * j: char -> string with set"
            """
module Meh

type Foo =
    member inline this.Item
        with get (i:int,j: char) : string = ""
        and set (i:int,j: char) (x:string) = printfn "%i %c" i j
"""
            (5, 27, "    member inline this.Item", "Item")

module AnonymousRecord =
    [<Fact>]
    let ``Anonymous record copy-and-update symbols usage`` () =
        let _, checkResults = getParseAndCheckResults """
module X
let f (x: {| A: int |}) =
    { x with A = 1 }
"""
        let getSymbolUses =
            checkResults.GetAllUsesOfAllSymbolsInFile()
            |> Array.ofSeq
            |> Array.filter(fun su ->
                match su.Symbol with
                | :? FSharpField as f when f.IsAnonRecordField -> true
                | _ -> false)

        Assert.Equal(2, getSymbolUses.Length)
        
    [<Fact>]
    let ``Anonymous anon record copy-and-update symbols usage`` () =
        let _, checkResults = getParseAndCheckResults """
module X
let f (x: {| A: int |}) =
    {| x with A = 1 |}
"""
        let getSymbolUses =
            checkResults.GetAllUsesOfAllSymbolsInFile()
            |> Array.ofSeq
            |> Array.filter(fun su ->
                match su.Symbol with
                | :? FSharpField as f when f.IsAnonRecordField -> true
                | _ -> false)

        Assert.Equal(2, getSymbolUses.Length)
        
    [<Fact>]
    let ``Anonymous record copy-and-update symbols usages`` () =
        let _, checkResults = getParseAndCheckResults """
        
module X
let f (r: {| A: int; C: int |}) =
    { r with A = 1; B = 2; C = 3 }
"""

        let getSymbolUses =
            checkResults.GetAllUsesOfAllSymbolsInFile()
            |> Array.ofSeq
            |> Array.filter(fun su ->
                match su.Symbol with
                | :? FSharpField as f when f.IsAnonRecordField -> true
                | _ -> false)

        Assert.Equal(4, getSymbolUses.Length)
        
    [<Fact>]
    let ``Anonymous anon record copy-and-update symbols usages`` () =
        let _, checkResults = getParseAndCheckResults """
        
module X
let f (r: {| A: int; C: int |}) =
    {| r with A = 1; B = 2; C = 3 |}
"""

        let getSymbolUses =
            checkResults.GetAllUsesOfAllSymbolsInFile()
            |> Array.ofSeq
            |> Array.filter(fun su ->
                match su.Symbol with
                | :? FSharpField as f when f.IsAnonRecordField -> true
                | _ -> false)

        Assert.Equal(5, getSymbolUses.Length)

    [<Fact>]
    let ``Symbols for fields in nested copy-and-update are present`` () =
        let _, checkResults = getParseAndCheckResults """
type RecordA<'a> = { Foo: 'a; Bar: int; Zoo: RecordA<'a> }

let nestedFunc (a: RecordA<int>) = { a with Zoo.Foo = 1; Zoo.Zoo.Bar = 2; Zoo.Bar = 3; Foo = 4 }
"""

        let line = "let nestedFunc (a: RecordA<int>) = { a with Zoo.Foo = 1; Zoo.Zoo.Bar = 2; Zoo.Bar = 3; Foo = 4 }"

        let fieldSymbolUse =
            checkResults.GetSymbolUsesAtLocation(4, 47, line, [ "Zoo" ])
            |> List.exactlyOne
       
        match fieldSymbolUse.Symbol with
        | :? FSharpField as field ->
            Assert.Equal ("Zoo", field.Name)
            Assert.Equal ("RecordA`1", field.DeclaringEntity.Value.CompiledName)
            assertRange (4, 44) (4, 47) fieldSymbolUse.Range

        | _ -> failwith "Symbol was not FSharpField"


        let fieldSymbolUse =
            checkResults.GetSymbolUsesAtLocation(4, 51, line, [ "Foo" ])
            |> List.exactlyOne
       
        match fieldSymbolUse.Symbol with
        | :? FSharpField as field ->
            Assert.Equal ("Foo", field.Name)
            Assert.Equal ("RecordA`1", field.DeclaringEntity.Value.CompiledName)
            assertRange (4, 48) (4, 51) fieldSymbolUse.Range

        | _ -> failwith "Symbol was not FSharpField"


        let fieldSymbolUse =
            checkResults.GetSymbolUsesAtLocation(4, 60, line, [ "Zoo" ])
            |> List.exactlyOne
       
        match fieldSymbolUse.Symbol with
        | :? FSharpField as field ->
            Assert.Equal ("Zoo", field.Name)
            Assert.Equal ("RecordA`1", field.DeclaringEntity.Value.CompiledName)
            assertRange (4, 57) (4, 60) fieldSymbolUse.Range

        | _ -> failwith "Symbol was not FSharpField"


        let fieldSymbolUse =
            checkResults.GetSymbolUsesAtLocation(4, 64, line, [ "Zoo" ])
            |> List.exactlyOne
       
        match fieldSymbolUse.Symbol with
        | :? FSharpField as field ->
            Assert.Equal ("Zoo", field.Name)
            Assert.Equal ("RecordA`1", field.DeclaringEntity.Value.CompiledName)
            assertRange (4, 61) (4, 64) fieldSymbolUse.Range

        | _ -> failwith "Symbol was not FSharpField"


        let fieldSymbolUse =
            checkResults.GetSymbolUsesAtLocation(4, 68, line, [ "Bar" ])
            |> List.exactlyOne
       
        match fieldSymbolUse.Symbol with
        | :? FSharpField as field ->
            Assert.Equal ("Bar", field.Name)
            Assert.Equal ("RecordA`1", field.DeclaringEntity.Value.CompiledName)
            assertRange (4, 65) (4, 68) fieldSymbolUse.Range

        | _ -> failwith "Symbol was not FSharpField"


        let fieldSymbolUse =
            checkResults.GetSymbolUsesAtLocation(4, 77, line, [ "Zoo" ])
            |> List.exactlyOne
       
        match fieldSymbolUse.Symbol with
        | :? FSharpField as field ->
            Assert.Equal ("Zoo", field.Name)
            Assert.Equal ("RecordA`1", field.DeclaringEntity.Value.CompiledName)
            assertRange (4, 74) (4, 77) fieldSymbolUse.Range

        | _ -> failwith "Symbol was not FSharpField"


        let fieldSymbolUse =
            checkResults.GetSymbolUsesAtLocation(4, 81, line, [ "Bar" ])
            |> List.exactlyOne
       
        match fieldSymbolUse.Symbol with
        | :? FSharpField as field ->
            Assert.Equal ("Bar", field.Name)
            Assert.Equal ("RecordA`1", field.DeclaringEntity.Value.CompiledName)
            assertRange (4, 78) (4, 81) fieldSymbolUse.Range

        | _ -> failwith "Symbol was not FSharpField"


        let fieldSymbolUse =
            checkResults.GetSymbolUsesAtLocation(4, 90, line, [ "Foo" ])
            |> List.exactlyOne
       
        match fieldSymbolUse.Symbol with
        | :? FSharpField as field ->
            Assert.Equal ("Foo", field.Name)
            Assert.Equal ("RecordA`1", field.DeclaringEntity.Value.CompiledName)
            assertRange (4, 87) (4, 90) fieldSymbolUse.Range

        | _ -> failwith "Symbol was not FSharpField"

module ComputationExpressions =
    [<Fact>]
    let ``IsFromComputationExpression only returns true for 'builder' in 'builder { … }'`` () =
        let _, checkResults = getParseAndCheckResults """
type Builder () =
    member _.Return x = x
    member _.Run x = x

let builder = Builder ()

let x = builder { return 3 }
let y = builder
let z = Builder () { return 3 }

type A () =
    let builder = Builder ()
    let _ = builder { return 3 }

    static member Builder = Builder ()

type System.Object with
    static member Builder = Builder ()

let c = A.Builder { return 3 }
let d = System.Object.Builder { return 3 }
"""
        shouldEqual checkResults.Diagnostics [||]

        shouldEqual
            [
                // type Builder () =
                (2, 5), false

                // let builder = …
                (6, 4), false

                // … = Builder ()
                (6, 14), false

                // let x = builder { return 3 }
                (8, 8), false   // Item.Value _
                (8, 8), true    // Item.CustomBuilder _

                // let y = builder
                (9, 8), false

                // let z = Builder () { return 3 }
                (10, 8), false

                // let builder = …
                (13, 8), false

                // … = Builder ()
                (13, 18), false

                // let x = builder { return 3 }
                (14, 12), false   // Item.Value _
                (14, 12), true    // Item.CustomBuilder _

                // static member Builder = …
                (16, 18), false

                // … = Builder ()
                (16, 28), false

                // static member Builder = …
                (19, 18), false

                // … = Builder ()
                (19, 28), false

                // A.Builder { return 3 }
                (21, 8), false

                // System.Object.Builder { return 3 }
                (22, 8), false
            ]
            [
                for symbolUse in checkResults.GetAllUsesOfAllSymbolsInFile() |> Seq.sortBy (fun x -> x.Range.StartLine, x.Range.StartColumn) do
                    match symbolUse.Symbol.DisplayName with
                    | "Builder" | "builder" -> (symbolUse.Range.StartLine, symbolUse.Range.StartColumn), symbolUse.IsFromComputationExpression
                    | _ -> ()
            ]

    [<Fact>]
    let ``IsFromComputationExpression only returns true for 'builder' in 'builder<…> { … }'`` () =
        let _, checkResults = getParseAndCheckResults """
type Builder<'T> () =
    member _.Return x = x
    member _.Run x = x

let builder<'T> = Builder<'T> ()

let x = builder { return 3 }
let y = builder<int> { return 3 }
let z = builder<_> { return 3 }
let p = builder<int>
let q<'T> = builder<'T>
"""

        shouldEqual
            [
                // let builder<'T> = Builder<'T> ()
                (6, 4), false

                // let x = builder { return 3 }
                (8, 8), false   // Item.Value _
                (8, 8), true    // Item.CustomBuilder _

                // let y = builder<int> { return 3 }
                (9, 8), false   // Item.Value _
                (9, 8), true    // Item.CustomBuilder _

                // let z = builder<_> { return 3 }
                (10, 8), false  // Item.Value _
                (10, 8), true   // Item.CustomBuilder _

                // let p = builder<int>
                (11, 8), false

                // let q<'T> = builder<'T>
                (12, 12), false
            ]
            [
                for symbolUse in checkResults.GetAllUsesOfAllSymbolsInFile() do
                    if symbolUse.Symbol.DisplayName = "builder" then
                        (symbolUse.Range.StartLine, symbolUse.Range.StartColumn), symbolUse.IsFromComputationExpression
            ]

    [<Fact>]
    let ``IsFromComputationExpression only returns true for 'builder' in 'builder () { … }'`` () =
        let _, checkResults = getParseAndCheckResults """
type Builder () =
    member _.Return x = x
    member _.Run x = x

let builder () = Builder ()

let x = builder () { return 3 }
let y = builder ()
let z = builder
"""

        shouldEqual
            [
                // let builder () = Builder ()
                (6, 4), false

                // let x = builder () { return 3 }
                (8, 8), false   // Item.Value _
                (8, 8), true    // Item.CustomBuilder _

                // let y = builder ()
                (9, 8), false

                // let z = builder
                (10, 8), false
            ]
            [
                for symbolUse in checkResults.GetAllUsesOfAllSymbolsInFile() do
                    if symbolUse.Symbol.DisplayName = "builder" then
                        (symbolUse.Range.StartLine, symbolUse.Range.StartColumn), symbolUse.IsFromComputationExpression
            ]

module Member =
    [<Fact>]
    let ``Inherit 01`` () =
        let _, checkResults = getParseAndCheckResults """
type T() =
    inherit Foo()

    let i = 1
"""
        assertHasSymbolUsages ["i"] checkResults

module Event =
    [<Fact>]
    let ``CLIEvent member does not produce additional property symbol`` () =
        let _, checkResults = getParseAndCheckResults """
type T() = 
    [<CLIEvent>]
    member this.Event = Event<int>().Publish
"""
        let allSymbols =
            checkResults.GetSymbolUsesAtLocation(4, 21, "    member this.Event = Event<int>().Publish", [ "Event" ])

        let hasPropertySymbols =
            allSymbols
            |> List.exists (fun su ->
                match su.Symbol with
                | :? FSharpMemberOrFunctionOrValue as mfv -> mfv.IsProperty
                | _ -> false
            )

        Assert.False hasPropertySymbols

module Delegates =
    [<Fact>]
    let ``IL metadata`` () =
        let _, checkResults = getParseAndCheckResults """
    module Delegates
    open System

    typeof<Delegate>
    typeof<MulticastDelegate>
    typeof<EventHandler>
    typeof<Action>
    """

        let symbols =
            checkResults.GetAllUsesOfAllSymbolsInFile()
            |> Seq.choose (fun su -> match su.Symbol with :? FSharpEntity as entity -> Some entity | _ -> None)
            |> Seq.map (fun su -> su.DisplayName, su)
            |> dict

        let delegateType = symbols["Delegate"]
        delegateType.IsDelegate |> shouldEqual false
        delegateType.IsClass |> shouldEqual true

        let multicastDelegateType = symbols["MulticastDelegate"]
        multicastDelegateType.IsDelegate |> shouldEqual false
        multicastDelegateType.IsClass |> shouldEqual true

        symbols["EventHandler"].IsDelegate |> shouldEqual true
        symbols["Action"].IsDelegate |> shouldEqual true
