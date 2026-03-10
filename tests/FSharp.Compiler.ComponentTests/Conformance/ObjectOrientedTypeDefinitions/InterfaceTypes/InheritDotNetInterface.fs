// #Regression #Conformance #ObjectOrientedTypes #InterfacesAndImplementations 
// Regression test for FSHARP1.0:4919
// Can't define an interface that inherits from IComparable

open System.Collections
open System.Collections.Generic

type Queue<'a> =
  inherit IEnumerable<'a>
  inherit IEnumerable
  inherit System.IEquatable<Queue<'a>>
  inherit System.IComparable
  abstract IsEmpty : bool
  abstract PushBack : 'a -> Queue<'a>
  abstract PopFront : unit -> 'a * Queue<'a>

exit 0
