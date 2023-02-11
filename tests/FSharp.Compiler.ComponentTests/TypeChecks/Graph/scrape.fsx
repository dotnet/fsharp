#r "nuget: MSBuild.StructuredLogger, 2.1.746"

open System.IO
open Microsoft.Build.Logging.StructuredLogger

/// Create a text file with the F# compiler arguments scrapped from an binary log file.
/// Run `dotnet build --no-incremental -bl` to create the binlog file.
/// The --no-incremental flag is essential for this scraping code.
let mkCompilerArgsFromBinLog file =
    let build = BinaryLog.ReadBuild file

    let projectName =
        build.Children
        |> Seq.choose (function
            | :? Project as p -> Some p.Name
            | _ -> None)
        |> Seq.distinct
        |> Seq.exactlyOne

    let message (fscTask: FscTask) =
        fscTask.Children
        |> Seq.tryPick (function
            | :? Message as m when m.Text.Contains "fsc" -> Some m.Text
            | _ -> None)

    let mutable args = None

    build.VisitAllChildren<Task>(fun task ->
        match task with
        | :? FscTask as fscTask ->
            match fscTask.Parent.Parent with
            | :? Project as p when p.Name = projectName -> args <- message fscTask
            | _ -> ()
        | _ -> ())

    match args with
    | None -> printfn "Could not process the binlog file. Did you build using '--no-incremental'?"
    | Some args ->
        let content =
            let idx = args.IndexOf "-o:"
            args.Substring(idx)

        let directory = FileInfo(file).Directory.FullName

        let argsPath =
            Path.Combine(directory, $"{Path.GetFileNameWithoutExtension(projectName)}.args.txt")

        File.WriteAllText(argsPath, content)
        printfn "Wrote %s" argsPath

// Example:
// The binlog was created by running `dotnet build --no-incremental -bl`
mkCompilerArgsFromBinLog @"C:\Projects\fantomas\src\Fantomas.Core\msbuild.binlog"
mkCompilerArgsFromBinLog @"C:\Projects\FsAutoComplete\src\FsAutoComplete\msbuild.binlog"
mkCompilerArgsFromBinLog @"C:\Projects\fsharp\src\Compiler\msbuild.binlog"
mkCompilerArgsFromBinLog @"C:\Projects\fsharp\tests\FSharp.Compiler.ComponentTests\msbuild.binlog"
