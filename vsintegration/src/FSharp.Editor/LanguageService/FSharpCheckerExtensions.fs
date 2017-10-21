[<AutoOpen>]
module internal Microsoft.VisualStudio.FSharp.Editor.FSharpCheckerExtensions

open System
open System.Threading.Tasks
open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Text
open Microsoft.FSharp.Compiler
open Microsoft.FSharp.Compiler.Ast
open Microsoft.FSharp.Compiler.SourceCodeServices

type CheckResults =
    | Ready of (FSharpParseFileResults * FSharpCheckFileResults) option
    | StillRunning of Async<(FSharpParseFileResults * FSharpCheckFileResults) option>
    
type FSharpChecker with
    member checker.ParseDocument(document: Document, parsingOptions: FSharpParsingOptions, sourceText: string, userOpName: string) =
        asyncMaybe {
            let! fileParseResults = checker.ParseFile(document.FilePath, sourceText, parsingOptions, userOpName=userOpName) |> liftAsync
            return! fileParseResults.ParseTree
        }

    member checker.ParseDocument(document: Document, parsingOptions: FSharpParsingOptions, sourceText: SourceText, userOpName: string) =
        checker.ParseDocument(document, parsingOptions, sourceText=sourceText.ToString(), userOpName=userOpName)

    member checker.ParseAndCheckDocument(filePath: string, textVersionHash: int, sourceText: string, options: FSharpProjectOptions, allowStaleResults: bool, userOpName: string) =
        let parseAndCheckFile =
            async {
                let! parseResults, checkFileAnswer = checker.ParseAndCheckFileInProject(filePath, textVersionHash, sourceText, options, userOpName=userOpName)
                return
                    match checkFileAnswer with
                    | FSharpCheckFileAnswer.Aborted -> 
                        None
                    | FSharpCheckFileAnswer.Succeeded(checkFileResults) ->
                        Some (parseResults, checkFileResults)
            }

        let tryGetFreshResultsWithTimeout() : Async<CheckResults> =
            async {
                try
                    let! worker = Async.StartChild(parseAndCheckFile, 2000)
                    let! result = worker 
                    return Ready result
                with :? TimeoutException ->
                    return StillRunning parseAndCheckFile
            }

        let bindParsedInput(results: (FSharpParseFileResults * FSharpCheckFileResults) option) =
            match results with
            | Some(parseResults, checkResults) ->
                match parseResults.ParseTree with
                | Some parsedInput -> Some (parseResults, parsedInput, checkResults)
                | None -> None
            | None -> None

        if allowStaleResults then
            async {
                let! freshResults = tryGetFreshResultsWithTimeout()
                    
                let! results =
                    match freshResults with
                    | Ready x -> async.Return x
                    | StillRunning worker ->
                        async {
                            match allowStaleResults, checker.TryGetRecentCheckResultsForFile(filePath, options) with
                            | true, Some (parseResults, checkFileResults, _) ->
                                return Some (parseResults, checkFileResults)
                            | _ ->
                                return! worker
                        }
                return bindParsedInput results
            }
        else parseAndCheckFile |> Async.map bindParsedInput


    member checker.ParseAndCheckDocument(document: Document, options: FSharpProjectOptions, allowStaleResults: bool, userOpName: string, ?sourceText: SourceText) =
        async {
            let! cancellationToken = Async.CancellationToken
            let! sourceText =
                match sourceText with
                | Some x -> async.Return x
                | None -> document.GetTextAsync(cancellationToken)  |> Async.AwaitTask
            let! textVersion = document.GetTextVersionAsync(cancellationToken) |> Async.AwaitTask
            return! checker.ParseAndCheckDocument(document.FilePath, textVersion.GetHashCode(), sourceText.ToString(), options, allowStaleResults, userOpName=userOpName)
        }


    member checker.TryParseAndCheckFileInProject (projectOptions, fileName, source, userOpName) = async {
        let! (parseResults, checkAnswer) = checker.ParseAndCheckFileInProject (fileName,0, source,projectOptions, userOpName=userOpName)
        match checkAnswer with
        | FSharpCheckFileAnswer.Aborted ->  return  None
        | FSharpCheckFileAnswer.Succeeded checkResults -> return Some (parseResults,checkResults)
    }


    member checker.GetAllUsesOfAllSymbolsInSourceString (projectOptions, fileName, source: string, checkForUnusedOpens, userOpName) = async {
                  
        let! parseAndCheckResults = checker.TryParseAndCheckFileInProject (projectOptions, fileName, source, userOpName=userOpName)
        match parseAndCheckResults with
        | None -> return [||]
        | Some(_parseResults,checkResults) ->
            let! fsharpSymbolsUses = checkResults.GetAllUsesOfAllSymbolsInFile()
            let allSymbolsUses =
                fsharpSymbolsUses
                |> Array.map (fun symbolUse -> 
                    let fullNames = 
                        match symbolUse.Symbol with
                        // Make sure that unsafe manipulation isn't executed if unused opens are disabled
                        | _ when not checkForUnusedOpens -> None
                        | Symbol.MemberFunctionOrValue func when func.IsExtensionMember ->
                            if func.IsProperty then
                                let fullNames =
                                    [|  if func.HasGetterMethod then
                                            match func.GetterMethod.EnclosingEntity with 
                                            | Some e -> yield e.TryGetFullName()
                                            | None -> ()
                                        if func.HasSetterMethod then
                                            match func.SetterMethod.EnclosingEntity with 
                                            | Some e -> yield e.TryGetFullName()
                                            | None -> ()
                                    |]
                                    |> Array.choose id
                                match fullNames with
                                | [||]  -> None 
                                | _     -> Some fullNames
                            else 
                                match func.EnclosingEntity with
                                // C# extension method
                                | Some (Symbol.FSharpEntity Symbol.Class) ->
                                    let fullName = symbolUse.Symbol.FullName.Split '.'
                                    if fullName.Length > 2 then
                                        (* For C# extension methods FCS returns full name including the class name, like:
                                            Namespace.StaticClass.ExtensionMethod
                                            So, in order to properly detect that "open Namespace" actually opens ExtensionMethod,
                                            we remove "StaticClass" part. This makes C# extension methods looks identically 
                                            with F# extension members.
                                        *)
                                        let fullNameWithoutClassName =
                                            Array.append fullName.[0..fullName.Length - 3] fullName.[fullName.Length - 1..]
                                        Some [|String.Join (".", fullNameWithoutClassName)|]
                                    else None
                                | _ -> None
                        // Operators
                        | Symbol.MemberFunctionOrValue func ->
                            match func with
                            | Symbol.Constructor _ ->
                                // full name of a constructor looks like "UnusedSymbolClassifierTests.PrivateClass.( .ctor )"
                                // to make well formed full name parts we cut "( .ctor )" from the tail.
                                let fullName = func.FullName
                                let ctorSuffix = ".( .ctor )"
                                let fullName =
                                    if fullName.EndsWith ctorSuffix then 
                                        fullName.[0..fullName.Length - ctorSuffix.Length - 1]
                                    else fullName
                                Some [| fullName |]
                            | _ -> 
                                Some [| yield func.FullName 
                                        match func.TryGetFullCompiledOperatorNameIdents() with
                                        | Some idents -> yield String.concat "." idents
                                        | None -> ()
                                    |]
                        | Symbol.FSharpEntity e ->
                            match e with
                            | e, Symbol.Attribute, _ ->
                                e.TryGetFullName ()
                                |> Option.map (fun fullName ->
                                    [| fullName; fullName.Substring(0, fullName.Length - "Attribute".Length) |])
                            | e, _, _ -> 
                                e.TryGetFullName () |> Option.map (fun fullName -> [| fullName |])
                        | Symbol.RecordField _
                        | Symbol.UnionCase _ as symbol ->
                            Some [| let fullName = symbol.FullName
                                    yield fullName
                                    let idents = fullName.Split '.'
                                    // Union cases/Record fields can be accessible without mentioning the enclosing type. 
                                    // So we add a FullName without having the type part.
                                    if idents.Length > 1 then
                                        yield String.Join (".", Array.append idents.[0..idents.Length - 3] idents.[idents.Length - 1..])
                                |]
                        |  _ -> None
                        |> Option.defaultValue [|symbolUse.Symbol.FullName|]
                        |> Array.map (fun fullName -> fullName.Split '.')
                  
                    {   SymbolUse = symbolUse
                        IsUsed = true
                        FullNames = fullNames 
                    })
            return allSymbolsUses 
        }

