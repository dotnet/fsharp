
namespace FooBarSoftware
open System.Collections.Generic
[<Struct>]
//[<Class>]
type Foo<'T> = 
  new : 'T -> 'T Foo
  member public GetEnumerator: unit -> IEnumerator<'T> 
  interface IEnumerable<'T>


