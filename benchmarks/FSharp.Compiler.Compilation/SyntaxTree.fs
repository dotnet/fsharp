namespace FSharp.Compiler.Service

open Microsoft.CodeAnalysis.Text
open FSharp.Compiler.Service.Utilities

type Source =
    {
        FilePath: string
        SourceTextOption: SourceText option
    }

type SyntaxTree =
    {
        filePath: string
        asyncLazyWeakGetParseResult: AsyncLazyWeak<ParseResult>
    }

    member this.FilePath = this.filePath

    member this.GetParseResultAsync () =
        this.asyncLazyWeakGetParseResult.GetValueAsync ()
