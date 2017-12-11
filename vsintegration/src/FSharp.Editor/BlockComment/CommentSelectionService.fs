namespace Microsoft.VisualStudio.FSharp.Editor

open Microsoft.CodeAnalysis.CommentSelection
open Microsoft.CodeAnalysis.Host.Mef
open System.Composition
open System.Threading.Tasks

[<Shared>]
[<ExportLanguageService(typeof<ICommentSelectionService>, FSharpConstants.FSharpLanguageName)>]
type CommentSelectionService() =
    interface ICommentSelectionService with
        member this.GetInfoAsync(_document, _textSpan, _cancellationToken) =
            Task.FromResult(CommentSelectionInfo(supportsSingleLineComment=true,
                                                 supportsBlockComment=true,
                                                 singleLineCommentString="//",
                                                 blockCommentStartString="(*",
                                                 blockCommentEndString="*)"))

        member this.FormatAsync(document, _changes, _cancellationToken) = Task.FromResult(document)
