//Nested types in nested namespaces

open N0041.N0042

let f = new Foo()
let b = new Foo.Bar()
let bz = new Baz()

let rv = f.getValue() + b.getValue() + bz.getValue()
exit rv
