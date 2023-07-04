module V

let create () =
    { new Object() with
        override _.ToString() = ""
      interface Interface1 with
          member _.Foo1 s = s

      interface Interface2 with
          member _.Foo2 s = s }
