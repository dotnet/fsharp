// #Regression #Conformance #TypesAndModules #GeneratedEqualityAndHashing 
// FSharp1.0:4913 - elevate warning to an error for structural equality case
// Make sure F# compiler emits an error message when Record, Union, Struct types override default System.Object.GetHashCode()
// without explicitly specifying CustomEquality attribute

//<Expects id="FS0344" status="error" span="(13,6-13,7)">The struct, record or union type 'R' has an explicit implementation of 'Object\.GetHashCode' or 'Object\.Equals'\. You must apply the 'CustomEquality' attribute to the type</Expects>
//<Expects id="FS0344" status="error" span="(17,6-17,7)">The struct, record or union type 'U' has an explicit implementation of 'Object\.GetHashCode' or 'Object\.Equals'\. You must apply the 'CustomEquality' attribute to the type</Expects>
//<Expects id="FS0344" status="error" span="(21,6-21,7)">The struct, record or union type 'S' has an explicit implementation of 'Object\.GetHashCode' or 'Object\.Equals'\. You must apply the 'CustomEquality' attribute to the type</Expects>


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
