// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests.CodeGen.EmittedIL

open System.IO
open FSharp.Test
open FSharp.Test.Compiler
open NUnit.Framework

[<TestFixture>]
module DeterministicTests =

    let commonOptions = ["--refonly";"--deterministic"]//;"--nooptimizationdata"]
    let inputPath = CompilerAssert.GenerateFsInputPath()
    let outputPath = CompilerAssert.GenerateDllOutputPath()



    let getMvid codeSnippet compileOptions =
        File.WriteAllText(inputPath, codeSnippet)

        let mvid1 =
            FSharpWithInputAndOutputPath codeSnippet inputPath outputPath
            |> withOptions compileOptions
            |> compileGuid

        mvid1

    let calculateRefAssMvids referenceCodeSnippet codeAfterChangeIsDone = 

        let mvid1 = getMvid referenceCodeSnippet commonOptions
        let mvid2 = getMvid codeAfterChangeIsDone commonOptions

        mvid1 , mvid2

    [<Literal>]
    let basicCodeSnippet = """
module ReferenceAssembly

open System

let private privTest() =
    Console.WriteLine("Private Hello World!")

let test() =
    privTest()
    Console.WriteLine("Hello World!")"""


    [<Test>]
    let ``Simple assembly should be deterministic``() =  
        File.WriteAllText(inputPath, basicCodeSnippet)

        let getMvid() =
            FSharpWithInputAndOutputPath basicCodeSnippet inputPath outputPath
            |> withOptions ["--deterministic";"--nooptimizationdata"]
            |> compileGuid

        // Two identical compilations should produce the same MVID
        Assert.AreEqual(getMvid(), getMvid())

    [<Test>]
    let ``Simple assembly with different platform should not be deterministic``() = 
        let mvid1 = getMvid basicCodeSnippet ["--deterministic"]
        let mvid2 = getMvid basicCodeSnippet ["--deterministic";"--platform:Itanium"]
        // No two platforms should produce the same MVID
        Assert.AreNotEqual(mvid1, mvid2)

    [<Test>]
    let ``Simple reference assembly should be deterministic``() =
        let mvid1, mvid2 = calculateRefAssMvids basicCodeSnippet basicCodeSnippet
        Assert.AreEqual(mvid1, mvid2)

    [<Test>]
    let ``Simple reference assembly with different platform should not be deterministic``() =
        let mvid1 = getMvid basicCodeSnippet ["--refonly";"--deterministic"]
        let mvid2 = getMvid basicCodeSnippet ["--refonly";"--deterministic";"--platform:Itanium"]

        // No two platforms should produce the same MVID
        Assert.AreNotEqual(mvid1, mvid2)

    [<Test>]
    let ``False-positive reference assemblies test, different aseemblies' mvid should not match`` () =
        let src2 = basicCodeSnippet.Replace("test()","test2()")
        let mvid1, mvid2 = calculateRefAssMvids basicCodeSnippet src2
        Assert.AreNotEqual(mvid1, mvid2)

    [<Test>] 
    let ``Reference assemblies should be deterministic when only private function name is different with the same function name length`` () =
        let privCode1 = basicCodeSnippet.Replace("privTest()","privTest1()")
        let privCode2 = basicCodeSnippet.Replace("privTest()","privTest2()")
        let mvid1, mvid2 = calculateRefAssMvids privCode1 privCode2
        Assert.AreEqual(mvid1, mvid2)
    
    
    [<Test>] 
    let ``Reference assemblies should be deterministic when only private function name is different with the different function name length`` () =
        let src2 = basicCodeSnippet.Replace("privTest()","privTest11()")
        let mvid1, mvid2 = calculateRefAssMvids basicCodeSnippet src2
        Assert.AreEqual(mvid1, mvid2)
    
    [<Test>] 
    let ``Reference assemblies should be deterministic when only private function body is different`` () =
        let src2 = basicCodeSnippet.Replace("""Console.WriteLine("Private Hello World!")""","""Console.Write("Private Hello World!")""")
        let mvid1, mvid2 = calculateRefAssMvids basicCodeSnippet src2
        Assert.AreEqual(mvid1, mvid2)
        
    [<Test>] 
    let ``Reference assemblies should be deterministic when only private function return type is different`` () =
        let src =
            """
module ReferenceAssembly

open System

let private privTest1() : string = "Private Hello World!"

let test() =
    privTest1() |> ignore
    Console.WriteLine()            """


        let src2 =
            """
module ReferenceAssembly

open System

let private privTest1() : int = 0

let test() =
    privTest1() |> ignore
    Console.WriteLine()
            """
        let mvid1, mvid2 = calculateRefAssMvids src src2
        Assert.AreEqual(mvid1, mvid2)
     
    [<Test>] 
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
        Assert.AreEqual(mvid1, mvid2)

    [<Test>] 
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
        Assert.AreEqual(mvid1, mvid2)
     
    [<Test>] 
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
        Assert.AreEqual(mvid1, mvid2)
        
    [<Test>] 
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
        Assert.AreEqual(mvid1, mvid2)

    [<Test>] 
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
        Assert.AreEqual(mvid1, mvid2)

    [<Test>] 
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
        Assert.AreEqual(mvid1, mvid2)

    [<Test>]
    let ``Reference assemblies must change when a must-inline function changes body`` () =
        let codeBefore = """module ReferenceAssembly
let inline myFunc x y = x + y"""
        let codeAfter = codeBefore.Replace("+","-")
        let mvid1, mvid2 = calculateRefAssMvids codeBefore codeAfter
        Assert.AreNotEqual(mvid1,mvid2)

    [<Test>]
    let ``Reference assemblies must not change when a must-inline function does not change`` () =
        let codeBefore = """module ReferenceAssembly
let inline myFunc x y = x - y"""       
        let mvid1, mvid2 = calculateRefAssMvids codeBefore codeBefore
        Assert.AreEqual(mvid1,mvid2)
