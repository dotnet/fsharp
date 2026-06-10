// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// Abstractions for Edit-and-Continue log recording.
/// Used by both full assembly emission (ilwrite.fs) and delta emission (IlxDeltaEmitter.fs)
/// to provide a unified interface for tracking metadata changes.
module internal FSharp.Compiler.AbstractIL.ILEncLogWriter

open FSharp.Compiler.AbstractIL.BinaryConstants
open FSharp.Compiler.AbstractIL.ILDeltaHandles

/// Interface for recording Edit-and-Continue log entries.
/// Full assembly emission uses a no-op implementation.
/// Delta emission records entries for runtime metadata update.
type IEncLogWriter =
    /// Record an addition to a metadata table.
    /// Used for new rows in TypeDef, MethodDef, Field, Property, Event, etc.
    /// The operation specifies what kind of addition (AddMethod, AddField, etc.)
    abstract RecordAddition: table: TableName * rowId: int * operation: EditAndContinueOperation -> unit

    /// Record an update to an existing metadata row.
    /// Used when modifying existing items (method body changes, attribute updates, etc.)
    abstract RecordUpdate: table: TableName * rowId: int -> unit

    /// Record a row for the EncMap table.
    /// EncMap contains all tokens that appear in this delta (both updates and additions).
    abstract RecordEncMapEntry: table: TableName * rowId: int -> unit

/// No-op implementation for full assembly emission.
/// Full assemblies don't need EncLog/EncMap tables.
type NullEncLogWriter() =
    interface IEncLogWriter with
        member _.RecordAddition(_, _, _) = ()
        member _.RecordUpdate(_, _) = ()
        member _.RecordEncMapEntry(_, _) = ()

/// Entry in the EncLog table.
[<Struct>]
type EncLogEntry =
    { Table: TableName
      RowId: int
      Operation: EditAndContinueOperation }

/// Entry in the EncMap table.
[<Struct>]
type EncMapEntry =
    { Table: TableName
      RowId: int }

/// Delta emission implementation that accumulates EncLog/EncMap entries.
type DeltaEncLogWriter() =
    let encLogEntries = ResizeArray<EncLogEntry>()
    let encMapEntries = ResizeArray<EncMapEntry>()

    interface IEncLogWriter with
        member _.RecordAddition(table, rowId, operation) =
            encLogEntries.Add({ Table = table; RowId = rowId; Operation = operation })

        member _.RecordUpdate(table, rowId) =
            encLogEntries.Add({ Table = table; RowId = rowId; Operation = EditAndContinueOperation.Default })

        member _.RecordEncMapEntry(table, rowId) =
            encMapEntries.Add({ Table = table; RowId = rowId })

    /// Get all accumulated EncLog entries.
    member _.EncLogEntries: EncLogEntry list = encLogEntries |> Seq.toList

    /// Get all accumulated EncMap entries.
    member _.EncMapEntries: EncMapEntry list = encMapEntries |> Seq.toList

    /// Get EncLog entries as tuples for compatibility with existing code.
    member _.EncLogTuples: struct (TableName * int * EditAndContinueOperation) list =
        encLogEntries
        |> Seq.map (fun e -> struct (e.Table, e.RowId, e.Operation))
        |> Seq.toList

    /// Get EncMap entries as tuples for compatibility with existing code.
    member _.EncMapTuples: struct (TableName * int) list =
        encMapEntries
        |> Seq.map (fun e -> struct (e.Table, e.RowId))
        |> Seq.toList

    /// Clear all accumulated entries.
    member _.Clear() =
        encLogEntries.Clear()
        encMapEntries.Clear()
