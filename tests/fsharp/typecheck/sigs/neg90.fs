module Test
let foo<'a when 'a : (new : unit -> 'a)>() = new 'a()
type Recd = {f : int}
let _ = foo<Recd>()

// See https://github.com/Microsoft/visualfsharp/issues/38
type [<Measure>] N = foo // foo is undefined
type M2 = float<N>


// See https://github.com/Microsoft/visualfsharp/issues/95
module First = 
  [<RequireQualifiedAccess>]
  type DU = Member of int

let _ = First.Member(0)