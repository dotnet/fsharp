module FSharp.Compiler.LanguageServer.Workspace

#nowarn "57"

open System
open FSharp.Compiler.CodeAnalysis.ProjectSnapshot

type FSharpWorkspace =
    {
        Projects: Map<FSharpProjectIdentifier, FSharpProjectSnapshot>
        OpenFiles: Map<string, string>
    }

    static member Create(projects: FSharpProjectSnapshot seq) =
        {
            Projects = Map.ofSeq (projects |> Seq.map (fun p -> p.Identifier, p))
            OpenFiles = Map.empty
        }
