// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.
module FSharp.Compiler.Service.Tests.ILBinaryReaderMemoryTests

open System
open System.Collections.Generic
open System.IO
open System.Reflection
open FSharp.Compiler.AbstractIL.ILBinaryReader
open FSharp.Compiler.IO
open Xunit

let private instanceFields =
    BindingFlags.Instance ||| BindingFlags.Public ||| BindingFlags.NonPublic

let rec private allFields (ty: Type | null) =
    match ty with
    | Null -> Seq.empty
    | NonNull ty -> Seq.append (ty.GetFields instanceFields) (allFields ty.BaseType)

/// The metadata of a stable file is held weakly (see `WeakByteFile`) so that it can be dropped under memory
/// pressure and re-read on demand. Capturing a view over it, for example in a lazy value, defeats that.
/// Weakly held data is not reported: a weak reference has no field pointing to its target.
let private assertDoesNotRetainMetadata (root: obj) =
    let visited = HashSet<obj>(HashIdentity.Reference)
    let queue = Queue<obj * string>()

    let enqueue path (value: objnull) =
        match value with
        | NonNull value when not (value.GetType().IsPrimitive) && visited.Add value -> queue.Enqueue(value, path)
        | _ -> ()

    enqueue (root.GetType().Name) root

    while queue.Count > 0 do
        match queue.Dequeue() with
        | (:? ByteMemory), path -> failwith $"The metadata view is retained by: {path}"
        | (:? Array as array), path -> array |> Seq.cast<obj> |> Seq.iteri (fun i o -> enqueue $"{path}[{i}]" o)
        | value, path ->
            for field in allFields (value.GetType()) do
                enqueue $"{path}.{field.Name}" (field.GetValue value)

let private readerOptions =
    {
        pdbDirPath = None
        reduceMemoryUsage = ReduceMemoryFlag.Yes
        metadataOnly = MetadataOnlyFlag.Yes
        tryGetMetadataSnapshot = fun _ -> None
    }

[<Fact>]
let ``Reading type defs does not retain the metadata view`` () =
    // The bytes are only held weakly for files that look stable, which is decided by location (`IsStableFileHeuristic`).
    let directory =
        Path.Combine(FileSystem.GetTempPathShim(), "packages", Guid.NewGuid().ToString())
        |> FileSystem.DirectoryCreateShim

    let path = Path.Combine(directory, "TestAssembly.dll")
    FileSystem.CopyShim(typeof<FactAttribute>.Assembly.Location, path, false)

    try
        let reader = OpenILModuleReader path readerOptions

        // The lazily read parts of the type defs, such as the interface impls, are left unforced on purpose.
        assertDoesNotRetainMetadata (reader.ILModuleDef.TypeDefs.AsArray())
    finally
        FileSystem.FileDeleteShim path
        FileSystem.DirectoryDeleteShim directory
