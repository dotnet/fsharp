namespace FSharp.Compiler.Service

open FSharp.Compiler.Text

[<Sealed>]
type Source (filePath: string, sourceText: ISourceText) = 

    member __.FilePath = filePath

    member __.SourceText = sourceText