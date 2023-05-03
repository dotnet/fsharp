module FSharp.Compiler.Service.Tests.DependencyGraphTests

#nowarn "57"

open NUnit.Framework

open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Service.Tests.Common
open FSharp.Compiler.Text

[<Test>]
let testGetDependencyGraph () =
    async {
        let files =
            Map.ofArray
                [| "A.fs",
                   SourceText.ofString """
module A

let bar a b = a - b
"""
                   "B.fs",
                   SourceText.ofString """
open A
bar 1 1
"""             |]
        let documentSource fileName = Map.tryFind fileName files |> async.Return
        let projectOptions =
            let _, projectOptions = mkTestFileAndOptions "" Array.empty
            { projectOptions with SourceFiles = [| "A.fs"; "B.fs" |] }
        let checker =
            FSharpChecker.Create(documentSource = DocumentSource.Custom documentSource, useSyntaxTreeCache = true)
        let! graph = checker.GetDependencyGraph(projectOptions)
        
        Assert.IsNotEmpty(graph.Keys)
        let aDeps = graph[0, "A.fs"]
        Assert.IsEmpty(aDeps)
        let b = graph[1, "B.fs"]
        Assert.AreEqual(b.Length, 1)
        
        return ()
    }
    