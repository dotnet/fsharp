module SignatureWithMissingOverride

type MyCollection(count:int) =
    member _.Count =count
    // This changes nullable annotation from nullable to non-nullable
    override _.ToString() : string = "MyCollection"
    // This does not change anything
    override _.GetHashCode() = 0
    // This must keep the inferred argument as nullable obj!
    override _.Equals(obj) = false