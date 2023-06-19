type Foo() =
    member f.X with internal get (key1, key2) = true and public set (key1, key2) value = ()
