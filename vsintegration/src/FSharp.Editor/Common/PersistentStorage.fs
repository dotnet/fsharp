// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Collections.Immutable
open System.IO
open System.Security.Cryptography
open System.Text
open System.Threading
open System.Threading.Tasks

open Microsoft.CodeAnalysis

/// A hash of what persisted data was computed from. 16 bytes, the size of Roslyn's `Checksum`, so that Roslyn's own
/// storage can take it as it is.
[<Struct>]
type internal FSharpChecksum =
    {
        Data1: int64
        Data2: int64
    }

    static member Create(bytes: byte array) : FSharpChecksum =
        use sha256 = SHA256.Create()
        let hash = sha256.ComputeHash bytes

        {
            Data1 = BitConverter.ToInt64(hash, 0)
            Data2 = BitConverter.ToInt64(hash, 8)
        }

    static member Create(bytes: ImmutableArray<byte>) : FSharpChecksum =
        FSharpChecksum.Create(Seq.toArray bytes)

    static member Create(value: string) : FSharpChecksum =
        FSharpChecksum.Create(Encoding.UTF8.GetBytes value)

    /// The values in order, each told apart from its neighbours and from a missing one.
    static member Create(values: string seq) : FSharpChecksum =
        use buffer = new MemoryStream()

        do
            use writer = new BinaryWriter(buffer, Encoding.UTF8, leaveOpen = true)

            for value in values do
                match value with
                | null -> writer.Write false
                | value ->
                    writer.Write true
                    writer.Write value

        FSharpChecksum.Create(buffer.ToArray())

    static member Create(first: FSharpChecksum, second: FSharpChecksum) : FSharpChecksum =
        FSharpChecksum.Create(Array.append (first.ToBytes()) (second.ToBytes()))

    member this.ToBytes() : byte array =
        Array.append (BitConverter.GetBytes this.Data1) (BitConverter.GetBytes this.Data2)

    override this.ToString() = Convert.ToBase64String(this.ToBytes())

/// The per-document part of Roslyn's `IChecksummedPersistentStorage`, which this mirrors so that the store behind it
/// can become Roslyn's own.
type internal IFSharpChecksummedPersistentStorage =

    /// The data stored for the document under the name; null if there is none, or it was written with another
    /// checksum.
    abstract ReadStreamAsync:
        document: Document * name: string * checksum: FSharpChecksum * cancellationToken: CancellationToken -> Task<Stream>

    /// Stores the data for the document under the name, with the checksum a read compares; false if it was not stored.
    abstract WriteStreamAsync:
        document: Document * name: string * stream: Stream * checksum: FSharpChecksum * cancellationToken: CancellationToken -> Task<bool>

/// Hands out the storage of a solution, as Roslyn's `IChecksummedPersistentStorageService` does.
type internal IFSharpChecksummedPersistentStorageService =
    abstract GetStorageAsync: solution: Solution * cancellationToken: CancellationToken -> Task<IFSharpChecksummedPersistentStorage>

/// Where the data of a solution is kept, as Roslyn's `IPersistentStorageConfiguration` decides it.
type internal IFSharpPersistentStorageConfiguration =

    /// The directory, or ValueNone to keep nothing for the solution. Asynchronous, unlike Roslyn's, because Visual
    /// Studio answers where a solution keeps its state only on the main thread.
    abstract TryGetStorageLocationAsync: solution: Solution * cancellationToken: CancellationToken -> Task<string voption>
