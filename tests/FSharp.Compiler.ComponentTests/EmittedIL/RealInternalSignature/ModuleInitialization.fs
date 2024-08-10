// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace EmittedIL.RealInternalSignature

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module ModuleInitialization =

    let withRealInternalSignature realSig compilation =
        compilation
        |> withOptions [if realSig then "--realsig+" else "--realsig-" ]

    let simplePublicModule =
        FSharp """
namespace MyLibraryNamespace
module FirstModule =
    let public   x1 = 1100 + System.Random().Next(0)
    let internal y1 = 1200 + System.Random().Next(0)
    let private  z1 = 1300 + System.Random().Next(0)
    printfn "Hello, World from FirstModule"
        """

    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<Theory>]
    let ``Simple module in library`` (realSig) =

        let SimplePublicModule =
            simplePublicModule
            |> withRealInternalSignature realSig
            |> asLibrary

        FSharp """
open MyLibraryNamespace
printfn $"Hello, World!!!! {FirstModule.x1}"
        """
        |> withReferences [ SimplePublicModule ]
        |> withRealInternalSignature realSig
        |> compileExeAndRun
        |> shouldSucceed
        |> withStdOutContainsAllInOrder ["Hello, World from FirstModule"]

    let nestedPublicModule =
        FSharp """
namespace MyLibraryNamespace
module FirstModule =
    let public   x1 = 1100 + System.Random().Next(0)
    let internal y1 = 1200 + System.Random().Next(0)
    let private  z1 = 1300 + System.Random().Next(0)
    printfn "Hello, World from FirstModule"

    module FirstNestedModule =
        let public   x11 = 11100 + System.Random().Next(0)
        let internal y12 = 11200 + System.Random().Next(0)
        let private  z13 = 11300 + System.Random().Next(0)
        printfn "Hello, World from FirstModule:FirstNestedModule"
        """

    let wideNestedPublicModule =
        FSharp """
namespace MyLibraryNamespace

    module FirstModule =
        let x1 = 1101 + System.Random().Next(0)
        printfn "Hello, World from FirstModule"

        module public FirstNestedModule =
            let x11 = 1104 + System.Random().Next(0)
            printfn "Hello, World from FirstModule:FirstNestedModule"

            module private FirstDeepNestedModule =
                let x111 = 1107 + System.Random().Next(0)
                printfn "Hello, World from FirstModule:FirstNestedModule:FirstDeepNestedModule"

        module internal SecondNestedModule =
            let secondNestedDoIt() = ()
            module private SecondDeepNestedModule =
                let secondNestedDoIt() = ()

        printfn "Hello, World from between FirstModule:SecondNested and FirstModule:ThirdNestedModule"

        module private ThirdNestedModule =
            let x13 = 1113 + System.Random().Next(0)
            printfn "Hello, World from FirstModule:ThirdNestedModule"

            module public ThirdDeepNestedModule =
                let x132 = 1116 + System.Random().Next(0)
                printfn "Hello, World from FirstModule:ThirdNestedModule:ThirdDeepNestedModule"

    module SecondModule =
        let x2 = 2101 + System.Random().Next(0)
        printfn "Hello, World from SecondModule"

        module public FirstNestedModule =
            let x21 = 2104 + System.Random().Next(0)
            printfn "Hello, World from SecondModule:FirstNestedModule"

            module private FirstDeepNestedModule =
                let x211 = 2107 + System.Random().Next(0)
                printfn "Hello, World from SecondModule:FirstNestedModule:FirstDeepNestedModule"

        module internal SecondNestedModule =
            let secondNestedDoIt() = ()
            module private SecondDeepNestedModule =
                let secondNestedDoIt() = ()

        printfn "Hello, World from between SecondModule:SecondNested and FirstModule:ThirdNestedModule"

        module private ThirdNestedModule =
            let x23 = 2113 + System.Random().Next(0)

            module private ThirdDeepNestedModule =
                let x233 = 2116 + System.Random().Next(0)
                printfn "Hello, World from SecondModule:ThirdNestedModule:ThirdDeepNestedModule"
        """

    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<Theory>]
    let ``Wide Nested module in library`` (realSig) =

        let WideNestedPublicModule =
            wideNestedPublicModule
            |> withRealInternalSignature realSig
            |> asLibrary

        FSharp """
open MyLibraryNamespace
printfn $"Hello, World!!!! {FirstModule.x1}"
        """
        |> withReferences [ WideNestedPublicModule ]
        |> withRealInternalSignature realSig
        |> compileExeAndRun
        |> shouldSucceed
        |> withStdOutContainsAllInOrder [
            "Hello, World from FirstModule"
            "Hello, World from FirstModule:FirstNestedModule"
            "Hello, World from FirstModule:FirstNestedModule:FirstDeepNestedModule"
            "Hello, World from between FirstModule:SecondNested and FirstModule:ThirdNestedModule"
            "Hello, World from FirstModule:ThirdNestedModule"
            "Hello, World from FirstModule:ThirdNestedModule:ThirdDeepNestedModule"
            "Hello, World from SecondModule"
            "Hello, World from SecondModule:FirstNestedModule"
            "Hello, World from SecondModule:FirstNestedModule:FirstDeepNestedModule"
            "Hello, World from between SecondModule:SecondNested and FirstModule:ThirdNestedModule"
            "Hello, World from SecondModule:ThirdNestedModule:ThirdDeepNestedModule"
            ]

    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<Theory>]
    let ``Nested module in library`` (realSig) =

        let NestedPublicModule =
            nestedPublicModule
            |> withRealInternalSignature realSig
            |> asLibrary

        FSharp """
open MyLibraryNamespace
printfn $"Hello, World!!!! {FirstModule.FirstNestedModule.x11}"
        """
        |> withReferences [ NestedPublicModule ]
        |> withRealInternalSignature realSig
        |> compileExeAndRun
        |> shouldSucceed
        |> withStdOutContainsAllInOrder [
            "Hello, World from FirstModule"
            "Hello, World from FirstModule:FirstNestedModule"
            ]

    let deeplyNestedPublicModule =
        FSharp """
namespace MyLibraryNamespace

    module FirstModule =
        let public   x1 = 1101 + System.Random().Next(0)
        let internal y1 = 1102 + System.Random().Next(0)
        let private  z1 = 1103 + System.Random().Next(0)
        printfn "Hello, World from FirstModule"

        module FirstNestedModule =
            let x11 = 1104 + System.Random().Next(0)
            let y11 = 1105 + System.Random().Next(0)
            let z11 = 1106 + System.Random().Next(0)
            printfn "Hello, World from FirstModule:FirstNestedModule"

            module FirstDeepNestedModule =
                let x111 = 1107 + System.Random().Next(0)
                let y111 = 1108+ System.Random().Next(0)
                let z111 = 1109 + System.Random().Next(0)
                printfn "Hello, World from FirstModule:FirstNestedModule:FirstDeepNestedModule"

        module internal SecondNestedModule =
            module private SecondDeepNestedModule =
                let x121 = 1110 + System.Random().Next(0)
                let y121 = 1111 + System.Random().Next(0)
                let z121 = 1112 + System.Random().Next(0)
                printfn "Hello, World from FirstModule:SecondNestedModule:SecondDeepNestedModule"

        printfn "Hello, World from between FirstModule:SecondNested and FirstModule:ThirdNestedModule"

        module private ThirdNestedModule =
            let x13 = 1113 + System.Random().Next(0)
            let y13 = 1114 + System.Random().Next(0)
            let z13 = 1115 + System.Random().Next(0)
            printfn "Hello, World from FirstModule:ThirdNestedModule"

            module private ThirdDeepNestedModule =
                let x132 = 1116 + System.Random().Next(0)
                let y132 = 1117 + System.Random().Next(0)
                let z132 = 1118 + System.Random().Next(0)
                printfn "Hello, World from FirstModule:ThirdNestedModule:ThirdDeepNestedModule"
            """

    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<Theory>]
    let ``Deeply Nested module in library - Deep Activation`` (realSig) =

        let DeeplyNestedPublicModule =
            deeplyNestedPublicModule
            |> withRealInternalSignature realSig
            |> asLibrary

        FSharp """
open MyLibraryNamespace
printfn $"Hello, World!!!! {FirstModule.FirstNestedModule.FirstDeepNestedModule.x111}"
        """
        |> withReferences [ DeeplyNestedPublicModule ]
        |> withRealInternalSignature realSig
        |> compileExeAndRun
        |> shouldSucceed
        |> withStdOutContainsAllInOrder [
            "Hello, World from FirstModule"
            "Hello, World from FirstModule:FirstNestedModule"
            "Hello, World from FirstModule:FirstNestedModule:FirstDeepNestedModule"
            "Hello, World from FirstModule:SecondNestedModule:SecondDeepNestedModule"
            "Hello, World from between FirstModule:SecondNested and FirstModule:ThirdNestedModule"
            "Hello, World from FirstModule:ThirdNestedModule"
            "Hello, World from FirstModule:ThirdNestedModule:ThirdDeepNestedModule"
            ]


    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<Theory>]
    let ``Nested module in library - Nested Activation`` (realSig) =

        let DeeplyNestedPublicModule =
            deeplyNestedPublicModule
            |> withRealInternalSignature realSig
            |> asLibrary

        FSharp """
open MyLibraryNamespace
printfn $"Hello, World!!!! {FirstModule.FirstNestedModule.x11}"
        """
        |> withReferences [ DeeplyNestedPublicModule ]
        |> withRealInternalSignature realSig
        |> compileExeAndRun
        |> shouldSucceed
        |> withStdOutContainsAllInOrder [
            "Hello, World from FirstModule"
            "Hello, World from FirstModule:FirstNestedModule"
            "Hello, World from FirstModule:FirstNestedModule:FirstDeepNestedModule"
            "Hello, World from FirstModule:SecondNestedModule:SecondDeepNestedModule"
            "Hello, World from between FirstModule:SecondNested and FirstModule:ThirdNestedModule"
            "Hello, World from FirstModule:ThirdNestedModule"
            "Hello, World from FirstModule:ThirdNestedModule:ThirdDeepNestedModule"
            ]


    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<Theory>]
    let ``Deeply Nested module in library - Shallow Activation`` (realSig) =

        let DeeplyNestedPublicModule =
            deeplyNestedPublicModule
            |> withRealInternalSignature realSig
            |> asLibrary

        FSharp """
open MyLibraryNamespace
printfn $"Hello, World!!!! {FirstModule.x1}"
        """
        |> withReferences [ DeeplyNestedPublicModule ]
        |> withRealInternalSignature realSig
        |> compileExeAndRun
        |> shouldSucceed
        |> withStdOutContainsAllInOrder [
            "Hello, World from FirstModule"
            "Hello, World from FirstModule:FirstNestedModule"
            "Hello, World from FirstModule:FirstNestedModule:FirstDeepNestedModule"
            "Hello, World from FirstModule:SecondNestedModule:SecondDeepNestedModule"
            "Hello, World from between FirstModule:SecondNested and FirstModule:ThirdNestedModule"
            "Hello, World from FirstModule:ThirdNestedModule"
            "Hello, World from FirstModule:ThirdNestedModule:ThirdDeepNestedModule"
            ]

    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<Theory>]
    let ``Simple MyLibrary`` (realSig) =

        FSharp """
module MyLibrary
let x1 = 1100 + System.Random().Next(0)
let y1 = 1200 + System.Random().Next(0)
let z1 = 1300 + System.Random().Next(0)
printfn "Hello, World from MyLibrary"
        """
        |> withRealInternalSignature realSig
        |> compileExeAndRun
        |> shouldSucceed
        |> withStdOutContainsAllInOrder ["Hello, World from MyLibrary"]

    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<Theory>]
    let ``Simple MyLibrary with nested types`` (realSig) =

        FSharp """
module MyLibrary
let x1 = 1100 + System.Random().Next(0)
let y1 = 1200 + System.Random().Next(0)
let z1 = 1300 + System.Random().Next(0)
printfn "Hello, World from MyLibrary"

type MyType =
    static let x1 = 1100 + System.Random().Next(0)
    static let _ = printfn "Hello, World from MyLibrary.MyType"
        """
        |> withRealInternalSignature realSig
        |> compileExeAndRun
        |> shouldSucceed
        |> withStdOutContainsAllInOrder [
            "Hello, World from MyLibrary"
            "Hello, World from MyLibrary.MyType"
        ]

    [<InlineData(true, true)>]        // RealSig, recursive module
    [<InlineData(false, true)>]       // Regular, recursive module
    [<InlineData(true, false)>]       // RealSig, module
    [<InlineData(false, false)>]      // Regular, module
    [<Theory>]
    let ``Simple MyLibrary with nested types and ref fields`` (realSig) recursiveModule =
        let recursive = if recursiveModule then "rec" else ""
        FSharp $"""
            namespace {recursive} testns
                module StaticInitializerTest3 =
                    let x = ref 2
                    do x := 3

                    type C() = 
                        static let mutable v = x.Value + 1
                        static do v <- 3

                        member x.P = v
                        static member P2 = v+x.Value

                    printfn $"{{(new C()).P}}"
                    printfn $"{{C.P2}}"
                    if C.P2 <> 6 then failwith $"Invalid result:  C.P2 <> 6 - actual: {{C.P2}}"
        """
        |> withRealInternalSignature realSig
        |> withOptions [ "--nowarn:3370"; "--debug+"; "--optimize-" ]
        |> compileExeAndRun
        |> shouldSucceed
        |> withStdOutContainsAllInOrder [ "3"; "6" ]

    [<InlineData(true, true)>]        // RealSig, recursive module
    [<InlineData(false, true)>]       // Regular, recursive module
    [<InlineData(true, false)>]       // RealSig, module
    [<InlineData(false, false)>]      // Regular, module
    [<Theory>]
    let ``Simple TypeOne and TypeTwo and TypeThree in module`` (realSig, recursiveModule) =

        let recursive = if recursiveModule then "rec" else ""
        FSharp $"""
module {recursive} MyModule =

    type public MyFirstType =
        static let x1 = 1100 + System.Random().Next(0)
        static let _ = printfn "Hello, World from MyModule.MyFirstType"

    and internal MySecondType =
        static let x2 = 2100 + System.Random().Next(0)
        static let _ = printfn "Hello, World from MyModule.MySecondType"

    and private MyThirdType =
        static let x3 = 3100 + System.Random().Next(0)
        static let _ = printfn "Hello, World from MyModule.MyThirdType"

    printfn "Hello from main method"
        """
        |> withRealInternalSignature realSig
        |> compileExeAndRun
        |> shouldSucceed
        |> withStdOutContainsAllInOrder [
            "Hello, World from MyModule.MyFirstType"
            "Hello, World from MyModule.MySecondType"
            "Hello, World from MyModule.MyThirdType"
            "Hello from main method"
        ]

    [<InlineData(true, true)>]        // RealSig, recursive module
    [<InlineData(false, true)>]       // Regular, recursive module
    [<InlineData(true, false)>]       // RealSig, module
    [<InlineData(false, false)>]      // Regular, module
    [<Theory>]
    let ``Simple TypeOne and TypeTwo in nested module`` (realSig, recursiveModule) =

        let recursive = if recursiveModule then "rec" else ""
        FSharp $$"""
module {{recursive}} MyModule =

    do  printfn $"Do start MyModule"

    module private MyNestedModule =

        do  printfn $"Do start MyNestedModule"

        type internal MyFirstType =
            static let _ =  printfn "Hello, World from MyModule.MyNestedModule.MyFirstType"
            static let _ =  printfn $"{MyFirstType.FirstDoSomething}"
            static member FirstDoSomething = "My goodness I'm 'MyFirstType'"

        do  printfn $"between types in MyNestedModule"

        type private MySecondType =
            static let x11 = 1100 + System.Random().Next(0)
            static let _ =  printfn "Hello, World from MyModule.MyNestedModule.MySecondType"
            static let _ =  printfn $"{MySecondType.SecondDoSomething}"
            static member SecondDoSomething = "My goodness I'm 'MySecondType'"

        do  printfn $"Do end MyNestedModule"

    do  printfn $"Do end MyModule"

    [<EntryPoint>]
    let main args =
        printfn "Hello from main method"
        0
        """
        |> withRealInternalSignature realSig
        |> compileExeAndRun
        |> shouldSucceed
        |> withStdOutContainsAllInOrder [
            "Do start MyModule"
            "Do start MyNestedModule"
            "Hello, World from MyModule.MyNestedModule.MyFirstType"
            "My goodness I'm 'MyFirstType'"
            "between types in MyNestedModule"
            "Hello, World from MyModule.MyNestedModule.MySecondType"
            "My goodness I'm 'MySecondType'"
            "Do end MyNestedModule"
            "Do end MyModule"
        ]

    let withFlavor release compilation =
        if not release then
            compilation |> withDebug
        else
            compilation

    [<InlineData(true, true)>]          // RealSig, release
    [<InlineData(false, true)>]         // Regular, release
    [<InlineData(true, false)>]         // RealSig, debug
    [<InlineData(false, false)>]        // Regular, debug
    [<Theory>]
    let ``recursive types in module`` (realSig, release) =
        FSharp $$"""
module rec MyModule =
    type Node = { Next: Node; Value: int }

    let one = { Next = two; Value = 1 }

    // An intervening type declaration
    type M() = static member X() = one

    let two = { Next = one; Value = 2 }

    let test t s1 s2 =
      if s1 <> s2 then
        stdout.WriteLine ($"test:{t} '{s1}' '{s2}' failed")
      else
        stdout.WriteLine ($"test:{t} '{s1}' '{s2}' succeeded")

    [<EntryPoint>]
    let main args =
        test "one.Value 1" one.Value 1
        test "one.Next.Value 2" one.Next.Value 2
        test "(M.X()).Value 1" (M.X()).Value 1
        test "(M.X()).Next.Value 2" (M.X()).Next.Value 2
        test "(M.X()).Next.Next.Value 1" (M.X()).Next.Next.Value 1
        test "two.Value 2" two.Value 2
        test "two.Next.Value 1" two.Next.Value 1
        test "two.Next.Next.Value 2" two.Next.Next.Value 2
        0
            """
        |> withFlavor release
        |> withRealInternalSignature realSig
        |> compileExeAndRun
        |> shouldSucceed
        |> withStdOutContainsAllInOrder [
            "test:one.Value 1 '1' '1' succeeded"
            "test:one.Next.Value 2 '2' '2' succeeded"
            "test:(M.X()).Value 1 '1' '1' succeeded"
            "test:(M.X()).Next.Value 2 '2' '2' succeeded"
            "test:(M.X()).Next.Next.Value 1 '1' '1' succeeded"
            "test:two.Value 2 '2' '2' succeeded"
            "test:two.Next.Value 1 '1' '1' succeeded"
            "test:two.Next.Next.Value 2 '2' '2' succeeded"
        ]

    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<Theory>]
    let ``Module Nested Type visibility`` (realSig) =

        FSharp """
module internal PrintfImpl

    module private FormatString =
        let x = 0
        let findNextFormatSpecifier () = $"FormatString.findNextFormatSpecifier () - {x}"

    type FormatParser () =
        let parseAndCreateStepsForCapturedFormat () =
            $"FormatParser.prefix : " + FormatString.findNextFormatSpecifier()
        member _.GetStepsForCapturedFormat() =
            parseAndCreateStepsForCapturedFormat ()

    printfn $"FormatParser.prefix: {FormatParser().GetStepsForCapturedFormat()}"
    printfn "Main program"
        """
        |> withRealInternalSignature realSig
        |> compileExeAndRun
        |> shouldSucceed
        |> withStdOutContainsAllInOrder [
            "FormatParser.prefix: FormatParser.prefix : FormatString.findNextFormatSpecifier ()"
            "Main program"
        ]

    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<Theory>]
    let ``Class Type visibility in module - public type - private ctor`` (realSig) =

        FSharp """
module FSharp.Compiler.CodeAnalysis

open System
open System.IO


[<AbstractClass>]
type FSharpSource () =
    abstract FilePath: string

type public FSharpSourceFromFile internal (filePath: string) =
    inherit FSharpSource()

    override _.FilePath = filePath

let createFromFile filePath = FSharpSourceFromFile (filePath) :> FSharpSource

module doit =
    createFromFile("Hello") |> ignore
    printfn "Main program"
        """
        |> withRealInternalSignature realSig
        |> compileExeAndRun
        |> shouldSucceed
        |> withStdOutContainsAllInOrder [
            "Main program"
        ]

    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<Theory>]
    let ``Lazy nested module - private`` (realSig) =

        FSharp """
module private TestReferences =

    [<RequireQualifiedAccess>]
    module NetStandard20 =
        module Files =
            let netStandard = lazy "Hello, World!!!"

module doSomething =
    printfn $"{TestReferences.NetStandard20.Files.netStandard.Value}"
        """
        |> withRealInternalSignature realSig
        |> compileExeAndRun
        |> shouldSucceed
        |> withStdOutContainsAllInOrder [
            "Hello, World"
        ]

    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<Theory>]
    let ``module visibility - Datum`` (realSig) =

        FSharp """
module Test6
    module internal HelperModule = 
            
        type public Data = 
            private
                {
                    Datum: int
                }
                    
        let internal handle (data:Data): int = data.Datum
            
    module public Module =
            
        type public Data = 
            private
                {
                    Thing: HelperModule.Data
                }

        let public getInt (data:Data): int = HelperModule.handle data.Thing               
        """
        |> withRealInternalSignature realSig
        |> asLibrary
        |> compile
        |> shouldSucceed
