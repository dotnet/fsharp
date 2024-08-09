// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace EmittedIL

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module RealInternalSignature =

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


    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<Theory>]
    let ``Simple MyType in namespace`` (realSig) =

        FSharp """
namespace MyLibrary
type MyFirstType =
    static let x1 = 1100 + System.Random().Next(0)
    static let _ = printfn "Hello, World from MyLibrary.MyFirstType"

type MySecondType =
    static let x2 = 2100 + System.Random().Next(0)
    static let _ = printfn "Hello, World from MyLibrary.MySecondType"

module MyModule =
    [<EntryPoint>]
    let main args =
        printfn "Hello from main method"
        0
        """
        |> withRealInternalSignature realSig
        |> compileExeAndRun
        |> shouldSucceed
        |> withStdOutContainsAllInOrder [
            "Hello, World from MyLibrary.MyFirstType"
            "Hello, World from MyLibrary.MySecondType"
            "Hello from main method"
        ]

    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<Theory>]
    let ``Simple TypeOne and TypeTwo in namespace`` (realSig) =

        FSharp """
namespace MyLibrary
type MyFirstType =
    static let x1 = 1100 + System.Random().Next(0)
    static let _ = printfn "Hello, World from MyLibrary.MyFirstType"

and MySecondType =
    static let x2 = 2100 + System.Random().Next(0)
    static let _ = printfn "Hello, World from MyLibrary.MySecondType"

module MyModule =
    [<EntryPoint>]
    let main args =
        printfn "Hello from main method"
        0
        """
        |> withRealInternalSignature realSig
        |> compileExeAndRun
        |> shouldSucceed
        |> withStdOutContainsAllInOrder [
            "Hello, World from MyLibrary.MyFirstType"
            "Hello, World from MyLibrary.MySecondType"
            "Hello from main method"
        ]

    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<Theory>]
    let ``Simple TypeOne and TypeTwo in module`` (realSig) =

        FSharp """
module MyModule =

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

    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<Theory>]
    let ``Simple TypeOne and TypeTwo in nested module`` (realSig) =

        FSharp """
module MyModule =

    module private MyNestedModule =
        type internal MyFirstType =
            static let x1 = 1100 + System.Random().Next(0)
            static let _ =  printfn "Hello, World from MyModule.MyNestedModule.MyFirstType"
            static let _ =  printfn $"{MyFirstType.DoSomething}"
            static member DoSomething = "My goodness"

        and private MySecondType =
            static let x11 = 1100 + System.Random().Next(0)
            static let _ =  printfn "Hello, World from MyModule.MyNestedModule.MySecondType"

    [<EntryPoint>]
    let main args =
        printfn "Hello from main method"
        0
        """
        |> withRealInternalSignature realSig
        |> compileExeAndRun
        |> shouldSucceed
        |> withStdOutContainsAllInOrder [
            "Hello, World from MyModule.MyNestedModule.MyFirstType"
            "My goodness"
            "Hello, World from MyModule.MyNestedModule.MySecondType"
            "Hello from main method"
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
    let ``Class Type visibility - public type - public ctor`` (realSig) =

        FSharp """
namespace FSharp.Compiler.CodeAnalysis

open System
open System.IO


[<AbstractClass>]
type FSharpSource () =
    abstract FilePath: string

type FSharpSourceFromFile public (filePath: string) =
    inherit FSharpSource()

    override _.FilePath = filePath

type FSharpSource with

    static member CreateFromFile (filePath: string) =
        ()  //FSharpSourceFromFile(filePath) :> FSharpSource

module doit =
    FSharpSource.CreateFromFile("Hello") |> ignore
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
    let ``Class Type visibility - public type - internal ctor`` (realSig) =

        FSharp """
namespace FSharp.Compiler.CodeAnalysis

open System
open System.IO


[<AbstractClass>]
type FSharpSource () =
    abstract FilePath: string

type FSharpSourceFromFile internal (filePath: string) =
    inherit FSharpSource()

    override _.FilePath = filePath

type FSharpSource with

    static member CreateFromFile(filePath: string) =
        FSharpSourceFromFile(filePath) :> FSharpSource

module doit =
    FSharpSource.CreateFromFile("Hello") |> ignore
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
    let ``Class Type visibility - public type - private ctor`` (realSig) =

        FSharp """
namespace FSharp.Compiler.CodeAnalysis

open System
open System.IO


[<AbstractClass>]
type FSharpSource () =
    abstract FilePath: string

type FSharpSourceFromFile private (filePath: string) =
    inherit FSharpSource()

    override _.FilePath = filePath

