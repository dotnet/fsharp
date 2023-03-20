
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
                for line in sourceText.Lines do
                    let lineFirstPos = line.Start
                    let tokens = Tokenizer.tokenizeLine(doc.Id, sourceText, line.Span.Start, doc.FilePath, defines)
                    let commentTokens = tokens |> Array.filter(fun t -> t.Kind = LexerSymbolKind.Comment)
                    for ct in commentTokens do
                        let text = line.ToString()
                        for d in desc do 
                            let idx = text.IndexOf(value = text,startIndex = ct.LeftColumn, count = (ct.RightColumn - ct.LeftColumn), comparisonType = StringComparison.OrdinalIgnoreCase)                    
                            if idx > -1 && not(Char.IsLetter(text[idx+d.Text.Length])) then 
                                let taskLength = ct.RightColumn-idx                                
                                foundTaskItems.Add(new FSharpTaskListItem(d, text.Substring(idx,taskLength), doc, new TextSpan(line.Span.Start+idx, taskLength)))
                       
                foundTaskItems.ToImmutable()
            } 
            |> Async.map Option.toNullable
            |> RoslynHelpers.StartAsyncAsTask cancellationToken