// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// The navigable items of a document as persistent storage keeps them, after Roslyn's `AbstractSyntaxIndex`: stored
/// with a checksum of what they were parsed from, compressed.
module internal Microsoft.VisualStudio.FSharp.Editor.NavigableItemsIndex

open System
open System.Collections.Generic
open System.IO
open System.IO.Compression
open System.Security
open System.Text

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Text

open FSharp.Compiler.EditorServices
open FSharp.Compiler.Text
open CancellableTasks

[<Literal>]
let PersistenceName = "FSharpNavigableItemsIndex"

/// Changes with what is written: the layout below, and what `NavigateTo.GetNavigableItems` makes of a tree, which
/// ships in another assembly and so is tied to that assembly's build.
let private formatChecksum =
    FSharpChecksum.Create
        [
            "2"
            typeof<NavigableItem>.Assembly.ManifestModule.ModuleVersionId.ToString()
        ]

/// Identifies a parse whose tree holds no conditional directives, and so reads the same under any defines.
let textChecksum (text: SourceText) =
    FSharpChecksum.Create(FSharpChecksum.Create(text.GetChecksum()), formatChecksum)

/// Identifies a parse whose tree does hold them, which only stands for the defines it was made with.
let textAndDefinesChecksum textChecksum (defines: string) =
    FSharpChecksum.Create(textChecksum, FSharpChecksum.Create defines)

[<Literal>]
let private FileTag = 0uy

[<Literal>]
let private ContainerTag = 1uy

[<Literal>]
let private ItemTag = 2uy

[<Literal>]
let private EndTag = 3uy

let private containerTypeTag containerType =
    match containerType with
    | NavigableContainerType.File -> 0uy
    | NavigableContainerType.Namespace -> 1uy
    | NavigableContainerType.Module -> 2uy
    | NavigableContainerType.Type -> 3uy
    | NavigableContainerType.Exception -> 4uy

let private containerTypeOfTag tag =
    match tag with
    | 0uy -> NavigableContainerType.File
    | 1uy -> NavigableContainerType.Namespace
    | 2uy -> NavigableContainerType.Module
    | 3uy -> NavigableContainerType.Type
    | 4uy -> NavigableContainerType.Exception
    | tag -> raise (InvalidDataException $"No container type has tag %d{tag}.")

let private kindTag kind =
    match kind with
    | NavigableItemKind.Module -> 0uy
    | NavigableItemKind.ModuleAbbreviation -> 1uy
    | NavigableItemKind.Exception -> 2uy
    | NavigableItemKind.Type -> 3uy
    | NavigableItemKind.ModuleValue -> 4uy
    | NavigableItemKind.Field -> 5uy
    | NavigableItemKind.Property -> 6uy
    | NavigableItemKind.Constructor -> 7uy
    | NavigableItemKind.Member -> 8uy
    | NavigableItemKind.EnumCase -> 9uy
    | NavigableItemKind.UnionCase -> 10uy

let private kindOfTag tag =
    match tag with
    | 0uy -> NavigableItemKind.Module
    | 1uy -> NavigableItemKind.ModuleAbbreviation
    | 2uy -> NavigableItemKind.Exception
    | 3uy -> NavigableItemKind.Type
    | 4uy -> NavigableItemKind.ModuleValue
    | 5uy -> NavigableItemKind.Field
    | 6uy -> NavigableItemKind.Property
    | 7uy -> NavigableItemKind.Constructor
    | 8uy -> NavigableItemKind.Member
    | 9uy -> NavigableItemKind.EnumCase
    | 10uy -> NavigableItemKind.UnionCase
    | tag -> raise (InvalidDataException $"No navigable item kind has tag %d{tag}.")

/// Containers are numbered in the order they are written, each after its parent, and an item names its container by
/// that number: items of one container share it once read back, as they did when parsed.
let private write (writer: BinaryWriter) (items: NavigableItem array) =
    let numbers = Dictionary<NavigableContainer, int>(HashIdentity.Reference)

    let rec writeContainer container =
        if not (numbers.ContainsKey container) then
            match container with
            | NavigableContainer.File fileName ->
                writer.Write FileTag
                writer.Write fileName
            | NavigableContainer.Container(containerType, nameParts, parent) ->
                writeContainer parent
                writer.Write ContainerTag
                writer.Write(containerTypeTag containerType)
                writer.Write nameParts.Length

                for part in nameParts do
                    writer.Write part

                writer.Write numbers[parent]

            numbers[container] <- numbers.Count

    for item in items do
        writeContainer item.Container
        writer.Write ItemTag
        writer.Write item.Name
        writer.Write item.NeedsBackticks
        writer.Write item.IsSignature
        writer.Write(kindTag item.Kind)
        writer.Write numbers[item.Container]
        writer.Write item.Range.FileName
        writer.Write item.Range.StartLine
        writer.Write item.Range.StartColumn
        writer.Write item.Range.EndLine
        writer.Write item.Range.EndColumn

    writer.Write EndTag

