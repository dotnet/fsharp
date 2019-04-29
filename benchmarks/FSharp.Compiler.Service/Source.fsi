namespace FSharp.Compiler.Service

open FSharp.Compiler.Text

[<Sealed>]
type Source = 

   new: filePath: string * sourceText: ISourceText -> Source

   member FilePath: string

   member SourceText: ISourceText