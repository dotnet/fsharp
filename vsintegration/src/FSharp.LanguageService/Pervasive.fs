namespace Microsoft.VisualStudio.FSharp.LanguageService

open Microsoft.VisualStudio.TextManager.Interop
open Microsoft.FSharp.Compiler
open Microsoft.FSharp.Compiler.SourceCodeServices

[<AutoOpen>]
module internal Pervasive =

    let fsharpRangeToTextSpan (m:Range.range) =
        TextSpan (iStartLine = m.StartLine-1, iEndLine = m.StartLine-1, iStartIndex = m.StartColumn, iEndIndex = m.StartColumn)

