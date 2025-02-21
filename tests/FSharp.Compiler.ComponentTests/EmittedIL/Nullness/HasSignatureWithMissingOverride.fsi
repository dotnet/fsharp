module SignatureWithMissingOverride
[<Class>]
type MyCollection =
    member Count : int
    override ToString: unit -> string