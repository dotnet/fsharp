

type hidden = Lib.abstractType
type 'a visible = Lib.abstractType * 'a

let f1 (x: hidden) = ()
let f2 (x: hidden visible) = ()

exception D1 = Lib.A
exception D2 = Lib.C

let e3 = D1
let e2 = D2
