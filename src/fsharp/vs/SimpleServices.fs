// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

#if SILVERLIGHT
namespace Microsoft.FSharp.Compiler.Interactive

module Runner = 

    type public InteractiveConsole(argv:string[],reader:System.IO.TextReader, writer:System.IO.TextWriter, error:System.IO.TextWriter) =
        do
            Microsoft.FSharp.Core.Printf.setWriter writer
            Microsoft.FSharp.Core.Printf.setError error
        let session = Microsoft.FSharp.Compiler.Interactive.Shell.FsiEvaluationSession(argv, reader, writer, error)
        member x.Run() = session.Run()
        member x.Interrupt() = session.Interrupt()
#endif

namespace Microsoft.FSharp.Compiler.SimpleSourceCodeServices

    open System
    open System.IO
    open System.Text
    open Microsoft.FSharp.Compiler.SourceCodeServices
    open Microsoft.FSharp.Compiler.Driver
    open Microsoft.FSharp.Compiler
    open Microsoft.FSharp.Compiler.ErrorLogger

    [<AutoOpen>]
    module private Utils =

        let buildFormatComment (xmlCommentRetriever: string * string -> string) cmt (sb: StringBuilder) =
            match cmt with
            | XmlCommentText(s) -> sb.AppendLine(s) |> ignore
            | XmlCommentSignature(file, signature) ->
                let comment = xmlCommentRetriever (file, signature)
                if (not (comment.Equals(null))) && comment.Length > 0 then sb.AppendLine(comment) |> ignore
            | XmlCommentNone -> ()

        let buildFormatElement isSingle el (sb: StringBuilder) xmlCommentRetriever =
            match el with
            | DataTipElementNone -> ()
            | DataTipElement(it, comment) ->
                sb.AppendLine(it) |> buildFormatComment xmlCommentRetriever comment
            | DataTipElementGroup(items) ->
                let items, msg =
                  if items.Length > 10 then
                    (items |> Seq.take 10 |> List.ofSeq),
                      sprintf "   (+%d other overloads)" (items.Length - 10)
                  else items, null
                if isSingle && items.Length > 1 then
                  sb.AppendLine("Multiple overloads") |> ignore
                for (it, comment) in items do
                  sb.AppendLine(it) |> buildFormatComment xmlCommentRetriever comment
                if msg <> null then sb.AppendFormat(msg) |> ignore
            | DataTipElementCompositionError(err) ->
                sb.Append("Composition error: " + err) |> ignore

        // Convert DataTipText to string
        let formatTip tip xmlCommentRetriever =
            let commentRetriever = defaultArg xmlCommentRetriever (fun _ -> "")
            let sb = new StringBuilder()
            match tip with
            | DataTipText([single]) -> buildFormatElement true single sb commentRetriever
            | DataTipText(its) -> for item in its do buildFormatElement false item sb commentRetriever
            sb.ToString().Trim('\n', '\r')

    /// Represents a declaration returned by GetDeclarations
    type Declaration internal (name: string, description: unit -> string) = 
        /// Get the name of a declaration
        member x.Name = name
        /// Compute the description for a declaration
        member x.GetDescription() = description()

    /// Represents the results of type checking
    type TypeCheckResults internal (info: Microsoft.FSharp.Compiler.SourceCodeServices.UntypedParseInfo,
                                    results:Microsoft.FSharp.Compiler.SourceCodeServices.TypeCheckResults,
                                    source: string[]) = 

        let identToken = Microsoft.FSharp.Compiler.Parser.tagOfToken (Microsoft.FSharp.Compiler.Parser.IDENT "")
        let hasChangedSinceLastTypeCheck _ = false

        /// Return the errors resulting from the type-checking
        member x.Errors = results.Errors

        /// Get the declarations at the given code location.
        member x.GetDeclarations(line, col, names, residue, ?xmlCommentRetriever) =
            async { let! items = results.GetDeclarations(Some info, (line, col), source.[line], (names, residue), hasChangedSinceLastTypeCheck)
                    return [| for i in items.Items -> Declaration(i.Name, (fun () -> formatTip i.DescriptionText xmlCommentRetriever)) |] }

        member x.GetRawDeclarations(line, col, names, residue, formatter:DataTipText->string[]) =
            async { let! items = results.GetDeclarations(Some info, (line, col), source.[line], (names, residue), hasChangedSinceLastTypeCheck)
                    return [| for i in items.Items -> i.Name, (fun() -> formatter i.DescriptionText), i.Glyph |] }

        /// Get the Visual Studio F1-help keyword for the item at the given position
        member x.GetF1Keyword(line, col, names) =
            results.GetF1Keyword((line, col), source.[line], names)

        /// Get the data tip text at the given position
        member x.GetDataTipText(line, col, names, ?xmlCommentRetriever) =
            let tip = results.GetDataTipText((line, col), source.[line], names, identToken)
            formatTip tip xmlCommentRetriever

        member x.GetRawDataTipText(line, col, names) =
            results.GetDataTipText((line, col), source.[line], names, identToken)

        /// Get the location of the declaration at the given position
        member x.GetDeclarationLocation(line: int, col: int, names, isDecl) =
            results.GetDeclarationLocation((line, col), source.[line], names, identToken, isDecl)

        /// Get the full type checking results 
        member x.FullResults = results

    /// Provides simple services for checking and compiling F# scripts
    type public SimpleSourceCodeServices() =

        let checker = InteractiveChecker.Create(NotifyFileTypeCheckStateIsDirty(fun _ -> ()))
        let fileversion = 0
        let loadTime = DateTime.Now
 
        /// Tokenize a single line, returning token information and a tokenization state represented by an integer
        member x.TokenizeLine (line: string, state: int64) : TokenInformation[] * int64 = 
            let tokenizer = SourceTokenizer([], "example.fsx")
            let lineTokenizer = tokenizer.CreateLineTokenizer line
            let state = ref (None, state)
            let tokens = 
                [| while (state := lineTokenizer.ScanToken (snd !state); (fst !state).IsSome) do
                       yield (fst !state).Value |]
            tokens, snd !state 

        /// Tokenize an entire file, line by line
        member x.TokenizeFile (source: string) : TokenInformation[][] = 
            let lines = source.Split('\n')
            let tokens = 
                [| let state = ref 0L
                   for line in lines do 
                         let tokens, n = x.TokenizeLine(line, !state) 
                         state := n; 
                         yield tokens |]
            tokens

        /// Return information about matching braces in a single file.
        member x.MatchBraces (filename, source: string) : (Range * Range) [] = 
            let options = checker.GetCheckOptionsFromScriptRoot(filename, source, loadTime)
            checker.MatchBraces(filename, source,  options)

        /// For errors, quick info, goto-definition, declaration list intellisense, method overload intellisense
        member x.TypeCheckScript (filename:string,source:string,otherFlags:string[]) = 
            let options = checker.GetCheckOptionsFromScriptRoot(filename, source, loadTime, otherFlags)
            checker.StartBackgroundCompile options
            // wait for the antecedent to appear
            checker.WaitForBackgroundCompile()
            // do an untyped parse
            let info = checker.UntypedParse(filename, source, options)
            // do an typecheck
            let textSnapshotInfo = "" // TODO
            let typedInfo = checker.TypeCheckSource(info, filename, fileversion, source, options, IsResultObsolete (fun _ -> false), textSnapshotInfo)
            // return the info
            match typedInfo with 
            | NoAntecedant -> invalidOp "no antecedant"
            | Aborted -> invalidOp "aborted"
            | TypeCheckSucceeded res -> TypeCheckResults(info, res, source.Split('\n'))

        /// Compile using the given flags.  Source files names are resolved via the FileSystem API. The output file must be given by a -o flag. 
        member x.Compile (argv: string[])  = 
            let errors = ResizeArray<_>()

            let errorSink warn exn = 
                let mainError,relatedErrors = Build.SplitRelatedErrors exn 
                let oneError trim e = errors.Add(ErrorInfo.CreateFromException (e, warn, trim, Range.range0))
                oneError false mainError
                List.iter (oneError true) relatedErrors

            let errorLogger = 
                { new ErrorLogger("CompileAPI") with 
                    member x.WarnSinkImpl(exn) = errorSink true exn
                    member x.ErrorSinkImpl(exn) = errorSink false exn
                    member x.ErrorCount = errors |> Seq.filter (fun e -> e.Severity = Severity.Error) |> Seq.length }

            let createErrorLogger _ =  errorLogger
      
            let result = 
                use unwindParsePhase = PushThreadBuildPhaseUntilUnwind (BuildPhase.Parse)            
                use unwindEL_2 = PushErrorLoggerPhaseUntilUnwind (fun _ -> errorLogger)
                let exiter = { new Exiter with member x.Exit n = raise StopProcessing }
                try 
                    mainCompile (argv, true, exiter, createErrorLogger); 
                    0
                with e -> 
                    stopProcessingRecovery e Range.range0
                    1
        
            errors.ToArray(), result

        /// Compiles to a dynamic assembly usinng the given flags.  Any source files names 
        /// are resolved via the FileSystem API. An output file name must be given by a -o flag, but this will not
        /// be written - instead a dynamic assembly will be created and loaded.
        ///
        /// If the 'execute' parameter is given the entry points for the code are executed and 
        /// the given TextWriters are used for the stdout and stderr streams respectively. In this 
        /// case, a global setting is modified during the execution.
        member x.CompileToDynamicAssembly (otherFlags: string[], execute: (TextWriter * TextWriter) option)  = 
            match execute with
            | Some (writer,error) -> 
