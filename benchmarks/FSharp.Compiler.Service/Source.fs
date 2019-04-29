namespace FSharp.Compiler.Service

open FSharp.Compiler.Service.Utilities

type Source = 
    {
        filePath: string
        asyncLazyParseResult: AsyncLazy<ParseResult>
    }

    member this.FilePath = this.filePath
    
    member this.GetParseResultAsync () =
        this.asyncLazyParseResult.GetValueAsync ()

    static member Create (filePath, parsingInfo) =
        {
            filePath = filePath
            asyncLazyParseResult =
                AsyncLazy(async {
                    return Parser.Parse parsingInfo
                })
        }
