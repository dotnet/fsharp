// #Regression #Conformance #TypesAndModules 
// Regression test for FSHARP1.0:4949
// unusual interaction with type abbreviations, inference, and forward references

type IWibble = 
     abstract member wibble : int -> int

let g (y : double) (x : IWibble) : IWibble = x 
  
let f = g 2.0 
type mywibble = IWibble 

try
  let (m : mywibble) = failwith "foo"
  printf "foo=%A\n" (f m)
with 
  | ex -> ()
