module FSharp.Compiler.ComponentTests.Miscellaneous.TestUtilities

open System
open System.Threading
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
let ``TestConsole captures output`` () =
    let rnd = Random()

    let task n =
        async {
            use console = new TestConsole.ExecutionCapture()
            do! Async.Sleep(rnd.Next 50)
            printf $"Hello, world! {n}"
            do! Async.Sleep(rnd.Next 50)
            eprintf $"Some error {n}"
            return console.OutText, console.ErrorText
        }

    let expected =
        [ for n in 0..9 -> $"Hello, world! {n}", $"Some error {n}" ]

    let results =
        Seq.init 10 task |> Async.Parallel |> Async.RunSynchronously

    Assert.Equal(expected, results)

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