#if SILVERLIGHT
                Microsoft.FSharp.Core.Printf.setWriter writer
                Microsoft.FSharp.Core.Printf.setError error
#else
                System.Console.SetOut writer
                System.Console.SetError error
#endif
            | None -> ()
            let tcImportsRef = ref None
            let res = ref None
            tcImportsCapture <- Some (fun tcImports -> tcImportsRef := Some tcImports)
            dynamicAssemblyCreator <- 
                Some (fun (_tcConfig,ilGlobals,_errorLogger,outfile,_pdbfile,ilxMainModule,_signingInfo) ->
                    let assemblyBuilder = System.AppDomain.CurrentDomain.DefineDynamicAssembly(System.Reflection.AssemblyName(System.IO.Path.GetFileNameWithoutExtension outfile),System.Reflection.Emit.AssemblyBuilderAccess.Run)
                    let debugInfo = false
                    let moduleBuilder = assemblyBuilder.DefineDynamicModule("IncrementalModule",debugInfo)     
                    let _emEnv,execs = 
                        Microsoft.FSharp.Compiler.AbstractIL.ILRuntimeWriter.emitModuleFragment 
                            (ilGlobals ,
                             Microsoft.FSharp.Compiler.AbstractIL.ILRuntimeWriter.emEnv0,
                             assemblyBuilder,moduleBuilder,
                             ilxMainModule,
                             debugInfo,
                             (fun s -> 
                                 match tcImportsRef.Value.Value.TryFindExistingFullyQualifiedPathFromAssemblyRef s with 
                                 | Some res -> Some (Choice1Of2 res)
                                 | None -> None))
                    if execute.IsSome then 
                        for exec in execs do 
                            match exec() with 
                            | None -> ()
                            | Some exn -> raise exn
                    for resource in ilxMainModule.Resources.AsList do 
                        if Build.IsReflectedDefinitionsResource resource then 
                            Quotations.Expr.RegisterReflectedDefinitions(assemblyBuilder, moduleBuilder.Name, resource.Bytes);
                    res := Some assemblyBuilder)
            

            try 
                let errorsAndWarnings, result = x.Compile otherFlags
                let assemblyOpt = 
                    match res.Value with 
                    | None -> None
                    | Some a ->  Some (a :> System.Reflection.Assembly)
                errorsAndWarnings, result, assemblyOpt
            finally
                tcImportsCapture <- None
                dynamicAssemblyCreator <- None

