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
open FSharp.Test.ScriptHelpers

open Xunit

type InteractiveTests() =

    [<Fact>]
    member _.``ValueRestriction error message should not have type variables fully solved``() =
        use script = new FSharpScript()
        let code = "id id"
        let _, errors = script.Eval(code)
        Assert.Equal(1, errors.Length)
        let msg = errors[0].Message
        Assert.DoesNotMatch("obj -> obj", msg)

    [<Fact>]
    member _.``Eval object value``() =
        use script = new FSharpScript()
        let opt = script.Eval("1+1") |> getValue
        let value = opt.Value
        Assert.Equal(typeof<int>, value.ReflectionType)
        Assert.Equal(2, value.ReflectionValue :?> int)

    [<Fact>]
    member _.``Declare and eval object value``() =
        use script = new FSharpScript()
        let opt = script.Eval("let x = 1 + 2\r\nx") |> getValue
        let value = opt.Value
        Assert.Equal(typeof<int>, value.ReflectionType)
        Assert.Equal(3, value.ReflectionValue :?> int)

    [<Fact>]
    member _.``Capture console input``() =
        use input = new RedirectConsoleInput()
        use script = new FSharpScript()
        input.ProvideInput "stdin:1234\r\n"
        let opt = script.Eval("System.Console.ReadLine()") |> getValue
        let value = opt.Value
        Assert.Equal(typeof<string>, value.ReflectionType)
        Assert.Equal("stdin:1234", downcast value.ReflectionValue)

    [<Fact>]
    member _.``Capture console output/error``() =
        use output = new RedirectConsoleOutput()
        use script = new FSharpScript()
        use sawOutputSentinel = new ManualResetEvent(false)
        use sawErrorSentinel = new ManualResetEvent(false)
        output.OutputProduced.Add (fun line -> if line = "stdout:1234" then sawOutputSentinel.Set() |> ignore)
        output.ErrorProduced.Add (fun line -> if line = "stderr:5678" then sawErrorSentinel.Set() |> ignore)
        script.Eval("printfn \"stdout:1234\"; eprintfn \"stderr:5678\"") |> ignoreValue
        Assert.True(sawOutputSentinel.WaitOne(TimeSpan.FromSeconds(5.0)), "Expected to see output sentinel value written")
        Assert.True(sawErrorSentinel.WaitOne(TimeSpan.FromSeconds(5.0)), "Expected to see error sentinel value written")

    [<Fact>]
    member _.``Maintain state between submissions``() =
        use script = new FSharpScript()
        script.Eval("let add x y = x + y") |> ignoreValue
        let opt = script.Eval("add 2 3") |> getValue
        let value = opt.Value
        Assert.Equal(typeof<int>, value.ReflectionType)
        Assert.Equal(5, downcast value.ReflectionValue)

    [<Fact>]
    member _.``Assembly reference event successful``() =
        use script = new FSharpScript()
        let testCode = """
#r "System.dll"
let stacktype= typeof<System.Collections.Stack>
stacktype.Name = "Stack"
"""
        let opt = script.Eval(testCode) |> getValue
        let value = opt.Value
        Assert.Equal(true, downcast value.ReflectionValue)

    [<Fact>]
    member _.``Assembly reference unsuccessful``() =
        use script = new FSharpScript()
        let testAssembly = "not-an-assembly-that-can-be-found.dll"
        let _result, errors = script.Eval(sprintf "#r \"%s\"" testAssembly)
        Assert.Equal(1, errors.Length)

    [<Fact>]
    member _.``Compilation errors report a specific exception``() =
        use script = new FSharpScript()
        let result, _errors = script.Eval("abc")
        match result with
        | Ok(_) -> Assert.False(true, "expected a failure")
        | Error(ex) -> Assert.IsAssignableFrom(typeof<FsiCompilationException>, ex)

    [<Fact>]
    member _.``Runtime exceptions are propagated``() =
        use script = new FSharpScript()
        let result, errors = script.Eval("System.IO.File.ReadAllText(\"not-a-file-path-that-can-be-found-on-disk.txt\")")
        Assert.Empty(errors)
        match result with
        | Ok(_) -> Assert.True(false, "expected a failure")
        | Error(ex) -> Assert.IsAssignableFrom(typeof<FileNotFoundException>, ex)

    [<Fact>]
    member _.``Script with #r "" errors``() =
        use script = new FSharpScript()
        let result, errors = script.Eval("#r \"\"")
        Assert.NotEmpty(errors)
        match result with
        | Ok(_) -> Assert.False(true, "expected a failure")
        | Error(ex) -> Assert.IsAssignableFrom(typeof<FsiCompilationException>, ex)

    [<Fact>]
    member _.``Script with #r "    " errors``() =
        use script = new FSharpScript()
        let result, errors = script.Eval("#r \"    \"")
        Assert.NotEmpty(errors)
        match result with
        | Ok(_) -> Assert.False(true, "expected a failure")
        | Error(ex) -> Assert.IsAssignableFrom(typeof<FsiCompilationException>, ex)


    [<Fact>]
    member _.``Script using System.Configuration succeeds``() =
        use script = new FSharpScript()
        let result, errors = script.Eval("""
#r "nuget:System.Configuration.ConfigurationManager,5.0.0"
open System.Configuration
System.Configuration.ConfigurationManager.AppSettings.Item "Environment" <- "LOCAL" """)
        Assert.Empty(errors)
        match result with
        | Ok(_) -> ()
        | Error(ex) -> Assert.True(true, "expected no failures")

    [<Theory>]
    [<InlineData("""#i""", "input.fsx (1,1)-(1,3) interactive warning Invalid directive '#i '")>]                                               // No argument
    [<InlineData("""#i "" """, "input.fsx (1,1)-(1,6) interactive error #i is not supported by the registered PackageManagers")>]               // empty argument
    [<InlineData("""#i "        " """, "input.fsx (1,1)-(1,14) interactive error #i is not supported by the registered PackageManagers")>]      // whitespace only argument
    member _.``Script with #i syntax errors fail``(code, error0) =
        use script = new FSharpScript()
        let result, errors = script.Eval(code)
        Assert.NotEmpty(errors)
        Assert.Equal(errors.[0].ToString(), error0)

    [<Theory>]
    [<InlineData("""#i " """,                                                                           // Single quote
                 "input.fsx (1,4)-(1,5) parse error End of file in string begun at or before here",
                 "input.fsx (1,1)-(1,3) interactive warning Invalid directive '#i '")>]
    member _.``Script with more #i syntax errors fail``(code, error0, error1) =
        use script = new FSharpScript()
        let result, errors = script.Eval(code)
        Assert.NotEmpty(errors)
        Assert.Equal(errors.Length, 2)
        Assert.Equal(error0, errors.[0].ToString())
        Assert.Equal(error1, errors.[1].ToString())

    [<Theory>]
    [<InlineData("""#i "Obviously I am not a package manager" """,
                 "input.fsx (1,1)-(1,42) interactive error #i is not supported by the registered PackageManagers")>]
    member _.``Script with #i and no package manager specified``(code, error0) =
        use script = new FSharpScript()
        let result, errors = script.Eval(code)
        Assert.NotEmpty(errors)
        Assert.Equal(errors.Length, 1)
        Assert.Equal(errors.[0].ToString(), error0)

    [<Theory>]
    [<InlineData("""#i "nuget:foo" """,
                 "input.fsx (1,1)-(1,15) interactive error Invalid URI: The format of the URI could not be determined.")>]
    member _.``Script with #i and forgot to add quotes``(code, error) =
        use script = new FSharpScript()
        let result, errors = script.Eval(code)
        Assert.NotEmpty(errors)
        Assert.Equal(1, errors.Length)
        Assert.Equal(error, errors.[0].ToString())

    [<Fact>]
    member _.``#i to a directory that exists``() =
        let path = Path.GetTempPath()
        let code = sprintf "#i @\"nuget:%s\" " path
        use script = new FSharpScript()
        let result, errors = script.Eval(code)
        Assert.Empty(errors)
        Assert.Equal(0, errors.Length)

    [<Fact>]
    member _.``#i to a directory that doesn't exist``() =
        let path =
            let newGuid() = Guid.NewGuid().ToString().Replace("{", "").Replace("}", "")
            Path.Combine(Path.GetTempPath(), newGuid(), newGuid())
        let code = sprintf "#i @\"nuget:%s\"" path
        let error = sprintf "interactive error The source directory '%s' not found" path
        use script = new FSharpScript()
        let result, errors = script.Eval(code)
        Assert.NotEmpty(errors)
        Assert.Equal(1, errors.Length)
        Assert.True(errors.[0].ToString().EndsWith(error))

