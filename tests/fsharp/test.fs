
let fail msg =
    printfn "%s" msg
    failwith msg

[<Struct>]
type T1 = { v1: int }
and T2 = 
  | T2C1 of int * string
  | T2C2 of T1 * T2
and [<Struct>] T3 = { v3: T2 }
and T4() =
    let mutable _v4 = { v3 = T2C2({v1=0}, T2C1(1, "hey")) }
    member __.v4 with get() = _v4 and set (x) = _v4 <- x

[<return:Struct>] 
let (|P1|_|) =
    function
    | 0 -> ValueNone
    | _ -> ValueSome()

[<return:Struct>] 
let (|P2|_|) =
    function
    | "foo" -> ValueNone
    | _ -> ValueSome "bar"

[<return:Struct>] 
let (|P3|_|) (x: T2) =
  match x with
  | T2C1(a, b) -> ValueSome(a, b)
  | _ -> ValueNone

[<return:Struct>] 
let (|P4|_|) (x: T4) =
  match x.v4 with
  | { v3 = T2C2 ({v1=a}, P3(b, c)) } -> ValueSome (a, b, c)
  | _ -> ValueNone

match 0, 1 with
| P1, _ -> fail "unit"
| _, P1 -> ()
| _     -> fail "unit"

match "foo", "bar" with
| P2 _, _ -> fail "string"
| _, P2("bar") -> ()
| _ -> fail "string"

let t4 = T4()
match t4 with
| P4 (0, 1, "hey") -> ()
| _ -> fail "nested"
            