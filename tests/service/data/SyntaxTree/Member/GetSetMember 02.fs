module Foo

type Foo() =
    member f.W with private set (key1, key2) value = () and get (key1, key2) = true
    member f.X with get (key1, key2) = true and private set (key1, key2) value = ()
    member f.Y with private get (key1, key2) = true and public set (key1, key2) value = ()
    member f.Z with public set (key1, key2) value = () and internal get (key1, key2) = true
