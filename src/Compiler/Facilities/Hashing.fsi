namespace Internal.Utilities.Hashing

open System

/// Tools for hashing things with MD5 into a string that can be used as a cache key.
module internal Md5StringHasher =

    val hashString: s: string -> byte array

    val empty: string

    val addBytes: bytes: byte array -> s: string -> string

    val addString: s: string -> s2: string -> string

    val addSeq: items: 'item seq -> addItem: ('item -> string -> string) -> s: string -> string

    val addStrings: strings: string seq -> (string -> string)

    val addBool: b: bool -> s: string -> string

    val addDateTime: dt: System.DateTime -> s: string -> string

module internal Md5Hasher =

    val computeHash: bytes: byte array -> byte array

    val empty: 'a array

    /// Computes the MD5 hash of a string directly into a caller-allocated 16-byte buffer,
    /// avoiding the extra allocation of an intermediate hash-result array.
    val hashStringInto: s: string -> destination: Span<byte> -> unit

    /// Computes the MD5 hash of a string and returns it as a hex string, without allocating
    /// an intermediate byte array for the UTF8-encoded input (only the 16-byte hash result is allocated).
    val hashStringToString: s: string -> string

    val addBytes: bytes: byte array -> s: byte array -> byte array

    val addString: s: string -> s2: byte array -> byte array

    val addSeq: items: 'item seq -> addItem: ('item -> byte array -> byte array) -> s: byte array -> byte array

    val addStrings: strings: string seq -> (byte array -> byte array)

    val addBytes': bytes: byte array seq -> (byte array -> byte array)

    val addBool: b: bool -> s: byte array -> byte array

    val addDateTime: dt: System.DateTime -> s: byte array -> byte array

    val addDateTimes: dts: System.DateTime seq -> s: byte array -> byte array

    val addIntegers: items: int seq -> s: byte array -> byte array

    val addBooleans: items: bool seq -> s: byte array -> byte array

    val toString: bytes: byte array -> string
