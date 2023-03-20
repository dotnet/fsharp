
// Microsoft.CodeAnalysis.ExternalAccess.FSharp.TaskList.FSharpTaskListService

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.ComponentModel.Composition
open Microsoft.CodeAnalysis.Text
open FSharp.Compiler.CodeAnalysis
open System.Runtime.InteropServices
open Microsoft.CodeAnalysis.ExternalAccess.FSharp.Editor
open Microsoft.CodeAnalysis.ExternalAccess.FSharp.TaskList
open FSharp.Compiler
open System.Collections.Immutable

open System.Diagnostics

[<Export(typeof<IFSharpTaskListService>)>]
type internal FSharpTaskListService 
    [<ImportingConstructor>]
    (
    ) =

    interface IFSharpTaskListService with
        member _.GetTaskListItemsAsync(doc,desc,cancellationToken) = 
            asyncMaybe{
                let foundTaskItems = ImmutableArray.CreateBuilder()
                let! sourceText = doc.GetTextAsync(cancellationToken)
                let! _, _, parsingOptions, _ = doc.GetFSharpCompilationOptionsAsync(nameof(FSharpTaskListService)) |> liftAsync
                let defines = CompilerEnvironment.GetConditionalDefinesForEditing parsingOptions
                Debug.WriteLine($"{doc.FilePath} is having {sourceText.Lines.Count} lines")
                for line in sourceText.Lines do                
                    let lineTxt = line.ToString()
                    Debug.WriteLine($"Text is = '{lineTxt}'")
                    let granularTokens = 
                        Tokenizer.tokenizeLine(doc.Id, sourceText, line.Span.Start, doc.FilePath, defines) 
                        |> Array.filter(fun t -> t.Kind = LexerSymbolKind.Comment)

                    let contractedTokens = 
                        ([],granularTokens) 
                        ||> Array.fold (fun acc token -> 
                            let token = {|Left = token.LeftColumn; Right = token.RightColumn|} 
                            match acc with
                            | [] -> [token]
                            | head :: tail when token.Left-head.Right <= 1   -> {|token with Left = head.Left|} :: tail
                            | _  -> token :: acc )

                    let hmmcontractedCommentTokens = contractedTokens |> List.map (fun x -> x.Left, x.Right, lineTxt.Substring(x.Left,1 + x.Right - x.Left))
                    Debug.WriteLine($"AFTER %A{hmmcontractedCommentTokens}")

                    for ct in contractedTokens do                        
                        for d in desc do 
                            let idx = lineTxt.IndexOf(value = d.Text,startIndex = ct.Left, count = 1+(ct.Right - ct.Left), comparisonType = StringComparison.OrdinalIgnoreCase)  
                            
                            if idx > -1 then 
                                let taskLength = 1+ct.Right-idx                
                                // A descriptor followed by another letter is not a todocomment, like todoabc. But TODO, TODO2 or TODO: should be.
                                if (idx+taskLength) >= lineTxt.Length || not (Char.IsLetter(lineTxt.[idx+taskLength])) then
                                    foundTaskItems.Add(new FSharpTaskListItem(d, lineTxt.Substring(idx,taskLength), doc, new TextSpan(line.Span.Start+idx, taskLength)))
                                else Debug.WriteLine($"Failed terminating criteria: idx= {idx}, len = {taskLength}, %A{ct}")
                       
                return foundTaskItems.ToImmutable()
            } 
            |> Async.map (Option.defaultValue ImmutableArray.Empty)
            |> RoslynHelpers.StartAsyncAsTask cancellationToken