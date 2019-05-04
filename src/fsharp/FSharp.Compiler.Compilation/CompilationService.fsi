namespace FSharp.Compiler.Compilation

[<Sealed>]
type CompilationService =

    new: compilationCacheSize: int * keepStrongly: int * Microsoft.CodeAnalysis.Workspace -> CompilationService

    member CreateSourceSnapshot: filePath: string * Microsoft.CodeAnalysis.Text.SourceText -> SourceSnapshot

    member CreateCompilation: CompilationOptions -> Compilation
