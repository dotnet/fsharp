let meh =
    { new Interface with
        override this.Foo () = ()
        member this.Bar () = ()
      interface SomethingElse with
        member this.Blah () = () }