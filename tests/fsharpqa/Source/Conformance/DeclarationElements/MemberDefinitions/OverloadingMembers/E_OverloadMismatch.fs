//<Expects id="FS0366" status="error">No implementation was given for</Expects>

open System
open System.IO
type IFoo =
  abstract member Foo : t:Type * r:TextReader -> obj
  abstract member Foo<'t> : TextReader -> 't


type Foo() =
  interface IFoo with
    member x.Foo(t, reader) = obj()

exit 1
