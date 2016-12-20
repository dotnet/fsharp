[<AutoOpen>]
module Microsoft.VisualStudio.FSharp.Pervasive

open System


[<RequireQualifiedAccess>]
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Option =

    /// Gets the value associated with the option or the supplied default value.
    let inline getOrElse v = function
        | Some x -> x | None -> v

[<RequireQualifiedAccess>]
module String =   
    open System.IO

    let getLines (str: string) =
        use reader = new StringReader(str)
        [|  let mutable line = reader.ReadLine()
            while not (isNull line) do
                yield line
                line <- reader.ReadLine()
            if str.EndsWith("\n") then
            // last trailing space not returned
            // http://stackoverflow.com/questions/19365404/stringreader-omits-trailing-linebreak
                yield String.Empty
        |]


type System.IServiceProvider with
    member x.GetService<'T>() = x.GetService(typeof<'T>) :?> 'T
    member x.GetService<'S, 'T>() = x.GetService(typeof<'S>) :?> 'T

open Microsoft.VisualStudio.Text

type ITextSnapshot with
    /// SnapshotSpan of the entirety of this TextSnapshot
    member x.FullSpan =
        SnapshotSpan(x, 0, x.Length)

    /// Get the start and end line numbers of a snapshotSpan based on this textSnapshot
    /// returns a tuple of (startLineNumber, endLineNumber)
    member inline x.LineBounds (snapshotSpan:SnapshotSpan) =
        let startLineNumber = x.GetLineNumberFromPosition (snapshotSpan.Span.Start)
        let endLineNumber = x.GetLineNumberFromPosition (snapshotSpan.Span.End)
        (startLineNumber, endLineNumber)

    /// Get the text at line `num`
    member inline x.LineText num =  x.GetLineFromLineNumber(num).GetText()
 
type SnapshotSpan with
    member inline x.StartLine = x.Snapshot.GetLineFromPosition (x.Start.Position)
    member inline x.StartLineNum = x.Snapshot.GetLineNumberFromPosition x.Start.Position
    member inline x.StartColumn = x.Start.Position - x.StartLine.Start.Position 
    member inline x.EndLine = x.Snapshot.GetLineFromPosition (x.End.Position)
    member inline x.EndLineNum  = x.Snapshot.GetLineNumberFromPosition x.End.Position
    member inline x.EndColumn = x.End.Position - x.EndLine.Start.Position