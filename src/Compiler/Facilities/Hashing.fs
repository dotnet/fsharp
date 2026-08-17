namespace Internal.Utilities.Hashing

open System
open System.Security.Cryptography
open System.Threading

/// Tools for hashing things with MD5 into a string that can be used as a cache key.
module internal Md5StringHasher =

    let private md5 =
        new ThreadLocal<_>(fun () -> System.Security.Cryptography.MD5.Create())

    let private computeHash (bytes: byte array) = md5.Value.ComputeHash(bytes)

    let hashString (s: string) =
        System.Text.Encoding.UTF8.GetBytes(s) |> computeHash

    let empty = String.Empty

    let addBytes (bytes: byte array) (s: string) =
        let sbytes = s |> hashString

        Array.append sbytes bytes
        |> computeHash
        |> BitConverter.ToString
        |> (fun x -> x.Replace("-", ""))

    let addString (s: string) (s2: string) =
        s |> System.Text.Encoding.UTF8.GetBytes |> addBytes <| s2

    let addSeq<'item> (items: 'item seq) (addItem: 'item -> string -> string) (s: string) =
        items |> Seq.fold (fun s a -> addItem a s) s

    let addStrings strings = addSeq strings addString

    // If we use this make it an extension method?
    //let addVersions<'a, 'b when 'a :> ICacheKey<'b, string>> (versions: 'a seq) (s: string) =
    //    versions |> Seq.map (fun x -> x.GetVersion()) |> addStrings <| s

    let addBool (b: bool) (s: string) =
        b |> BitConverter.GetBytes |> addBytes <| s

    let addDateTime (dt: DateTime) (s: string) = dt.Ticks.ToString() |> addString <| s

module internal Md5Hasher =

#if NETSTANDARD2_0
    let private md5 = new ThreadLocal<_>(fun () -> MD5.Create())

    let computeHash (bytes: byte array) = md5.Value.ComputeHash(bytes)
#else
    let computeHash (bytes: byte array) = MD5.HashData(bytes)
#endif

    let empty = Array.empty

    /// Computes the MD5 hash of a string directly into a caller-allocated 16-byte buffer,
    /// avoiding the extra allocation of an intermediate hash-result array.
    /// The UTF8 encoding buffer is rented from the shared ArrayPool to avoid allocating
    /// a byte array the size of the input string on every call.
    let hashStringInto (s: string) (destination: Span<byte>) =
        let encoding = System.Text.Encoding.UTF8
        let maxByteCount = encoding.GetMaxByteCount(s.Length)
        let rented = System.Buffers.ArrayPool<byte>.Shared.Rent(maxByteCount)

        try
            let byteCount = encoding.GetBytes(s, 0, s.Length, rented, 0)
#if NETSTANDARD2_0
            let hash = md5.Value.ComputeHash(rented, 0, byteCount)
            hash.CopyTo(destination)
#else
            let mutable bytesWritten = 0

            MD5.TryHashData(ReadOnlySpan(rented, 0, byteCount), destination, &bytesWritten)
            |> ignore
#endif
        finally
            System.Buffers.ArrayPool<byte>.Shared.Return(rented)

    /// Computes the MD5 hash of a string and returns it as a hex string (matching the format of
    /// `toString`), without allocating an intermediate byte array for the UTF8-encoded input
    /// (only the 16-byte hash result is allocated).
    let hashStringToString (s: string) =
        let bytes = Array.zeroCreate<byte> 16
        hashStringInto s (Span bytes)
        BitConverter.ToString(bytes)

    let addBytes (bytes: byte array) (s: byte array) =

        Array.append s bytes |> computeHash

    let addString (s: string) (s2: byte array) =
        s |> System.Text.Encoding.UTF8.GetBytes |> addBytes <| s2

    let addSeq<'item> (items: 'item seq) (addItem: 'item -> byte array -> byte array) (s: byte array) =
        items |> Seq.fold (fun s a -> addItem a s) s

    let addStrings strings = addSeq strings addString
    let addBytes' bytes = addSeq bytes addBytes

    // If we use this make it an extension method?
    //let addVersions<'a, 'b when 'a :> ICacheKey<'b, string>> (versions: 'a seq) (s: string) =
    //    versions |> Seq.map (fun x -> x.GetVersion()) |> addStrings <| s

    let addBool (b: bool) (s: byte array) =
        b |> BitConverter.GetBytes |> addBytes <| s

    let addDateTime (dt: DateTime) (s: byte array) =
        dt.Ticks |> BitConverter.GetBytes |> addBytes <| s

    let addDateTimes (dts: DateTime seq) (s: byte array) = s |> addSeq dts addDateTime

    let addInt (i: int) (s: byte array) =
        i |> BitConverter.GetBytes |> addBytes <| s

    let addIntegers (items: int seq) (s: byte array) = addSeq items addInt s

    let addBooleans (items: bool seq) (s: byte array) = addSeq items addBool s

    let toString (bytes: byte array) = bytes |> BitConverter.ToString
