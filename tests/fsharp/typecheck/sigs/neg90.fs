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

let _ = First.Member(0)     // compiles, but should not
let _ = First.DU.Member(0) // correct

// See https://github.com/Microsoft/visualfsharp/issues/95 - part 2
module ModuleWithRecord =
    [<RequireQualifiedAccess>]
    type Record1 = { Field1 : int }

let _ = { ModuleWithRecord.Record1.Field1 = 42 } // correct

open ModuleWithRecord
let _ = { Record1.Field1 = 42 }                  // correct
let _ = { ModuleWithRecord.Field1 = 42 }         // compiles, but should not
let _ = { ModuleWithRecord.Record1.Field1 = 42 } // correct