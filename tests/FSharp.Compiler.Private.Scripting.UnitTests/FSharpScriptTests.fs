// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.Scripting.UnitTests

open System
open System.Diagnostics
open System.IO
open System.Reflection
open System.Threading
open System.Threading.Tasks
open FSharp.Compiler.Interactive.Shell
open FSharp.Compiler.Scripting
open NUnit.Framework

[<TestFixture>]
type InteractiveTests() =

    [<Test>]
    member __.``Eval object value``() =
        use script = new FSharpScript()
        let opt = script.Eval("1+1") |> getValue
        let value = opt.Value
        Assert.AreEqual(typeof<int>, value.ReflectionType)
        Assert.AreEqual(2, value.ReflectionValue :?> int)

    [<Test>]
    member __.``Declare and eval object value``() =
        use script = new FSharpScript()
        let opt = script.Eval("let x = 1 + 2\r\nx") |> getValue
        let value = opt.Value
        Assert.AreEqual(typeof<int>, value.ReflectionType)
        Assert.AreEqual(3, value.ReflectionValue :?> int)

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
    member __.``Add include path event successful``() =
        use script = new FSharpScript()
        let includePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
        let mutable includePathEventCount = 0
        let mutable foundIncludePath = false
        Event.add (fun (inc: string) ->
            includePathEventCount <- includePathEventCount + 1
            foundIncludePath <- foundIncludePath || String.Compare(includePath, inc, StringComparison.OrdinalIgnoreCase) = 0)
            script.IncludePathAdded
        script.Eval(sprintf "#I @\"%s\"" includePath) |> ignoreValue
        Assert.AreEqual(1, includePathEventCount)
        Assert.True(foundIncludePath)

    [<Test>]
    member __.``Add include path event unsuccessful``() =
        use script = new FSharpScript()
        let includePath = Path.Combine("a", "path", "that", "can't", "be", "found")
        let mutable foundIncludePath = false
        Event.add (fun _ -> foundIncludePath <- true) script.IncludePathAdded
        let _result, errors = script.Eval(sprintf "#I @\"%s\"" includePath)
        Assert.AreEqual(1, errors.Length)
        Assert.False(foundIncludePath)

    [<Test>]
    member _.``Compilation errors report a specific exception``() =
        use script = new FSharpScript()
        let result, _errors = script.Eval("abc")
        match result with
        | Ok(_) -> Assert.Fail("expected a failure")
        | Error(ex) -> Assert.IsInstanceOf<FsiCompilationException>(ex)

    [<Test>]
    member _.``Runtime exceptions are propagated``() =
        use script = new FSharpScript()
        let result, errors = script.Eval("System.IO.File.ReadAllText(\"not-a-file-path-that-can-be-found-on-disk.txt\")")
        Assert.IsEmpty(errors)
        match result with
        | Ok(_) -> Assert.Fail("expected a failure")
        | Error(ex) -> Assert.IsInstanceOf<FileNotFoundException>(ex)

    [<Test>]
    member __.``Nuget reference fires multiple events``() =
        use script = new FSharpScript(additionalArgs=[|"/langversion:preview"|])
        let mutable assemblyRefCount = 0
        let mutable includeAddCount = 0
        Event.add (fun _ -> assemblyRefCount <- assemblyRefCount + 1) script.AssemblyReferenceAdded
        Event.add (fun _ -> includeAddCount <- includeAddCount + 1) script.IncludePathAdded
        script.Eval("#r \"nuget:include=NUnitLite, version=3.11.0\"") |> ignoreValue
        script.Eval("0") |> ignoreValue
        Assert.GreaterOrEqual(assemblyRefCount, 2)
        Assert.GreaterOrEqual(includeAddCount, 1)

/// Native dll resolution is not implemented on desktop
#if NETSTANDARD
    [<Test>]
    member __.``ML - use assembly with native dependencies``() =
        let code = @"
#r ""nuget:RestoreSources=https://dotnet.myget.org/F/dotnet-corefxlab/api/v3/index.json""
#r ""nuget:Microsoft.ML,version=1.4.0-preview""
#r ""nuget:Microsoft.ML.AutoML,version=0.16.0-preview""
#r ""nuget:Microsoft.Data.DataFrame,version=0.1.1-e191008-1""

open System
open System.IO
open System.Linq
open Microsoft.Data

let Shuffle (arr:int[]) =
    let rnd = Random()
    for i in 0 .. arr.Length - 1 do
        let r = i + rnd.Next(arr.Length - i)
        let temp = arr.[r]
        arr.[r] <- arr.[i]
        arr.[i] <- temp
    arr

