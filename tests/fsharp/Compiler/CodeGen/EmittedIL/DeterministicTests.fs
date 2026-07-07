// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests.CodeGen.EmittedIL

open System
open System.IO
open FSharp.Test
open FSharp.Test.Compiler
open Xunit


[<RunTestCasesInSequence>]
module DeterministicTests =

    let commonOptions = ["--refonly";"--deterministic";"--nooptimizationdata"]
    let inputPath = CompilerAssert.GenerateFsInputPath()
    let outputPath = CompilerAssert.GenerateDllOutputPath()

    [<Literal>]
    let ivtSnippet = """
[<assembly:System.Runtime.CompilerServices.InternalsVisibleToAttribute("Assembly.Name")>]
do() 
"""

    [<Literal>]
    let basicCodeSnippet = """
module ReferenceAssembly

open System

//PLACEHOLDER

let private privTest() =
    Console.WriteLine("Private Hello World!")

let test() =
    privTest()
    Console.WriteLine("Hello World!")"""


    let getMvid (codeSnippet: string) compileOptions =
        File.WriteAllText(inputPath, codeSnippet)

        let mvid1 =
            FSharpWithInputAndOutputPath codeSnippet inputPath outputPath
            |> withOptions compileOptions
            |> compileGuid

        mvid1

    let commonOptionsBasicMvid = lazy(getMvid basicCodeSnippet commonOptions)

    let calculateRefAssMvids referenceCodeSnippet codeAfterChangeIsDone = 

        let mvid1 = 
            if referenceCodeSnippet = basicCodeSnippet then
                commonOptionsBasicMvid.Value
            else
                getMvid referenceCodeSnippet commonOptions

        let mvid2 = getMvid codeAfterChangeIsDone commonOptions
        mvid1 , mvid2


    [<Fact>]
    let ``Simple assembly should be deterministic``() =  
        File.WriteAllText(inputPath, basicCodeSnippet)

        let getMvid() =
            FSharpWithInputAndOutputPath basicCodeSnippet inputPath outputPath
            |> withOptions ["--deterministic"]
            |> compileGuid

        // Two identical compilations should produce the same MVID
        Assert.Equal(getMvid(), getMvid())

    [<Fact>]
    let ``Simple assembly with different platform should not be deterministic``() = 
        let mvid1 = getMvid basicCodeSnippet ["--deterministic"]
        let mvid2 = getMvid basicCodeSnippet ["--deterministic";"--platform:Itanium"]
        // No two platforms should produce the same MVID
        Assert.NotEqual(mvid1, mvid2)

    [<Fact>]
    let ``Simple reference assembly should be deterministic``() =
        let mvid1, mvid2 = calculateRefAssMvids basicCodeSnippet basicCodeSnippet
        Assert.Equal(mvid1, mvid2)

    [<Fact>]
    let ``Simple reference assembly with different platform should not be deterministic``() =
        let mvid1 = getMvid basicCodeSnippet ["--refonly";"--deterministic"]
        let mvid2 = getMvid basicCodeSnippet ["--refonly";"--deterministic";"--platform:Itanium"]

        // No two platforms should produce the same MVID
        Assert.NotEqual(mvid1, mvid2)

    [<Fact>]
    let ``False-positive reference assemblies test, different assemblies' mvid should not match`` () =
        let src2 = basicCodeSnippet.Replace("test()","test2()")
        let mvid1, mvid2 = calculateRefAssMvids basicCodeSnippet src2
        Assert.NotEqual(mvid1, mvid2)

    [<Fact>] 
    let ``Reference assemblies should be deterministic when only private function name is different with the same function name length`` () =
        let privCode1 = basicCodeSnippet.Replace("privTest()","privTest1()")
        let privCode2 = basicCodeSnippet.Replace("privTest()","privTest2()")
        let mvid1, mvid2 = calculateRefAssMvids privCode1 privCode2
        Assert.Equal(mvid1, mvid2)
    
    
    [<Fact>] 
    let ``Reference assemblies should be deterministic when only private function name is different with the different function name length`` () =
        let src2 = basicCodeSnippet.Replace("privTest()","privTest11()")
        let mvid1, mvid2 = calculateRefAssMvids basicCodeSnippet src2
        Assert.Equal(mvid1, mvid2)
    
    [<Fact>] 
    let ``Reference assemblies should be deterministic when only private function body is different`` () =
        let src2 = basicCodeSnippet.Replace("""Console.WriteLine("Private Hello World!")""","""Console.Write("Private Hello World!")""")
        let mvid1, mvid2 = calculateRefAssMvids basicCodeSnippet src2
        Assert.Equal(mvid1, mvid2)
        
    [<Fact>] 
    let ``Reference assemblies should be deterministic when only private function return type is different`` () =

        let src2 =
            """
module ReferenceAssembly

open System

let private privTest() : int = 0

let test() =
    privTest() |> ignore
    Console.WriteLine()
            """
        let mvid1, mvid2 = calculateRefAssMvids basicCodeSnippet src2
        Assert.Equal(mvid1, mvid2)
     
    [<Fact>] 
    let ``Reference assemblies should be deterministic when only private function parameter count is different`` () =
        let src =
            """
module ReferenceAssembly

open System

let private privTest1 () : string = "Private Hello World!"

let test() =
    privTest1 () |> ignore
    Console.WriteLine()
            """
       
        let src2 =
            """
module ReferenceAssembly

open System

let private privTest1 () () : string = "Private Hello World!"

let test() =
    privTest1 () () |> ignore
    Console.WriteLine()
            """

        let mvid1, mvid2 = calculateRefAssMvids src src2
        Assert.Equal(mvid1, mvid2)

    [<Fact>] 
    let ``Reference assemblies should be deterministic when only private function parameter count is different and private function is unused`` () =
        let src =
            """
module ReferenceAssembly

open System

let private privTest1 () : string = "Private Hello World!"

let test() =
    Console.WriteLine()
            """
        let src2 =
            """
module ReferenceAssembly

open System

let private privTest1 () () : string = "Private Hello World!"

let test() =
    Console.WriteLine()
            """

        let mvid1, mvid2 = calculateRefAssMvids src src2
        Assert.Equal(mvid1, mvid2)
     
    [<Fact>] 
    let ``Reference assemblies should be deterministic when only private function parameter types are different`` () =
        let src =
            """
module ReferenceAssembly

open System

let private privTest1 () = "Private Hello World!"

let test() =
    privTest1() |> ignore
    Console.WriteLine()
            """

        let src2 =
            """
module ReferenceAssembly

open System

let private privTest1 (_: string) = "Private Hello World!"

let test() =
    privTest1 "" |> ignore
    Console.WriteLine()
            """
       
        let mvid1, mvid2 = calculateRefAssMvids src src2
        Assert.Equal(mvid1, mvid2)
        
    [<Fact>] 
    let ``Reference assemblies should be deterministic when private function is missing in one of them`` () =
        let src =
            """
module ReferenceAssembly

open System

let private privTest1 () = "Private Hello World!"

let test() =
    privTest1() |> ignore
    Console.WriteLine()
            """

        let src2 =
            """
module ReferenceAssembly

open System

let test() =
    Console.WriteLine()
            """

        let mvid1, mvid2 = calculateRefAssMvids src src2
        Assert.Equal(mvid1, mvid2)

    [<Fact>] 
    let ``Reference assemblies should be deterministic when inner function is removed`` () =
        let src =
            """
module ReferenceAssembly

open System

let test() =
    let innerFunc number = 
        string number

    let stringVal = innerFunc 15
    Console.WriteLine(stringVal)
            """
     
        let src2 =
            """
module ReferenceAssembly

open System

let test() =
    Console.WriteLine()
            """
       
        let mvid1, mvid2 = calculateRefAssMvids src src2
        Assert.Equal(mvid1, mvid2)

    [<Fact>] 
    let ``Reference assemblies should be same when contents of quoted expression change`` () =
        let src =
            """
module ReferenceAssembly

let foo () = <@ 2 + 2 @>
            """

        let src2 =
            """
module ReferenceAssembly

let foo () = <@ 2 + 3 @>
            """

        let mvid1, mvid2 = calculateRefAssMvids src src2
        Assert.Equal(mvid1, mvid2)

    [<Fact>]
    let ``Reference assemblies must change when a must-inline function changes body`` () =
        let codeBefore = """module ReferenceAssembly
let inline myFunc x y = x + y"""
        let codeAfter = codeBefore.Replace("+","-")
        let mvid1, mvid2 = calculateRefAssMvids codeBefore codeAfter
        Assert.NotEqual(mvid1,mvid2)

    [<Fact>]
    let ``Reference assemblies must not change when a must-inline function does not change`` () =
        let codeBefore = """module ReferenceAssembly
let inline myFunc x y = x - y"""       
        let mvid1, mvid2 = calculateRefAssMvids codeBefore codeBefore
        Assert.Equal(mvid1,mvid2)

    [<Theory>]
    [<InlineData(ivtSnippet, false)>] // If IVT provided -> MVID must reflect internal binding
    [<InlineData("", true )>] // No IVT => internal binding can be ignored for mvid purposes
    let ``Reference assemblies MVID when having internal binding``(additionalSnippet:string, shouldBeStable:bool) =
        let codeAfter = 
            basicCodeSnippet
                .Replace("private","internal")
                .Replace("//PLACEHOLDER", additionalSnippet)

        let mvid1, mvid2 = calculateRefAssMvids basicCodeSnippet codeAfter

        if shouldBeStable then
            Assert.Equal(mvid1,mvid2)
        else
            Assert.NotEqual(mvid1,mvid2)

    // https://github.com/dotnet/fsharp/issues/19732
    // Multi-file optimized compilation exercises DetupleArgs and TLR (tuple-arg
    // functions + nested lambdas); without source-order Val iteration their
    // output races under parallel optimization. The full race needs eng/test-determinism.ps1
    // in Release — this is a regression guard.
    [<Fact>]
    let ``Optimized multi-file assembly should be deterministic`` () =
        let outputDir = DirectoryInfo(Path.Combine(Path.GetTempPath(), $"fsharp-determinism-{Guid.NewGuid():N}"))
        outputDir.Create()

        let fileBody i = $"""
module File%d{i}

let processTuple%d{i} (a: int, b: string) =
    let inner x = x + a
    (inner 1, b.Length)

let callSite%d{i} () =
    let r1 = processTuple%d{i} (42, "hello")
    let r2 = processTuple%d{i} (99, "world")
    let nested () =
        let deep () = fst r1 + fst r2
        deep ()
    nested ()
"""

        let getMvid () =
            FSharp(fileBody 1)
            |> withAdditionalSourceFiles [ for i in 2..8 -> FsSourceWithFileName $"File%d{i}.fs" (fileBody i) ]
            |> asLibrary
            |> withOptimize
            |> withName "DetTest"
            |> withOutputDirectory (Some outputDir)
            |> withOptions [ "--deterministic" ]
            |> compileGuid

        try
            let mvids = [| for _ in 1..10 -> getMvid () |]

            for i in 1 .. mvids.Length - 1 do
                Assert.Equal(mvids.[0], mvids.[i])
        finally
            outputDir.Delete(true)

    [<Fact>]
    let ``Reference assemblies MVID must change when literal constant value changes`` () =
        let codeWithLiteral42 = """
module TestModule
[<Literal>]
let X = 42
"""

        let codeWithLiteral43 = """
module TestModule
[<Literal>]
let X = 43
"""

        let mvid1, mvid2 = calculateRefAssMvids codeWithLiteral42 codeWithLiteral43
        // Different literal values should produce different MVIDs
        Assert.NotEqual(mvid1, mvid2)

    // Guards stableValKey total ordering: same-arity overloads (f(int) vs f(string))
    // must serialize in deterministic order in p_ModuleInfo optimization data.
    [<Fact>]
    let ``Overloaded members produce deterministic MVID`` () =
        let outputDir = DirectoryInfo(Path.Combine(Path.GetTempPath(), $"fsharp-overload-{Guid.NewGuid():N}"))
        outputDir.Create()

        let libFile = """
module OverloadLib

type Processor() =
    member _.Handle(x: int) = x * 2
    member _.Handle(x: string) = x.Length
    member _.Handle(x: float) = int x
    member _.Handle(x: int, y: int) = x + y
"""

        let consumerFile = """
module Consumer

let run () =
    let p = OverloadLib.Processor()
    p.Handle(42) + p.Handle("hello") + p.Handle(3.14) + p.Handle(1, 2)
"""

        let getMvid () =
            FSharp(libFile)
            |> withAdditionalSourceFiles [ FsSourceWithFileName "Consumer.fs" consumerFile ]
            |> asLibrary
            |> withOptimize
            |> withName "OverloadTest"
            |> withOutputDirectory (Some outputDir)
            |> withOptions [ "--deterministic"; "--nowarn:75" ]
            |> compileGuid

        try
            Assert.Equal(getMvid (), getMvid ())
        finally
            outputDir.Delete(true)

    // https://github.com/dotnet/fsharp/issues/19928
    // The core SEQ=PAR invariant: sequential and parallel codegen must produce identical output.
    // Exercises cross-file inlined closures, byte arrays, and the full deferred drain pipeline.
    [<Fact>]
    let ``Sequential and parallel codegen produce identical MVID`` () =
        let outputDir = DirectoryInfo(Path.Combine(Path.GetTempPath(), $"fsharp-seqpar-{Guid.NewGuid():N}"))
        outputDir.Create()

        let libFile = """
module Lib

let inline withClosure f x = (fun y -> f y + x) 1

let data1 = [| 0uy .. 200uy |]
let data2 = [| 1us; 2us; 3us; 4us; 5us; 6us; 7us; 8us |]
"""

        let consumerFile i = $"""
module Consumer%d{i}

let result%d{i} () =
    let v = Lib.withClosure (fun y -> y * %d{i}) %d{i + 10}
    v + int Lib.data1.[%d{i}] + int Lib.data2.[%d{i % 8}]
"""

        let getMvid (parallelFlag: string) =
            FSharp(libFile)
            |> withAdditionalSourceFiles [ for i in 1..30 -> FsSourceWithFileName $"Consumer%d{i}.fs" (consumerFile i) ]
            |> asLibrary
            |> withOptimize
            |> withName "SeqParTest"
            |> withOutputDirectory (Some outputDir)
            |> withOptions [ "--deterministic"; "--nowarn:75"; parallelFlag ]
            |> compileGuid

        try
            let seqMvid = getMvid "--parallelcompilation-"
            let parMvid = getMvid "--parallelcompilation+"
            Assert.Equal(seqMvid, parMvid)
        finally
            outputDir.Delete(true)

    // bytesKey correctness: two distinct inline byte arrays in one function body get the same
    // range after remarkExpr inlining. Without bytesKey they would alias → wrong runtime values.
    [<Fact>]
    let ``Inline byte arrays with same range produce distinct correct values`` () =
        let libFile = """
module Lib
let inline twoArrays () = ([| 10uy; 20uy; 30uy; 40uy; 50uy; 60uy |], [| 99uy; 88uy; 77uy; 66uy; 55uy; 44uy |])
"""

        let mainFile = """
module Main
[<EntryPoint>]
let main _ =
    let (a, b) = Lib.twoArrays ()
    if a.[0] = 10uy && b.[0] = 99uy then
        printfn "OK"
        0
    else
        printfn "FAIL: %d %d" a.[0] b.[0]
        1
"""

        FSharp(libFile)
        |> withAdditionalSourceFiles [ FsSourceWithFileName "Main.fs" mainFile ]
        |> asExe
        |> withOptimize
        |> withOptions [ "--deterministic"; "--nowarn:75"; "--parallelcompilation+" ]
        |> compileAndRun
        |> shouldSucceed
        |> withStdOutContains "OK"
        |> ignore

    // Verify that compiled output actually runs correctly — not just that IL is identical.
    // Guards against the FSharpPlus-class runtime hang caused by dropped .cctor initialization.
    [<Theory>]
    [<InlineData("--parallelcompilation+")>]
    [<InlineData("--parallelcompilation-")>]
    let ``Deterministic multi-file compile produces correct runtime behavior`` (parallelFlag: string) =
        let mainFile = """
module Main

[<EntryPoint>]
let main _ =
    let r1 = ModuleA.valueA
    let r2 = ModuleB.valueB
    if r1 = 42 && r2 = 99 then
        printfn "OK"
        0
    else
        printfn "FAIL: %d %d" r1 r2
        1
"""

        let moduleA = """
module ModuleA

let mutable sideEffect = 0
do sideEffect <- 42
let valueA = sideEffect
"""

        let moduleB = """
module ModuleB

let mutable sideEffect = 0
do sideEffect <- 99
let valueB = sideEffect
"""

        FSharp(moduleA)
        |> withAdditionalSourceFiles [
            FsSourceWithFileName "ModuleB.fs" moduleB
            FsSourceWithFileName "Main.fs" mainFile
        ]
        |> asExe
        |> withOptimize
        |> withOptions [ "--deterministic"; "--nowarn:75"; parallelFlag ]
        |> compileAndRun
        |> shouldSucceed
        |> withStdOutContains "OK"
        |> ignore
