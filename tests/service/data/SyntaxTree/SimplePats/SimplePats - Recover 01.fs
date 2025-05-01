module SimplePats

let x = fun (i: int,) -> i
let y = fun (a,b,) -> ()
let z = fun ([<Foo>] bar, [<Foo>] v: V,) -> ()
let ignore = fun (_,) -> ()
