namespace FSharp.Compiler.Service

open Microsoft.CodeAnalysis.Text
open FSharp.Compiler.Service.Utilities

[<NoEquality;NoComparison>]
type Source =
    {
        FilePath: string
        SourceTextOption: SourceText option
    }

[<NoEquality;NoComparison>]
type internal SyntaxTree =
    {
        filePath: string
        asyncLazyWeakGetParseResult: AsyncLazyWeak<ParseResult>
    }

    member FilePath: string

    member GetParseResultAsync: unit -> Async<ParseResult>