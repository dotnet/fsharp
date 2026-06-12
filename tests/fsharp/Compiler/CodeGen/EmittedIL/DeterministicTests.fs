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
            Assert.Equal(getMvid (), getMvid ())
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
