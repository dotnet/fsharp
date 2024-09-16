

open System
open System.IO
type IFoo =
  abstract member Foo : t:Type * r:TextReader -> obj
  abstract member Foo<'t> : TextReader -> 't


type Foo() =
  interface IFoo with
    member x.Foo(t, reader) = obj()
