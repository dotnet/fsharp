// #Regression #Conformance #TypesAndModules #GeneratedEqualityAndHashing 
// FSharp1.0:4913 - elevate warning to an error for structural equality case
// Make sure everything is OK when explicitly setting 'CustomEquality' attribute
// on structs/records/union while overriding GetHashCode/Equals.
// Also do verify there's no need in explicit attribute for class types.

#light

[<CustomEquality>]
[<NoComparison>]
type R =
  { a : int; b : string }
  override x.Equals(obj) = true
  override x.GetHashCode() = 0

[<CustomEquality>]
[<NoComparison>]
type U = 
  | A | B
  override x.Equals(obj) = true
  override x.GetHashCode() = 0

[<CustomEquality>]
[<NoComparison>]
type S = 
  struct
    val mutable a : int
    override x.Equals(obj) = true
    override x.GetHashCode() = 0
  end
  
type C = 
  class
    override x.Equals(obj) = true
    override x.GetHashCode() = 0
  end
