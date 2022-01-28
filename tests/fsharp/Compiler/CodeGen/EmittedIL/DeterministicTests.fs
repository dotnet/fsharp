// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests.CodeGen.EmittedIL

open System.IO
open FSharp.Test
open FSharp.Test.Compiler
open NUnit.Framework

[<TestFixture>]
module DeterministicTests =

    [<Test>]
    let ``Simple assembly should be deterministic``() =
        let inputFilePath = CompilerAssert.GenerateFsInputPath()
        let outputFilePath = CompilerAssert.GenerateDllOutputPath()
        let src =
            """
module Assembly

open System

let test() =
    Console.WriteLine("Hello World!")
            """

        File.WriteAllText(inputFilePath, src)

        let mvid1 =
            FSharpWithInputAndOutputPath inputFilePath outputFilePath
            |> withOptions ["--deterministic"]
            |> compileGuid
        let mvid2 =
            FSharpWithInputAndOutputPath inputFilePath outputFilePath
            |> withOptions ["--deterministic"]
            |> compileGuid

        // Two identical compilations should produce the same MVID
        Assert.AreEqual(mvid1, mvid2)

    [<Test>]
    let ``Simple assembly with different platform should not be deterministic``() =
        let inputFilePath = CompilerAssert.GenerateFsInputPath()
        let outputFilePath = CompilerAssert.GenerateDllOutputPath()
        let src =
            """
module Assembly

open System

let test() =
    Console.WriteLine("Hello World!")
            """

        File.WriteAllText(inputFilePath, src)

        let mvid1 =
            FSharpWithInputAndOutputPath inputFilePath outputFilePath
            |> withOptions ["--deterministic"]
            |> compileGuid
        let mvid2 =
            FSharpWithInputAndOutputPath inputFilePath outputFilePath
            |> withOptions ["--deterministic";"--platform:Itanium"]
            |> compileGuid

        // No two platforms should produce the same MVID
        Assert.AreNotEqual(mvid1, mvid2)

    [<Test>]
    let ``Simple reference assembly should be deterministic``() =
        let inputFilePath = CompilerAssert.GenerateFsInputPath()
        let outputFilePath = CompilerAssert.GenerateDllOutputPath()
        let src =
            """
module ReferenceAssembly

open System

let private privTest() =
    Console.WriteLine("Private Hello World!")

let test() =
    privTest()
    Console.WriteLine("Hello World!")
            """

        File.WriteAllText(inputFilePath, src)

        let mvid1 =
            FSharpWithInputAndOutputPath inputFilePath outputFilePath
            |> withOptions ["--refonly";"--deterministic"]
            |> compileGuid
        let mvid2 =
            FSharpWithInputAndOutputPath inputFilePath outputFilePath
            |> withOptions ["--refonly";"--deterministic"]
            |> compileGuid

        // Two identical compilations should produce the same MVID
        Assert.AreEqual(mvid1, mvid2)

    [<Test>]
    let ``Simple reference assembly with different platform should not be deterministic``() =
        let inputFilePath = CompilerAssert.GenerateFsInputPath()
        let outputFilePath = CompilerAssert.GenerateDllOutputPath()
        let src =
            """
module ReferenceAssembly

open System

let private privTest() =
    Console.WriteLine("Private Hello World!")

let test() =
    privTest()
    Console.WriteLine("Hello World!")
            """

        File.WriteAllText(inputFilePath, src)

        let mvid1 =
            FSharpWithInputAndOutputPath inputFilePath outputFilePath
            |> withOptions ["--refonly";"--deterministic"]
            |> compileGuid
        let mvid2 =
            FSharpWithInputAndOutputPath inputFilePath outputFilePath
            |> withOptions ["--refonly";"--deterministic";"--platform:Itanium"]
            |> compileGuid

        // No two platforms should produce the same MVID
        Assert.AreNotEqual(mvid1, mvid2)


    [<Test>]
    let ``False-positive reference assemblies test, different aseemblies' mvid should not match`` () =
        let inputFilePath = CompilerAssert.GenerateFsInputPath()
        let outputFilePath = CompilerAssert.GenerateDllOutputPath()
        let src =
            """
module ReferenceAssembly

open System

let test() =
    Console.WriteLine("Hello World!")
            """

        File.WriteAllText(inputFilePath, src)

        let mvid1 =
            FSharpWithInputAndOutputPath inputFilePath outputFilePath
            |> withOptions ["--refonly";"--deterministic"]
            |> compileGuid


        let inputFilePath2 = CompilerAssert.GenerateFsInputPath()
        let outputFilePath2 = CompilerAssert.GenerateDllOutputPath()
        let src2 =
            """
module ReferenceAssembly

open System

let test2() =
    Console.WriteLine("Hello World!")
            """

        File.WriteAllText(inputFilePath2, src2)

        let mvid2 =
            FSharpWithInputAndOutputPath inputFilePath2 outputFilePath2
            |> withOptions ["--refonly";"--deterministic"]
            |> compileGuid

        // Two different compilations should _not_ produce the same MVID
        Assert.AreNotEqual(mvid1, mvid2)

    [<Test; Ignore("TEMP: skip until sigdata cleanup work is done.")>]
    let ``Reference assemblies should be deterministic when only private function name is different with the same function name length`` () =
        let inputFilePath = CompilerAssert.GenerateFsInputPath()
        let outputFilePath = CompilerAssert.GenerateDllOutputPath()
        let src =
            """
module ReferenceAssembly

open System

let private privTest1() =
    Console.WriteLine("Private Hello World!")

let test() =
    privTest1()
    Console.WriteLine("Hello World!")
            """

        File.WriteAllText(inputFilePath, src)

        let mvid1 =
            FSharpWithInputAndOutputPath inputFilePath outputFilePath
            |> withOptions ["--refonly";"--deterministic"]
            |> compileGuid


        let inputFilePath2 = CompilerAssert.GenerateFsInputPath()
        let outputFilePath2 = CompilerAssert.GenerateDllOutputPath()
        let src2 =
            """
module ReferenceAssembly

open System

let private privTest2() =
    Console.WriteLine("Private Hello World!")

let test() =
    privTest2()
    Console.WriteLine("Hello World!")
            """

        File.WriteAllText(inputFilePath2, src2)

        let mvid2 =
            FSharpWithInputAndOutputPath inputFilePath2 outputFilePath2
            |> withOptions ["--refonly";"--deterministic"]
            |> compileGuid

        // Two compilations with changes only to private code should produce the same MVID
        Assert.AreEqual(mvid1, mvid2)
    
    
    [<Test; Ignore("TEMP: skip until sigdata cleanup work is done.")>]
    let ``Reference assemblies should be deterministic when only private function name is different with the different function name length`` () =
        let inputFilePath = CompilerAssert.GenerateFsInputPath()
        let outputFilePath = CompilerAssert.GenerateDllOutputPath()
        let src =
            """
module ReferenceAssembly

open System

let private privTest1() =
    Console.WriteLine("Private Hello World!")

let test() =
    privTest1()
    Console.WriteLine("Hello World!")
            """

        File.WriteAllText(inputFilePath, src)

        let mvid1 =
            FSharpWithInputAndOutputPath inputFilePath outputFilePath
            |> withOptions ["--refonly";"--deterministic"]
            |> compileGuid


        let inputFilePath2 = CompilerAssert.GenerateFsInputPath()
        let outputFilePath2 = CompilerAssert.GenerateDllOutputPath()
        let src2 =
            """
module ReferenceAssembly

open System

let private privTest11() =
    Console.WriteLine("Private Hello World!")

let test() =
    privTest11()
    Console.WriteLine("Hello World!")
            """

        File.WriteAllText(inputFilePath2, src2)

        let mvid2 =
            FSharpWithInputAndOutputPath inputFilePath2 outputFilePath2
            |> withOptions ["--refonly";"--deterministic"]
            |> compileGuid

        // Two compilations with changes only to private code should produce the same MVID
        Assert.AreEqual(mvid1, mvid2)
    
    [<Test; Ignore("TEMP: skip until sigdata cleanup work is done.")>]
    let ``Reference assemblies should be deterministic when only private function body is different`` () =
        let inputFilePath = CompilerAssert.GenerateFsInputPath()
        let outputFilePath = CompilerAssert.GenerateDllOutputPath()
        let src =
            """
module ReferenceAssembly

open System

let private privTest1() =
    Console.WriteLine("Private Hello World!")

let test() =
    privTest1()
    Console.WriteLine("Hello World!")
            """

        File.WriteAllText(inputFilePath, src)

        let mvid1 =
            FSharpWithInputAndOutputPath inputFilePath outputFilePath
            |> withOptions ["--refonly";"--deterministic"]
            |> compileGuid


        let inputFilePath2 = CompilerAssert.GenerateFsInputPath()
        let outputFilePath2 = CompilerAssert.GenerateDllOutputPath()
        let src2 =
            """
module ReferenceAssembly

open System

let private privTest1() =
    Console.Write("Private Hello World!")

let test() =
    privTest1()
    Console.WriteLine("Hello World!")
            """

        File.WriteAllText(inputFilePath2, src2)

        let mvid2 =
            FSharpWithInputAndOutputPath inputFilePath2 outputFilePath2
            |> withOptions ["--refonly";"--deterministic"]
            |> compileGuid

        // Two compilations with changes only to private code should produce the same MVID
        Assert.AreEqual(mvid1, mvid2)
        
    [<Test; Ignore("TEMP: skip until sigdata cleanup work is done.")>]
    let ``Reference assemblies should be deterministic when only private function return type is different`` () =
        let inputFilePath = CompilerAssert.GenerateFsInputPath()
        let outputFilePath = CompilerAssert.GenerateDllOutputPath()
        let src =
            """
module ReferenceAssembly

open System

let private privTest1() : string = "Private Hello World!"

let test() =
    privTest1() |> ignore
    Console.WriteLine()
            """

        File.WriteAllText(inputFilePath, src)

        let mvid1 =
            FSharpWithInputAndOutputPath inputFilePath outputFilePath
            |> withOptions ["--refonly";"--deterministic"]
            |> compileGuid


        let inputFilePath2 = CompilerAssert.GenerateFsInputPath()
        let outputFilePath2 = CompilerAssert.GenerateDllOutputPath()
        let src2 =
            """
module ReferenceAssembly

open System

let private privTest1() : int = 0

let test() =
    privTest1() |> ignore
    Console.WriteLine()
            """

        File.WriteAllText(inputFilePath2, src2)

        let mvid2 =
            FSharpWithInputAndOutputPath inputFilePath2 outputFilePath2
            |> withOptions ["--refonly";"--deterministic"]
            |> compileGuid

        // Two compilations with changes only to private code should produce the same MVID
        Assert.AreEqual(mvid1, mvid2)
     
    [<Test; Ignore("TEMP: skip until sigdata cleanup work is done.")>]
    let ``Reference assemblies should be deterministic when only private function parameter count is different`` () =
        let inputFilePath = CompilerAssert.GenerateFsInputPath()
        let outputFilePath = CompilerAssert.GenerateDllOutputPath()
        let src =
            """
module ReferenceAssembly

open System

let private privTest1 () : string = "Private Hello World!"

let test() =
    privTest1 () |> ignore
    Console.WriteLine()
            """

        File.WriteAllText(inputFilePath, src)

        let mvid1 =
            FSharpWithInputAndOutputPath inputFilePath outputFilePath
            |> withOptions ["--refonly";"--deterministic"]
            |> compileGuid


        let inputFilePath2 = CompilerAssert.GenerateFsInputPath()
        let outputFilePath2 = CompilerAssert.GenerateDllOutputPath()
        let src2 =
            """
module ReferenceAssembly

open System

let private privTest1 () () : string = "Private Hello World!"

let test() =
    privTest1 () () |> ignore
    Console.WriteLine()
            """

        File.WriteAllText(inputFilePath2, src2)

        let mvid2 =
            FSharpWithInputAndOutputPath inputFilePath2 outputFilePath2
            |> withOptions ["--refonly";"--deterministic"]
            |> compileGuid

        // Two compilations with changes only to private code should produce the same MVID
        Assert.AreEqual(mvid1, mvid2)


    [<Test; Ignore("TEMP: skip until sigdata cleanup work is done.")>]
    let ``Reference assemblies should be deterministic when only private function parameter count is different and private function is unused`` () =
        let inputFilePath = CompilerAssert.GenerateFsInputPath()
        let outputFilePath = CompilerAssert.GenerateDllOutputPath()
        let src =
            """
module ReferenceAssembly

open System

let private privTest1 () : string = "Private Hello World!"

let test() =
    Console.WriteLine()
            """

        File.WriteAllText(inputFilePath, src)

        let mvid1 =
            FSharpWithInputAndOutputPath inputFilePath outputFilePath
            |> withOptions ["--refonly";"--deterministic"]
            |> compileGuid


        let inputFilePath2 = CompilerAssert.GenerateFsInputPath()
        let outputFilePath2 = CompilerAssert.GenerateDllOutputPath()
        let src2 =
            """
module ReferenceAssembly

open System

let private privTest1 () () : string = "Private Hello World!"

let test() =
    Console.WriteLine()
            """

        File.WriteAllText(inputFilePath2, src2)

        let mvid2 =
            FSharpWithInputAndOutputPath inputFilePath2 outputFilePath2
            |> withOptions ["--refonly";"--deterministic"]
            |> compileGuid

        // Two compilations with changes only to private code should produce the same MVID
        Assert.AreEqual(mvid1, mvid2)
     
    [<Test; Ignore("TEMP: skip until sigdata cleanup work is done.")>]
    let ``Reference assemblies should be deterministic when only private function parameter types are different`` () =
        let inputFilePath = CompilerAssert.GenerateFsInputPath()
        let outputFilePath = CompilerAssert.GenerateDllOutputPath()
        let src =
            """
module ReferenceAssembly

open System

let private privTest1 () = "Private Hello World!"

let test() =
    privTest1() |> ignore
    Console.WriteLine()
            """

        File.WriteAllText(inputFilePath, src)

        let mvid1 =
            FSharpWithInputAndOutputPath inputFilePath outputFilePath
            |> withOptions ["--refonly";"--deterministic"]
            |> compileGuid


        let inputFilePath2 = CompilerAssert.GenerateFsInputPath()
        let outputFilePath2 = CompilerAssert.GenerateDllOutputPath()
        let src2 =
            """
module ReferenceAssembly

open System

let private privTest1 (_: string) = "Private Hello World!"

let test() =
    privTest1 "" |> ignore
    Console.WriteLine()
            """

        File.WriteAllText(inputFilePath2, src2)

        let mvid2 =
            FSharpWithInputAndOutputPath inputFilePath2 outputFilePath2
            |> withOptions ["--refonly";"--deterministic"]
            |> compileGuid

        // Two compilations with changes only to private code should produce the same MVID
        Assert.AreEqual(mvid1, mvid2)
        
    [<Test; Ignore("TEMP: skip until sigdata cleanup work is done.")>]
    let ``Reference assemblies should be deterministic when private function is missing in one of them`` () =
        let inputFilePath = CompilerAssert.GenerateFsInputPath()
        let outputFilePath = CompilerAssert.GenerateDllOutputPath()
        let src =
            """
module ReferenceAssembly

open System

let private privTest1 () = "Private Hello World!"

let test() =
    privTest1() |> ignore
    Console.WriteLine()
            """

        File.WriteAllText(inputFilePath, src)

        let mvid1 =
            FSharpWithInputAndOutputPath inputFilePath outputFilePath
            |> withOptions ["--refonly";"--deterministic"]
            |> compileGuid


        let inputFilePath2 = CompilerAssert.GenerateFsInputPath()
        let outputFilePath2 = CompilerAssert.GenerateDllOutputPath()
        let src2 =
            """
module ReferenceAssembly

open System

let test() =
    Console.WriteLine()
            """

        File.WriteAllText(inputFilePath2, src2)

        let mvid2 =
            FSharpWithInputAndOutputPath inputFilePath2 outputFilePath2
            |> withOptions ["--refonly";"--deterministic"]
            |> compileGuid

        // Two compilations with changes only to private code should produce the same MVID
        Assert.AreEqual(mvid1, mvid2)

    // TODO: Add tests for Internal types (+IVT), (private, internal, public) fields, properties, events.