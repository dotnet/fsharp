module FSharpChecker.ScopedNowarn

open System
open System.IO
open System.Threading.Tasks
open Xunit
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Text

#nowarn "57" // FSharpProjectSnapshot is experimental

let private source =
    """module A

#nowarn "1182"

let f x =
    let unused = 1
    x
"""

let private diagnostics answer =
    match answer with
    | FSharpCheckFileAnswer.Succeeded results ->
        results.Diagnostics |> Array.map (fun d -> $"%A{d.Severity} FS%04d{d.ErrorNumber}") |> List.ofArray
    | FSharpCheckFileAnswer.Aborted -> failwith "the check was aborted"

/// A one-file project whose file suppresses the FS1182 that --warnon:1182 turns on, and which references a copy of
/// an assembly, so a test can move the reference's last-write time.
type private Project(useTransparentCompiler: bool, warnAsError: bool) =
    let dir =
        Path.Combine(Path.GetTempPath(), "ScopedNowarn_" + Guid.NewGuid().ToString("N").Substring(0, 8))

    let file = Path.Combine(dir, "A.fs")
    let reference = Path.Combine(dir, "Reference.dll")

    do
        Directory.CreateDirectory dir |> ignore
        File.WriteAllText(file, source)
        File.Copy(typeof<FactAttribute>.Assembly.Location, reference)

    member val Checker = FSharpChecker.Create(useTransparentCompiler = useTransparentCompiler)

    member this.Options =
        let baseOptions, _ =
            this.Checker.GetProjectOptionsFromScript("file.fsx", SourceText.ofString "", assumeDotNetFramework = false)
            |> Async.RunSynchronously

        { baseOptions with
            ProjectFileName = Path.Combine(dir, "Project.fsproj")
            SourceFiles = [| file |]
            OtherOptions =
                [|
                    yield! baseOptions.OtherOptions
                    "--warnon:1182"
                    if warnAsError then
                        "--warnaserror+"
                    $"-r:{reference}"
                |] }

    member _.File = file

    member _.MoveReferenceStamp() =
        File.SetLastWriteTimeUtc(reference, DateTime.UtcNow.AddMinutes 1.0)

    member this.Check() =
        task {
            let! _, answer = this.Checker.ParseAndCheckFileInProject(file, 0, SourceText.ofString source, this.Options)
            return diagnostics answer
        }

[<Theory>]
[<InlineData(false, false)>]
[<InlineData(false, true)>]
[<InlineData(true, false)>]
[<InlineData(true, true)>]
let ``#nowarn applies when a file is checked again after a reference's stamp moved``
    (useTransparentCompiler: bool)
    (warnAsError: bool)
    : Task =
    task {
        let project = Project(useTransparentCompiler, warnAsError)

        let! cold = project.Check()
        Assert.Empty cold

        project.MoveReferenceStamp()

        let! afterStampMoved = project.Check()
        Assert.Empty afterStampMoved
    }

[<Fact>]
let ``#nowarn applies when the file was parsed on its own before the check`` () : Task =
    task {
        let project = Project(useTransparentCompiler = true, warnAsError = true)

        let! snapshot = FSharpProjectSnapshot.FromOptions(project.Options, DocumentSource.FileSystem)
        let! _ = project.Checker.ParseFile(project.File, snapshot)
        let! _, answer = project.Checker.ParseAndCheckFileInProject(project.File, snapshot)
        Assert.Empty(diagnostics answer)
    }
