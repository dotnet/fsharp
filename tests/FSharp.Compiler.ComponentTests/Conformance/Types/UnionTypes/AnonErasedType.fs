// Anonymous Union Types
// Ensure that erased type is not an interface
module Test

type I = interface end
type A() = interface I
type B() = interface I

let f (x: (A|B)) = x

[<EntryPoint>]
let main _ =
    let mi = typeof<A>.Assembly.GetType("Test").GetMethod("f")
    let paramTy = mi.GetParameters().[0].ParameterType
    let returnTy = mi.ReturnType

    if paramTy <> typeof<obj> then
        failwithf "Expected parameter type to be System.Object, but was %s" paramTy.FullName

    if returnTy <> typeof<obj> then
        failwithf "Expected return type to be System.Object, but was %s" returnTy.FullName

    if typeof<I>.IsAssignableFrom(paramTy) then
        failwith "Parameter type should NOT be (or be assignable to) the interface I"

    printfn "OK: erased type is obj, not I"
    0