module Test
let foo<'a when 'a : (new : unit -> 'a)>() = new 'a()
[<CLIMutable>]
type Recd = {f : int}
// records with CLIMutable should satisfy 'default ctor' constraint
foo<Recd>().f = 0