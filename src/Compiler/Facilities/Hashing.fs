namespace Internal.Utilities.Hashing

open System
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
        |> System.BitConverter.ToString
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

    let addDateTime (dt: System.DateTime) (s: string) = dt.Ticks.ToString() |> addString <| s

module internal Md5Hasher =

    let private md5 =
        new ThreadLocal<_>(fun () -> System.Security.Cryptography.MD5.Create())

    let computeHash (bytes: byte array) = md5.Value.ComputeHash(bytes)

    let empty = Array.empty

    let hashString (s: string) =
        s |> System.Text.Encoding.UTF8.GetBytes |> computeHash

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

    let addDateTime (dt: System.DateTime) (s: byte array) =
        dt.Ticks |> BitConverter.GetBytes |> addBytes <| s

    let addDateTimes (dts: System.DateTime seq) (s: byte array) = s |> addSeq dts addDateTime

    let addInt (i: int) (s: byte array) =
        i |> BitConverter.GetBytes |> addBytes <| s

    let addIntegers (items: int seq) (s: byte array) = addSeq items addInt s

    let addBooleans (items: bool seq) (s: byte array) = addSeq items addBool s

    let toString (bytes: byte array) = bytes |> System.BitConverter.ToString
