namespace Microsoft.FSharp.Compiler.Server

open System
open System.IO
open System.Reflection
open System.Diagnostics
open System.Collections.Generic

open Microsoft.FSharp.Compiler
open Microsoft.FSharp.Compiler.Range
open Microsoft.FSharp.Compiler.SourceCodeServices

[<Sealed>]
type internal CompilerServerInProcess(checker: FSharpChecker) =

    interface ICompilerServer with 

        member __.GetSemanticClassificationAsync(checkerOptions, classifyRange) = async {
            let! _, fileAnswer = checker.ParseAndCheckFileInProject(checkerOptions.FilePath, checkerOptions.TextVersionHash, checkerOptions.SourceText, checkerOptions.ProjectOptions.ToFSharpProjectOptions(), checkerOptions.UserOpName)
            match fileAnswer with
            | FSharpCheckFileAnswer.Aborted -> return [||]
            | FSharpCheckFileAnswer.Succeeded(fileResults) ->
                let targetRange = classifyRange.ToFSharpRange(checkerOptions.FilePath)
                return
                    fileResults.GetSemanticClassification(Some(targetRange))
                    |> Array.map (fun (m, t) -> SemanticClassificationItem.FromFSharp(m, t))
        }

        member __.GetErrorInfosAsync(cmd) = async {
            let! result = GetErrorInfos.Execute checker cmd
            return Some(result)
        }

    interface IDisposable with

        member __.Dispose() = ()