let private read (reader: BinaryReader) =
    let containers = ResizeArray<NavigableContainer>()
    let items = ResizeArray<NavigableItem>()

    let container number =
        if number >= 0 && number < containers.Count then
            containers[number]
        else
            raise (InvalidDataException $"No container has number %d{number}.")

    let readContainer () =
        let containerType = containerTypeOfTag (reader.ReadByte())
        let nameParts = List.init (reader.ReadInt32()) (fun _ -> reader.ReadString())
        let parent = container (reader.ReadInt32())

        NavigableContainer.Container(containerType, nameParts, parent)

    let readItem () =
        let name = reader.ReadString()
        let needsBackticks = reader.ReadBoolean()
        let isSignature = reader.ReadBoolean()
        let kind = kindOfTag (reader.ReadByte())
        let itemContainer = container (reader.ReadInt32())
        let fileName = reader.ReadString()
        let start = Position.mkPos (reader.ReadInt32()) (reader.ReadInt32())
        let finish = Position.mkPos (reader.ReadInt32()) (reader.ReadInt32())

        {
            Name = name
            NeedsBackticks = needsBackticks
            Range = Range.mkRange fileName start finish
            IsSignature = isSignature
            Kind = kind
            Container = itemContainer
        }

    let mutable finished = false

    while not finished do
        match reader.ReadByte() with
        | FileTag -> containers.Add(NavigableContainer.File(reader.ReadString()))
        | ContainerTag -> containers.Add(readContainer ())
        | ItemTag -> items.Add(readItem ())
        | EndTag -> finished <- true
        | tag -> raise (InvalidDataException $"No entry has tag %d{tag}.")

    items.ToArray()

let private serialize items =
    let stream = new MemoryStream()

    do
        use compressed = new GZipStream(stream, CompressionLevel.Optimal, leaveOpen = true)
        use writer = new BinaryWriter(compressed, Encoding.UTF8, leaveOpen = true)
        write writer items

    stream.Position <- 0L
    stream

let private deserialize (stream: Stream) =
    use compressed =
        new GZipStream(stream, CompressionMode.Decompress, leaveOpen = true)

    use reader = new BinaryReader(compressed, Encoding.UTF8, leaveOpen = true)
    read reader

/// What Roslyn's `IOUtilities.IsNormalIOException` lets storage throw — a damaged entry included — without failing
/// the feature that asked.
let private isNormalIOException (e: exn) =
    match e with
    | :? IOException
    | :? SecurityException
    | :? ArgumentException
    | :? UnauthorizedAccessException
    | :? NotSupportedException
    | :? InvalidOperationException
    | :? InvalidDataException -> true
    | _ -> false

/// The items stored for the document, if they were stored with the checksum.
let tryLoad (storageService: IFSharpChecksummedPersistentStorageService) (document: Document) checksum =
    cancellableTask {
        let! ct = CancellableTask.getCancellationToken ()

        try
            let! storage = storageService.GetStorageAsync(document.Project.Solution, ct)
            let! stream = storage.ReadStreamAsync(document, PersistenceName, checksum, ct)

            match stream with
            | null -> return ValueNone
            | stream ->
                use stream = stream
                return ValueSome(deserialize stream)
        with e when isNormalIOException e ->
            return ValueNone
    }

let save (storageService: IFSharpChecksummedPersistentStorageService) (document: Document) checksum items =
    cancellableTask {
        let! ct = CancellableTask.getCancellationToken ()

        try
            let! storage = storageService.GetStorageAsync(document.Project.Solution, ct)
            use stream = serialize items
            let! _ = storage.WriteStreamAsync(document, PersistenceName, stream, checksum, ct)
            ()
        with e when isNormalIOException e ->
            ()
    }
