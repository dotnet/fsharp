namespace Microsoft.VisualStudio.FSharp.Editor

open System

open Microsoft.CodeAnalysis.Completion
open Microsoft.CodeAnalysis.ExternalAccess.FSharp.Completion

open FSharp.Compiler.SourceCodeServices

type internal KeywordCompletionProvider(projectInfoManager: FSharpProjectOptionsManager) =

    static let [<Literal>] IsKeywordPropName = "IsKeyword"

    static let keywordCompletionItems =
        Keywords.KeywordsWithDescription
        |> List.filter (fun (keyword, _) -> not (PrettyNaming.IsOperatorName keyword))
        |> List.sortBy (fun (keyword, _) -> keyword)
        |> List.mapi (fun n (keyword, description) ->
             FSharpCommonCompletionItem.Create(keyword, null, CompletionItemRules.Default, Nullable Glyph.Keyword, sortText = sprintf "%06d" (1000000 + n))
                .AddProperty("description", description)
                .AddProperty(IsKeywordPropName, ""))

    interface IFSharpCommonCompletionProvider with
         member _.ProvideCompletionsAsync(context) =
             asyncMaybe {    
                 let document = context.Document

                 let! ct = liftAsync Async.CancellationToken
                 let! sourceText = document.GetTextAsync(ct)
                 let defines = projectInfoManager.GetCompilationDefinesForEditingDocument(document)

                 do! Option.guard (CompletionUtils.shouldProvideCompletion(document.Id, document.FilePath, defines, sourceText, context.Position))

                 context.AddItems(keywordCompletionItems)
             } 
             |> Async.Ignore
             |> RoslynHelpers.StartAsyncUnitAsTask context.CancellationToken
 
         member _.IsInsertionTrigger(text, position, _) = FSharpCommonCompletionUtilities.IsStartingNewWord(text, position, (fun x -> Char.IsLetter x), (fun x -> Char.IsLetterOrDigit x))
 
         member _.GetTextChangeAsync(baseGetTextChangeAsync, selectedItem, ch, cancellationToken) = baseGetTextChangeAsync.Invoke(selectedItem, ch, cancellationToken)