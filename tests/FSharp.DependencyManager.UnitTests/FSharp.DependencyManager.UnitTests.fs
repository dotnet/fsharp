// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.DependencyManager.UnitTests

open System
open System.IO
open System.Threading
open FSharp.Compiler.Interactive.Shell
open FSharp.Compiler.SourceCodeServices

open NUnit.Framework

[<TestFixture>]
type DependencyManagerInteractiveTests() =

    let getValue ((value: Result<FsiValue option, exn>), (errors: FSharpErrorInfo[])) =
        if errors.Length > 0 then
            failwith <| sprintf "Evaluation returned %d errors:\r\n\t%s" errors.Length (String.Join("\r\n\t", errors))
        match value with
        | Ok(value) -> value
        | Error ex -> raise ex

    let ignoreValue = getValue >> ignore

    [<Test>]
    member __.``SmokeTest - #r nuget``() =
        let text = """
#r @"nuget:System.Collections.Immutable, version=1.5.0"
0"""
        use script = new FSharpScript()
        let mutable assemblyResolveEventCount = 0
        let mutable foundAssemblyReference = false
        Event.add (fun (assembly: string) ->
            assemblyResolveEventCount <- assemblyResolveEventCount + 1
            foundAssemblyReference <- String.Compare("System.Collections.Immutable.dll", Path.GetFileName(assembly), StringComparison.OrdinalIgnoreCase) = 0)
            script.AssemblyReferenceAdded
        let opt = script.Eval(text) |> getValue
        let value = opt.Value
        Assert.AreEqual(typeof<int>, value.ReflectionType)
        Assert.AreEqual(0, value.ReflectionValue :?> int)
        Assert.AreEqual(1, assemblyResolveEventCount)
        Assert.AreEqual(true, foundAssemblyReference)

    [<Test>]
    member __.``SmokeTest - #r nuget package not found``() =
        let text = """
#r @"nuget:System.Collections.Immutable.DoesNotExist, version=1.5.0"
0"""
        use script = new FSharpScript()
        let mutable assemblyResolveEventCount = 0
        Event.add (fun (assembly: string) ->
            assemblyResolveEventCount <- assemblyResolveEventCount + 1)
            script.AssemblyReferenceAdded
        let opt = script.Eval(text) |> getValue
        let value = opt.Value
        Assert.AreEqual(typeof<int>, value.ReflectionType)
        Assert.AreEqual(0, value.ReflectionValue :?> int)
        Assert.AreEqual(0, assemblyResolveEventCount)
