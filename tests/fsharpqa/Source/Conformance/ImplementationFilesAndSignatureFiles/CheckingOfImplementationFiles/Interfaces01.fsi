// #Conformance #SignatureFiles 
#light

module Interfaces01
[<CustomComparisonAttribute (); CustomEqualityAttribute ()>]
type T = struct
  member s : string

  override Equals : System.Object -> bool
  override GetHashCode : unit -> int
  interface System.Collections.IStructuralEquatable
  interface System.IComparable
  interface System.Collections.IStructuralComparable
end
