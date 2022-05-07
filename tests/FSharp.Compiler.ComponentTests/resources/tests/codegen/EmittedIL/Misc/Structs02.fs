// regression test for 767815: Invalid code is generated when using field initializers in struct constructor

module Experiment.Test

[<Struct>]
type Repro =
  val hash : int
  new(length) =
    { hash =
        let mutable h = 0
        for i=0 to length-1 do
          h <- 26*h
        h 
    }

let test() = 
    let t = Repro(42)
    t.hash