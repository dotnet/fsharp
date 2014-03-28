// #Conformance #FSI 

module Test1

let x = 1
type x_t = int
type t = X of int

module Nested = begin

  let x = 1
  type x_t = int
  type t = X of int

end
