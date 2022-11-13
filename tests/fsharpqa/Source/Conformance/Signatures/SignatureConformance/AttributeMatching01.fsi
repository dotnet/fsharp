// #Conformance #SignatureFiles #Attributes #Regression
// Regression for 6446 - verifying spec matches implementation when fs/fsi files attributes differ
//<Expects status="error" id="FS1200" span="(17,7-17,23)">The attribute 'ObsoleteAttribute' appears in both the implementation and the signature, but the attribute arguments differ\. Only the attribute from the signature will be included in the compiled code\.</Expects>

module M

open System

[<Class>]
type T =
    member NoAttrInSig : unit -> unit

[<Class>]
type U =
    [<Obsolete("Sig")>]
    member NoAttrInImpl : unit -> unit

[<Class>]
type V =
    [<Obsolete("Sig")>]
    member DiffAttr : unit -> unit