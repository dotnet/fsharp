let r = ref 3
r := 4   // generates an informational in preview
let rv = !r   // generates an informational in preview
incr r   // generates an informational in preview
decr r   // generates an informational in preview

type X() =
    member x.M(a:int) = a
    member x.M(b:int) = b