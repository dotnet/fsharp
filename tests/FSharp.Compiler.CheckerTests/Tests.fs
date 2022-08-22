module Tests.Sample

open Xunit

[<Fact>]
let ``empty test`` () = ()

(*
Example usage of `mkFSharpProjectOptions`

open Xunit.Abstractions

type SampleTests(output: ITestOutputHelper) =
    [<Fact>]
    member _.Sample() =
        let projects =
            ProjectInfoHelpers.mkFSharpProjectOptions
                @"C:\Users\nojaf\Projects\fsharp-compiler-playground\FSharpPlaygroundProject\FSharpPlaygroundProject.fsproj"

        Assert.True(projects.Length > 0)

    [<Fact>]
    member _.WithLogger() =
        let projects =
            ProjectInfoHelpers.mkFSharpProjectOptionsWithLogger
                output
                @"C:\Users\nojaf\Projects\fsharp-compiler-playground\FSharpPlaygroundProject\FSharpPlaygroundProject.fsproj" // projectFileName

        output.WriteLine(string projects)
*)