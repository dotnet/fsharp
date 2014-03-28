
#light


let internal x = System.DateTime.Now

let useInternalValue() = x


type r = internal { x : int }

let rValue = { x = 1 }
let useInternalField(r) = r.x

type u = internal |  C of int

let useInternalTag() = C(1)

type internal x = XX | YY

let useInternalType() = box XX

