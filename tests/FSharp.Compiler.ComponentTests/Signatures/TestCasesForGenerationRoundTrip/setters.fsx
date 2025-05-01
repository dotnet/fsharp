type Foo() =
    member f.X with internal get (key1, key2) = true and public set (key1, key2) value = ()
    member f.Y with public get () = 'y' and internal set y = ignore<char> y