type FSharpSource with

    static member CreateFromFile (filePath: string) =
        ()  //FSharpSourceFromFile(filePath) :> FSharpSource

module doit =
    FSharpSource.CreateFromFile("Hello") |> ignore
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
    let ``Class Type visibility - public type - unspecified ctor`` (realSig) =

        FSharp """
namespace FSharp.Compiler.CodeAnalysis

open System
open System.IO


[<AbstractClass>]
type FSharpSource () =
    abstract FilePath: string

type FSharpSourceFromFile (filePath: string) =
    inherit FSharpSource()

    override _.FilePath = filePath

type FSharpSource with

    static member CreateFromFile (filePath: string) =
        ()  //FSharpSourceFromFile(filePath) :> FSharpSource

module doit =
    FSharpSource.CreateFromFile("Hello") |> ignore
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
    let ``Class Type visibility - private type - public ctor`` (realSig) =

        FSharp """
namespace FSharp.Compiler.CodeAnalysis

open System
open System.IO


[<AbstractClass>]
type FSharpSource () =
    abstract FilePath: string

type private FSharpSourceFromFile public (filePath: string) =
    inherit FSharpSource()

    override _.FilePath = filePath

type FSharpSource with

    static member CreateFromFile (filePath: string) =
        ()  //FSharpSourceFromFile(filePath) :> FSharpSource

