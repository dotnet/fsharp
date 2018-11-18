namespace Microsoft.FSharp.Compiler.Server

open System
open System.IO
open System.Reflection
open System.Diagnostics
open System.Collections.Generic

open Microsoft.FSharp.Compiler
open Microsoft.FSharp.Compiler.Range
open Microsoft.FSharp.Compiler.SourceCodeServices

module internal GetErrorInfos =

    let private errorInfoEqualityComparer =
        { new IEqualityComparer<FSharpErrorInfo> with 
            member __.Equals (x, y) =
                x.FileName = y.FileName &&
                x.StartLineAlternate = y.StartLineAlternate &&
                x.EndLineAlternate = y.EndLineAlternate &&
                x.StartColumn = y.StartColumn &&
                x.EndColumn = y.EndColumn &&
                x.Severity = y.Severity &&
                x.Message = y.Message &&
                x.Subcategory = y.Subcategory &&
                x.ErrorNumber = y.ErrorNumber
            member __.GetHashCode x =
                let mutable hash = 17
                hash <- hash * 23 + x.StartLineAlternate.GetHashCode()
                hash <- hash * 23 + x.EndLineAlternate.GetHashCode()
                hash <- hash * 23 + x.StartColumn.GetHashCode()
                hash <- hash * 23 + x.EndColumn.GetHashCode()
                hash <- hash * 23 + x.Severity.GetHashCode()
                hash <- hash * 23 + x.Message.GetHashCode()
                hash <- hash * 23 + x.Subcategory.GetHashCode()
                hash <- hash * 23 + x.ErrorNumber.GetHashCode()
                hash 
        }

    let Execute (checker: FSharpChecker) (cmd: Command.GetErrorInfos) = async {
        let parsingOptions = cmd.ParsingOptions
        let checkerOptions = cmd.CheckerOptions
        let! parseResults = checker.ParseFile(checkerOptions.FilePath, checkerOptions.SourceText, parsingOptions.ToFSharpParsingOptions(), defaultArg checkerOptions.UserOpName String.Empty)
        let! errors =
            async {
                match cmd.Type with
                | GetErrorInfosCommandType.Semantic ->
                    let! checkResultsAnswer = checker.CheckFileInProject(parseResults, checkerOptions.FilePath, checkerOptions.TextVersionHash, checkerOptions.SourceText, checkerOptions.ProjectOptions.ToFSharpProjectOptions(), checkerOptions.UserOpName) 
                    match checkResultsAnswer with
                    | FSharpCheckFileAnswer.Aborted -> return [||]
                    | FSharpCheckFileAnswer.Succeeded results ->
                        // In order to eleminate duplicates, we should not return parse errors here because they are returned by `AnalyzeSyntaxAsync` method.
                        let allErrors = HashSet(results.Errors, errorInfoEqualityComparer)
                        allErrors.ExceptWith(parseResults.Errors)
                        return Seq.toArray allErrors
                | GetErrorInfosCommandType.Syntax ->
                    return parseResults.Errors
                | _ -> 
                    return failwith "should not happen"
            }

        return CommandResult.GetErrorInfos.Create(HashSet(errors, errorInfoEqualityComparer) |> Seq.map (fun x -> ErrorInfo.FromFSharpErrorInfo(x)) |> Seq.toArray)
    }
        