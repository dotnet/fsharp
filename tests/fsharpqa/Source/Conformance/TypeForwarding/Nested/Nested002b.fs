//Type nested in namespace with open 


open N002
let f = new Foo()
let bz = new Baz()

let rv = f.getValue() + bz.getValue() 
exit rv
