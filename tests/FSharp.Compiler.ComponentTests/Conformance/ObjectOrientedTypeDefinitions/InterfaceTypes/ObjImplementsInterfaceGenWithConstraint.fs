// #Regression #Conformance #ObjectOrientedTypes #InterfacesAndImplementations 
// Regression test for FSHARP1.0:4943
// interface implementations in object expressions leave object this type under constrained (was: unverifiable code when implementing an interface)
//<Expects status="success"></Expects>

open System.Collections
open System.Collections.Generic

type Queue<'a> =
  inherit IEnumerable
  inherit IEnumerable<'a>

let mk() =
  { new Queue<'a > with

        override q.GetEnumerator() =
          (q.GetEnumerator() :> IEnumerator)

        override q.GetEnumerator() : IEnumerator<'a> =
          (q :> IEnumerable<'a>).GetEnumerator() }
