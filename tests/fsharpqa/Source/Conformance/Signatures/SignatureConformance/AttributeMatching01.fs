// #Conformance #SignatureFiles #Attributes #Regression
// Regression for 6446 - verifying spec matches implementation when fs/fsi files attributes differ
//<Expects status="error" id="FS1200" span="(17,7-17,23)">The attribute 'ObsoleteAttribute' appears in both the implementation and the signature, but the attribute arguments differ\. Only the attribute from the signature will be included in the compiled code\.</Expects>

module M

open System

type T() =
    [<Obsolete("Impl")>]
    member x.NoAttrInSig() = ()

type U() =
    member x.NoAttrInImpl() = ()

type V() =
    [<Obsolete("Impl")>]
    member x.DiffAttr() = ()

let t = T()
let u = U()
let v = V()

let t_attrs = t.GetType().GetMethod("NoAttrInSig").GetCustomAttributes(false).[0].GetType() = typeof<ObsoleteAttribute>
let u_attrs = u.GetType().GetMethod("NoAttrInImpl").GetCustomAttributes(false).[0].GetType() = typeof<ObsoleteAttribute>
let v_attrs = v.GetType().GetMethod("DiffAttr").GetCustomAttributes(false).[0].GetType() = typeof<ObsoleteAttribute>

type W() =
    inherit T()

let w = W()
let w_attrs = w.GetType().GetMethod("NoAttrInSig").GetCustomAttributes(false).[0].GetType() = typeof<ObsoleteAttribute>

exit <| if (t_attrs && u_attrs && v_attrs && w_attrs) then 0 else 1