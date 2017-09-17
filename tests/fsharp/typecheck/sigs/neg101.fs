
module M

type MyRec = { Foo: string }

let x: int = 1
let y = x.Foo 
let f1 z = z.Foo
let f2 (z: MyRec) = z.Foo
