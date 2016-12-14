namespace Microsoft.VisualStudio.FSharp.Editor

open System.ComponentModel.Composition
open Microsoft.CodeAnalysis.Editor
open Microsoft.CodeAnalysis.Shared.Extensions
open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.Text.Shared.Extensions
open Microsoft.VisualStudio.Text
open Microsoft.VisualStudio.Text.Operations
open Microsoft.VisualStudio.Utilities
open Microsoft.CodeAnalysis.Editor.Implementation.CommentSelection
open Microsoft.CodeAnalysis.Host.Mef
open Microsoft.CodeAnalysis
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