/// mutable class with auto-properties
type Person(name : string, age : int) =
    /// Full name
    member val Name = name with get, set