namespace FSharp.Compiler.SourceCodeServices.ProjectCrackerTool

[<CLIMutable>]
type ProjectOptions =
  {
    ProjectFile: string
    Options: string[]
    ReferencedProjectOptions: (string * ProjectOptions)[]
    LogOutput: string
    Error: string
  }
