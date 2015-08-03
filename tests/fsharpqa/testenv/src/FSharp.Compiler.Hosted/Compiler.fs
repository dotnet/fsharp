namespace FSharp.Compiler.Hosted
open System
open System.Text.RegularExpressions

type CoreCompiler = Microsoft.FSharp.Compiler.Driver.InProcCompiler

/// build issue location
type Location =
    {
        StartLine : int
        StartColumn : int
        EndLine : int
        EndColumn : int
    }

type CompilationIssueType = Warning | Error

/// build issue details
type CompilationIssue = 
    { 
        Location : Location
        Subcategory : string
        Code : string
        File : string
        Text : string 
        Type : CompilationIssueType
    }

/// combined warning and error details
type FailureDetails = 
    {
        Warnings : CompilationIssue list
        Errors : CompilationIssue list
    }

type CompilationResult = 
    | Success of CompilationIssue list
    | Failure of FailureDetails

/// in-proc version of fsc.exe
type FscCompiler() =
    let compiler = CoreCompiler()

    let emptyLocation = 
        { 
            StartColumn = 0
            EndColumn = 0
            StartLine = 0
            EndLine = 0
        }

    /// converts short and long issue types to the same CompilationIssue reprsentation
    let convert issue : CompilationIssue = 
        match issue with
        | Microsoft.FSharp.Compiler.CompileOps.ErrorOrWarning.Short(isError, text) -> 
            {
                Location = emptyLocation
                Code = ""
                Subcategory = ""
                File = ""
                Text = text
                Type = if isError then CompilationIssueType.Error else CompilationIssueType.Warning
            }
        | Microsoft.FSharp.Compiler.CompileOps.ErrorOrWarning.Long(isError, details) ->
            let loc, file = 
                match details.Location with
                | Some l when not l.IsEmpty -> 
                    { 
                        StartColumn = l.Range.StartColumn
                        EndColumn = l.Range.EndColumn
                        StartLine = l.Range.StartLine
                        EndLine = l.Range.EndLine
                    }, l.File
                | _ -> emptyLocation, ""
            {
                Location = loc
                Code = sprintf "FS%04d" details.Canonical.ErrorNumber
                Subcategory = details.Canonical.Subcategory
                File = file
                Text = details.Message
                Type = if isError then CompilationIssueType.Error else CompilationIssueType.Warning
            }

    /// test if --test:ErrorRanges flag is set
    let errorRangesArg =
        let regex = Regex(@"^(/|--)test:ErrorRanges$", RegexOptions.Compiled ||| RegexOptions.IgnoreCase)
        fun arg -> regex.IsMatch(arg)

    /// test if --vserrors flag is set
    let vsErrorsArg =
        let regex = Regex(@"^(/|--)vserrors$", RegexOptions.Compiled ||| RegexOptions.IgnoreCase)
        fun arg -> regex.IsMatch(arg)

    /// test if an arg is a path to fsc.exe
    let fscExeArg = 
        let regex = Regex(@"fsc(\.exe)?$", RegexOptions.Compiled ||| RegexOptions.IgnoreCase)
        fun arg -> regex.IsMatch(arg)

    /// do compilation as if args was argv to fsc.exe
    member this.Compile(args : string array) =
        // args.[0] is later discarded, assuming it is just the path to fsc.
        // compensate for this in case caller didn't know
        let args =
            match args with
            | [||] | null -> [|"fsc"|]
            | a when not <| fscExeArg a.[0] -> Array.append [|"fsc"|] a
            | _ -> args

        let errorRanges = args |> Seq.exists errorRangesArg
        let vsErrors = args |> Seq.exists vsErrorsArg

        let (ok, result) = compiler.Compile(args)
        let exitCode = if ok then 0 else 1
        
        let lines =
            Seq.append result.Errors result.Warnings
            |> Seq.map convert
            |> Seq.map (fun issue ->
                let issueTypeStr = 
                    match issue.Type with
                    | Error -> if vsErrors then sprintf "%s error" issue.Subcategory else "error"
                    | Warning -> if vsErrors then sprintf "%s warning" issue.Subcategory else "warning"

                let locationStr =
                    if vsErrors then
                        sprintf "(%d,%d,%d,%d)" issue.Location.StartLine issue.Location.StartColumn issue.Location.EndLine issue.Location.EndColumn
                    elif errorRanges then
                        sprintf "(%d,%d-%d,%d)" issue.Location.StartLine issue.Location.StartColumn issue.Location.EndLine issue.Location.EndColumn
                    else
                        sprintf "(%d,%d)" issue.Location.StartLine issue.Location.StartColumn

                sprintf "%s: %s %s: %s" locationStr issueTypeStr issue.Code issue.Text
                )
            |> Array.ofSeq
        (exitCode, lines)
