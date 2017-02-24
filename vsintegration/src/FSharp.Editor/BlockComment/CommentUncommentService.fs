namespace Microsoft.VisualStudio.FSharp.Editor

open Microsoft.CodeAnalysis.Editor.Implementation.CommentSelection
open Microsoft.CodeAnalysis.Host.Mef
open System.Composition

[<Shared>]
[<ExportLanguageService(typeof<ICommentUncommentService>, FSharpCommonConstants.FSharpLanguageName)>]
type CommentUncommentService() =
    interface ICommentUncommentService with
        member this.SingleLineCommentString = "//"
        member this.SupportsBlockComment = true
        member this.BlockCommentStartString = "(*"
        member this.BlockCommentEndString = "*)"
        member this.Format(document, _changes, _cancellationToken) = document