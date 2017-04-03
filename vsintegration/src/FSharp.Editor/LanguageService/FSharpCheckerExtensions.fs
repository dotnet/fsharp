[<AutoOpen>]
module internal Microsoft.VisualStudio.FSharp.Editor.FSharpCheckerExtensions

open System
open System.Threading.Tasks
open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Text
open Microsoft.FSharp.Compiler
open Microsoft.FSharp.Compiler.Ast
open Microsoft.FSharp.Compiler.SourceCodeServices
open TypedAstUtils

type CheckResults =
    | Ready of (FSharpParseFileResults * FSharpCheckFileResults) option
    | StillRunning of Async<(FSharpParseFileResults * FSharpCheckFileResults) option>
    
type FSharpChecker with
    member this.ParseDocument(document: Document, options: FSharpProjectOptions, sourceText: string) =
        asyncMaybe {
            let! fileParseResults = this.ParseFileInProject(document.FilePath, sourceText, options) |> liftAsync
            return! fileParseResults.ParseTree
        }

    member this.ParseDocument(document: Document, options: FSharpProjectOptions, ?sourceText: SourceText) =
        asyncMaybe {
            let! sourceText =
                match sourceText with
                | Some x -> Task.FromResult x
                | None -> document.GetTextAsync()
            return! this.ParseDocument(document, options, sourceText.ToString())
        }

    member this.ParseAndCheckDocument(filePath: string, textVersionHash: int, sourceText: string, options: FSharpProjectOptions, allowStaleResults: bool) : Async<(FSharpParseFileResults * Ast.ParsedInput * FSharpCheckFileResults) option> =
        let parseAndCheckFile =
            async {
                let! parseResults, checkFileAnswer = this.ParseAndCheckFileInProject(filePath, textVersionHash, sourceText, options)
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
                            match allowStaleResults, this.TryGetRecentCheckResultsForFile(filePath, options) with
                            | true, Some (parseResults, checkFileResults, _) ->
                                return Some (parseResults, checkFileResults)
                            | _ ->
                                return! worker
                        }
                return bindParsedInput results
            }
        else parseAndCheckFile |> Async.map bindParsedInput


    member this.ParseAndCheckDocument(document: Document, options: FSharpProjectOptions, allowStaleResults: bool, ?sourceText: SourceText) : Async<(FSharpParseFileResults * Ast.ParsedInput * FSharpCheckFileResults) option> =
        async {
            let! cancellationToken = Async.CancellationToken
            let! sourceText =
                match sourceText with
                | Some x -> Task.FromResult x
                | None -> document.GetTextAsync()
            let! textVersion = document.GetTextVersionAsync(cancellationToken)
            return! this.ParseAndCheckDocument(document.FilePath, textVersion.GetHashCode(), sourceText.ToString(), options, allowStaleResults)
        }


            member self.TryParseAndCheckFileInProject (projectOptions, fileName, source) = async {
        let! (parseResults, checkAnswer) = self.ParseAndCheckFileInProject (fileName,0, source,projectOptions)
        match checkAnswer with
        | FSharpCheckFileAnswer.Aborted ->  return  None
        | FSharpCheckFileAnswer.Succeeded checkResults -> return Some (parseResults,checkResults)
    }


    member self.GetAllUsesOfAllSymbolsInSourceString (projectOptions, fileName, source: string, checkForUnusedOpens) = async {
                  
        let! parseAndCheckResults = self.TryParseAndCheckFileInProject (projectOptions, fileName, source)
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
                        | TypedAstPatterns.MemberFunctionOrValue func when func.IsExtensionMember ->
                            if func.IsProperty then
                                let fullNames =
                                    [|  if func.HasGetterMethod then
                                            yield func.GetterMethod.EnclosingEntity.TryGetFullName()
                                        if func.HasSetterMethod then
                                            yield func.SetterMethod.EnclosingEntity.TryGetFullName()
                                    |]
                                    |> Array.choose id
                                match fullNames with
                                | [||]  -> None 
                                | _     -> Some fullNames
                            else 
                                match func.EnclosingEntity with
                                // C# extension method
                                | TypedAstPatterns.FSharpEntity TypedAstPatterns.Class ->
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
                        | TypedAstPatterns.MemberFunctionOrValue func ->
                            match func with
                            | TypedAstPatterns.Constructor _ ->
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
                        | TypedAstPatterns.FSharpEntity e ->
                            match e with
                            | e, TypedAstPatterns.Attribute, _ ->
                                e.TryGetFullName ()
                                |> Option.map (fun fullName ->
                                    [| fullName; fullName.Substring(0, fullName.Length - "Attribute".Length) |])
                            | e, _, _ -> 
                                e.TryGetFullName () |> Option.map (fun fullName -> [| fullName |])
                        | TypedAstPatterns.RecordField _
                        | TypedAstPatterns.UnionCase _ as symbol ->
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

