//Nested types in namespace


let f = new N003.Foo()
let b = new N003.Foo.Bar()
let bz = new N003.Baz()

let rv = f.getValue() + b.getValue() + bz.getValue()
exit rv
