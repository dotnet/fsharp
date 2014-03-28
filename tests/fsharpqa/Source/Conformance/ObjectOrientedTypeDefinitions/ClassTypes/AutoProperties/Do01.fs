// #Conformance #ObjectOrientedTypes #Classes #MethodsAndProperties

// modify backing field in a do
type T() =
    let mutable field = 0
    static let mutable otherField = 0
    do field <- field + 1
    static do
        otherField <- otherField + 1
    member val Property = field with get, set
    static member val OtherProperty = otherField with get,set

let x = T()
if x.Property <> 1 then exit 1
let y = T()
if T.OtherProperty <> 1 then exit 1

exit 0