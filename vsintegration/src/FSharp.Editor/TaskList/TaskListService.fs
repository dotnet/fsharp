
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

    let getDefines (doc:Microsoft.CodeAnalysis.Document) = 
        asyncMaybe { 
            let! _, _, parsingOptions, _ = doc.GetFSharpCompilationOptionsAsync(nameof(FSharpTaskListService)) |> liftAsync 
            return CompilerEnvironment.GetConditionalDefinesForEditing parsingOptions }
        |> Async.map (Option.defaultValue [])


    interface IFSharpTaskListService with
        member _.GetTaskListItemsAsync(doc,desc,cancellationToken) = 
            backgroundTask{
                let foundTaskItems = ImmutableArray.CreateBuilder(initialCapacity=0)
                let! sourceText = doc.GetTextAsync(cancellationToken)             
                let! defines = doc |> getDefines
               
                for line in sourceText.Lines do  
                 
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

                    for ct in contractedTokens do    
                        let lineTxt = line.ToString()
                        let tokenSize = 1+(ct.Right - ct.Left)

                        for d in desc do                     
                            let idx = lineTxt.IndexOf(d.Text, ct.Left, tokenSize, StringComparison.OrdinalIgnoreCase)  
                            
                            if idx > -1 then 
                                let taskLength = 1+ct.Right-idx                
                                // A descriptor followed by another letter is not a todocomment, like todoabc. But TODO, TODO2 or TODO: should be.
                                if (idx+taskLength) >= lineTxt.Length || not (Char.IsLetter(lineTxt.[idx+taskLength])) then
                                    let taskText = lineTxt.Substring(idx,taskLength).TrimEnd([|'*';')'|])
                                    let taskSpan = new TextSpan(line.Span.Start+idx, taskText.Length)
                                    foundTaskItems.Add(new FSharpTaskListItem(d, taskText, doc, taskSpan))                         
                       
    
                return foundTaskItems.ToImmutable()
            } 