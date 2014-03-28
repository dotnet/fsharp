// #Conformance #ObjectOrientedTypes #Classes #MethodsAndProperties
// assigning values to properties at initialization
type T() =
    member val Property = 0 with get,set

let x = T(Property=5)
if x.Property <> 5 then exit 1

exit 0
