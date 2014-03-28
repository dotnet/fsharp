//Nested types in namespace with open


open N003
let f = new Foo()
let b = new Foo.Bar()
let bz = new Baz()

let rv = f.getValue() + b.getValue() + bz.getValue()
exit rv
