module FSharp.Compiler.LanguageServer.Workspace

#nowarn "57"

open System
open FSharp.Compiler.CodeAnalysis.ProjectSnapshot

type FSharpWorkspace =
    {
        Projects: Map<FSharpProjectIdentifier, FSharpProjectSnapshot>
        OpenFiles: Map<string, string>
    }
