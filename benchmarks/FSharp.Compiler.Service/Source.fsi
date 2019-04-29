namespace FSharp.Compiler.Service

open FSharp.Compiler.Service.Utilities

type internal Source = 
   {
        filePath: string
        asyncLazyParseResult: AsyncLazy<ParseResult>
   }

   member FilePath: string

   member GetParseResultAsync: unit -> Async<ParseResult>
 
   static member Create: parsingInfo: ParsingInfo -> Source