// Copied from https://github.com/dungpa/fantomas and modified by Vasily Kirichenko

module FSharp.Compiler.Service.Tests.ServiceFormatting.TestHelper

open FsUnit

open System
open Microsoft.FSharp.Compiler.SourceCodeServices.ServiceFormatting.FormatConfig
open Microsoft.FSharp.Compiler.SourceCodeServices.ServiceFormatting
open Microsoft.FSharp.Compiler.SourceCodeServices
open Microsoft.VisualStudio.FSharp.Editor.Pervasive

let internal config = FormatConfig.Default
let newline = "\n"

let argsDotNET451 =
        [|"--noframework"; "--debug-"; "--optimize-"; "--tailcalls-";
          // Some constants are used in unit tests
          "--define:DEBUG"; "--define:TRACE"; "--define:SILVERLIGHT";
          @"-r:C:\Program Files (x86)\Reference Assemblies\Microsoft\FSharp\.NETFramework\v4.0\4.3.1.0\FSharp.Core.dll";
          @"-r:C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5.1\mscorlib.dll";
          @"-r:C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5.1\System.dll";
          @"-r:C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5.1\System.Core.dll";
          @"-r:C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5.1\System.Drawing.dll";
          @"-r:C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5.1\System.Numerics.dll";
          @"-r:C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5.1\System.Windows.Forms.dll"|]

let internal projectOptions fileName =
    {   ProjectFileName = @"C:\Project.fsproj"
        SourceFiles = [| fileName |]
        OtherOptions = argsDotNET451
        ReferencedProjects = Array.empty
        IsIncompleteTypeCheckEnvironment = false
        UseScriptResolutionRules = true
        LoadTime = DateTime.UtcNow
        UnresolvedReferences = None
        OriginalLoadReferences = List.empty
        ExtraProjectInfo = None 
        Stamp = None }

let private sharedChecker = lazy (FSharpChecker.Create())

let internal formatSourceString isFsiFile (s: string) config =
    asyncMaybe {
        // On Linux/Mac this will exercise different line endings
        let s = s.Replace("\r\n", Environment.NewLine)
        let fileName = if isFsiFile then "/src.fsi" else "/src.fsx"
        let! result = sharedChecker.Value.ParseFileInProject(fileName, s, projectOptions fileName) |> liftAsync
        let! ast = result.ParseTree
        return CodeFormatter.FormatAST(ast, fileName, Some s, config).Replace("\r\n", "\n")
    } 
    |> Async.RunSynchronously
    |> function Some x -> x | None -> ""

let internal formatSelectionFromString isFsiFile r (s: string) config = 
    asyncMaybe {
        let s = s.Replace("\r\n", Environment.NewLine)
        let fileName = if isFsiFile then "/tmp.fsi" else "/tmp.fsx"
        let! result = sharedChecker.Value.ParseFileInProject(fileName, s, projectOptions fileName) |> liftAsync
        let! ast = result.ParseTree
        return CodeFormatter.FormatSelectionInDocument(fileName, r, s, config, ast).Replace("\r\n", "\n")
    }
    |> Async.RunSynchronously
    |> function Some x -> x | None -> ""

let internal formatSelectionOnly isFsiFile r (s : string) config = 
    asyncMaybe {
        let s = s.Replace("\r\n", Environment.NewLine)
        let fileName = if isFsiFile then "/tmp.fsi" else "/tmp.fsx"
        let! result = sharedChecker.Value.ParseFileInProject(fileName, s, projectOptions fileName) |> liftAsync
        let! ast = result.ParseTree
        return CodeFormatter.FormatSelection(fileName, r, s, config, ast).Replace("\r\n", "\n")
    }
    |> Async.RunSynchronously
    |> function Some x -> x | None -> ""

let internal formatAroundCursor isFsiFile p (s : string) config = 
    asyncMaybe {
        let s = s.Replace("\r\n", Environment.NewLine)
        let fileName = if isFsiFile then "/tmp.fsi" else "/tmp.fsx"
        let! result = sharedChecker.Value.ParseFileInProject(fileName, s, projectOptions fileName) |> liftAsync
        let! ast = result.ParseTree
        return CodeFormatter.FormatAroundCursor(fileName, p, s, config, ast).Replace("\r\n", "\n")
    }
    |> Async.RunSynchronously
    |> function Some x -> x | None -> ""

let internal parse isFsiFile s =
    asyncMaybe {
        let fileName = if isFsiFile then "/tmp.fsi" else "/tmp.fsx"
        // Run the first phase (untyped parsing) of the compiler
        let projectOptions = projectOptions fileName
        let! untypedRes = sharedChecker.Value.ParseFileInProject(fileName, s, projectOptions) |> liftAsync
        if untypedRes.ParseHadErrors then
            let errors = 
                untypedRes.Errors
                |> Array.filter (fun e -> e.Severity = FSharpErrorSeverity.Error)
            if not <| Array.isEmpty errors then
                raise <| FormatException (sprintf "Parsing failed with errors: %A\nAnd options: %A" errors projectOptions.OtherOptions)
        match untypedRes.ParseTree with
        | Some tree -> return tree
        | None -> return raise <| FormatException "Parsing failed. Please select a complete code fragment to format."
    } 
    |> Async.RunSynchronously

let internal formatAST a s c =
    CodeFormatter.FormatAST(a, "/tmp.fsx",s, c)

let internal makeRange l1 c1 l2 c2 = 
    CodeFormatter.MakeRange("/tmp.fsx", l1, c1, l2, c2)

let internal makePos l1 c1 = 
    CodeFormatter.MakePos(l1, c1)

let internal equal x = 
    let x = 
        match box x with
        | :? String as s -> s.Replace("\r\n", "\n") |> box
        | x -> x
    equal x

let inline prepend s content = s + content
let inline append s content = content + s
