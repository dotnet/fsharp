namespace FSharp.Compiler.Compilation.Tests

open System
open System.Collections.Immutable
open FSharp.Compiler.Compilation
open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Text
open NUnit.Framework

[<AutoOpen>]
module Helpers =

    let createTestModules name amount =
        [
            for i = 1 to amount do
                yield
                    sprintf
                    """
module TestModule%i =

    type %s () =

                    member val X = 1

                    member val Y = 2

                    member val Z = 3
                    
    let testFunction (x: %s) =
                    x.X + x.Y + x.Z
                    """ i name name
        ]
        |> List.reduce (+)

    let createSource name amount =
        (sprintf """namespace Test.%s""" name) + createTestModules name amount

[<TestFixture>]
type CompilationTests() =

    [<Test>]
    member __.``Basic Check``() =
        let sources =
            [
                for i = 1 to 3 do
                    yield ("test" + i.ToString() + ".fs", SourceText.From (createSource "CompilationTest" 1))
            ]
        let workspace = new AdhocWorkspace ()
        let compilationService = CompilationService (3, 8, workspace)

        let sourceSnapshots =
            sources
            |> List.map (fun (filePath, sourceText) -> compilationService.CreateSourceSnapshot (filePath, sourceText))
            |> ImmutableArray.CreateRange

        let options = CompilationOptions.Create ("""C:\test.dll""", """C:\""", sourceSnapshots, ImmutableArray.Empty)
        let c = compilationService.CreateCompilation options

        c.CheckAsync "test3.fs" |> Async.RunSynchronously |> ignore
        Assert.Throws (fun () -> c.CheckAsync "badfile.fs" |> Async.RunSynchronously) |> ignore
