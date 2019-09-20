// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.Scripting.UnitTests

open System
open System.IO
open System.Threading
open FSharp.Compiler.Interactive.Shell
open FSharp.Compiler.Scripting
open FSharp.Compiler.SourceCodeServices
open NUnit.Framework

[<TestFixture>]
type InteractiveTests() =

    let getValue ((value: Result<FsiValue option, exn>), (errors: FSharpErrorInfo[])) =
        if errors.Length > 0 then
            failwith <| sprintf "Evaluation returned %d errors:\r\n\t%s" errors.Length (String.Join("\r\n\t", errors))
        match value with
        | Ok(value) -> value
        | Error ex -> raise ex

    let ignoreValue = getValue >> ignore

    [<Test>]
    member __.``Eval object value``() =
        use script = new FSharpScript()
        let opt = script.Eval("1+1") |> getValue
        let value = opt.Value
        Assert.AreEqual(typeof<int>, value.ReflectionType)
        Assert.AreEqual(2, value.ReflectionValue :?> int)

    [<Test>]
    member __.``Capture console input``() =
        use script = new FSharpScript(captureInput=true)
        script.ProvideInput "stdin:1234\r\n"
        let opt = script.Eval("System.Console.ReadLine()") |> getValue
        let value = opt.Value
        Assert.AreEqual(typeof<string>, value.ReflectionType)
        Assert.AreEqual("stdin:1234", value.ReflectionValue)

    [<Test>]
    member __.``Capture console output/error``() =
        use script = new FSharpScript(captureOutput=true)
        use sawOutputSentinel = new ManualResetEvent(false)
        use sawErrorSentinel = new ManualResetEvent(false)
        script.OutputProduced.Add (fun line -> if line = "stdout:1234" then sawOutputSentinel.Set() |> ignore)
        script.ErrorProduced.Add (fun line -> if line = "stderr:5678" then sawErrorSentinel.Set() |> ignore)
        script.Eval("printfn \"stdout:1234\"; eprintfn \"stderr:5678\"") |> ignoreValue
        Assert.True(sawOutputSentinel.WaitOne(TimeSpan.FromSeconds(5.0)), "Expected to see output sentinel value written")
        Assert.True(sawErrorSentinel.WaitOne(TimeSpan.FromSeconds(5.0)), "Expected to see error sentinel value written")

    [<Test>]
    member __.``Maintain state between submissions``() =
        use script = new FSharpScript()
        script.Eval("let add x y = x + y") |> ignoreValue
        let opt = script.Eval("add 2 3") |> getValue
        let value = opt.Value
        Assert.AreEqual(typeof<int>, value.ReflectionType)
        Assert.AreEqual(5, value.ReflectionValue :?> int)

    [<Test>]
    member __.``Assembly reference event successful``() =
        use script = new FSharpScript()
        let testAssembly = "System.dll"
        let mutable assemblyResolveEventCount = 0
        let mutable foundAssemblyReference = false
        Event.add (fun (assembly: string) ->
            assemblyResolveEventCount <- assemblyResolveEventCount + 1
            foundAssemblyReference <- String.Compare(testAssembly, Path.GetFileName(assembly), StringComparison.OrdinalIgnoreCase) = 0)
            script.AssemblyReferenceAdded
        script.Eval(sprintf "#r \"%s\"" testAssembly) |> ignoreValue
        Assert.AreEqual(1, assemblyResolveEventCount)
        Assert.True(foundAssemblyReference)

    [<Test>]
    member __.``Assembly reference event unsuccessful``() =
        use script = new FSharpScript()
        let testAssembly = "not-an-assembly-that-can-be-found.dll"
        let mutable foundAssemblyReference = false
        Event.add (fun _ -> foundAssemblyReference <- true) script.AssemblyReferenceAdded
        let _result, errors = script.Eval(sprintf "#r \"%s\"" testAssembly)
        Assert.AreEqual(1, errors.Length)
        Assert.False(foundAssemblyReference)

    [<Test>]
    member __.``Assembly reference dependent assemblies``() =
        let fsxText = """
#r @"DependentAssemblyLocation/DependentAssembly.dll"
#r @"TopAssemblyLocation/TopAssembly.dll"
open TopAssembly
let msg = (new TopAssemblyClass()).GetTheString("from Test-case : 'Assembly reference dependent assemblies'")
"""
        use script = new FSharpScript()
        let mutable assemblyResolveEventCount = 0
        Event.add (fun (assembly: string) ->
            assemblyResolveEventCount <- assemblyResolveEventCount + 1)
            script.AssemblyReferenceAdded
        script.Eval(fsxText) |> ignore
        let opt = script.Eval("msg") |> getValue
        let value = opt.Value.ReflectionValue
        Assert.AreEqual(2, assemblyResolveEventCount)
        Assert.AreEqual(value, "Hello from Test-case : 'Assembly reference dependent assemblies'")


