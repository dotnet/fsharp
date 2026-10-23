module FSharp.Compiler.ComponentTests.Miscellaneous.TestUtilities

open System
open System.Diagnostics
open System.IO
open System.Threading
open System.Threading.Tasks
open Xunit
open Xunit.Sdk
open FSharp.Test

type RunOrFail(name) =
    let mutable count = 0

    member _.Run(shouldFail: bool) =
        let count = Interlocked.Increment &count

        if shouldFail && count = 42 then
            failwith $"{name}, failed as expected on {count}"
        else
            printfn $"{name}, iteration {count} passed"
            count

let passing = RunOrFail "Passing"

[<Fact>]
let ``TestConsole captures output`` () : Task =
    task {
        let rnd = Random()

        let capture n =
            async {
                use console = new TestConsole.ExecutionCapture()
                do! Async.Sleep(rnd.Next 50)
                printf $"Hello, world! {n}"
                do! Async.Sleep(rnd.Next 50)
                eprintf $"Some error {n}"
                return console.OutText, console.ErrorText
            }

        let expected = [ for n in 0..9 -> $"Hello, world! {n}", $"Some error {n}" ]

        let! results = Seq.init 10 capture |> Async.Parallel

        Assert.Equal(expected, results)
    }

[<Theory>]
[<InlineData(false)>]
[<InlineData(true)>]
let ``TestConsole preserves nested bulk output`` useError =
    let text (capture: TestConsole.ExecutionCapture) =
        if useError then capture.ErrorText else capture.OutText

    use outer = new TestConsole.ExecutionCapture()
    let writer = if useError then Console.Error else Console.Out
    writer.Write("before")

    do
        use inner = new TestConsole.ExecutionCapture()
        writer.Write("string")
        writer.Write(null: string)
        writer.Write("!array!".ToCharArray(), 1, 5)
        writer.Write('!')
        writer.WriteLine()
        Assert.Equal("stringarray!" + Environment.NewLine, text inner)

    writer.Write("after")
    Assert.Equal("beforestringarray!" + Environment.NewLine + "after", text outer)

#if NETCOREAPP
[<FactForNETCOREAPP>]
let ``Finite process timeout bounds inherited output drain after parent exit`` () =
    let directory = TestFramework.createTemporaryDirectory ()
    let config = TestFramework.initialConfig
    let childScript = Path.Combine(directory.FullName, "child.fsx")
    let parentScript = Path.Combine(directory.FullName, "parent.fsx")
    let pidFile = Path.Combine(directory.FullName, "child.pid")

    File.WriteAllText(
        childScript,
        $"""
use child = System.Diagnostics.Process.GetCurrentProcess()
printf "inherited stdout"
eprintf "inherited stderr"
System.Console.Out.Flush()
System.Console.Error.Flush()
System.IO.File.WriteAllLines(@"{pidFile}", [| string child.Id; string (child.StartTime.ToUniversalTime().Ticks) |])
System.Threading.Thread.Sleep(60000)
    """
    )

    File.WriteAllText(
        parentScript,
        $"""
let start = System.Diagnostics.ProcessStartInfo(@"{config.DotNetExe}", UseShellExecute = false)
for argument in [ @"{config.FSI}"; "--nologo"; "--exec"; @"{childScript}" ] do
    start.ArgumentList.Add argument
use child = System.Diagnostics.Process.Start start
if not (System.Threading.SpinWait.SpinUntil((fun () -> System.IO.File.Exists(@"{pidFile}")), 15000)) then
    failwith "Inherited-output child did not start"
exit 17
    """
    )

    try
        let stopwatch = Stopwatch.StartNew()

        let error =
            Assert.Throws<TimeoutException>(fun () ->
                FSharp.Test.Compiler.runFsiProcessWithTimeout 15000 [ "--nologo"; "--exec"; parentScript ]
                |> ignore)

        Assert.True(stopwatch.ElapsedMilliseconds < 35000L, $"Inherited output drain took {stopwatch.Elapsed}")
        Assert.Contains("Process timed out after 15000ms", error.Message)
        Assert.Contains("Exit code 17", error.Message)
        Assert.Contains("output-drained=False", error.Message)
        Assert.Contains("inherited stdout", error.Message)
        Assert.Contains("inherited stderr", error.Message)
        printfn "%s\nelapsed-ms=%d" error.Message stopwatch.ElapsedMilliseconds
    finally
        if File.Exists pidFile then
            let identity = File.ReadAllLines pidFile

            try
                use child = Process.GetProcessById(int identity.[0])

                if
                    child.StartTime.ToUniversalTime().Ticks = int64 identity.[1]
                    && not child.HasExited
                then
                    child.Kill(entireProcessTree = true)
                    Assert.True(child.WaitForExit(5000), "Inherited-output child did not exit")
            with :? ArgumentException ->
                ()

        directory.Delete(true)
#endif

/// Roundtrip-serialize a CompilationHelper through xUnit3's XunitSerializationInfo
/// and verify all fields survive the trip.
let private roundtripCompilationHelper (filename: obj) (directory: obj) (realsig: obj) (optimize: obj) =
    let original = CompilationHelper(filename, directory, realsig, optimize)
    let helper = SerializationHelper.Instance

    // Serialize
    let info = XunitSerializationInfo(helper, original)
    let serialized = info.ToSerializedString()

    // Deserialize into a fresh instance
    let deserialized = CompilationHelper()
    let info2 = XunitSerializationInfo(helper, serialized)
    (deserialized :> IXunitSerializable).Deserialize(info2)

    // Compare via ToString which encodes all field values
    Assert.Equal(original.ToString(), deserialized.ToString())

[<Fact>]
let ``CompilationHelper serialization roundtrip with all fields set`` () =
    roundtripCompilationHelper "test.fs" @"C:\src\tests" (box true) (box false)

[<Fact>]
let ``CompilationHelper serialization roundtrip with null optional fields`` () =
    roundtripCompilationHelper "test.fs" @"C:\src\tests" null null

[<Fact>]
let ``CompilationHelper serialization roundtrip with all nulls`` () =
    roundtripCompilationHelper null null null null

[<Fact>]
let ``CompilationHelper serialization roundtrip with realsig false optimize true`` () =
    roundtripCompilationHelper "myfile.fs" "/some/dir" (box false) (box true)
