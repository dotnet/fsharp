
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
    ) as this=

    let getDefines (doc:Microsoft.CodeAnalysis.Document) = 
        asyncMaybe { 
            let! _, _, parsingOptions, _ = doc.GetFSharpCompilationOptionsAsync(nameof(FSharpTaskListService)) |> liftAsync 
            return CompilerEnvironment.GetConditionalDefinesForEditing parsingOptions }
        |> Async.map (Option.defaultValue [])

    let extractContractedComments (tokens:Tokenizer.SavedTokenInfo[]) = 
        let granularTokens = tokens |> Array.filter(fun t -> t.Kind = LexerSymbolKind.Comment)               

        let contractedTokens = 
            ([],granularTokens) 
            ||> Array.fold (fun acc token -> 
                let token = {|Left = token.LeftColumn; Right = token.RightColumn|} 
                match acc with
                | [] -> [token]
                | head :: tail when token.Left-head.Right <= 1   -> {|token with Left = head.Left|} :: tail
                | _  -> token :: acc )
        contractedTokens
        

    member _.GetTaskListItems(
        doc:Microsoft.CodeAnalysis.Document, 
        sourceText : SourceText,
        defines : string list,
        descriptors: (string*FSharpTaskListDescriptor)[], 
        cancellationToken) = 
           
            let foundTaskItems = ImmutableArray.CreateBuilder(initialCapacity=0)

            for line in sourceText.Lines do  

                let contractedTokens = 
                    Tokenizer.tokenizeLine(doc.Id, sourceText, line.Span.Start, doc.FilePath, defines, cancellationToken)
                    |> extractContractedComments                      

                for ct in contractedTokens do    
                    let lineTxt = line.ToString()
                    let tokenSize = 1+(ct.Right - ct.Left)

                    for (dText,d) in descriptors do                     
                        let idx = lineTxt.IndexOf(dText, ct.Left, tokenSize, StringComparison.OrdinalIgnoreCase)  
                            
                        if idx > -1 then 
                            let taskLength = 1+ct.Right-idx
                            let idxAfterDesc = idx + dText.Length
                            // A descriptor followed by another letter is not a todocomment, like todoabc. But TODO, TODO2 or TODO: should be.
                            if idxAfterDesc >= lineTxt.Length || not (Char.IsLetter(lineTxt.[idxAfterDesc])) then
                                let taskText = lineTxt.Substring(idx,taskLength).TrimEnd([|'*';')'|])
                                let taskSpan = new TextSpan(line.Span.Start+idx, taskText.Length)

                                foundTaskItems.Add(new FSharpTaskListItem(d, taskText, doc, taskSpan))                         
                       
    
            foundTaskItems.ToImmutable()
             

    interface IFSharpTaskListService with
        member _.GetTaskListItemsAsync(doc,desc,cancellationToken) = 
            backgroundTask{
                let descriptors = desc |> Seq.map (fun d -> d.Text, d) |> Array.ofSeq
                let! sourceText = doc.GetTextAsync(cancellationToken)             
                let! defines = doc |> getDefines
                return this.GetTaskListItems(doc, sourceText, defines,descriptors, cancellationToken)
            }