let housingPath = ""housing.csv""
let housingData = DataFrame.ReadCsv(housingPath)
let randomIndices = (Shuffle(Enumerable.Range(0, (int (housingData.RowCount) - 1)).ToArray()))
let testSize = int (float (housingData.RowCount) * 0.1)
let trainRows = randomIndices.[testSize..]
let testRows = randomIndices.[..testSize]
let housing_train = housingData.[trainRows]

open Microsoft.ML
open Microsoft.ML.Data
open Microsoft.ML.AutoML

let mlContext = MLContext()
let experiment = mlContext.Auto().CreateRegressionExperiment(maxExperimentTimeInSeconds = 15u)
let result = experiment.Execute(housing_train, labelColumnName = ""median_house_value"")
let details = result.RunDetails
printfn ""%A"" result
123
"
        use script = new FSharpScript(additionalArgs=[|"/langversion:preview"|])
        let mutable assemblyRefCount = 0;
        Event.add (fun _ -> assemblyRefCount <- assemblyRefCount + 1) script.AssemblyReferenceAdded
        let opt = script.Eval(code)  |> getValue
        let value = opt.Value
        Assert.AreEqual(123, value.ReflectionValue :?> int32)
#endif

    [<Test>]
    member __.``Simple pinvoke should not be impacted by native resolver``() =
        let code = @"
open System
open System.Runtime.InteropServices

module Imports =
    [<DllImport(""kernel32.dll"")>]
    extern uint32 GetCurrentProcessId()

    [<DllImport(""c"")>]
    extern uint32 getpid()

// Will throw exception if fails
if RuntimeInformation.IsOSPlatform(OSPlatform.Windows) then
    printfn ""Current process: %d"" (Imports.GetCurrentProcessId())
else
    printfn ""Current process: %d"" (Imports.getpid())
123
"
        use script = new FSharpScript(additionalArgs=[|"/langversion:preview"|])
        let mutable assemblyRefCount = 0;
        let opt = script.Eval(code)  |> getValue
        let value = opt.Value
        Assert.AreEqual(123, value.ReflectionValue :?> int32)

    [<Test; Ignore("This timing test fails in different environments. Skipping so that we don't assume an arbitrary CI environment has enough compute/etc. for what we need here.")>]
    member _.``Evaluation can be cancelled``() =
        use script = new FSharpScript()
        let sleepTime = 10000
        let mutable result = None
        let mutable wasCancelled = false
        use tokenSource = new CancellationTokenSource()
        let eval () =
            try
                result <- Some(script.Eval(sprintf "System.Threading.Thread.Sleep(%d)\n2" sleepTime, tokenSource.Token))
                // if execution gets here (which it shouldn't), the value `2` will be returned
            with
            | :? OperationCanceledException -> wasCancelled <- true
        let sw = Stopwatch.StartNew()
        let evalTask = Task.Run(eval)
        // cancel and wait for finish
        tokenSource.Cancel()
        evalTask.GetAwaiter().GetResult()
        // ensure we cancelled and didn't complete the sleep or evaluation
        Assert.True(wasCancelled)
        Assert.LessOrEqual(sw.ElapsedMilliseconds, sleepTime)
        Assert.AreEqual(None, result)

    [<Test>]
    member _.``Values bound at the root trigger an event``() =
        let mutable foundX = false
        let mutable foundY = false
        let mutable foundCount = 0
        use script = new FSharpScript()
        script.ValueBound
        |> Event.add (fun (value, typ, name) ->
            foundX <- foundX || (name = "x" && typ = typeof<int> && value :?> int = 1)
            foundY <- foundY || (name = "y" && typ = typeof<int> && value :?> int = 2)
            foundCount <- foundCount + 1)
        let code = @"
let x = 1
let y = 2
"
        script.Eval(code) |> ignoreValue
        Assert.True(foundX)
        Assert.True(foundY)
        Assert.AreEqual(2, foundCount)

    [<Test>]
    member _.``Values re-bound trigger an event``() =
        let mutable foundXCount = 0
        use script = new FSharpScript()
        script.ValueBound
        |> Event.add (fun (_value, typ, name) ->
            if name = "x" && typ = typeof<int> then foundXCount <- foundXCount + 1)
        script.Eval("let x = 1") |> ignoreValue
        script.Eval("let x = 2") |> ignoreValue
        Assert.AreEqual(2, foundXCount)

    [<Test>]
    member _.``Nested let bindings don't trigger event``() =
        let mutable foundInner = false
        use script = new FSharpScript()
        script.ValueBound
        |> Event.add (fun (_value, _typ, name) ->
            foundInner <- foundInner || name = "inner")
        let code = @"
let x =
    let inner = 1
    ()
"
        script.Eval(code) |> ignoreValue
        Assert.False(foundInner)
