// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.CodeAnalysis

open System
open System.IO
open FSharp.Compiler.Text

[<RequireQualifiedAccess>]
type internal TextContainer =
    | OnDisk
    | Stream of Stream
    | SourceText of ISourceText

    interface IDisposable

/// The storage container for a F# source item that could either be on-disk or in-memory.
/// TODO: Make this public.
[<AbstractClass>]
type internal FSharpSource =

    /// The file path of the source.
    abstract FilePath: string

    /// The timestamp of the source.
    abstract GetTimeStamp: unit -> DateTime

    /// Gets the internal text container. Text may be on-disk, in a stream, or a source text.
    abstract internal GetTextContainer: unit -> TextContainer

    /// Creates a FSharpSource from disk. Only used internally.
    static member internal CreateFromFile: filePath: string -> FSharpSource

    /// Creates a FSharpSource from the specified file path by shadow-copying the file.
    //static member CreateCopyFromFile: filePath: string -> FSharpSource

    /// Creates a FSharpSource.
    static member Create:
        filePath: string * getTimeStamp: (unit -> DateTime) * getSourceText: (unit -> ISourceText) -> FSharpSource
