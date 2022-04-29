// #Regression #NoMono #NoMT #CodeGen #EmittedIL   
// Regression test for FSharp1.0:5057
// Title: Inefficient access of mutable field of struct

module Experiment.Test

type Test = struct
     new (i) = {Field = i}
     val mutable Field: int
end

let test() =
     let t = new Test(2)
     t.Field