module doit =
    FSharpSource.CreateFromFile("Hello") |> ignore
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
    let ``Class Type visibility - private type - internal ctor`` (realSig) =

        FSharp """
namespace FSharp.Compiler.CodeAnalysis

open System
open System.IO


[<AbstractClass>]
type FSharpSource () =
    abstract FilePath: string

type private FSharpSourceFromFile internal (filePath: string) =
    inherit FSharpSource()

    override _.FilePath = filePath

type FSharpSource with

    static member CreateFromFile(filePath: string) =
        FSharpSourceFromFile(filePath) :> FSharpSource

module doit =
    FSharpSource.CreateFromFile("Hello") |> ignore
    printfn "Main program"
        """
        |> withRealInternalSignature realSig
        |> compileExeAndRun
        |> shouldSucceed
        |> verifyIL [
            if realSig = false then
                // Initialization
                """
.class private abstract auto ansi sealed '<StartupCode$assembly>'.$Test
       extends [runtime]System.Object
{
  .field static assembly class FSharp.Compiler.CodeAnalysis.FSharpSource arg@1
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly int32 init@
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public static void  main@() cil managed
  {
    .entrypoint
    
    .maxstack  8
    IL_0000:  ldstr      "Hello"
    IL_0005:  newobj     instance void FSharp.Compiler.CodeAnalysis.FSharpSourceFromFile::.ctor(string)
    IL_000a:  stsfld     class FSharp.Compiler.CodeAnalysis.FSharpSource '<StartupCode$assembly>'.$Test::arg@1
    IL_000f:  ldstr      "Main program"
    IL_0014:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::.ctor(string)
    IL_0019:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
    IL_001e:  pop
    IL_001f:  ret
  }"""

                // FSharpSource visibility
                """.class public abstract auto ansi serializable FSharp.Compiler.CodeAnalysis.FSharpSource
       extends [runtime]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.AbstractClassAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
  .method public hidebysig specialname abstract virtual 
          instance string  get_FilePath() cil managed
  {
  } 

  .method public specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  callvirt   instance void [runtime]System.Object::.ctor()
    IL_0006:  ldarg.0
    IL_0007:  pop
    IL_0008:  ret
  } 

  .method public static class FSharp.Compiler.CodeAnalysis.FSharpSource 
          CreateFromFile(string filePath) cil managed
  {
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  newobj     instance void FSharp.Compiler.CodeAnalysis.FSharpSourceFromFile::.ctor(string)
    IL_0006:  ret
  } 

  .property instance string FilePath()
  {
    .get instance string FSharp.Compiler.CodeAnalysis.FSharpSource::get_FilePath()
  } 
}"""

                /// FSharpSourceFromFile
                """.class private auto ansi serializable FSharp.Compiler.CodeAnalysis.FSharpSourceFromFile
       extends FSharp.Compiler.CodeAnalysis.FSharpSource
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
  .field assembly string filePath
  .method public specialname rtspecialname 
          instance void  .ctor(string filePath) cil managed
  {
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  callvirt   instance void FSharp.Compiler.CodeAnalysis.FSharpSource::.ctor()
    IL_0006:  ldarg.0
    IL_0007:  pop
    IL_0008:  ldarg.0
    IL_0009:  ldarg.1
    IL_000a:  stfld      string FSharp.Compiler.CodeAnalysis.FSharpSourceFromFile::filePath
    IL_000f:  ret
  } 

  .method public hidebysig specialname virtual 
          instance string  get_FilePath() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      string FSharp.Compiler.CodeAnalysis.FSharpSourceFromFile::filePath
    IL_0006:  ret
  } 

  .property instance string FilePath()
  {
    .get instance string FSharp.Compiler.CodeAnalysis.FSharpSourceFromFile::get_FilePath()
  } 
}"""
                //doit
                """.class public abstract auto ansi sealed FSharp.Compiler.CodeAnalysis.doit
       extends [runtime]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .method assembly specialname static class FSharp.Compiler.CodeAnalysis.FSharpSource 
          get_arg@1() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     class FSharp.Compiler.CodeAnalysis.FSharpSource '<StartupCode$assembly>'.$Test::arg@1
    IL_0005:  ret
  } 

  .property class FSharp.Compiler.CodeAnalysis.FSharpSource
          arg@1()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class FSharp.Compiler.CodeAnalysis.FSharpSource FSharp.Compiler.CodeAnalysis.doit::get_arg@1()
  } 
}"""
        else
                // Initialization
                """.class private abstract auto ansi sealed '<StartupCode$assembly>'.$Test
       extends [runtime]System.Object
{
  .field static assembly int32 init@
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public static void  main@() cil managed
  {
    .entrypoint
    
    .maxstack  8
    IL_0000:  call       void FSharp.Compiler.CodeAnalysis.doit::staticInitialization@()
    IL_0005:  ret
  }"""

                // FSharpSource visibility
                """.class public abstract auto ansi serializable FSharp.Compiler.CodeAnalysis.FSharpSource
       extends [runtime]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.AbstractClassAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
  .method public hidebysig specialname abstract virtual 
          instance string  get_FilePath() cil managed
  {
  } 

  .method public specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  callvirt   instance void [runtime]System.Object::.ctor()
    IL_0006:  ldarg.0
    IL_0007:  pop
    IL_0008:  ret
  } 

  .method public static class FSharp.Compiler.CodeAnalysis.FSharpSource 
          CreateFromFile(string filePath) cil managed
  {
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  newobj     instance void FSharp.Compiler.CodeAnalysis.FSharpSourceFromFile::.ctor(string)
    IL_0006:  ret
  } 

  .property instance string FilePath()
  {
    .get instance string FSharp.Compiler.CodeAnalysis.FSharpSource::get_FilePath()
  } 
}"""

                // FSharpSourceFromFile
                """.class private auto ansi serializable FSharp.Compiler.CodeAnalysis.FSharpSourceFromFile
       extends FSharp.Compiler.CodeAnalysis.FSharpSource
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
  .field assembly string filePath
  .method assembly specialname rtspecialname 
          instance void  .ctor(string filePath) cil managed
  {
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  callvirt   instance void FSharp.Compiler.CodeAnalysis.FSharpSource::.ctor()
    IL_0006:  ldarg.0
    IL_0007:  pop
    IL_0008:  ldarg.0
    IL_0009:  ldarg.1
    IL_000a:  stfld      string FSharp.Compiler.CodeAnalysis.FSharpSourceFromFile::filePath
    IL_000f:  ret
  } 

  .method public hidebysig specialname virtual 
          instance string  get_FilePath() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      string FSharp.Compiler.CodeAnalysis.FSharpSourceFromFile::filePath
    IL_0006:  ret
  } 

  .property instance string FilePath()
  {
    .get instance string FSharp.Compiler.CodeAnalysis.FSharpSourceFromFile::get_FilePath()
  } 
}"""

                // doit
                """.class public abstract auto ansi sealed FSharp.Compiler.CodeAnalysis.doit
       extends [runtime]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .field static assembly class FSharp.Compiler.CodeAnalysis.FSharpSource arg@1
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .method assembly specialname static class FSharp.Compiler.CodeAnalysis.FSharpSource 
          get_arg@1() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     class FSharp.Compiler.CodeAnalysis.FSharpSource FSharp.Compiler.CodeAnalysis.doit::arg@1
    IL_0005:  ret
  } 

  .method private specialname rtspecialname static 
          void  .cctor() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldc.i4.0
    IL_0001:  stsfld     int32 '<StartupCode$assembly>'.$Test::init@
    IL_0006:  ldsfld     int32 '<StartupCode$assembly>'.$Test::init@
    IL_000b:  pop
    IL_000c:  ret
  } 

  .method assembly static void  staticInitialization@() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldstr      "Hello"
    IL_0005:  newobj     instance void FSharp.Compiler.CodeAnalysis.FSharpSourceFromFile::.ctor(string)
    IL_000a:  stsfld     class FSharp.Compiler.CodeAnalysis.FSharpSource FSharp.Compiler.CodeAnalysis.doit::arg@1
    IL_000f:  ldstr      "Main program"
    IL_0014:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::.ctor(string)
    IL_0019:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
    IL_001e:  pop
    IL_001f:  ret
  } 

  .property class FSharp.Compiler.CodeAnalysis.FSharpSource
          arg@1()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class FSharp.Compiler.CodeAnalysis.FSharpSource FSharp.Compiler.CodeAnalysis.doit::get_arg@1()
  } 
}"""
          ]
        //|> withStdOutContainsAllInOrder [
        //    "Main program"
        //]


    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<Theory>]
    let ``Class Type visibility - private type - private ctor`` (realSig) =

        FSharp """
namespace FSharp.Compiler.CodeAnalysis

open System
open System.IO


[<AbstractClass>]
type public FSharpSourcePublic () =
    abstract PublicFilePath: string

[<AbstractClass>]
type internal FSharpSource () =
    inherit FSharpSourcePublic()
    abstract InternalFilePath: string

type private FSharpSourceFromFile private (filePath: string) =
    inherit FSharpSource()

    override _.PublicFilePath = filePath
    override _.InternalFilePath = filePath
    member   _.PrivateFilePath = filePath

type FSharpSource with

    static member public PublicCreateFromFile (filePath: string) = ()
    static member internal InternalCreateFromFile (filePath: string) = ()
    static member private PrivateCreateFromFile (filePath: string) = ()

module doit =
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
    let ``Class Type visibility - private type - unspecified ctor`` (realSig) =

        FSharp """
