namespace FSharp.Compiler.Compilation

open System.Threading
open System.Runtime.CompilerServices
open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.Host
open FSharp.Compiler.Compilation.Utilities
open FSharp.Compiler.AbstractIL.Internal.Library
open FSharp.Compiler
open FSharp.Compiler.Text
open FSharp.Compiler.CompileOps

[<RequireQualifiedAccess>]
type SourceValueStorage =
    | SourceText of ITemporaryTextStorage 
    | Stream of ITemporaryStreamStorage

[<Sealed>]
type SourceSnapshot (filePath: string, value: SourceValueStorage) =

    let gate = obj ()
    
    member __.FilePath = filePath

    member __.GetSourceValueAsync () =
        async {
            let! cancellationToken = Async.CancellationToken
            try
                Monitor.Enter gate
                match value with
                | SourceValueStorage.SourceText storage -> 
                    let! sourceText = storage.ReadTextAsync cancellationToken |> Async.AwaitTask
                    return SourceValue.SourceText sourceText
                | SourceValueStorage.Stream storage -> 
                    let! stream = storage.ReadStreamAsync cancellationToken |> Async.AwaitTask
                    return SourceValue.Stream stream
            finally
                Monitor.Exit gate
        }

    member __.GetSourceTextAsync () =
        async {
            let! cancellationToken = Async.CancellationToken
            match value with
            | SourceValueStorage.SourceText storage -> 
                let! sourceText = storage.ReadTextAsync cancellationToken |> Async.AwaitTask
                return sourceText
            | SourceValueStorage.Stream storage -> 
                let! stream = storage.ReadStreamAsync cancellationToken |> Async.AwaitTask
                return SourceText.From (stream)
        }


//type ParsingInfo =
//    {
//        tcConfig: TcConfig
//        isLastFileOrScript: bool
//        isExecutable: bool
//        conditionalCompilationDefines: string list
//        filePath: string
//    }

//[<RequireQualifiedAccess>]
//module Parser =

//    let Parse (info: ParsingInfo) sourceValue =
//        let tcConfig = info.tcConfig
//        let filePath = info.filePath
//        let errorLogger = CompilationErrorLogger("ParseFile", tcConfig.errorSeverityOptions)

//        let input =
//            match sourceValue with
//            | SourceValue.SourceText sourceText ->
//                ParseOneInputSourceText (info.tcConfig, Lexhelp.LexResourceManager (), info.conditionalCompilationDefines, filePath, sourceText.ToFSharpSourceText (), (info.isLastFileOrScript, info.isExecutable), errorLogger)
//            | SourceValue.Stream stream ->
//                ParseOneInputStream (info.tcConfig, Lexhelp.LexResourceManager (), info.conditionalCompilationDefines, filePath, stream, (info.isLastFileOrScript, info.isExecutable), errorLogger)
//        (input, errorLogger.GetErrors ())

[<Sealed;AbstractClass;Extension>]
type ITemporaryStorageServiceExtensions =

    [<Extension>]
    static member CreateSourceSnapshot (this: ITemporaryStorageService, filePath: string, sourceText: SourceText) =
        cancellable {
            let! cancellationToken = Cancellable.token ()
            let storage = this.CreateTemporaryTextStorage cancellationToken
            storage.WriteText (sourceText, cancellationToken)
            let sourceValueStorage = SourceValueStorage.SourceText storage
            return SourceSnapshot (filePath, sourceValueStorage)
        }

[<Sealed>]
type SyntaxTree (filePath: string, parsingInfo: ParsingInfo, asyncLazyWeakGetParseResult: AsyncLazyWeak<ParseResult>) =

    member __.FilePath = filePath

    member __.ParsingInfo = parsingInfo

    member this.GetParseResultAsync () =
        asyncLazyWeakGetParseResult.GetValueAsync ()
