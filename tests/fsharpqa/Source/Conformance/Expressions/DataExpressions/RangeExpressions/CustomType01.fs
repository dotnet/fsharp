// #Conformance #DataExpressions 
// Verify the ability to use custom types in range expressions
// All it needs is a One property and a member (+)

type CustomType =
    | Start of int
    | Incr of int
    | Element of int
    static member (+) (lhs : CustomType, rhs : CustomType) =
        match lhs, rhs with
        | Start(x),   Incr(y) -> Element(x + y)
        | Element(x), Incr(y) -> Element(x + y)
        | Incr(_), _ -> failwithf "Error: lhs is Incr"
        | _, Start(_)
        | _, Element(_) -> failwith "Error: rhs is not Incr"


    static member One = Incr(1)

let t = [Start(0) .. Element(4)]

if t <> [Start 0; Element 1; Element 2; Element 3; Element 4] then
    exit 1

exit 0
