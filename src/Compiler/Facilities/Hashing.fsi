namespace Internal.Utilities.Hashing

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

    /// Computes the MD5 hash of the UTF-8 encoding of a string. The encoded bytes go to a buffer rented from
    /// the shared ArrayPool, and the result is the array that the hash algorithm returns, so this function
    /// allocates no array of its own.
    val hashString: s: string -> byte array

    /// Computes the MD5 hash of the UTF-8 encoding of a string as a hex string in the format of `toString`.
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
