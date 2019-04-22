namespace GetTypesVSUnitTests

open NUnit.Framework
open System.IO
open System.Reflection

[<TestFixture>]
type VerifyUnitTests() =

    [<Test>]
    member this.GetTypesForVSUnitTests () =
        try
            let location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
            Assembly.LoadFrom(Path.Combine(location, "VisualFSharp.UnitTests.dll")).GetTypes() |> ignore
        with | :? ReflectionTypeLoadException as e ->
            let message = e.LoaderExceptions |> Seq.fold (fun (acc:string) e -> acc + (sprintf "TypeLoad failure: %s\n" ) (e.Message) ) ""
            Assert.Fail(message)
