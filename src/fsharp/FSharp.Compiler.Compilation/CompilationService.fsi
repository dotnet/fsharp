namespace FSharp.Compiler.Compilation

[<NoEquality;NoComparison>]
type CompilationServiceOptions =
    {
        CompilationGlobalOptions: CompilationGlobalOptions
        Workspace: Microsoft.CodeAnalysis.Workspace
    }

    static member Create: Microsoft.CodeAnalysis.Workspace -> CompilationServiceOptions

[<Sealed>]
type CompilationService =

    new: CompilationServiceOptions -> CompilationService

    member CreateSourceSnapshot: filePath: string * Microsoft.CodeAnalysis.Text.SourceText -> SourceSnapshot

    member CreateCompilation: CompilationOptions -> Compilation
