// #Regression #Conformance #TypesAndModules #GeneratedEqualityAndHashing 
// FSharp1.0:4913 - elevate warning to an error for structural equality case
// Make sure F# compiler emits an error message when Record, Union, Struct types override default System.Object.GetHashCode()
// without explicitly specifying CustomEquality attribute






#light

type R =
  { a : int; b : string }
  override x.GetHashCode() = x.a + x.b.Length

type U = 
  | A | B
  override x.GetHashCode() = 0

type S = 
  struct
    val mutable a : int
    override x.GetHashCode() = x.a
  end
