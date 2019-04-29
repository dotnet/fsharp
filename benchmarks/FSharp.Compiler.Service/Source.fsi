namespace FSharp.Compiler.Service

open FSharp.Compiler.Ast
open FSharp.Compiler.ErrorLogger
open FSharp.Compiler.Service.Utilities

type internal Source = 
   {
        filePath: string
        asyncLazyParseResult: AsyncLazy<ParseResult>
   }

   member FilePath: string

   member GetParseResultAsync: unit -> Async<ParseResult>
 
   static member Create: filePath: string * parsingInfo: ParsingInfo -> Source