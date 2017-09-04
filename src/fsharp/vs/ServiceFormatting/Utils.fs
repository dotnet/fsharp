// Copied from https://github.com/dungpa/fantomas and modified by Vasily Kirichenko

namespace Microsoft.FSharp.Compiler.SourceCodeServices.ServiceFormatting

open System

[<RequireQualifiedAccess>]
module String =
    let normalizeNewLine (str : string) =
        str.Replace("\r\n", "\n").Replace("\r", "\n")

    let normalizeThenSplitNewLine (str : string) =
        (normalizeNewLine str).Split('\n')

    let startsWithOrdinal (prefix : string) (str : string) =
        str.StartsWith(prefix, StringComparison.Ordinal)