/// Native dll resolution is not implemented on desktop
#if NETSTANDARD
    [<Fact>]
    member _.``ML - use assembly with native dependencies``() =
        let code = @"
#r ""nuget:Microsoft.ML,version=1.4.0-preview""
#r ""nuget:Microsoft.ML.AutoML,version=0.16.0-preview""
#r ""nuget:Microsoft.Data.Analysis,version=0.4.0""

open System
open System.IO
open System.Linq
open Microsoft.Data.Analysis

let Shuffle (arr:int[]) =
    let rnd = Random()
    for i in 0 .. arr.Length - 1 do
        let r = i + rnd.Next(arr.Length - i)
        let temp = arr.[r]
        arr.[r] <- arr.[i]
        arr.[i] <- temp
    arr

let housingPath = ""housing.csv""
let housingData = DataFrame.LoadCsv(housingPath)
let randomIndices = (Shuffle(Enumerable.Range(0, (int (housingData.Rows.Count) - 1)).ToArray()))
let testSize = int (float (housingData.Rows.Count) * 0.1)
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
        Assert.Equal(123, value.ReflectionValue :?> int32)
#endif

    [<Fact>]
    member _.``Eval script with package manager invalid key``() =
        use script = new FSharpScript(additionalArgs=[|"/langversion:preview"|])
        let result, _errors = script.Eval(@"#r ""nugt:FSharp.Data""")
        match result with
        | Ok(_) -> Assert.False(true, "expected a failure")
        | Error(ex) -> Assert.IsAssignableFrom(typeof<FsiCompilationException>, ex)

    [<Fact>]
    member _.``Eval script with invalid PackageName should fail immediately``() =
        use output = new RedirectConsoleOutput()
        use script = new FSharpScript(additionalArgs=[|"/langversion:preview"|])
        let mutable found = 0
        let outp = System.Collections.Generic.List<string>()
        output.OutputProduced.Add(
            fun line ->
                if line.Contains("error NU1101:") && line.Contains("FSharp.Really.Not.A.Package") then
                    found <- found + 1
                outp.Add(line))
        let result, errors = script.Eval("""#r "nuget:FSharp.Really.Not.A.Package" """)
        Assert.True( (found = 0), "Did not expect to see output contains 'error NU1101:' and 'FSharp.Really.Not.A.Package'")
        Assert.True( errors |> Seq.exists (fun error -> error.Message.Contains("error NU1101:")), "Expect to error containing 'error NU1101:'")
        Assert.True( errors |> Seq.exists (fun error -> error.Message.Contains("FSharp.Really.Not.A.Package")), "Expect to error containing 'FSharp.Really.Not.A.Package'")

    [<Fact>]
    member _.``Eval script with invalid PackageName should fail immediately and resolve one time only``() =
        use output = new RedirectConsoleOutput()
        use script = new FSharpScript(additionalArgs=[|"/langversion:preview"|])
        let mutable foundResolve = 0
        output.OutputProduced.Add (fun line -> if line.Contains("error NU1101:") then foundResolve <- foundResolve + 1)
        let result, errors =
            script.Eval("""
#r "nuget:FSharp.Really.Not.A.Package"
#r "nuget:FSharp.Really.Not.Another.Package"
                """)
        Assert.True( (foundResolve = 0), (sprintf "Did not expected to see 'error NU1101:' in output" ))
        Assert.Equal(2, (errors |> Seq.filter (fun error -> error.Message.Contains("error NU1101:")) |> Seq.length))
        Assert.Equal(1, (errors |> Seq.filter (fun error -> error.Message.Contains("FSharp.Really.Not.A.Package")) |> Seq.length))
        Assert.Equal(1, (errors |> Seq.filter (fun error -> error.Message.Contains("FSharp.Really.Not.Another.Package")) |> Seq.length))

    [<Fact>]
    member _.``ML - use assembly with ref dependencies``() =
        let code = """
#r "nuget:Microsoft.ML.OnnxTransformer,1.4.0"
#r "nuget:System.Memory,4.5.4"

open System
open System.Numerics.Tensors
let inputValues = [| 12.0; 10.0; 17.0; 5.0 |]
let tInput = new DenseTensor<float>(inputValues.AsMemory(), new ReadOnlySpan<int>([|4|]))
tInput.Length
"""
        use script = new FSharpScript(additionalArgs=[|"/langversion:preview"|])
        let opt = script.Eval(code)  |> getValue
        let value = opt.Value
        Assert.Equal(4L, downcast value.ReflectionValue)

    [<Fact>]
    member _.``System.Device.Gpio - Ensure we reference the runtime version of the assembly``() =
        let code = """
#r "nuget:System.Device.Gpio, 1.0.0"
typeof<System.Device.Gpio.GpioController>.Assembly.Location
"""
        use script = new FSharpScript(additionalArgs=[|"/langversion:preview"|])
        let opt = script.Eval(code)  |> getValue
        let value = opt.Value

        if RuntimeInformation.IsOSPlatform(OSPlatform.Windows) then
            Assert.True( (value.ReflectionValue :?> string).EndsWith(@"runtimes\win\lib\netstandard2.0\System.Device.Gpio.dll") )
        else if RuntimeInformation.IsOSPlatform(OSPlatform.Linux) then
            Assert.True( (value.ReflectionValue :?> string).EndsWith(@"runtimes/linux/lib/netstandard2.0/System.Device.Gpio.dll") )
        else
            // Only Windows/Linux supported.
            ()

    [<Fact>]
    member _.``Simple pinvoke should not be impacted by native resolver``() =
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
        Assert.Equal(123, value.ReflectionValue :?> int32)

    [<Fact(Skip="This timing test fails in different environments. Skipping so that we don't assume an arbitrary CI environment has enough compute/etc. for what we need here.")>]
    member _.``Evaluation can be cancelled``() =
        use script = new FSharpScript()
        let sleepTime = 10000L
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
        Assert.Equal(sw.ElapsedMilliseconds, sleepTime)
        Assert.Equal(None, result)

    [<Fact>]
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
        Assert.Equal(2, foundCount)

    [<Fact>]
    member _.``Values re-bound trigger an event``() =
        let mutable foundXCount = 0
        use script = new FSharpScript()
        script.ValueBound
        |> Event.add (fun (_value, typ, name) ->
            if name = "x" && typ = typeof<int> then foundXCount <- foundXCount + 1)
        script.Eval("let x = 1") |> ignoreValue
        script.Eval("let x = 2") |> ignoreValue
        Assert.Equal(2, foundXCount)

    [<Fact>]
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

    [<Fact>]
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
        Assert.True(true = downcast value.ReflectionValue)
