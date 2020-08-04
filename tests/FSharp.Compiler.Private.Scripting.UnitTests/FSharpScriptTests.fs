// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.Scripting.UnitTests

open System
open System.Diagnostics
open System.IO
open System.Reflection
open System.Runtime.InteropServices
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
        use input = new RedirectConsoleInput()
        use script = new FSharpScript()
        input.ProvideInput "stdin:1234\r\n"
        let opt = script.Eval("System.Console.ReadLine()") |> getValue
        let value = opt.Value
        Assert.AreEqual(typeof<string>, value.ReflectionType)
        Assert.AreEqual("stdin:1234", value.ReflectionValue)

    [<Test>]
    member __.``Capture console output/error``() =
        use output = new RedirectConsoleOutput()
        use script = new FSharpScript()
        use sawOutputSentinel = new ManualResetEvent(false)
        use sawErrorSentinel = new ManualResetEvent(false)
        output.OutputProduced.Add (fun line -> if line = "stdout:1234" then sawOutputSentinel.Set() |> ignore)
        output.ErrorProduced.Add (fun line -> if line = "stderr:5678" then sawErrorSentinel.Set() |> ignore)
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
        let testCode = """
#r "System.dll"
let stacktype= typeof<System.Collections.Stack>
stacktype.Name = "Stack"
"""
        let opt = script.Eval(testCode) |> getValue
        let value = opt.Value
        Assert.AreEqual(true, value.ReflectionValue :?> bool)

    [<Test>]
    member __.``Assembly reference unsuccessful``() =
        use script = new FSharpScript()
        let testAssembly = "not-an-assembly-that-can-be-found.dll"
        let _result, errors = script.Eval(sprintf "#r \"%s\"" testAssembly)
        Assert.AreEqual(1, errors.Length)

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
    member _.``Script with #r "" errors``() =
        use script = new FSharpScript()
        let result, errors = script.Eval("#r \"\"")
        Assert.IsNotEmpty(errors)
        match result with
        | Ok(_) -> Assert.Fail("expected a failure")
        | Error(ex) -> Assert.IsInstanceOf<FsiCompilationException>(ex)

    [<Test>]
    member _.``Script with #r "    " errors``() =
        use script = new FSharpScript()
        let result, errors = script.Eval("#r \"    \"")
        Assert.IsNotEmpty(errors)
        match result with
        | Ok(_) -> Assert.Fail("expected a failure")
        | Error(ex) -> Assert.IsInstanceOf<FsiCompilationException>(ex)

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
        let opt = script.Eval(code)  |> getValue
        let value = opt.Value
        Assert.AreEqual(123, value.ReflectionValue :?> int32)
#endif

    [<Test>]
    member __.``Eval script with package manager invalid key``() =
        use script = new FSharpScript(additionalArgs=[|"/langversion:preview"|])
        let result, _errors = script.Eval(@"#r ""nugt:FSharp.Data""")
        match result with
        | Ok(_) -> Assert.Fail("expected a failure")
        | Error(ex) -> Assert.IsInstanceOf<FsiCompilationException>(ex)

    [<Test>]
    member __.``Eval script with invalid PackageName should fail immediately``() =
        use output = new RedirectConsoleOutput()
        use script = new FSharpScript(additionalArgs=[|"/langversion:preview"|])
        let mutable found = 0
        let outp = System.Collections.Generic.List<string>()
        output.OutputProduced.Add(
            fun line ->
                if line.Contains("error NU1101:") && line.Contains("FSharp.Really.Not.A.Package") then
                    found <- found + 1
                outp.Add(line))
        let _result, _errors = script.Eval("""#r "nuget:FSharp.Really.Not.A.Package" """)
        Assert.True( (found = 1), "Expected to see output contains 'error NU1101:' and 'FSharp.Really.Not.A.Package'")

    [<Test>]
    member __.``Eval script with invalid PackageName should fail immediately and resolve one time only``() =
        use output = new RedirectConsoleOutput()
        use script = new FSharpScript(additionalArgs=[|"/langversion:preview"|])
        let mutable foundResolve = 0
        output.OutputProduced.Add (fun line -> if line.Contains("Microsoft (R) Build Engine version") then foundResolve <- foundResolve + 1)
        let _result, _errors =
            script.Eval("""
#r "nuget:FSharp.Really.Not.A.Package"
#r "nuget:FSharp.Really.Not.Another.Package"
                """)
        Assert.True( (foundResolve = 1), (sprintf "Expected to see 'Microsoft (R) Build Engine version' only once actually resolved %d times" foundResolve))

    [<Test>]
    member __.``ML - use assembly with ref dependencies``() =
        let code = @"
#r ""nuget:Microsoft.ML.OnnxTransformer,1.4.0""

open System
open System.Numerics.Tensors
let inputValues = [| 12.0; 10.0; 17.0; 5.0 |]
let tInput = new DenseTensor<float>(inputValues.AsMemory(), new ReadOnlySpan<int>([|4|]))
tInput.Length
"
        use script = new FSharpScript(additionalArgs=[|"/langversion:preview"|])
        let opt = script.Eval(code)  |> getValue
        let value = opt.Value
        Assert.AreEqual(4L, value.ReflectionValue :?> int64)


    [<Test>]
    member __.``System.Device.Gpio - Ensure we reference the runtime version of the assembly``() =
        let code = """
#r "nuget:System.Device.Gpio"
typeof<System.Device.Gpio.GpioController>.Assembly.Location
"""
        use script = new FSharpScript(additionalArgs=[|"/langversion:preview"|])
        let opt = script.Eval(code)  |> getValue
        let value = opt.Value

        if RuntimeInformation.IsOSPlatform(OSPlatform.Windows) then
            Assert.IsTrue( (value.ReflectionValue :?> string).EndsWith(@"runtimes\win\lib\netstandard2.0\System.Device.Gpio.dll") )
        else if RuntimeInformation.IsOSPlatform(OSPlatform.Linux) then
            Assert.IsTrue( (value.ReflectionValue :?> string).EndsWith(@"runtimes/linux/lib/netstandard2.0/System.Device.Gpio.dll") )
        else
            // Only Windows/Linux supported.
            ()

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

    [<Test>]
    member _.``Script with nuget package that yields out of order dependencies works correctly``() =
        // regression test for: https://github.com/dotnet/fsharp/issues/9217

        let code = """
#r "nuget: FParsec,1.1.1"

open FParsec

let test p str =
    match run p str with
    | Success(result, _, _)   ->
        printfn "Success: %A" result
        true
    | Failure(errorMsg, _, _) ->
        printfn "Failure: %s" errorMsg
        false
test pfloat "1.234"
"""
        use script = new FSharpScript(additionalArgs=[|"/langversion:preview"|])
        let opt = script.Eval(code)  |> getValue
        let value = opt.Value
        Assert.AreEqual(true, value.ReflectionValue :?> bool)
