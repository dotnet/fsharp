//Nested type in namespace


let f = new N0041.N0042.Foo()
let b = new N0041.N0042.Foo.Bar()
let bz = new N0041.N0042.Baz()

let rv = f.getValue() + b.getValue() + bz.getValue()
exit rv
