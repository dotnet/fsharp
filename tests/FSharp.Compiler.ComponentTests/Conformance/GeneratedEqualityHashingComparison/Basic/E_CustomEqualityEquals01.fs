// #Regression #Conformance #TypesAndModules #GeneratedEqualityAndHashing 
// FSharp1.0:4913 - elevate warning to an error for structural equality case
// Make sure F# compiler emits an error message when Record, Union, Struct types override default System.Object.Equals()
// without explicitly specifying CustomEquality attribute






#light

type R =
  { a : int; b : string }
  override x.Equals(obj) = true

type U = 
  | A | B
  override x.Equals(obj) = true

type S = 
  struct
    val mutable a : int
    override x.Equals(obj) = true
  end
