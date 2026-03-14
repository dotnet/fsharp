// #Regression #Conformance #SignatureFiles 
// Regression test for FSharp1.0:4649
// Title: Having a signature file and implementing an interface incorrectly generates level 4 'This method will be made public in the underlying IL because it may implement an interface or override a method'
// Descr: Verify no level-4 warning message is displayed when implementing interface incorrectly

open System
open System.Collections

[<CustomComparison; CustomEquality>]
type T private (_i: int, _s: string) = struct

  member private x.i = _i
  member x.s = _s

  override x.Equals y = x.i = (y :?> T).i
  override x.GetHashCode () = hash x.i

  interface IStructuralEquatable with
    member x.Equals(y, cmp) = cmp.Equals(x.i, (y :?> T).i)
    member x.GetHashCode(_cmp) = hash x.i
  
  interface System.IComparable with
    member x.CompareTo y = compare x.i (y :?> T).i

  interface IStructuralComparable with
    member x.CompareTo(y, cmp) = cmp.Compare(x.i, (y :?> T).i)

end
