// #Conformance #FSI 

module Test1

let x = "a"
type x_t = string
let y = 4
type y_t = int

type t = X of string

module Nested = begin

  let x = "a"
  type x_t = string
  type t = X of string

end
