//Forwarding to multiple assemblies

let f = new Foo()
let b = new Bar()
let bz = new Baz()

let rv = f.getValue() + b.getValue() + bz.getValue()

exit rv
