[<NoEquality; NoComparison; DefaultAugmentation(false)>]
type T =
    | T of int
    static member op_Implicit (T t) = U t

and [<NoEquality; NoComparison; DefaultAugmentation(false)>] U =
    | U of int

#nowarn 3391

let r6 : {| A : T |} = {| A = T 3 |}
let r7 : {| A : U |} = {| A = T 3 |}
let r8 : {| A : U |} = {| ...r6 |}
