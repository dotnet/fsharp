let r = ref 3
r := 4
let rv = !r
incr r
decr r

type X() =
    member x.M(a:int) = a
    member x.M(b:int) = b