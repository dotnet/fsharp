module Test
let foo<'a when 'a : (new : unit -> 'a)>() = new 'a()
type Recd = {f : int}
let _ = foo<Recd>()