namespace FSharp.Compiler.CodeAnalysis

open System
open System.IO


[<AbstractClass>]
type FSharpSource () =
    abstract FilePath: string

type private FSharpSourceFromFile (filePath: string) =
    inherit FSharpSource()

    override _.FilePath = filePath

type FSharpSource with

    static member CreateFromFile (filePath: string) =
        FSharpSourceFromFile(filePath) :> FSharpSource

module doit =
    FSharpSource.CreateFromFile("Hello") |> ignore
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
    let ``Class Type visibility with computation expressions - private ctor`` (realSig) =

        FSharp """
module FSharp.Compiler.CodeAnalysis

open System
open System.IO

type MyBuilder() =
    member this.Bind(x, f) = f x
    member this.Return(x) = x

let my = new MyBuilder()

[<AbstractClass>]
type FSharpSource () =
    abstract FilePath: string

type public FSharpSourceFromFile private (filePath: string) =
    inherit FSharpSource()

    override _.FilePath = filePath

    static public MakeOne() =
        my {
            let! file = new  FSharpSourceFromFile ("Hello, World")
            return file
        }

let makeOne() = FSharpSourceFromFile.MakeOne()

printfn $"{makeOne().FilePath}"
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
    let ``Class Type visibility with lazy nested module - private`` (realSig) =

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
    let ``module visibility - various`` (realSig) =

        FSharp """
module outer_default =
    printfn $"outer_outer_default"

module public outer_public =
    printfn $"outer_public"

module internal outer_internal =
    printfn $"outer_internal"

module private outer_private =
    printfn $"outer_private"

module doSomething =
    printfn "Hello, World!"
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

    [<Fact>]
    let ``Calling protected static base member from `static do` does not raise MethodAccessException when --realsig+`` () =
        FSharp """
#nowarn "44" // using Uri.EscapeString just because it's protected static

type C(str : string) =
    inherit System.Uri(str)
    
    static do 
        System.Uri.EscapeString("http://www.myserver.com") |> ignore
        printfn "Hello, World"

module M =
    [<EntryPoint>]
    let main args =
       let res = C("http://www.myserver.com")
       0
        """
        |> withLangVersionPreview
        |> withRealInternalSignature true
        |> asLibrary
        |> compile
        |> compileExeAndRun
        |> shouldSucceed
        |> withStdOutContainsAllInOrder [
            "Hello, World"
        ]

    [<Fact>]
    let ``Calling protected static base member from `static do` raises MethodAccessException with --realsig-`` () =
        FSharp """
#nowarn "44" // using Uri.EscapeString just because it's protected static

type C(str : string) =
    inherit System.Uri(str)
    
    static do 
        System.Uri.EscapeString("http://www.myserver.com") |> ignore
        printfn "Hello, World"

module M =
    [<EntryPoint>]
    let main args =
       let res = C("http://www.myserver.com")
       0
        """
        |> withLangVersionPreview
        |> withRealInternalSignature false
        |> asLibrary
        |> compile
        |> compileExeAndRun
        |> shouldFail